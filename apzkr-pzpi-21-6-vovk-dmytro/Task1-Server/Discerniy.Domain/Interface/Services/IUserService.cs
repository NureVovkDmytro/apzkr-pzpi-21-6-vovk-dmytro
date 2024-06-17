using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;

namespace Discerniy.Domain.Interface.Services
{
    public interface IUserService : IClientService<UserModel, UserResponse>
    {
        Task<UserResponseDetailed> CreateUser(CreateUserRequest user);
        Task<UserResponse> GetSelfShort();
        Task<UserResponseDetailed> GetSelf();
        Task<UserResponseDetailed> GetUser(string id);
        Task<PageResponse<UserResponseDetailed>> GetAllUsers(PageRequest request);
        Task<PageResponse<UserResponseDetailed>> Search(UsersSearchRequest request);
        Task<UserResponseDetailed> GetUserByEmail(string email);

        Task<TokenResponse> ActivateUser(ActivateUserRequest request);

        Task<UserResponseDetailed> ResetPassword(string id);
        Task<UserResponseDetailed> ChangePassword(ChangePasswordRequest request);

        Task<UserResponseDetailed> UpdateBaseInformation(string id, UpdateUserBaseInformationRequest request);
        Task<UserResponse> UpdateSelfEmail(string email);
        Task<UserResponse> UpdatePermissions(string id, ClientPermissions permissions);
        Task<UserResponse> UpdateLocationSecondsInterval(string id, int secondsInterval);
    }
}
