using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace BackEnd.Models;

public class Subscribe
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    [JsonIgnore]
    [BsonElement("Books")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> BooksId { get; set; } = null!;

    [BsonIgnore]
    public List<Book> Books { get; set; } = null!;

    public string ImageLink { get; set; } = null!;

    public decimal? Price { get; set; } = null!;
}