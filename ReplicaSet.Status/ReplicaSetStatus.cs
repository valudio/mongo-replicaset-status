using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReplicaSet.Status
{
    [BsonIgnoreExtraElements]
    public class ReplicaSetStatus
    {
        [BsonElement("members")]
        public IEnumerable<Member> Members { get; set; }
    }
}
