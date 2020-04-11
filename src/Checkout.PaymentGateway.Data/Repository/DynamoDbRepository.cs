using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Checkout.PaymentGateway.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Checkout.PaymentGateway.Models.Configuration;
using Checkout.PaymentGateway.Models.ServiceModels;
using Microsoft.Extensions.Caching.Distributed;

namespace Checkout.PaymentGateway.Data.Repository
{
    public class DynamoDbRepository<TDataModel> : IDynamoDbRepository<TDataModel>
        where TDataModel : class, IDynamoDbSerializable, new()
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly DynamoDbConfiguration<TDataModel> _configuration;
        private readonly ILogger _logger;

        public DynamoDbRepository(IAmazonDynamoDB dynamoDbClient, DynamoDbConfiguration<TDataModel> configuration, ILogger<DynamoDbRepository<TDataModel>> logger)
        {
            _dynamoDbClient = dynamoDbClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ServiceResult> PutItemAsync(TDataModel dataModel)
        {
            var documentAttributeMap = dataModel.ToAttributeMap();

            var putItemRequest = new PutItemRequest(_configuration.TableName, documentAttributeMap);

            var response = await _dynamoDbClient.PutItemAsync(putItemRequest);

            if (response.HttpStatusCode == HttpStatusCode.OK)
                return ServiceResult.Succeeded();

            _logger.LogError(
                $"Error occurred saving entity {string.Join(" | ", response.ResponseMetadata.Metadata.Select(x => $"{x.Key}-{x.Value}").ToList())}");

            return ServiceResult.Failed(ErrorCodeStrings.InternalError);
        }

        public async Task<ServiceObjectResult<ICollection<TDataModel>>> QueryByPartitionAsync(string partitionKey)
        {
            var queryRequest = new QueryRequest(_configuration.TableName)
            {
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#partition", _configuration.PartitionKey }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":partition", new AttributeValue(partitionKey) }
                },
                KeyConditionExpression = $"#partition = :partition"
            };

            bool operationComplete = false;

            var results = new List<Dictionary<string, AttributeValue>>();

            while (!operationComplete)
            {
                var queryResponse = await _dynamoDbClient.QueryAsync(queryRequest);
                
                if(queryResponse.HttpStatusCode != HttpStatusCode.OK)
                    return ServiceObjectResult<ICollection<TDataModel>>.Failed(null, $"An error occurred querying DynamoDb");
                
                results.AddRange(queryResponse.Items);

                if (queryResponse.LastEvaluatedKey.Any())
                    queryRequest.ExclusiveStartKey = queryResponse.LastEvaluatedKey;
                else
                    operationComplete = true;
            }

            var dataModels = new List<TDataModel>();

            foreach (var result in results)
            {
                var dataModel = new TDataModel();
                dataModel.FromAttributeMap(result);
                dataModels.Add(dataModel);
            }

            return ServiceObjectResult<ICollection<TDataModel>>.Succeeded(dataModels);
        }
    }
}