using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Mnd.Service.SR;

namespace Mnd.Service.BgWorker;

public sealed class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, IHubContext<WsHub>, ValueTask>> _queue;

    public BackgroundTaskQueue(int capacity)
    {
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, IHubContext<WsHub>, ValueTask>>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, IHubContext<WsHub>, ValueTask> workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);

        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, IHubContext<WsHub>, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        Func<CancellationToken, IHubContext<WsHub>, ValueTask>? workItem = await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}
