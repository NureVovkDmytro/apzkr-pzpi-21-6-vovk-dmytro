using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Responses;

namespace Discerniy.Domain.Interface.Services
{
    public interface IConfirmationManager
    {
        Task<ActionConfirmationModel> Create<T>(object? data = null)
            where T : class;
        Task<ActionConfirmationResponse> Confirm(string actionType, string token);
    }
}
