using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.Models.Token
{
    public class RefreshToken
    {
        public string Token { get; set; } = null!;

        public DateTime? Expires { get; set; } = null!;

        [BsonIgnore]
        public bool IsValid => DateTime.UtcNow < Expires;
    }
}
