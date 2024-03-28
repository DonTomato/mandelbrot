using System.Collections.Concurrent;

namespace Mnd.Service.SR;

public class WsConnectionManager
{
    private ConcurrentDictionary<string, string> _connectedClients = new();
    private ConcurrentDictionary<string, string> _connectedUsers = new();

    public void AddConnection(string userId, string connectionId)
    {
        _connectedClients.TryAdd(connectionId, userId);
        _connectedUsers.TryAdd(userId, connectionId);
    }

    public void RemoveConnection(string connectionId)
    {
        string userId;
        if (_connectedClients.TryGetValue(connectionId, out userId))
        {
            _connectedClients.TryRemove(connectionId, out _);
            _connectedUsers.TryRemove(userId, out _);
        }   
    }

    public bool TryGetConnection(string userId, out string connectionId)
    {
        return _connectedUsers.TryGetValue(userId, out connectionId);
    }
}