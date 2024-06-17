using Discerniy.Domain.Attributes;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Responses;
using System.Reflection;

namespace Discerniy.Infrastructure.Services.ConfirmationHandlers
{
    public abstract class BaseConfirmationHandler
    {
        public string ActionType => GetType().GetCustomAttribute<ActionTypeAttribute>()?.ActionType ?? throw new Exception("ActionTypeAttribute not found");
        public abstract Task<ActionConfirmationResponse> Handle(ActionConfirmationModel confirmationModel);
    }
}
