using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Oocw.Backend.Services;

public class RecordUpdateService : BackgroundService
{
    [FromServices] public ILogger<RecordUpdateService> Logger { get; set; } = null!;
    private readonly TimeSpan _period = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoWorkAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred executing task.");
            }

            await Task.Delay(_period, stoppingToken);
        }
    }

    private async Task DoWorkAsync()
    {
        // TODO
        await Task.CompletedTask;
    }
}