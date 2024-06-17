using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class UpdateUserBaseInformationRequest
    {
        [Required]
        [MaxLength(64)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(64)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [MaxLength(64)]
        public string Nickname { get; set; } = string.Empty;
        [Required]
        [MaxLength(64)]
        public string TaxPayerId { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [MaxLength(256)]
        public string? Description { get; set; }
    }
}
