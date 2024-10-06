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

    protected IMongoCollection<CourseRecord> CourseRecords => DbService.Wrapper.CourseRecords;
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
        var cursor = CourseRecords.Find(x =>
            x.Dirty ||
            x.ContentRecord == null ||
            x.CodeRecord == null ||
            x.InfoRecord == null
        ).SortBy(x => x.UpdateTime).Limit(128);

        var items = await cursor.ToListAsync(cancellationToken: cancellationToken);
        foreach (var record in items) {

            var course = await (await Courses
                .FindAsync(c => c.Id == record.CourseId && !c.Deleted, cancellationToken: cancellationToken))
                .FirstAsync(cancellationToken: cancellationToken);
            
            if (course == null) {
                await CourseRecords.DeleteOneAsync(r => r.SystemId == record.SystemId, cancellationToken: cancellationToken);
                continue;
            }
            
            // content search record

            // TODO add chinese support
            
            var content = record.Language.StartsWith("ja") 
                ? SearchUtils.TokenizeJapanese(course.Content.Ja)
                : SearchUtils.TokenizeEnglish(course.Content.En);
            
            record.ContentRecord = string.Join(' ', content);

            var name = record.Language.StartsWith("ja") 
                ? SearchUtils.TokenizeJapanese(course.Name.Ja)
                : SearchUtils.TokenizeEnglish(course.Name.En);
            
            record.InfoRecord = string.Join(' ', name);

            record.CodeRecord = course.CourseCode;

            // mark clean

            record.Dirty = false;
            record.UpdateTime = currentTime;

            // TODO aggregate more fields
            
            await CourseRecords.UpdateOneAsync(
                x => x.SystemId == record.SystemId, 
                Builders<CourseRecord>.Update.Set(x => x, record), 
                cancellationToken: cancellationToken);
        }


        await Task.CompletedTask;
    }
}