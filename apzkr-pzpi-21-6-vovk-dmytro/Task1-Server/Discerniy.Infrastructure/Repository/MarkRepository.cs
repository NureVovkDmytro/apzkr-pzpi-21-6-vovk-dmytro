using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Interface.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Discerniy.Infrastructure.Repository
{
    public class MarkRepository : BaseMongoDbRepository<MarkModel>, IMarkRepository
    {
        public MarkRepository(MongoDbOptions dbOption) : base(dbOption, dbOption.Collections.Marks)
        {
        }

        public override Task<MarkModel> Create(MarkModel entity)
        {
            entity.Id = $"M{ObjectId.GenerateNewId()}";
            entity.CreatedAt = DateTime.UtcNow;
            return base.Create(entity); 
        }

        public async Task<IEnumerable<MarkModel>> GetFromGroup(string groupId)
        {
            var filter = Builders<MarkModel>.Filter.Eq(m => m.GroupId, groupId);
            return await Collection
                .Find(filter)
                .ToListAsync();
        }
    }
}
