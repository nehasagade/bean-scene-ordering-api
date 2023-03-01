using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanSceneWebAPI.Models
{
    public class Order
    {
        [BsonId, BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; }
        public int order_id { get; set; }
        public string table_id { get; set; }
        public OrderItem[] order_items { get; set; }
        public bool is_complete { get; set; }
        public string date { get; set; }
    }
}