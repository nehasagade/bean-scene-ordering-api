using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanSceneWebAPI.Models
{
    public class OrderItem
    {
        [BsonElement("menu")]
        public Menu menu { get; set; }
        public int qty { get; set; }
        public string note { get; set; }
    }
}