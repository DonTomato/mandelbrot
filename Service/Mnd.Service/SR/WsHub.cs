using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Mnd.Core.Contracts;
using Mnd.Service.BgWorker;
using Mnd.Service.Logic;
using Mnd.Service.Logic.Interfaces;

namespace Mnd.Service.SR;

public class WsHub(IBackgroundTaskQueue queue, ILogger<WsHub> logger, ISettingsService settings, WsConnectionManager connectionManager) : Hub
{
    public override Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        if (!string.IsNullOrEmpty(userId))
        {
            connectionManager.AddConnection(userId, Context.ConnectionId);
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        connectionManager.RemoveConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendFrame(string userId, string message)
    {
        if (connectionManager.TryGetConnection(userId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("NewFrame", message);
        }
    }
    
    // public async Task RegisterClient(string userId)
    // {
    //     _connectedClients.TryAdd(Context.ConnectionId, userId);
    //     _connectedUsers.TryAdd(userId, Context.ConnectionId);
    //
    //     Frame frame = new Frame
    //     {
    //         C0Re = 0,
    //         C0Im = 0,
    //         Width = 3.5,
    //         CenterX = -0.7,
    //         CenterY = 0,
    //         FileName = "mandelbrot.png",
    //         FilePath = settings.GetStaticPath()
    //     };
    //
    //     if (!Directory.Exists(frame.FilePath))
    //     {
    //         Directory.CreateDirectory(frame.FilePath);
    //     }
    //
    //     var workItem = WorkItemCreator.CreateWorkItem(frame, userId, settings);
    //
    //     await queue.QueueBackgroundWorkItemAsync(workItem);
    // }
}