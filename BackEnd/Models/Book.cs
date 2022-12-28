using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace BackEnd.Models;

public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    //[BsonElement("Names")]
    //[JsonPropertyName("Names")]
    public List<string> Names { get; set; } = null!;

    [JsonIgnore]
    [BsonElement("Authors")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> AuthorsId { get; set; } = null!;

    [BsonIgnore]
    public List<Author> Authors { get; set; } = null!;

    [JsonIgnore]
    [BsonElement("Genres")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> GenresId { get; set; } = null!;

    [BsonIgnore]
    public List<Genre> Genres { get; set; } = null!;

    public string OriginalLang { get; set; } = null!;

    public int? FistPublished { get; set; } = null!;

    public string ISBN { get; set; } = null!;

    public string ImageLink { get; set; } = null!;

    public string Content { get; set; } = null!;
}