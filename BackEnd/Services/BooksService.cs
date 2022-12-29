using BackEnd.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BackEnd.Services;

public class BooksService
{
    private readonly IMongoCollection<Book> _booksCollection;

    public BooksService(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _booksCollection = mongoDatabase.GetCollection<Book>(
            databaseSettings.Value.BooksCollectionName);
    }

    public async Task<List<Book>> GetAsync() =>
        await _booksCollection.Find(_ => true).ToListAsync();

    public async Task<List<Book>> GetByAuthorAsync(string id) =>
        await _booksCollection.Find(book => book.AuthorsId.Contains(id)).ToListAsync();

    public async Task<List<Book>> GetByNameAsync(string name) =>
        await _booksCollection.Find(book => book.Names.Any(bName => bName.Contains(name))).ToListAsync();

    public async Task<List<Book>> GetByISBNAsync(string isbn) =>
        await _booksCollection.Find(book => book.Names.Any(bName => bName.Contains(isbn))).ToListAsync();

    public async Task<List<Book>> GetByGenreAsync(string genre) =>
        await _booksCollection.Find(book => book.GenresId.Contains(genre)).ToListAsync();

    public async Task<List<Book>> GetByYearAsync(int year) =>
        await _booksCollection.Find(book => book.FistPublished == year).ToListAsync();

    public async Task<List<Book>> GetByLanguageAsync(string language) =>
        await _booksCollection.Find(book => book.OriginalLang == language).ToListAsync();

    public async Task<Book?> GetAsync(string id) =>
        await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Book> GetByISBNAsync(string isbn) =>
        await _booksCollection.Find(book => book.ISBN == isbn).FirstOrDefaultAsync();

    public async Task CreateAsync(Book newBook) =>
        await _booksCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, Book updatedBook) =>
        await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _booksCollection.DeleteOneAsync(x => x.Id == id);
}