using Discerniy.Domain.Attributes;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Exceptions;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Responses;

namespace Discerniy.Infrastructure.Services.ConfirmationHandlers
{
    [ActionType("change-email")]
    public class ChangeEmailConfirmationHandler : BaseConfirmationHandler
    {
        protected readonly IAuthService authService;
        protected readonly IUserRepository userRepository;

        public ChangeEmailConfirmationHandler(IAuthService authService, IUserRepository userRepository)
        {
            this.authService = authService;
            this.userRepository = userRepository;
        }

        public override async Task<ActionConfirmationResponse> Handle(ActionConfirmationModel confirmationModel)
        {
            var client = await authService.GetUser();
            string newEmail = confirmationModel.Data?.ToString();
            var user = await userRepository.Get(client.Id) ?? throw new NotFoundException("User not found");
            user.Email = newEmail;
            await userRepository.Update(user);
            Console.WriteLine("ChangeEmailConfirmationHandler.Handle");
            Console.WriteLine("client: " + client.Id);
            return new ActionConfirmationResponse(ActionType);
        }
    }
}
