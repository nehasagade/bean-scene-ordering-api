using BeanSceneAPI.Models;
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
    /// Controller for Menu related actions
    /// </summary>
    [BasicAuthentication]
    public class MenuController : ApiController
    {
        MongoClient client;
        string databaseName;
        string connectionStr;

        /// <summary>
        /// Constructor that contains the connection string and database name
        /// </summary>
        public MenuController()
        {
            connectionStr = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;
            client = new MongoClient(connectionStr);
            databaseName = MongoUrl.Create(connectionStr).DatabaseName;
        }

        /// <summary>
        /// Gets all menu items
        /// </summary>
        /// <returns>All menu items as JSON and/or status code</returns>
        // GET: api/Menu
        public HttpResponseMessage Get()
        {
            try
            {
                var collection = client.GetDatabase(databaseName).GetCollection<Menu>("menu").AsQueryable();
                string jsonResult = JsonConvert.SerializeObject(collection);

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
        /// Gets the menu items according to user search
        /// </summary>
        /// <param name="searchText">The text to search by</param>
        /// <returns>Menu items in JSON and/or status code.</returns>
        // GET: api/Menu/Search
        [Route("api/Menu/Search/{searchText}")]
        public HttpResponseMessage Get(string searchText)
        {
            try
            {
                var collection = client.GetDatabase(databaseName).GetCollection<Menu>("menu");
                var filteredResult = collection.AsQueryable().Where(m => m.name.ToLower().Contains(searchText)).ToList();

                string jsonResult = JsonConvert.SerializeObject(filteredResult);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (filteredResult.Count() == 0)
                {
                   response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
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
        /// Searches Menu by category
        /// </summary>
        /// <param name="categoryId">The category id to search by</param>
        /// <returns></returns>
        [Route("api/Menu/SearchCategory/{categoryId}")]
        [HttpGet]
        public HttpResponseMessage SearchCategory(string categoryId)        
        {
            try
            {
                var collection = client.GetDatabase(databaseName).GetCollection<Menu>("menu");
                var filteredResult = collection.AsQueryable().Where(m => m.category == categoryId).ToList();
                string jsonResult = JsonConvert.SerializeObject(filteredResult);

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
        /// Creates a new menu item
        /// </summary>
        /// <param name="menuItem">The Menu object to be created in the database</param>
        /// <returns></returns>
        // POST: api/Menu/Create
        [Route("api/Menu/Create")]
        public HttpResponseMessage Post([FromBody] Menu menuItem)
        {
            try
            {
                client.GetDatabase(databaseName).GetCollection<Menu>("menu").InsertOne(menuItem);

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
        /// Updates existing menu item
        /// </summary>
        /// <param name="id">The id of the menu item to update</param>
        /// <param name="menuItem">The update Menu object</param>
        /// <returns></returns>
        // PUT: api/Menu/Edit/{id}
        [Route("api/Menu/Edit/{id}")]
        public HttpResponseMessage Put(string id, [FromBody] Menu menuItem)
        {
            try
            {
                var filter = Builders<Menu>.Filter.Eq("_id", id);
                var update = Builders<Menu>.Update.Set("name", menuItem.name).Set("description", menuItem.description).Set("price", menuItem.price).Set("category", menuItem.category).Set("dietaryFlag", menuItem.dietaryFlag).Set("availability", menuItem.availability);
                client.GetDatabase(databaseName).GetCollection<Menu>("menu").UpdateOne(filter, update);

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
        /// Delete a menu item
        /// </summary>
        /// <param name="id">The id of the item to be deleted</param>
        /// <returns></returns>
        // DELETE: api/Menu/Delete/id
        [Route("api/Menu/Delete/{id}")]
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                var filter = Builders<Menu>.Filter.Eq("_id", id);
                client.GetDatabase(databaseName).GetCollection<Menu>("menu").DeleteOne(filter);

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