using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class LoginModelRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
