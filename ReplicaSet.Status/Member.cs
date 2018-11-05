using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReplicaSet.Status
{
    [BsonIgnoreExtraElements]
    public class Member
    {
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("stateStr")]
        public string StateStr { get; set; }
    }
}
