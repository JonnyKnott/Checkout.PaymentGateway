using System;
using System.Collections.Generic;
using System.Linq;
using Checkout.PaymentGateway.Data.Test.Fixtures;
using Checkout.PaymentGateway.Data.Test.Models;
using Xunit;

namespace Checkout.PaymentGateway.Data.Test
{
    public class DynamoDbRepositoryTests : IClassFixture<DynamoDbTestFixture>
    {
        private readonly DynamoDbTestFixture _testFixture;

        public DynamoDbRepositoryTests(DynamoDbTestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Fact]
        public async void DynamoDb_Repository_Should_Put_Item()
        {
            await _testFixture.EnsureDynamoDbTable();
            
            var item = new TestEntity
            {
                PartitionKey = Guid.NewGuid().ToString(), 
                SortKey = "Sort"
            };

            var result = await _testFixture.Repository.PutItemAsync(item);
            
            Assert.True(result.Success);

            await _testFixture.ClearTableItems(new List<TestEntity> {item});
        }

        [Fact]
        public async void DynamoDbRepository_Should_Retrieve_Items_In_Partition()
        {
            await _testFixture.EnsureDynamoDbTable();
            
            var item = new TestEntity
            {
                PartitionKey = Guid.NewGuid().ToString(), 
                SortKey = "Sort",
                AdditionalAttribute1 = "AdditionalAttribute"
            };

            var result = await _testFixture.Repository.PutItemAsync(item);
            
            Assert.True(result.Success);

            var getResult = await _testFixture.Repository.QueryByPartitionAsync(item.PartitionKey);
            
            Assert.True(getResult.Success);
            Assert.Single(getResult.Result);
            Assert.Equal(item.PartitionKey, getResult.Result.Single().PartitionKey);
            Assert.Equal(item.SortKey, getResult.Result.Single().SortKey);
            Assert.Equal(item.AdditionalAttribute1, getResult.Result.Single().AdditionalAttribute1);
            
            await _testFixture.ClearTableItems(new List<TestEntity> {item});
        }
    }
}