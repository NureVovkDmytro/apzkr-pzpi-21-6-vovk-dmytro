using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using MongoDB.Driver;

namespace Discerniy.Infrastructure.Repository
{
    public abstract class BaseMongoDbRepository<CollectionT> : IRepository<CollectionT> where CollectionT : class, IIdentifier
    {
        protected readonly IMongoDatabase Db;
        protected readonly MongoClient Client;
        protected string CollectionName { get; }
        protected IMongoCollection<CollectionT> Collection => Db.GetCollection<CollectionT>(CollectionName);

        public BaseMongoDbRepository(MongoDbOptions dbOption, string collectionName)
        {
            Client = new MongoClient(dbOption.ConnectionString);
            Db = Client.GetDatabase(dbOption.Database);
            CollectionName = collectionName;
        }

        public async virtual Task<long> GetCountAsync()
        {
            return await Collection.CountDocumentsAsync(Builders<CollectionT>.Filter.Empty);
        }

        public async virtual Task<CollectionT> Create(CollectionT entity)
        {
            return await Collection.InsertOneAsync(entity).ContinueWith(t => entity);
        }

        public async virtual Task<CollectionT?> Get(string id)
        {
            return await Collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async virtual Task<PageResponse<CollectionT>> GetAll(PageRequest request)
        {
            var filter = Builders<CollectionT>.Filter.Empty;
            var totalItems = await Collection.CountDocumentsAsync(filter);
            int skip = (request.Page - 1) * request.Limit;
            var result = await Collection
                .Find(filter)
                .Skip(skip)
                .Limit(request.Limit)
                .ToListAsync();
            var totalPage = (int)Math.Ceiling((double)totalItems / request.Limit);
            return new PageResponse<CollectionT>(result, totalPage, request);
        }

        public async virtual Task<CollectionT> Update(CollectionT entity)
        {
            return await Collection.ReplaceOneAsync(e => e.Id == entity.Id, entity).ContinueWith(t => entity);
        }

        public async virtual Task<CollectionT?> Delete(string id)
        {
            return await Collection.FindOneAndDeleteAsync(e => e.Id == id);
        }

        public async virtual Task<bool> Exists(string id)
        {
            return await Collection.Find(e => e.Id == id).AnyAsync();
        }
    }
}
