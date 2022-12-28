using BackEnd.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BackEnd.Services;

public class SubscribesService
{
    private readonly IMongoCollection<Subscribe> _subscribesCollection;

    public SubscribesService(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _subscribesCollection = mongoDatabase.GetCollection<Subscribe>(
            databaseSettings.Value.SubscribesCollectionName);
    }

    public async Task<List<Subscribe>> GetAsync() =>
        await _subscribesCollection.Find(_ => true).ToListAsync();

    public async Task<Subscribe?> GetAsync(string id) =>
        await _subscribesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Subscribe?> GetByNameAsync(string name) =>
        await _subscribesCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

    public async Task CreateAsync(Subscribe newSubscribe) =>
        await _subscribesCollection.InsertOneAsync(newSubscribe);

    public async Task UpdateAsync(string id, Subscribe updatedSubscribe) =>
        await _subscribesCollection.ReplaceOneAsync(x => x.Id == id, updatedSubscribe);

    public async Task RemoveAsync(string id) =>
        await _subscribesCollection.DeleteOneAsync(x => x.Id == id);
}