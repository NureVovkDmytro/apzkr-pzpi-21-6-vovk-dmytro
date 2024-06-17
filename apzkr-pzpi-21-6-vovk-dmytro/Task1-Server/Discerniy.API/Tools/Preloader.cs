using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;

namespace Discerniy.API.Tools
{
    public static class Preloader
    {
        private static object mutex = new object();
        private static bool isPreloaded = false;

        public static void Preload(IServiceProvider serviceProvider)
        {
            if (isPreloaded)
            {
                return;
            }
            lock (mutex)
            {
                CreateFirstUser(serviceProvider);
                isPreloaded = true;
            }
        }

        public static async void CreateFirstUser(IServiceProvider serviceProvider)
        {
            var userRepository = serviceProvider.GetService<IUserRepository>() ?? throw new ArgumentNullException("User repository is null");
            IHashService hashService = serviceProvider.GetRequiredService<IHashService>() ?? throw new ArgumentNullException("Hash service is null");

            if(await userRepository.GetCountAsync() > 0)
            {
                return;
            }

            var user = new UserModel
            {
                FirstName = "Admin",
                LastName = "Admin",
                Email = "email@example.com",
                Password = hashService.Hash("password"),
                Permissions = ClientPermissions.Admin,
                Status = ClientStatus.Active,
                NeedPasswordChange = false,
                AccessLevel = 999
            };
            await userRepository.Create(user);
        }
    }
}
