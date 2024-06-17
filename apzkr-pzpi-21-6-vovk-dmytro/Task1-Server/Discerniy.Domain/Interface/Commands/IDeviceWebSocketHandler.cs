using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

namespace Discerniy.Domain.Interface.Commands
{
    public interface IDeviceWebSocketHandler
    {
        string Command { get; }
        Task Handle(string userId, string message, WebSocket webSocket, HttpContext httpContext, CancellationToken cancellationToken = default);
    }
}
