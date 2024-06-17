using Discerniy.Domain.Entity.DomainEntity;

namespace Discerniy.Domain.Responses
{
    public class RobotTokenResponse
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Token => $"{Id}:{Key}";

        public RobotTokenResponse(RobotModel model)
        {
            Id = model.Id;
            Key = model.Key;
        }

        public static implicit operator RobotTokenResponse(RobotModel model)
        {
            return new RobotTokenResponse(model);
        }
    }
}
