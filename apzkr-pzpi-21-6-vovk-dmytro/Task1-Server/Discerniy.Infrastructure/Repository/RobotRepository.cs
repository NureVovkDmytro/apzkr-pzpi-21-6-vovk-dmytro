using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using Discerniy.Infrastructure.Extensions.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Infrastructure.Repository
{
    public class RobotRepository : BaseMongoDbRepository<RobotModel>, IRobotRepository
    {
        private readonly IRandomGenerator randomGenerator;
        private readonly RobotOptions robotOptions;
        public RobotRepository(MongoDbOptions dbOption, RobotOptions robotOptions, IRandomGenerator randomGenerator) : base(dbOption, dbOption.Collections.Robots)
        {
            this.robotOptions = robotOptions;

            var indexKeysDefinition = Builders<RobotModel>.IndexKeys.Geo2DSphere(x => x.Location);
            var indexModel = new CreateIndexModel<RobotModel>(indexKeysDefinition);
            Collection.Indexes.CreateOne(indexModel);
            this.randomGenerator = randomGenerator;
        }

        public override async Task<RobotModel> Create(RobotModel robot)
        {
            robot.Id = $"R{ObjectId.GenerateNewId()}";
            robot.CreatedAt = DateTime.UtcNow;
            robot.LastOnline = null;
            robot.Key = randomGenerator.GenerateString(robotOptions.TokenLength);
            robot.Location = new GeoJsonPoint<GeoJson2DProjectedCoordinates>(new GeoJson2DProjectedCoordinates(0, 0));
            return await Collection.InsertOneAsync(robot).ContinueWith(t => robot);
        }

        public async Task<IList<LocationResponse>> GetNearRobots(GeoJson2DProjectedCoordinates coordinates, double radius)
        {
            var filter = Builders<RobotModel>.Filter.NearGeo(x => x.Location, coordinates.Easting, coordinates.Northing, maxDistance: radius);
            var result = await Collection
                .Find(filter)
                .ToListAsync();
            return result.Select(x => new LocationResponse(x)).ToList();
        }

        public Task<PageResponse<RobotModel>> Search(ClientSearchRequest request, int accessLevel)
        {
            RobotsSearchRequest robotsSearchRequest = new RobotsSearchRequest
            {
                Page = request.Page,
                Limit = request.Limit,
                Status = request.Status,
                GroupId = request.GroupId,
                Nickname = request.Nickname
            };
            return Search(robotsSearchRequest, accessLevel);
        }

        public async Task<PageResponse<RobotModel>> Search(RobotsSearchRequest request, int accessLevel)
        {
            var filter = Builders<RobotModel>.Filter.Lt(x => x.AccessLevel, accessLevel);
            if (request.Status != null)
            {
                filter = filter & Builders<RobotModel>.Filter.Eq(x => x.Status, request.Status);
            }
            if (request.GroupId != null)
            {
                filter = filter & Builders<RobotModel>.Filter.Eq(x => x.GroupId, request.GroupId);
            }
            if (request.Nickname != null)
            {
                filter = filter & Builders<RobotModel>.Filter.Regex(x => x.Nickname, new BsonRegularExpression(request.Nickname, "i"));
            }

            var total = await Collection.CountDocumentsAsync(filter);
            int skip = (request.Page - 1) * request.Limit;
            var result = await Collection
                .Find(filter)
                .Skip(skip)
                .Limit(request.Limit)
                .ToListAsync();
            return new PageResponse<RobotModel>(result, total, request);
        }
    }
}
