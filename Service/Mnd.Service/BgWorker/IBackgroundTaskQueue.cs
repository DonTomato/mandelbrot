using Microsoft.AspNetCore.SignalR;
using Mnd.Service.SR;

namespace Mnd.Service.BgWorker;

public interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, IHubContext<WsHub>, ValueTask> workItem);
    ValueTask<Func<CancellationToken, IHubContext<WsHub>, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}
