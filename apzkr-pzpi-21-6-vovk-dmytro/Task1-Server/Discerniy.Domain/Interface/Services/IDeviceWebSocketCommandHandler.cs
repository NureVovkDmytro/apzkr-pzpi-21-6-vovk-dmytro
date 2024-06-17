using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

namespace Discerniy.Domain.Interface.Services
{
    public interface IDeviceWebSocketCommandHandler
    {
        Task HandleCommand(string deviceId, WebSocket client, HttpContext httpContext, string message, CancellationToken cancellationToken = default);
    }
}
