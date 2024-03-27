using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Mnd.Core.Contracts;
using Mnd.Service.BgWorker;

namespace Mnd.Service.SR;

public class WsHub(IBackgroundTaskQueue queue) : Hub
{
    private ConcurrentDictionary<string, string> _connectedClients = new();
    private ConcurrentDictionary<string, string> _connectedUsers = new();

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        string userId;
        string connectionId = Context.ConnectionId;
        if (_connectedClients.TryGetValue(connectionId, out userId))
        {
            _connectedClients.TryRemove(Context.ConnectionId, out _);
            _connectedUsers.TryRemove(userId, out _);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string userId, string message)
    {
        // await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
    
    public async Task RegisterClient(string userId)
    {
        _connectedClients.TryAdd(Context.ConnectionId, userId);
        _connectedUsers.TryAdd(userId, Context.ConnectionId);

        Frame frame = new Frame
        {
            C0Re = 0,
            C0Im = 0,
            Width = 3.5,
            CenterX = -0.7,
            CenterY = 0,
            FileName = "mandelbrot.png",
            FilePath = "../../../../../data/static"
        };

        await queue.QueueBackgroundWorkItemAsync(frame.GetBackgrounWorkItem(async () =>
        {
            await Clients.Client(Context.ConnectionId).SendAsync("Initial", JsonSerializer.Serialize(frame));
        }));
    }
}