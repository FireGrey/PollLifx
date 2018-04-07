using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using PollLifx.Model;

namespace PollLifx
{
    public class Function
    {
        private static DynamoDBContext _dynamoDbContext;
        private static Table _table;
        private static HttpClient _httpClient;

        public Function()
        {
            string dynamoDbTableName = Environment.GetEnvironmentVariable("DynamoDbTableName");
            AmazonDynamoDBClient dynamoDbClient = new AmazonDynamoDBClient();

            _dynamoDbContext = new DynamoDBContext(dynamoDbClient);
            _table = Table.LoadTable(dynamoDbClient, dynamoDbTableName);

            string lifxToken = Environment.GetEnvironmentVariable("LifxToken");
            string lifxApiUrl = Environment.GetEnvironmentVariable("LifxApiUrl");

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + lifxToken);
            _httpClient.BaseAddress = new Uri(lifxApiUrl);
        }

        /// <summary>
        /// A function to poll the lifx api and store the state in dynamodb
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void FunctionHandler(Stream input, ILambdaContext context)
        {
            LambdaLogger.Log("[DEBUG] Retrieving lights from Lifx Api");

            HttpResponseMessage lifxResponse = _httpClient.GetAsync("/v1/lights/all").Result;

            string json = lifxResponse.Content.ReadAsStringAsync().Result;

            LambdaLogger.Log($"[INFO] Lifx Api response status code: {lifxResponse.StatusCode} content: {json}");

            List<Lightbulb> lightbulbs = JsonConvert.DeserializeObject<List<Lightbulb>>(json);

            foreach (Lightbulb lightbulb in lightbulbs)
            {
                LambdaLogger.Log($"[INFO] Storing state for lightbulb with id: {lightbulb.id}");

                Document doc = _dynamoDbContext.ToDocument(lightbulb);
                var putItemResult = _table.PutItemAsync(doc).Result;
            }
        }
    }
}
