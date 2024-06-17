using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;

namespace Discerniy.Domain.Interface.Services
{
    public interface IDeviceService
    {
        Task<DeviceInfoResponse> GetDeviceInfo();
    }
}
