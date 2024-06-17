using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class ActivateUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [MaxLength(128)]
        public string OldPassword { get; set; } = null!;
        [Required]
        [MaxLength(128)]
        public string NewPassword { get; set; } = null!;
    }
}
