namespace Discerniy.Domain.Entity.Options
{
    public class MongoDbOptions
    {
        public string ConnectionString { get; set; } = default!;
        public string Database { get; set; } = default!;

        public MongoDbCollectionOption Collections { get; } = new MongoDbCollectionOption();
    }
}
