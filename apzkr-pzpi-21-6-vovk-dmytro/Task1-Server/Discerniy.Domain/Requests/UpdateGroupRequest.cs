using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class UpdateGroupRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = null!;
        [StringLength(500)]
        public string? Description { get; set; }
        [Range(0, 998)]
        [Required]
        public int AccessLevel { get; set; }
    }
}
