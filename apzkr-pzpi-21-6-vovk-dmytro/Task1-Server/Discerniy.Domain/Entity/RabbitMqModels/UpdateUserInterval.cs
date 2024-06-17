using Discerniy.Domain.Entity.DomainEntity;

namespace Discerniy.Domain.Entity.RabbitMqModels
{
    public class UpdateUserInterval
    {
        public string UserId { get; set; } = string.Empty;
        public int LocationSecondsInterval { get; set; }

        public UpdateUserInterval(string userId, int locationSecondsInterval)
        {
            UserId = userId;
            LocationSecondsInterval = LocationSecondsInterval;
        }

        public UpdateUserInterval(UserModel user)
        {
            UserId = user.Id;
            LocationSecondsInterval = user.UpdateLocationSecondsInterval;
        }

        public UpdateUserInterval()
        {
        }
    }
}
