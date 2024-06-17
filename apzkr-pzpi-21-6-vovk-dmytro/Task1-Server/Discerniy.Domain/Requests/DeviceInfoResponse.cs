using Discerniy.Domain.Entity.DomainEntity;

namespace Discerniy.Domain.Requests
{
    public class DeviceInfoResponse
    {
        public long UpdateInterval { get; set; }

        public DeviceInfoResponse() { }

        public DeviceInfoResponse(UserModel user)
        {
            UpdateInterval = user.UpdateLocationSecondsInterval;
        }

        public static implicit operator DeviceInfoResponse(UserModel user) 
        { 
            return new DeviceInfoResponse(user);
        }
    }
}
