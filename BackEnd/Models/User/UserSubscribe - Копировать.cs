using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BackEnd.Models.User
{
    public class UserSubscribe : Subscribe
    {
        public DateTime? Expires { get; set; } = null!;
    }
}
