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
using System.Web;
using System.Web.Http;

namespace BeanSceneWebAPI.Controllers
{
    /// <summary>
    /// Controller methods for Categories
    /// </summary>
    public class CategoryController : ApiController
    {
        MongoClient client;
        string databaseName;
        string connectionStr;

        /// <summary>
        /// Constructor for Category class
        /// </summary>
        public CategoryController()
        {
            connectionStr = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;
            client = new MongoClient(connectionStr);
            databaseName = MongoUrl.Create(connectionStr).DatabaseName;
        }
        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <returns>List of all Categories</returns>
        public HttpResponseMessage Get()
        {
            try
            {

                var collection = client.GetDatabase(databaseName).GetCollection<Category>("category").AsQueryable();
                string jsonResult = JsonConvert.SerializeObject(collection);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception)
            {

                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
        }
        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="category">The category object to be created</param>
        /// <returns>HTTP Status code</returns>
        [Route("api/Category/Create")]
        public HttpResponseMessage Post([FromBody] Category category)
        {
            try
            {
                client.GetDatabase(databaseName).GetCollection<Category>("category").InsertOne(category);
                var response = Request.CreateResponse(HttpStatusCode.Created);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception)
            {

                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
        }


        /// <summary>
        /// Edit a category
        /// </summary>
        /// <param name="id">The _id of the category</param>
        /// <param name="category">the updated category object</param>
        /// <returns>HTTP Status code</returns>
        [Route("api/Category/Edit/{id}")]
        public HttpResponseMessage Put(string id, [FromBody]Category category)
        {
            try
            {
                var filter = Builders<Category>.Filter.Eq("_id", id);
                var update = Builders<Category>.Update.Set("name", category.name);

                client.GetDatabase(databaseName).GetCollection<Category>("category").UpdateOne(filter, update);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception)
            {

                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
        }
        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">The id of the category</param>
        /// <returns>HTTP status code</returns>
        [Route("api/Category/Delete/{id}")]
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                var filter = Builders<Category>.Filter.Eq("_id", id);

                client.GetDatabase(databaseName).GetCollection<Category>("category").DeleteOne(filter);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception)
            {

                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }

        }
    }
}