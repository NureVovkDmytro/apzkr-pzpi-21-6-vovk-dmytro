using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Entity.RabbitMqModels;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Exceptions;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using Discerniy.Infrastructure.Services.ConfirmationHandlers;
using Microsoft.Extensions.Caching.Distributed;
using VoDA.AspNetCore.Services.Email;

namespace Discerniy.Infrastructure.Services
{
    public class UserService : ClientService<UserModel, UserResponse>, IUserService
    {
        private readonly IUserRepository repository;
        private readonly IHashService hashService;
        private readonly IDistributedCache cache;
        private readonly IEmailService emailService;
        private readonly IConfirmationManager confirmationManager;
        private readonly IRandomGenerator randomGenerator;
        private readonly IWebSocketMessagePublisher webSocketMessagePublisher;
        private readonly UrlOptions urlOptions;

        private async Task<UserModel> GetCurrentClient()
        {
            var client = await authService.GetUser() ?? throw new BadRequestException("Permission denied");

            switch (client.Status)
            {
                case ClientStatus.Banned:
                    throw new BadRequestException("Your account is banned");
                case ClientStatus.Inactive:
                    throw new BadRequestException("Your account is not active");
                case ClientStatus.Limited:
                    throw new BadRequestException("Your account has limited access");
                case ClientStatus.Active:
                    break;
            }
            return client;
        }

        public UserService(
            IUserRepository repository,
            IGroupRepository groupRepository,
            IAuthService authService,
            IHashService hashService,
            IDistributedCache cache,
            IEmailService emailService,
            IConfirmationManager confirmationManager,
            IRandomGenerator randomGenerator,
            IWebSocketMessagePublisher webSocketMessagePublisher,
            UrlOptions urlOptions)
            : base(repository, groupRepository, authService)
        {
            this.repository = repository;
            this.hashService = hashService;
            this.cache = cache;
            this.emailService = emailService;
            this.confirmationManager = confirmationManager;
            this.randomGenerator = randomGenerator;
            this.webSocketMessagePublisher = webSocketMessagePublisher;
            this.urlOptions = urlOptions;
        }

        protected override UserResponse CreateResponse(UserModel? entity)
        {
            return entity == null ? null : new UserResponse(entity);
        }

        public async Task<UserResponseDetailed> ResetPassword(string id)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanResetPassword);
            var user = await repository.Get(id) ?? throw new BadRequestException("Permission denied");

            if (user.Id == currentClient.Id)
            {
                throw new BadRequestException("You cannot change your own password using this method. Use the change password method instead.");
            }

            if (currentClient.AccessLevel <= user.AccessLevel)
            {
                throw new BadRequestException("You cannot reset the password of a user with a higher access level than your own.");
            }

            var newPassword = randomGenerator.GenerateString(48);
            await this.emailService.SendEmailUseTemplate(user.Email, "PasswordReset.html", new Dictionary<string, string>
            {
                { "baseHost", urlOptions.Web },
                { "name", user.FirstName },
                { "email", user.Email },
                { "password", newPassword },
            });
            user.NeedPasswordChange = true;
            user.Password = hashService.Hash(newPassword);

