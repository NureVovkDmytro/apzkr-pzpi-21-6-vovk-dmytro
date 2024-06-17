using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Interface.Entity;
using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class CreateUserRequest
    {
        [Required]
        [MaxLength(64)]
        public string TaxPayerId { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [MaxLength(64)]
        public string FirstName { get; set; } = null!;
        [Required]
        [MaxLength(64)]
        public string LastName { get; set; } = null!;
        [MaxLength(64)]
        public string Nickname { get; set; } = null!;
        [Required]
        [Range(0, 10_000)]
        public int ScanRadius { get; set; }
        [Required]
        [Range(0, 999)]
        public int AccessLevel { get; set; }
        public ClientStatus Status { get; set; } = ClientStatus.Inactive;
        public string? Description { get; set; }
        public ClientPermissions Permissions { get; set; } = new ClientPermissions();
    }
}
