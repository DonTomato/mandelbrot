using Microsoft.AspNetCore.SignalR;
using Mnd.Core.Contracts;
using Mnd.Service.Logic.Interfaces;
using Mnd.Service.Models;
using Mnd.Service.SR;

namespace Mnd.Service.Logic;

public static class WorkItemCreator
{
    public static Func<CancellationToken, IHubContext<WsHub>, ValueTask> CreateWorkItem(Frame frame, string userId, 
        ISettingsService settings, 
        WsConnectionManager connectionManager)
    {
        var frameSizes = settings.GetFrameSize();
        
        async ValueTask WorkItem(CancellationToken token, IHubContext<WsHub> hubContext)
        {
            frame.GeneratePicture(new CalcContext
            {
                Iterations = 1500, 
                FrameWidth = frameSizes.Width, 
                FrameHeight = frameSizes.Height, 
                RenderType = RenderType.Smooth
            }, token);

            if (connectionManager.TryGetConnection(userId, out string connectionId))
            {
                var frameResponse = new FrameResponse
                {
                    X = frame.CenterX,
                    Y = frame.CenterY,
                    W = frame.Width,
                    FileName = frame.FileName
                };
                
                await hubContext.Clients.Client(connectionId)
                    .SendAsync("FrameCreated", MndSerializer.Serialize(frameResponse), cancellationToken: token);
            }
        }

        return WorkItem;
    }
}