            return await repository.Update(user);
        }

        public async Task<UserResponseDetailed> ChangePassword(ChangePasswordRequest request)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanUpdateSelfEmail);

            var user = await repository.Get(currentClient.Id) ?? throw new BadRequestException("User not found");

            if (hashService.Verify(request.OldPassword, user.Password))
            {
                user.Password = hashService.Hash(request.NewPassword);
                user.NeedPasswordChange = false;
                return await repository.Update(user);
            }
            else
            {
                throw new BadRequestException("Invalid password");
            }
        }

        public async Task<UserResponseDetailed> CreateUser(CreateUserRequest user)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanCreate);

            if (await repository.ExistsByEmail(user.Email))
            {
                throw new BadRequestException("User with this email already exists");
            }

            if (user.AccessLevel < 0)
            {
                throw new BadRequestException("Access level must be greater than 0");
            }
            if (user.AccessLevel >= currentClient.AccessLevel)
            {
                throw new BadRequestException("Access level must be less than your own");
            }
            var password = randomGenerator.GenerateString(48);
            var userModel = new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Nickname = user.Nickname,
                Email = user.Email,
                TaxPayerId = user.TaxPayerId,
                Password = hashService.Hash(password),
                Description = user.Description,
                AccessLevel = user.AccessLevel,
                ScanRadius = user.ScanRadius,
                Status = user.Status,
                NeedPasswordChange = true
            };

            userModel.Permissions.Update(user.Permissions, currentClient.Permissions);

            await this.emailService.SendEmailUseTemplate(user.Email, "RegistrationNotice.html", new Dictionary<string, string> {
                { "baseHost", urlOptions.Web },
                { "name", user.FirstName },
                { "email", user.Email },
                { "password", password },
            });

            return await repository.Create(userModel);
        }

        public async Task<PageResponse<UserResponseDetailed>> GetAllUsers(PageRequest request)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanRead);
            var users = await repository.GetAll(request, currentClient.AccessLevel);
            return users.Convert(u => new UserResponseDetailed(u));
        }

        public async Task<UserResponseDetailed> GetUser(string id)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanRead);

            var user = await repository.Get(id) ?? throw new BadRequestException("User not found");

            if (user.Id != currentClient.Id && user.AccessLevel >= currentClient.AccessLevel)
            {
                throw new BadRequestException("Permission denied. You cannot view a user with a higher access level than your own.");
            }
            return user;
        }

        public async Task<UserResponseDetailed> GetUserByEmail(string email)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanRead);

            var user = await repository.GetByEmail(email);
            if (user == null)
            {
                throw new BadRequestException("User not found");
            }
            return user;
        }

        public async Task<PageResponse<UserResponseDetailed>> Search(UsersSearchRequest request)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanRead);
            var users = await repository.Search(request, currentClient.AccessLevel);
            return users.Convert(u => new UserResponseDetailed(u));
        }

        public async Task<UserResponse> GetSelfShort()
        {
            var client = await authService.GetUser() ?? throw new BadRequestException("Permission denied");
            return client ?? throw new BadRequestException("User not found");
        }

        public async Task<UserResponseDetailed> GetSelf()
        {
            var client = await GetCurrentClient();
            return await repository.Get(client.Id) ?? throw new BadRequestException("User not found");
        }

        public async Task<TokenResponse> ActivateUser(ActivateUserRequest request)
        {
            if (request.OldPassword == request.NewPassword)
            {
                throw new BadRequestException("Passwords must be different.");
            }

            var user = await repository.GetByEmail(request.Email) ?? throw new NotFoundException("User not found");
            if (!user.NeedPasswordChange)
            {
                throw new BadRequestException("The user is already activated.");
            }

            if (!hashService.Verify(request.OldPassword, user.Password))
            {
                throw new UnauthorizedAccessException();
            }

            user.Password = hashService.Hash(request.NewPassword);
            user.NeedPasswordChange = false;

            user = await repository.Update(user);



            return await authService.Login(new LoginModelRequest()
            {
                Email = user.Email,
                Password = request.NewPassword
            });
        }

        public async Task<UserResponseDetailed> UpdateBaseInformation(string id, UpdateUserBaseInformationRequest request)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanUpdateBaseInformation);
            var user = await repository.Get(id) ?? throw new BadRequestException("User not found");

            if (user.AccessLevel >= currentClient.AccessLevel)
            {
                throw new BadRequestException("You cannot update a user with a higher access level than your own");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Nickname = request.Nickname;
            if (user.Email != request.Email && await repository.ExistsByEmail(request.Email))
            {
                throw new BadRequestException("User with this email already exists");
            }
            user.Email = request.Email;
            if (user.TaxPayerId != request.TaxPayerId && await repository.ExistsByTaxPayerId(request.TaxPayerId))
            {
                throw new BadRequestException("User with this tax payer id already exists");
            }
            user.TaxPayerId = request.TaxPayerId;

            user.Description = request.Description;

            user = await repository.Update(user);

            return user;
        }

        public async Task<UserResponse> UpdateSelfEmail(string email)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanUpdateSelfEmail);
            var user = await repository.Get(currentClient.Id) ?? throw new BadRequestException("User not found");

            var confirmation = await confirmationManager.Create<ChangeEmailConfirmationHandler>(email);

            await emailService.SendEmailUseTemplate(email, "ChangeEmail.html", new Dictionary<string, string>
            {
                { "baseHost", urlOptions.Web },
                { "name", user.FirstName },
                { "token", confirmation.Token },
                { "type", confirmation.ActionType }
            });

            return user;
        }

        public async Task<UserResponse> UpdatePermissions(string id, ClientPermissions permissions)
        {
            var currentClient = await GetCurrentClient();

            var client = await repository.Get(id) ?? throw new BadRequestException("Client not found");
            if (client.Type == ClientType.Robot)
            {
                throw new BadRequestException("You cannot change the permissions of a robot");
            }
            currentClient.Permissions.Has(p => p.Users.CanUpdatePermissions);
            if (currentClient.AccessLevel <= client.AccessLevel)
            {
                throw new BadRequestException("You cannot change the permissions of a client with a higher access level than your own");
            }

            client.Permissions.Update(permissions, currentClient.Permissions);
            return await repository.Update(client);
        }

        public async Task<UserResponse> UpdateLocationSecondsInterval(string id, int secondsInterval)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanUpdateScanRadius);
            var user = await repository.Get(id) ?? throw new BadRequestException("User not found");

            if (user.AccessLevel >= currentClient.AccessLevel)
            {
                throw new BadRequestException("You cannot update a user with a higher access level than your own");
            }

            user.UpdateLocationSecondsInterval = secondsInterval;

            user = await repository.Update(user);

            webSocketMessagePublisher.Publish(new UpdateUserInterval(user), new Dictionary<string, string>()
            {
                { "function", "updateUserUpdateLocationInterval" }
            });

            return await repository.Update(user);
        }
    }
}
