using BeanSceneWebAPI.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
//using System.Web.Mvc;

namespace BeanSceneWebAPI.Controllers
{
    /// <summary>
    /// Controller for all order methods
    /// </summary>
    [BasicAuthentication]
    public class OrderController : ApiController
    {
        MongoClient client;
        string databaseName;
        string connectionStr;
        /// <summary>
        /// Constructor
        /// </summary>
        public OrderController()
        {
            connectionStr = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;
            client = new MongoClient(connectionStr);
            databaseName = MongoUrl.Create(connectionStr).DatabaseName;
        }
        /// <summary>
        /// Returns all current orders 
        /// </summary>
        /// <returns>All current orders in JSON and/or HTTP status</returns>
        // GET: api/Order
        public HttpResponseMessage Get()
        {
            try
            {
                string today = DateTime.Today.ToShortDateString();
                var collection = client.GetDatabase(databaseName).GetCollection<Order>("order")
                                    .AsQueryable()
                                    .Where(o => o.date.Contains(today))
                                    .Where(o => o.is_complete == false);
                var jsonResult = JsonConvert.SerializeObject(collection);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception)
            {

                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
            
        }
        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="order">THe order object to be saved to the db</param>
        /// <returns>The HTTP status code</returns>
        // POST: api/Order/Create
        [Route("api/Order/Create")]
        public HttpResponseMessage Create([FromBody] Order order)
        {
            var filter = Builders<Order>.Filter.Empty;
            var collection = client.GetDatabase(databaseName).GetCollection<Order>("order");
            var projection = Builders<Order>.Projection.Include("order_id").Exclude("_id");
            var result = collection.Find(filter).Project(projection).ToList();
            int lastOrderId = result[result.Count - 1].GetValue("order_id").ToInt32();
            order.order_id = lastOrderId + 1;
            order.date = DateTime.Now.ToString(); 
            client.GetDatabase(databaseName).GetCollection<Order>("order").InsertOne(order);

            var response = Request.CreateResponse(HttpStatusCode.Created);
            var jObject = new JObject();
            response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
            return response;
        }
        /// <summary>
        /// Completes an order
        /// </summary>
        /// <param name="id">The id of the order to be completed</param>
        /// <returns>HTTP status</returns>
        // PUT: api/Order/Complete/id
        [Route("api/Order/{status}/{id}")]
        public HttpResponseMessage Put(string id)
        {
            var filter = Builders<Order>.Filter.Eq("_id", id);
            var update = Builders<Order>.Update.Set("is_complete", true);
            client.GetDatabase(databaseName).GetCollection<Order>("order").UpdateOne(filter, update);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            var jObject = new JObject();
            response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
            return response;
        }

    }
}