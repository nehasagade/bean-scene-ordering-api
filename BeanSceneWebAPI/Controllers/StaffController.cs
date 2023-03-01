using BeanSceneAPI.Models;
using BeanSceneWebAPI;
//using MongoDB.Bson.IO;
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
using System.Web.Http.Cors;

namespace BeanSceneAPI.Controllers
{
    /// <summary>
    /// Methods for Staff collection
    /// </summary>
    [BasicAuthentication]
    public class StaffController : ApiController
    {
        MongoClient client;
        string databaseName;
        string connectionStr;
        /// <summary>
        /// Constructor for staff Controller
        /// </summary>
        public StaffController()
        {
            connectionStr = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;
            client = new MongoClient(connectionStr);
            databaseName = MongoUrl.Create(connectionStr).DatabaseName;
        }
        /// <summary>
        /// Gets all staff from the database
        /// </summary>
        /// <returns>All staff in JSON format and/or appropriate HTTP response</returns>
        // GET: api/Staff
        public HttpResponseMessage Get()
        {
            try
            {          
            
                var collection = client.GetDatabase(databaseName).GetCollection<Staff>("staff").AsQueryable().OrderBy(x => x.lastname);
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

        public HttpResponseMessage Get(string id)
        {
            try
            {

                var staffMember = client.GetDatabase(databaseName).GetCollection<Staff>("staff").AsQueryable().Where(s => s._id == id);
                string jsonResult = JsonConvert.SerializeObject(staffMember);

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
        /// Adds a new staff to the database
        /// </summary>
        /// <param name="staff">The staff object to be added</param>
        /// <returns>Appropriate HTTP response</returns>
        [Route("api/Staff/Create")]
        public HttpResponseMessage Create([FromBody] Staff staff)
        {
            try
            {
                // TODO Automatically add staff id
                var filter = Builders<Staff>.Filter.Empty;
                var collection = client.GetDatabase(databaseName).GetCollection<Staff>("staff");
                var projection = Builders<Staff>.Projection.Include("employee_id").Exclude("_id");
                var result = collection.Find(filter).Project(projection).ToList();
                int lastStaffId = result[result.Count - 1].GetValue("employee_id").ToInt32();
                staff.employee_id = lastStaffId + 1;
                staff.username = staff.employee_id;

                // Hash new password
                staff.password = BCrypt.Net.BCrypt.HashPassword(staff.password);

                client.GetDatabase(databaseName).GetCollection<Staff>("staff").InsertOne(staff);
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
        /// Updates and existing staff member
        /// </summary>
        /// <param name="id">The staff id to be updated</param>
        /// <param name="s">The staff object with all the updated fields</param>
        /// <returns>Appropriate HTTP status code</returns>
        [Route("api/Staff/Edit/{id}")]
        public HttpResponseMessage Put(int id, [FromBody] Staff s)
        {
            try
            {
                var filter = Builders<Staff>.Filter.Eq("employee_id", id);
                var update = Builders<Staff>.Update.Set("firstname", s.firstname).Set("lastname", s.lastname).Set("email", s.email).Set("phone", s.phone).Set("position", s.position);

                // Hash and add new password if the password is not empty
                if (s.password != string.Empty)
                {
                    s.password = BCrypt.Net.BCrypt.HashPassword(s.password);
                    update = Builders<Staff>.Update.Set("firstname", s.firstname).Set("lastname", s.lastname).Set("email", s.email).Set("phone", s.phone).Set("password", s.password).Set("position", s.position);
                }              
               

                client.GetDatabase(databaseName).GetCollection<Staff>("staff").UpdateOne(filter, update);

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
        /// Deletes one staff member
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Appropriate status code</returns>
        [Route("api/Staff/Delete/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var filter = Builders<Staff>.Filter.Eq("employee_id", id);
                
                client.GetDatabase(databaseName).GetCollection<Staff>("staff").DeleteOne(filter);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
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
        /// Finds and verifies the username and password of the staff member logging in.
        /// </summary>
        /// <param name="username">username (employee id) of the employee</param>
        /// <param name="password">password entered by the user</param>
        /// <returns>The staff who matches the username and password and/or the appropriate HTTP status</returns>
        [Route("api/Staff/{username}/{password}")]
        public HttpResponseMessage Get(int username, string password)
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Staff>("staff");
            var result = collection.Find(x => x.username == username).FirstOrDefault();

            if (result !=null)
            {
                try
                {
                    bool verified = BCrypt.Net.BCrypt.Verify(password, result.password);

                    if (!verified)
                    {
                        result = null;
                    }
                }
                catch (Exception)
                {
                    result = null;
                    throw;
                }
            }

            string jsonResult = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");

            return response;
        }
    }
}