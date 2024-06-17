using Amazon.Runtime.Internal;
using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Infrastructure.Repository
{
    public class GroupRepository : BaseMongoDbRepository<GroupModel>, IGroupRepository
    {
        private readonly IDistributedCache cache;

        public GroupRepository(MongoDbOptions dbOption, IDistributedCache cache) : base(dbOption, dbOption.Collections.Groups)
        {
            this.cache = cache;
        }


        public override Task<GroupModel> Create(GroupModel entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            if(entity.ResponsibilityArea == null)
            {
                var linearRingCoordinates = new GeoJsonLinearRingCoordinates<GeoJson2DProjectedCoordinates>(positions: new[]
                {
                    new GeoJson2DProjectedCoordinates(0, 0),
                    new GeoJson2DProjectedCoordinates(0, 0.1),
                    new GeoJson2DProjectedCoordinates(0.1, 0.1),
                    new GeoJson2DProjectedCoordinates(0.1, 0),
                    new GeoJson2DProjectedCoordinates(0, 0)
                });
                var polygonCoordinates = new GeoJsonPolygonCoordinates<GeoJson2DProjectedCoordinates>(linearRingCoordinates);

                entity.ResponsibilityArea = new GeoJsonPolygon<GeoJson2DProjectedCoordinates>(polygonCoordinates);
            }
            return base.Create(entity);
        }

        public async Task<IList<GroupModel>> GetByMember(string userId)
        {
            var filter = Builders<GroupModel>.Filter.AnyEq(x => x.Members.Keys, userId);
            var result = await Collection.FindAsync(filter);
            return await result.ToListAsync();
        }

        public async Task<PageResponse<GroupModel>> Search(GroupSearchRequest request)
        {
            var filter = Builders<GroupModel>.Filter.Empty;
            if (!string.IsNullOrEmpty(request.Name))
            {
                filter &= Builders<GroupModel>.Filter.Regex(x => x.Name, new BsonRegularExpression(request.Name, "i"));
            }
            if (request.AccessLevel >= 0 && request.AccessLevel < 999)
            {
                filter &= Builders<GroupModel>.Filter.Eq(x => x.AccessLevel, request.AccessLevel);
            }

            var total = await Collection.CountDocumentsAsync(filter);
            var skip = (request.Page - 1) * request.Limit;
            var data = await Collection
                .Find(filter)
                .Skip(skip)
                .Limit(request.Limit)
                .ToListAsync();

            return new PageResponse<GroupModel>(data, total, request);
        }

        public async Task<bool> IsInsideOnGroupArea(UserModel client)
        {
            if(client.Location == null || client.Groups.Count == 0)
            {
                return false;
            }

            var filter = Builders<GroupModel>.Filter.In(x => x.Id, client.Groups) &
                Builders<GroupModel>.Filter.GeoIntersects(
                field: x => x.ResponsibilityArea,
                geometry: client.Location);

            return await Collection.CountDocumentsAsync(filter) > 0;
        }

        public async Task<bool> IsInsideOnGroupArea(UserModel client, string groupId)
        {
            if (client.Location == null || client.Groups.Count == 0)
            {
                return false;
            }

            var filter = Builders<GroupModel>.Filter.Eq(x => x.Id, groupId) &
                Builders<GroupModel>.Filter.GeoIntersects(
                field: x => x.ResponsibilityArea,
                geometry: client.Location);

            return await Collection.CountDocumentsAsync(filter) > 0;
        }
    }
}
