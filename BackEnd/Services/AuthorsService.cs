using BackEnd.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BackEnd.Services;

public class AuthorsService
{
    private readonly IMongoCollection<Author> _authorsCollection;

    public AuthorsService(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _authorsCollection = mongoDatabase.GetCollection<Author>(
            databaseSettings.Value.AuthorsCollectionName);
    }

    public async Task<List<Author>> GetAsync() =>
        await _authorsCollection.Find(_ => true).ToListAsync();

    public async Task<Author?> GetAsync(string id) =>
        await _authorsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Author?> GetByNameAsync(string name) =>
        await _authorsCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

    public async Task CreateAsync(Author newAuthor) =>
        await _authorsCollection.InsertOneAsync(newAuthor);

    public async Task UpdateAsync(string id, Author updatedAuthor) =>
        await _authorsCollection.ReplaceOneAsync(x => x.Id == id, updatedAuthor);

    public async Task RemoveAsync(string id) =>
        await _authorsCollection.DeleteOneAsync(x => x.Id == id);
}