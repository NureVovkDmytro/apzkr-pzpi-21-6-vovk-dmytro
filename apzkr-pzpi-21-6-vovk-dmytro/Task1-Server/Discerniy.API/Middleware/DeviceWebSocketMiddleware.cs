using Discerniy.Domain.Entity;
using Discerniy.Domain.Interface.Services;
using Discerniy.Infrastructure.Services;
using System.Security.Claims;

namespace Discerniy.API.Middleware
{
    public class DeviceWebSocketMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<DeviceWebSocketMiddleware> logger;
        private readonly IDeviceWebSocketManager deviceWebSocketManager;

        public DeviceWebSocketMiddleware(RequestDelegate next, ILogger<DeviceWebSocketMiddleware> logger, IDeviceWebSocketManager deviceWebSocketManager)
        {
            this.next = next;
            this.logger = logger;
            this.deviceWebSocketManager = deviceWebSocketManager;
        }

        public async Task Invoke(HttpContext context)
        {
            if(context.Request.Path != "/connect/device")
            {
                await next(context);
                return;
            }

            if (context.WebSockets.IsWebSocketRequest)
            {
                try
                {
                    string userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                    var socket = await context.WebSockets.AcceptWebSocketAsync();

                    var connectionModel = new WebSocketConnetion(context, socket, userId);
                    deviceWebSocketManager.Add(connectionModel);
                    deviceWebSocketManager.AddToGroup(connectionModel.ConnectionId, userId);
                    await connectionModel.Handle();
                }
                catch (Exception ex)
                {
                    logger.LogDebug(ex, "Error while handling websocket connection");
                }
                finally
                {
                    deviceWebSocketManager.Remove(context.TraceIdentifier);
                }
            }
            else
            {
                context.Response.StatusCode = 405;
            }
        }
    }
}
