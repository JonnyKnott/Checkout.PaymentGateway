using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Checkout.PaymentGateway.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.WebApi.Services
{
    public class DevelopmentTableSetup : IHostedService
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ILogger<DevelopmentTableSetup> _logger;

        public DevelopmentTableSetup(IAmazonDynamoDB dynamoDbClient, ILogger<DevelopmentTableSetup> logger)
        {
            _dynamoDbClient = dynamoDbClient;
            _logger = logger;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var tables = await _dynamoDbClient.ListTablesAsync(CancellationToken.None);

            if (!tables.TableNames.Contains(Constants.DynamoDb.PaymentResultTableName))
                await CreatePaymentResultTable();
        }

        private async Task CreatePaymentResultTable()
        {
            _logger.LogInformation($"Creating new DynamoDb table {Constants.DynamoDb.PaymentResultTableName}");
            
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = Constants.DynamoDb.PaymentResultPartitionKey,
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = Constants.DynamoDb.PaymentResultSortKey,
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = Constants.DynamoDb.PaymentResultPartitionKey,
                        KeyType = "HASH"
                    },
                    new KeySchemaElement
                    {
                        AttributeName = Constants.DynamoDb.PaymentResultSortKey,
                        KeyType = "RANGE"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput()
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 10
                },
                TableName = Constants.DynamoDb.PaymentResultTableName
            };

            await _dynamoDbClient.CreateTableAsync(request);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}