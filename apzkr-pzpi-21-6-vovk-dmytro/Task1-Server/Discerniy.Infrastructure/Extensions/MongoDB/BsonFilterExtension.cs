using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Discerniy.Infrastructure.Extensions.MongoDB
{
    public static class BsonFilterExtension
    {
        public static FilterDefinition<T> NearGeo<T>(this FilterDefinitionBuilder<T> builder, Expression<Func<T, object>> field, double x, double y, double? maxDistance = null, double? minDistance = null)
        {
            var value = new BsonDocument("$nearSphere", new BsonDocument
                    {
                        { "$geometry", new BsonDocument
                        {
                            { "type", "Point" },
                            { "coordinates", new BsonArray
                            {
                                x,
                                y
                            } }
                        } },
                        { "$maxDistance", maxDistance ?? 0 },
                        { "$minDistance", minDistance ?? 0 }
                    });
            string fieldName = field.Body.ToString().Split('.')[1];
            return new BsonDocument(fieldName, value);
        }
    }
}
