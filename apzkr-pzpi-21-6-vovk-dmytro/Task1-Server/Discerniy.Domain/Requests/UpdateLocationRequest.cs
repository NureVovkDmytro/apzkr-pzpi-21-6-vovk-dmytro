using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Domain.Requests
{
    public class UpdateLocationRequest
    {
        public double Compass { get; set; }
        public double Easting { get; set; }
        public double Northing { get; set; }
    }
}
