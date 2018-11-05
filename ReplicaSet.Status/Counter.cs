using System;
using MongoDB.Bson;

namespace ReplicaSet.Status
{
    public class Counter
    {
        public ObjectId Id { get; set; }
        public int Count { get; set; }
        public DateTime DateTime { get; set; }
    }
}
