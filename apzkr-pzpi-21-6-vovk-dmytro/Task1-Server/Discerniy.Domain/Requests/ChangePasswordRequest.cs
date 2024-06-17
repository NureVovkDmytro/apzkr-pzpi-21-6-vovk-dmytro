using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        [MinLength(8)]
        [MaxLength(129)]
        public string OldPassword { get; set; } = null!;
        [Required]
        [MinLength(8)]
        [MaxLength(129)]
        public string NewPassword { get; set; } = null!;
    }
}
