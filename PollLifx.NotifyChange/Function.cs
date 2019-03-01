using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using PollLifx.Common.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace PollLifx.NotifyChange
{
    public class Function
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly IChangeProcessor _changeProcessor;
        private readonly string _snsArn;

        public Function()
        {
            var dynamoDbClient = new AmazonDynamoDBClient();
            _dynamoDbContext = new DynamoDBContext(dynamoDbClient);

            _snsClient = new AmazonSimpleNotificationServiceClient();
            _changeProcessor = new ChangeProcessor();

            _snsArn = Environment.GetEnvironmentVariable("SnsTopicArn");
        }

        public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            context.Logger.LogLine($"Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                var oldImage = Document.FromAttributeMap(record.Dynamodb.OldImage);
                var newImage = Document.FromAttributeMap(record.Dynamodb.NewImage);

                var oldState = _dynamoDbContext.FromDocument<Lightbulb>(oldImage);
                var newState = _dynamoDbContext.FromDocument<Lightbulb>(newImage);

                var changes = _changeProcessor.GetChanges(oldState, newState);

                if (changes.success)
                {
                    PublishResponse publishResponse = _snsClient.PublishAsync(_snsArn, changes.message).Result;

                    context.Logger.LogLine($"Sent Message '{changes.message}' with SNS MessageId: {publishResponse.MessageId}");
                }
                else
                {
                    context.Logger.LogLine("No useful changes detected");
                }
            }

            context.Logger.LogLine("Stream processing complete.");
        }
    }
}