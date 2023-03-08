using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TestProject1;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]
namespace RESTFULAPI_Automation_Test
{
    [TestClass]
    public class UnitTest1
    {
        private static HttpClient httpClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string UserEndpoint = "pet";

        private static string GetURl(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetUri(string endpoint) => new Uri(GetURl(endpoint));

        private readonly List<UserModel> cleanUpList = new List<UserModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            httpClient = new HttpClient();
        }
        public async void TestCleanUp()
        {
            foreach (var data in cleanUpList)
            {
                var httpResponse = await httpClient.DeleteAsync(GetURl($"{UserEndpoint}/{data.Id}"));
            }
        }

        [TestMethod]
        public async Task PutMethod()
        {
            #region create data

            // Create Json Object
            UserModel userData = new UserModel()
            {
               Id = 1,

               Category = new Category()
               {
                   Id = 2,
                   Name = "Test"
               },
               Name = "Kylo",
               PhotoUrls = new List<string>
               {
                   "Poring"
               },
               Tags = new List<Category>()
               {
                   new Category() 
                   {
                   Id = 3,
                   Name = "Test"
                   }
               },
               Status = "available"
            };

            //  Serialize Content
            var request = JsonConvert.SerializeObject(userData);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Post Request
            await httpClient.PostAsync(GetURl(UserEndpoint), postRequest);

            #endregion

            #region get data

            // Send Request
            var httpResponse = await httpClient.GetAsync(GetUri($"{UserEndpoint}/{userData.Id}"));

            // Get Content
            var httpResponseMessage = httpResponse.Content;

            // Get Status Code
            var statuscode = httpResponse.StatusCode;

            // Deserialize Content
            var listUserData = JsonConvert.DeserializeObject<UserModel>(httpResponseMessage.ReadAsStringAsync().Result);

            #endregion

            #region cleanupdata

            // Assertion

            Assert.AreEqual(HttpStatusCode.OK, statuscode, "Status code is not equal to 200");
            Assert.AreEqual(listUserData.Name, userData.Name, "Name are Not Matching");
            #endregion
        }

        
    }
}
