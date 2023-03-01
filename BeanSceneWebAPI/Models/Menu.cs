using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanSceneWebAPI.Models
{
    public class Menu
    {
        [BsonId, BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public string dietaryFlag { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string category { get; set; }
        public bool availability { get; set; }

    }
}