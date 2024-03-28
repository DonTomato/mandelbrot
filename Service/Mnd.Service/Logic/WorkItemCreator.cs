using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Mnd.Core.Contracts;
using Mnd.Service.Logic.Interfaces;
using Mnd.Service.SR;

namespace Mnd.Service.Logic;

public static class WorkItemCreator
{
    public static Func<CancellationToken, IHubContext<WsHub>, ValueTask> CreateWorkItem(Frame frame, string userId, ISettingsService settings)
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

            await hubContext.Clients.All.SendAsync("SendFrame", userId, JsonSerializer.Serialize(frame), cancellationToken: token);
        }

        return WorkItem;
    }
}
