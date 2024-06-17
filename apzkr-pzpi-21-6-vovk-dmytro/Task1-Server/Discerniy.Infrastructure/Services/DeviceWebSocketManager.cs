using Discerniy.Domain.Entity;
using Discerniy.Domain.Interface.Services;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Discerniy.Infrastructure.Services
{
    public class DeviceWebSocketManager : IDeviceWebSocketManager
    {
        private readonly ConcurrentDictionary<string, WebSocketConnetion> connections = new ConcurrentDictionary<string, WebSocketConnetion>();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocketConnetion>> groups = new ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocketConnetion>>();

        public void Add(WebSocketConnetion connection)
        {
            connections.TryAdd(connection.ConnectionId, connection);
        }

        public void AddToGroup(string connectionId, string groupName)
        {
            var connection = Get(connectionId);
            if (connection != null)
            {
                if (groups.TryGetValue(groupName, out var group))
                {
                    group.TryAdd(connectionId, connection);
                }
                else
                {
                    var newGroup = new ConcurrentDictionary<string, WebSocketConnetion>();
                    newGroup.TryAdd(connectionId, connection);
                    groups.TryAdd(groupName, newGroup);
                }
            }
        }

        public WebSocketConnetion? Get(string connectionId)
        {
            if (connections.TryGetValue(connectionId, out var connection))
            {
                return connection;
            }
            return null;
        }

        public bool Remove(string connectionId)
        {
            return connections.TryRemove(connectionId, out _) && RemoveFromGroups(connectionId);
        }

        public bool RemoveFromGroup(string connectionId, string groupName)
        {
            if (groups.TryGetValue(groupName, out var group))
            {
                return group.TryRemove(connectionId, out _);
            }
            return false;
        }

        public bool RemoveFromGroups(string connectionId)
        {
            bool result = false;
            foreach (var group in groups)
            {
                if (group.Value.TryRemove(connectionId, out _))
                {
                    result = true;
                }
            }
            return result;
        }

        public async Task<bool> SendToAll(string message)
        {
            foreach (var connection in connections)
            {
                await connection.Value.Socket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            return true;
        }

        public async Task<bool> SendToGroup(string groupName, string message)
        {
            if (groups.TryGetValue(groupName, out var group))
            {
                foreach (var connection in group)
                {
                    await connection.Value.Socket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                return true;
            }
            return false;
        }
    }
}
