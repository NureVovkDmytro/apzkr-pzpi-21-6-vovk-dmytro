using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Interface.Entity;
using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class CreateRobotRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        public string Nickname { get; set; } = null!;
        public string? Description { get; set; } = null;
        public ClientStatus Status { get; set; } = ClientStatus.Inactive;
        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        public string GroupId { get; set; } = null!;
        [Required]
        [Range(0, 10_000)]
        public int ScanRadius { get; set; }
        [Required]
        [Range(0, 3600)]
        public int UpdateLocationSecondsInterval { get; set; } = 10;
        [Required]
        [Range(0, 999)]
        public int AccessLevel { get; set; }
    }
}
