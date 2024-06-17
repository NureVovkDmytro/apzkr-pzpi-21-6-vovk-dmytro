using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Discerniy.Infrastructure
{
    [Authorize(Roles = "user")]
    public class UserConnectionHub : Hub
    {
        protected readonly IAuthService authService;
        protected readonly IClientRepository clientRepository;
        protected readonly IGroupRepository groupRepository;
        protected readonly IMarkRepository markRepository;
        protected readonly IDistributedCache cache;
        protected readonly ILogger logger;
        protected readonly IWebSocketMessagePublisher webSocketMessagePublisher;

        public UserConnectionHub(IAuthService authService, IClientRepository clientRepository, IGroupRepository groupRepository, IMarkRepository markRepository, IDistributedCache cache, ILogger<UserConnectionHub> logger, IWebSocketMessagePublisher webSocketMessagePublisher)
        {
            this.authService = authService;
            this.clientRepository = clientRepository;
            this.groupRepository = groupRepository;
            this.markRepository = markRepository;
            this.cache = cache;
            this.logger = logger;
            this.webSocketMessagePublisher = webSocketMessagePublisher;
        }

        public override async Task OnConnectedAsync()
        {
            this.authService.SetHttpContext(Context.GetHttpContext() ?? throw new ArgumentNullException());
            var client = await authService.GetUser();
            if (client != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, client.Id);
                foreach (var group in client.Groups)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, group);
                }
                if(!client.WebSocketConections.Contains(Context.ConnectionId))
                {
                    client.WebSocketConections.Add(Context.ConnectionId);
                    await clientRepository.Update(client);
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            this.authService.SetHttpContext(Context.GetHttpContext() ?? throw new ArgumentNullException());
            var client = await authService.GetUser();
            if (client != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, client.Id);
                foreach (var group in client.Groups)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
                }
                if (client.WebSocketConections.Contains(Context.ConnectionId))
                {
                    client.WebSocketConections.Remove(Context.ConnectionId);
                    await clientRepository.Update(client);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
