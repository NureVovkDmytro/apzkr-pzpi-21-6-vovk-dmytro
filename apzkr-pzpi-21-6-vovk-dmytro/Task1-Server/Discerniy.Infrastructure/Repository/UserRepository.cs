using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Exceptions;
using Discerniy.Domain.Interface.Entity;
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
    public class UserRepository : BaseMongoDbRepository<UserModel>, IUserRepository
    {
        protected readonly JwtOption jwtOption;
        protected readonly AuthServiceOption authServiceOption;
        protected readonly IRandomGenerator randomGenerator;
        public UserRepository(MongoDbOptions dbOption, JwtOption jwtOption, AuthServiceOption authServiceOption, IRandomGenerator randomGenerator) : base(dbOption, dbOption.Collections.Users)
        {
            this.jwtOption = jwtOption;
            this.authServiceOption = authServiceOption;
            this.randomGenerator = randomGenerator;

            var indexKeysDefinition = Builders<UserModel>.IndexKeys.Geo2DSphere(x => x.Location);
            var indexModel = new CreateIndexModel<UserModel>(indexKeysDefinition);
            Collection.Indexes.CreateOne(indexModel);
        }

        public override async Task<UserModel> Create(UserModel user)
        {
            user.Id = $"U{ObjectId.GenerateNewId()}";
            user.CreatedAt = DateTime.Now;
            user.LastOnline = null;
            user.SecretKey = randomGenerator.GenerateString(authServiceOption.SecretKeyCharacterCount);
            user.Location = new GeoJsonPoint<GeoJson2DProjectedCoordinates>(new GeoJson2DProjectedCoordinates(0, 0));
            await Collection.InsertOneAsync(user);
            return user;
        }

        public async Task<ClientSession> CreateSession(string userId)
        {
            if (!await Exists(userId))
            {
                throw new NotFoundException("User not found");
            }

            var session = new ClientSession
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(jwtOption.UserExpiresInMinutes),
                LastAccessed = DateTime.UtcNow
            };

            var filter = Builders<UserModel>.Filter.Eq(x => x.Id, userId);
            var update = Builders<UserModel>.Update.Push(x => x.Sessions, session);
            await Collection.UpdateOneAsync(filter, update);
            return session;
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Email, email);
            return await Collection.Find(filter).AnyAsync();
        }

        public async Task<bool> ExistsByTaxPayerId(string taxId)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.TaxPayerId, taxId);
            return await Collection.Find(filter).AnyAsync();
        }

        public async Task<PageResponse<UserModel>> GetAll(PageRequest request, int maxAccessLevel)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Type, ClientType.User) & Builders<UserModel>.Filter.Lt(x => x.AccessLevel, maxAccessLevel);
            var total = await Collection.CountDocumentsAsync(filter);
            int skip = (request.Page - 1) * request.Limit;
            var result = await Collection
                .Find(filter)
                .Skip(skip)
                .Limit(request.Limit)
                .ToListAsync();
            return new PageResponse<UserModel>(result, total, request);
        }

        public async Task<UserModel?> GetByEmail(string email)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Email, email);
            var result = await Collection.FindAsync(filter);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<IList<LocationResponse>> GetNearUsers(GeoJson2DProjectedCoordinates coordinates, double radius)
        {
            var filter = Builders<UserModel>.Filter.NearGeo(x => x.Location, coordinates.Easting, coordinates.Northing, maxDistance: radius, minDistance: 0);
            var result = await Collection
                .Find(filter)
                .ToListAsync();
            return result.Select(x => new LocationResponse(x)).ToList();
        }

        public async Task<UserModel?> RemoveSession(string userId, ClientSession session)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Id, userId);
            var update = Builders<UserModel>.Update.PullFilter(x => x.Sessions, Builders<ClientSession>.Filter.Eq(x => x.Id, session.Id));
            await Collection.UpdateOneAsync(filter, update);
            return await Get(userId);
        }

        public Task<PageResponse<UserModel>> Search(ClientSearchRequest request, int accessLevel)
        {
            UsersSearchRequest usersSearchRequest = new UsersSearchRequest
            {
                GroupId = request.GroupId,
                Status = request.Status,
                Page = request.Page,
                Limit = request.Limit
            };

            return Search(usersSearchRequest, accessLevel);
        }

        public async Task<PageResponse<UserModel>> Search(UsersSearchRequest request, int accessLevel)
        {
            var filter = Builders<UserModel>.Filter.Lt(x => x.AccessLevel, accessLevel);
            if (request.GroupId != null)
            {
                filter = filter & Builders<UserModel>.Filter.AnyEq(x => x.Groups, request.GroupId);
            }
            if (request.Status != null)
            {
                filter = filter & Builders<UserModel>.Filter.Eq(x => x.Status, request.Status);
            }
            if (request.FirstName != null)
            {
                filter = filter & Builders<UserModel>.Filter.Regex(x => x.FirstName, new BsonRegularExpression(request.FirstName, "i"));
            }
            if (request.LastName != null)
            {
                filter = filter & Builders<UserModel>.Filter.Regex(x => x.LastName, new BsonRegularExpression(request.LastName, "i"));
            }
            if (request.Email != null)
            {
                filter = filter & Builders<UserModel>.Filter.Regex(x => x.Email, new BsonRegularExpression(request.Email, "i"));
            }
            if (request.TaxPayerId != null)
            {
                filter = filter & Builders<UserModel>.Filter.Regex(x => x.TaxPayerId, new BsonRegularExpression(request.TaxPayerId, "i"));
            }

            var total = await Collection.CountDocumentsAsync(filter);
            int skip = (request.Page - 1) * request.Limit;
            var result = await Collection
                .Find(filter)
                .Skip(skip)
                .Limit(request.Limit)
                .ToListAsync();
            return new PageResponse<UserModel>(result, total, request);
        }


        public async Task<UserModel?> UpdateSession(string userId, ClientSession session)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Id, userId) & Builders<UserModel>.Filter.ElemMatch(x => x.Sessions, x => x.Id == session.Id);
            var update = Builders<UserModel>.Update
                .Set(x => x.Sessions[-1].LastAccessed, session.LastAccessed)
                .Set(x => x.Sessions[-1].ExpiresAt, session.ExpiresAt);
            await Collection.UpdateOneAsync(filter, update);
            return await Get(userId);
        }
    }
}
