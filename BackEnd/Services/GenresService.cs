using BackEnd.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BackEnd.Services;

public class GenresService
{
    private readonly IMongoCollection<Genre> _genresCollection;

    public GenresService(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _genresCollection = mongoDatabase.GetCollection<Genre>(
            databaseSettings.Value.GenresCollectionName);
    }

    public async Task<List<Genre>> GetAsync() =>
        await _genresCollection.Find(_ => true).ToListAsync();

    public async Task<Genre?> GetAsync(string id) =>
        await _genresCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Genre?> GetByNameAsync(string Name) =>
        await _genresCollection.Find(x => x.Name == Name).FirstOrDefaultAsync();

    public async Task CreateAsync(Genre newGenre) =>
        await _genresCollection.InsertOneAsync(newGenre);

    public async Task UpdateAsync(string id, Genre updatedGenre) =>
        await _genresCollection.ReplaceOneAsync(x => x.Id == id, updatedGenre);

    public async Task RemoveAsync(string id) =>
        await _genresCollection.DeleteOneAsync(x => x.Id == id);
}