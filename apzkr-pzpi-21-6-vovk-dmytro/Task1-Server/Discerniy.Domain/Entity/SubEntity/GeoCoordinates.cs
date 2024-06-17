using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Domain.Entity.SubEntity
{
    public class GeoCoordinates
    {
        public double Easting { get; set; }
        public double Northing { get; set; }

        public GeoCoordinates(double easting, double northing)
        {
            Easting = easting;
            Northing = northing;
        }

        public GeoCoordinates(GeoJson2DProjectedCoordinates geoJson)
        {
            Easting = geoJson.Easting;
            Northing = geoJson.Northing;
        }


        public GeoCoordinates()
        {
        }

        public static implicit operator GeoCoordinates(GeoJson2DProjectedCoordinates coordinates)
        {
            return new GeoCoordinates(coordinates);
        }

        public static implicit operator GeoJson2DProjectedCoordinates(GeoCoordinates coordinates)
        {
            return new GeoJson2DProjectedCoordinates(coordinates.Easting, coordinates.Northing);
        }
    }
}
