using Amazon.Auth.AccessControlPolicy;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Extensions;
using Discerniy.Domain.Interface.Commands;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Net.WebSockets;
using System.Text.Json;

namespace Discerniy.Infrastructure.Commands.DeviceWebSocketHandlers
{
    public class DeviceWebSocketUpdateLocation : IDeviceWebSocketHandler
    {
        public string Command => "updateLocation";

        protected readonly IAuthService authService;
        protected readonly IClientRepository clientRepository;
        protected readonly IUserRepository userRepository;
        protected readonly IGroupRepository groupRepository;
        protected readonly IMarkRepository markRepository;
        protected readonly IDistributedCache cache;
        protected readonly ILogger logger;
        protected readonly IWebSocketMessagePublisher webSocketMessagePublisher;

        public DeviceWebSocketUpdateLocation(IAuthService authService, IClientRepository clientRepository, IUserRepository userRepository, IGroupRepository groupRepository, IMarkRepository markRepository, IDistributedCache cache, ILogger<DeviceWebSocketUpdateLocation> logger, IWebSocketMessagePublisher webSocketMessagePublisher)
        {
            this.authService = authService;
            this.clientRepository = clientRepository;
            this.userRepository = userRepository;
            this.groupRepository = groupRepository;
            this.markRepository = markRepository;
            this.cache = cache;
            this.logger = logger;
            this.webSocketMessagePublisher = webSocketMessagePublisher;
        }

        public async Task Handle(string userId, string message, WebSocket webSocket, HttpContext httpContext, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Deserialize<DeviceCommand<UpdateLocationRequest>>(message);
            if (request == null)
            {
                await webSocket.SendAsync(new ErrorResponse("Invalid request. Payload is not valid", Command));
                logger.LogDebug($"Invalid request from user {userId}. Payload is not valid");
                return;
            }

            this.authService.SetHttpContext(httpContext);
            var user = await authService.GetUserByDevice();
            if (user != null)
            {
                DateTime dateTime = await GetLastTimeUpdateLocation(user);
                if (DateTime.UtcNow.Subtract(dateTime).TotalSeconds < user.UpdateLocationSecondsInterval)
                {
                    await webSocket.SendAsync(new ErrorResponse("You can update location only once per minute", Command));
                    return;
                }

                user.Location = new GeoJsonPoint<GeoJson2DProjectedCoordinates>(new GeoJson2DProjectedCoordinates(request.Payload.Easting, request.Payload.Northing));
                user.Compass = request.Payload.Compass;
                user = await userRepository.Update(user);

                if (user.Groups.Count == 0)
                {
                    webSocketMessagePublisher.Publish(new LocationResponse(user), new Dictionary<string, string>()
                    {
                        { "userId", userId },
                        { "function", "LocationUpdated" }
                    });
                    await webSocket.SendAsync(new ErrorResponse("You are not in any group", Command));
                    return;
                }

                webSocketMessagePublisher.Publish(new LocationResponse(user), new Dictionary<string, string>()
                {
                    { "groups", string.Join(',', user.Groups) },
                    { "function", "LocationUpdated" }
                });

                if (await groupRepository.IsInsideOnGroupArea(user))
                {
                    if (user.ScanRadius != 0 && user.Location != null && user.Status == ClientStatus.Active)
                    {
                        IList<LocationResponse> clients = await clientRepository.GetNearClients(user.Location.Coordinates, user.ScanRadius);
                        webSocketMessagePublisher.Publish(clients, new Dictionary<string, string>()
                        {
                            { "userId", userId },
                            { "function", "NearClients" }
                        });
                    }
                }
                else
                {
                    webSocketMessagePublisher.Publish(new ErrorResponse("You are not inside the group area", "UpdateLocation"), new Dictionary<string, string>()
                    {
                        { "userId", userId },
                        { "function", "warning" }
                    });
                }

                await SetLastTimeUpdateLocation(user);
            }
        }

        private async Task<DateTime> GetLastTimeUpdateLocation(IClient client)
        {
            string? lastUpdateTime = await this.cache.GetStringAsync($"{client.Id}.LastTimeUpdateLocation");
            if (lastUpdateTime == null)
            {
                return DateTime.MinValue;
            }
            try
            {
                DateTime lastTime = DateTime.Parse(lastUpdateTime);
                return lastTime;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error parsing last time update location for client {client.Id}");
                return DateTime.MinValue;
            }
        }

        private async Task SetLastTimeUpdateLocation(IClient client)
        {
            await this.cache.SetStringAsync($"{client.Id}.LastTimeUpdateLocation", DateTime.UtcNow.ToString());
        }
    }
}
