using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class UpdateRobotRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        public string Nickname { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public string GroupId { get; set; } = null!;
    }
}
