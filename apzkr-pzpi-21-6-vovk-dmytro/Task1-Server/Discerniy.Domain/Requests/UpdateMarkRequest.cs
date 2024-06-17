using Discerniy.Domain.Entity.SubEntity;
using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class UpdateMarkRequest
    {
        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = null!;
        [MaxLength(512)]
        public string Description { get; set; } = null!;
        [Required]
        public GeoCoordinates Location { get; set; } = null!;
        [Required]
        public double Radius { get; set; } = 0;
        [MaxLength(16)]
        public string? Icon { get; set; }
    }
}
