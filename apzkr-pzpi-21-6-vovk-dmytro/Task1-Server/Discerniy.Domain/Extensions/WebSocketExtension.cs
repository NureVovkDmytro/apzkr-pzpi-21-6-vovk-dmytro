using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Discerniy.Domain.Extensions
{
    public static class WebSocketExtension
    {
        public static async Task SendAsync<T>(this WebSocket client, T data)
        {
            var message = JsonSerializer.Serialize(data);
            var buffer = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
