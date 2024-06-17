using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Interface.Entity;

namespace Discerniy.Domain.Responses
{
    public class RobotResponse
    {
        public string Id { get; set; }
        public string Nickname { get; set; }
        public DateTime CreatedAt { get; set; }
        public ClientType Type => ClientType.Robot;
        public int UpdateLocationSecondsInterval { get; set; }
        public int ScanRadius { get; set; }
        public ClientStatus Status { get; set; } = ClientStatus.Inactive;
        public GeoCoordinates? Location { get; set; }
        public string GroupId { get; set; }
        public int AccessLevel { get; set; }

        public RobotResponse(RobotModel robot)
        {
            Id = robot.Id;
            Nickname = robot.Nickname;
            CreatedAt = robot.CreatedAt;
            UpdateLocationSecondsInterval = robot.UpdateLocationSecondsInterval;
            ScanRadius = robot.ScanRadius;
            Status = robot.Status;
            Location = robot.Location?.Coordinates != null ? new GeoCoordinates(robot.Location.Coordinates) : null;
            GroupId = robot.GroupId;
            AccessLevel = robot.AccessLevel;
        }

        public static implicit operator RobotResponse(RobotModel robot)
        {
            return new RobotResponse(robot);
        }
    }
}
