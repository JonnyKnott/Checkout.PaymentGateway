using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Util;
using Checkout.PaymentGateway.Data.Repository;
using Checkout.PaymentGateway.Data.Test.Models;
using Checkout.PaymentGateway.Models.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Checkout.PaymentGateway.Data.Test.Fixtures
{
    public class DynamoDbTestFixture : IDisposable
    {
        public IDynamoDbRepository<TestEntity> Repository { get; }

        private IAmazonDynamoDB _dynamoDbClient;
        private DynamoDbConfiguration<TestEntity> _configuration;

        public DynamoDbTestFixture()
        {
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "abc");
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "def");
            
            _configuration = new DynamoDbConfiguration<TestEntity>
            {
                PartitionKey = "PartitionKey",
                SortKey = "SortKey",
                TableName = "Test"
            };
            
            _dynamoDbClient = new AmazonDynamoDBClient(new EnvironmentVariablesAWSCredentials(), new AmazonDynamoDBConfig
            {
                UseHttp = true,
                ServiceURL = "http://localhost:8042/"
            });

            var logger = new Mock<ILogger<DynamoDbRepository<TestEntity>>>();
            
            Repository = new DynamoDbRepository<TestEntity>(_dynamoDbClient, _configuration, logger.Object);
        }

        public async Task EnsureDynamoDbTable()
        {
            var tables = await _dynamoDbClient.ListTablesAsync();

            if (!tables.TableNames.Contains(_configuration.TableName))
            {
                var request = new CreateTableRequest
                {
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = _configuration.PartitionKey,
                            AttributeType = "S"
                        },
                        new AttributeDefinition
                        {
                            AttributeName = _configuration.SortKey,
                            AttributeType = "S"
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = _configuration.PartitionKey,
                            KeyType = "HASH"
                        },
                        new KeySchemaElement
                        {
                            AttributeName = _configuration.SortKey,
                            KeyType = "RANGE"
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 10,
                        WriteCapacityUnits = 10
                    },
                    TableName = _configuration.TableName
                };

                await _dynamoDbClient.CreateTableAsync(request);
            }
        }

        public async Task ClearTableItems(ICollection<TestEntity> entities)
        {
            foreach (var entity in entities)
            {
                await _dynamoDbClient.DeleteItemAsync(_configuration.TableName,
                    new Dictionary<string, AttributeValue>
                    {
                        { _configuration.PartitionKey, new AttributeValue(entity.PartitionKey) },
                        { _configuration.SortKey, new AttributeValue(entity.SortKey) }
                    });
            }
        }
        
        public async void Dispose()
        {
            await _dynamoDbClient.DeleteTableAsync(_configuration.TableName);
        }
    }
}