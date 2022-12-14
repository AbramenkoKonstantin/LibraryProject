using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace BackEnd.Models;

public class Genre
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    //[BsonElement("Name")]
    //[JsonPropertyName("Name")]
    public string Name { get; set; } = null!;
}