using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Mnd.Service.SR;

public class WsHub : Hub
{
    private ConcurrentDictionary<string, string> _connectedClients = new();
    private ConcurrentDictionary<string, string> _connectedUsers = new();
    
    // public override Task OnConnectedAsync()
    // {
    //     // _connectedClients.TryAdd(Context.ConnectionId, Context.UserIdentifier);
    //     return base.OnConnectedAsync();
    // }

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
    }
}