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

namespace BeanSceneWebAPI.Controllers
{
    /// <summary>
    /// Provides the order summaries
    /// </summary>
    [BasicAuthentication]
    public class ReportController : ApiController
    {
        MongoClient client;
        string databaseName;
        string connectionStr;

        public ReportController()
        {
            connectionStr = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;
            client = new MongoClient(connectionStr);
            databaseName = MongoUrl.Create(connectionStr).DatabaseName;
        }

        /// <summary>
        /// Gets all current orders for today
        /// </summary>
        /// <returns>HTTP status and all orders in JSON</returns>
        [Route("api/Report/getCurrentOrder")]
        public HttpResponseMessage getCurrentOrder()
        {
            try
            {
                string today = DateTime.Today.ToShortDateString();
                var collection = client.GetDatabase(databaseName).GetCollection<Order>("order");
                var filteredResult = collection.AsQueryable()
                    .Where(o => o.date.Contains(today))
                    .Where(o => o.is_complete == false)
                    .ToList();
                var jsonResult = JsonConvert.SerializeObject(filteredResult);

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
        /// Gets all completed orders for today
        /// </summary>
        /// <returns>HTTP status code and list of orderss in JSON</returns>
        [Route("api/Report/getCompletedOrder")]
        public HttpResponseMessage getCompletedOrder()
            {
            try
            {
                string today = DateTime.Today.ToShortDateString();
                var collection = client.GetDatabase(databaseName).GetCollection<Order>("order").AsQueryable();
                var filteredResult = collection.AsQueryable()
                    .Where(o => o.date.Contains(today))
                    .Where(o => o.is_complete == true)
                    .ToList();
                var jsonResult = JsonConvert.SerializeObject(filteredResult);

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
    }
}