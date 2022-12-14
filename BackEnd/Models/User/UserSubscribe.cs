using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BackEnd.Models.User
{
    public class DBUserSubscribe
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? SubscribeId { get; set; }

        public DateTime? Expires { get; set; } = null!;
    }
}
