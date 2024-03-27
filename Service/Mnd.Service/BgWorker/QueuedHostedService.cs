namespace Mnd.Service.BgWorker;

public sealed class QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(QueuedHostedService)} is started.");
        return ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Func<CancellationToken, ValueTask>? workItem = await taskQueue.DequeueAsync(stoppingToken);

                await workItem(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(QueuedHostedService)} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}