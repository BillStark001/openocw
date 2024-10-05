using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Oocw.Base;
using Oocw.Database.Models;

namespace Oocw.Backend.Services;

public class SearchRecordService : BackgroundService
{
    [FromServices] public ILogger<SearchRecordService> Logger { get; set; } = null!;
    [FromServices] public DatabaseService DbService { get; set; } = null!;

    protected IMongoCollection<Course> Courses => DbService.Wrapper.Courses;

    // TODO use configuration file
    static readonly TimeSpan _period = TimeSpan.FromMinutes(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoWorkAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred executing task.");
            }

            await Task.Delay(_period, stoppingToken);
        }
    }

    protected async Task DoWorkAsync(CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;

        // find at most 128 records needing search record updates
        var cursor = Courses.Find(x =>
            x.Meta.Dirty ||
            x.Meta.SearchRecord == null ||
            x.Meta.SearchRecord == "" ||
            (x.UpdateTime != null && x.Meta.UpdateTime < x.UpdateTime)
        ).Limit(128);

        var items = await cursor.ToListAsync(cancellationToken: cancellationToken);
        foreach (var x in items) {
            
            // TODO add chinese support
            var colJa = string.Join(' ', SearchUtils.TokenizeJapanese(x.Content.Ja ?? ""));
            var colEn = string.Join(' ', SearchUtils.TokenizeJapanese(x.Content.En ?? ""));
            
            x.Meta.SearchRecordByLanguage = x.Meta.SearchRecordByLanguage ?? new();
            x.Meta.SearchRecordByLanguage.Ja = colJa;
            x.Meta.SearchRecordByLanguage.En = colEn;

            x.Meta.SearchRecord = colJa + " " + colEn;

            x.Meta.Dirty = false;
            x.Meta.UpdateTime = currentTime;

            // TODO aggregate more fields
            
            await Courses.UpdateOneAsync(c => c.SystemId == x.SystemId, Builders<Course>.Update.Set(c => c, x), cancellationToken: cancellationToken);
        }


        await Task.CompletedTask;
    }
}