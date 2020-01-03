using System;
using System.Configuration;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHomeMeasure.Services;

[assembly: FunctionsStartup(typeof(MyHomeMeasure.Startup))]
namespace MyHomeMeasure
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            builder.Services.AddSingleton<ICosmosDbService>(provider => {
                var config = provider.GetRequiredService<IConfiguration>();

                return InitializeCosmosClientInstanceAsync(
                    config.GetConnectionString("DefaultCosmosConnection"),
                    Environment.GetEnvironmentVariable("CosmosDb_DatabaseName"),
                    Environment.GetEnvironmentVariable("CosmosDb_ContainerName"))
                .GetAwaiter().GetResult();
            });

            builder.Services.AddHttpClient("NatureRemo", client =>
            {
                client.BaseAddress = new Uri("https://api.nature.global");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer", Environment.GetEnvironmentVariable("NatureRemo_Token"));

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            });
        }

        /// <summary>
        /// Creates a Cosmos DB database and a container with the specified partition key. 
        /// </summary>
        /// <returns></returns>
        private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(string connectionStrings, string databaseName, string containerName)
        {
            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(connectionStrings);
            CosmosClient client = clientBuilder
                                .WithConnectionModeDirect()
                                .Build();
            CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbService;
        }
    }
}
