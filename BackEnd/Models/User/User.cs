using BackEnd.Models.Token;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace BackEnd.Models.User;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [JsonIgnore]
    [BsonElement("Subscribes")]
    public List<DBUserSubscribe> SubscribesId { get; set; } = null!;

    [BsonIgnore]
    public List<UserSubscribe> Subscribes { get; set; } = null!;

    public string Email { get; set; } = null!;

    [BsonRepresentation(BsonType.Binary)]
    public byte[] PasswordHash { get; set; } = null!;

    [BsonRepresentation(BsonType.Binary)]
    public byte[] PasswordSalt { get; set; } = null!;

    [BsonIgnore]
    public string AccessToken { get; set; } = null!;

    [JsonIgnore]
    public RefreshToken RefreshToken { get; set; } = null!;

    public string IconLink { get; set; } = null!;

    public decimal? Vallet { get; set; } = null!;
}