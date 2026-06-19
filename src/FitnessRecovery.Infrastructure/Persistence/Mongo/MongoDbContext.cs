using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace FitnessRecovery.Infrastructure.Persistence.Mongo;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    protected MongoDbContext() { _database = null!; }

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoConnection") ?? "mongodb://localhost:27017";
        var databaseName = configuration.GetConnectionString("MongoDatabase") ?? "fitness_recovery";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public virtual IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}
