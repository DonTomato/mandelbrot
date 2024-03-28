using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Mnd.Core.Contracts;
using Mnd.Service.BgWorker;
using Mnd.Service.Logic.Interfaces;

namespace Mnd.Service.SR;

public class WsHub(IBackgroundTaskQueue queue, ILogger<WsHub> logger, ISettingsService settings) : Hub
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

    public async Task SendFrame(string userId, string message)
    {
        if (_connectedUsers.TryGetValue(userId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("NewFrame", message);
        }
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
            FilePath = settings.GetStaticPath()
        };

        if (!Directory.Exists(frame.FilePath))
        {
            Directory.CreateDirectory(frame.FilePath);
        }

        await queue.QueueBackgroundWorkItemAsync(frame.GetBackgrounWorkItem(async () =>
        {
            await Clients.Client(Context.ConnectionId).SendAsync("Initial", JsonSerializer.Serialize(frame));
        }));
    }
}