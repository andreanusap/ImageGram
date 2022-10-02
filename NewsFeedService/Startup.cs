using ImageGram.Application.Interfaces;
using ImageGram.Application.Services;
using ImageGram.Infrastructure.Repositories;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsFeedService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NewsFeedService;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        builder.Services.AddSingleton<IConfiguration>(config);

        builder.Services.AddSingleton(s => {
            var connectionString = config["NewsFeedDBConnection"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Please specify a valid CosmosDBConnection in the appSettings.json file or your Azure Functions Settings.");
            }

            return new CosmosClientBuilder(connectionString)
                .Build();
        });

        builder.Services.AddScoped<INewsFeedFunctionService, NewsFeedFunctionService>();
        builder.Services.AddScoped<INewsFeedRepository, NewsFeedRepository>();
    }
}
