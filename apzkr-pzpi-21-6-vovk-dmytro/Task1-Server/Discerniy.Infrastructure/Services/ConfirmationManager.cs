using Discerniy.Domain.Attributes;
using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Exceptions;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Responses;
using Discerniy.Infrastructure.Services.ConfirmationHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace Discerniy.Infrastructure.Services
{
    public class ConfirmationManager : IConfirmationManager
    {
        protected readonly IDistributedCache cache;
        protected readonly ConfirmationOptions options;
        protected readonly ILogger logger;
        protected readonly IAuthService authService;
        protected readonly HttpContext context;

        protected string CurrentNamespace => GetType().Namespace!;

        protected const string ConfirmationKey = "confirmation.{0}.{1}";

        public ConfirmationManager(IDistributedCache cache, ConfirmationOptions options, ILogger<ConfirmationManager> logger, IAuthService authService, IHttpContextAccessor context)
        {
            this.cache = cache;
            this.options = options;
            this.logger = logger;
            this.authService = authService;
            if (context.HttpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available");
            }
            this.context = context.HttpContext;
        }

        public async Task<ActionConfirmationResponse> Confirm(string actionType, string token)
        {
            var key = string.Format(ConfirmationKey, actionType, token);
            var json = await cache.GetStringAsync(key) ?? throw new NotFoundException("Confirmation not found");

            var confirmation = JsonSerializer.Deserialize<ActionConfirmationModel>(json) ?? throw new NotFoundException("Confirmation not found");

            var handler = GetHandler(actionType);
            if (handler == null)
            {
                throw new BadRequestException("Handler not found");
            }

            var response = await handler.Handle(confirmation);

            await cache.RemoveAsync(key);

            return response;
        }

        public async Task<ActionConfirmationModel> Create<T>(object? data = null)
            where T : class
        {
            string actionType = typeof(T).GetCustomAttribute<ActionTypeAttribute>()?.ActionType ?? throw new InvalidOperationException("Action type not found");

            var user = await authService.GetUser() ?? throw new UnauthorizedAccessException("User not found");

            var confirmation = new ActionConfirmationModel(actionType, user.Id, data);

            var key = string.Format(ConfirmationKey, actionType, confirmation.Token);
            var json = JsonSerializer.Serialize(confirmation);

            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(this.options.ExpirationTimeInMinutes)
            });
            return confirmation;
        }

        protected BaseConfirmationHandler? GetHandler(string actionType)
        {
            var handlerType = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.Namespace == $"{CurrentNamespace}.ConfirmationHandlers" && t.GetCustomAttribute<ActionTypeAttribute>()?.ActionType == actionType);

            if (handlerType == null)
            {
                return null;
            }
            if (!typeof(BaseConfirmationHandler).IsAssignableFrom(handlerType))
            {
                return null;
            }

            var constructorParameters = handlerType.GetConstructors().First().GetParameters();
            List<object> constructorArguments = new List<object>();
            foreach (var parameter in constructorParameters)
            {
                var parameterType = parameter.ParameterType;
                var service = context.RequestServices.GetService(parameterType);
                if (service is null)
                {
                    throw new InvalidOperationException($"Unable to resolve service for type {parameterType}.");
                }
                constructorArguments.Add(service);
            }

            var handlerInstance = Activator.CreateInstance(handlerType, constructorArguments.ToArray());

            return handlerInstance as BaseConfirmationHandler;
        }
    }
}
