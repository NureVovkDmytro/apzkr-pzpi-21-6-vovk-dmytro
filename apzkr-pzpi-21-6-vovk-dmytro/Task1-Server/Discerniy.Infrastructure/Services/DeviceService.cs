using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;

namespace Discerniy.Infrastructure.Services
{
    public class DeviceService : IDeviceService
    {
        protected readonly IAuthService authService;

        public DeviceService(IAuthService authService)
        {
            this.authService = authService;
        }

        public async Task<DeviceInfoResponse> GetDeviceInfo()
        {
            var client = await authService.GetUserByDevice();
            return client;
        }
    }
}
