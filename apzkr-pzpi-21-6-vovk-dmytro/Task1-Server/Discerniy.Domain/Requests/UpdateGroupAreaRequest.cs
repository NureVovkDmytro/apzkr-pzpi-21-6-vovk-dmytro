using MongoDB.Driver.GeoJsonObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class UpdateGroupAreaRequest
    {
        [Required]
        [MinLength(3)]
        public List<GeoJson2DProjectedCoordinates> Coordinates { get; set; } = new List<GeoJson2DProjectedCoordinates>();
    }
}
