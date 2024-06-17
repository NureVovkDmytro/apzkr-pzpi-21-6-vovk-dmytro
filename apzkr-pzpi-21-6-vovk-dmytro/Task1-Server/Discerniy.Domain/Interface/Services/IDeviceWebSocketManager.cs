using Discerniy.Domain.Entity;

namespace Discerniy.Domain.Interface.Services
{
    public interface IDeviceWebSocketManager
    {
        WebSocketConnetion? Get(string connectionId);

        void Add(WebSocketConnetion connection);
        void AddToGroup(string connectionId, string groupName);
        bool RemoveFromGroup(string connectionId, string groupName);
        bool RemoveFromGroups(string connectionId);
        bool Remove(string connectionId);

        Task<bool> SendToAll(string message);
        Task<bool> SendToGroup(string groupName, string message);
    }
}
