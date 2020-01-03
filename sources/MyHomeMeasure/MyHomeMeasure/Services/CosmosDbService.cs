using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using MyHomeMeasure.Models;

namespace MyHomeMeasure.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddMeasureValue(MeasureValue value)
        {
            await _container.CreateItemAsync(value, value.PertitionKey);
        }

        public async Task<MeasureValue> GetMeasureValueAsync(string id)
        {
            try
            {
                var result = await _container.ReadItemAsync<MeasureValue>(id, new PartitionKey(id));
                return result.Resource;
            }
            catch(CosmosException ex) when(ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<MeasureValue>> GetMeasureValuesAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _container.GetItemLinqQueryable<MeasureValue>().AsQueryable();

            if (from != null)
            {
                query = query.Where(e => e.CreatedAt >= from);
            }

            if (to != null)
            {
                query = query.Where(e => e.CreatedAt <= to);
            }

            var iterator = query.OrderByDescending(e => e.CreatedAt).ToFeedIterator();

            return await iterator.ReadNextAsync();
        }

    }
}
