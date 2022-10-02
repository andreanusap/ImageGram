using CommentService;
using ImageGram.Application.Interfaces;
using ImageGram.Application.Services;
using ImageGram.Infrastructure.Repositories;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace CommentService;

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
            var connectionString = config["CosmosDBConnection"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Please specify a valid CosmosDBConnection in the appSettings.json file or your Azure Functions Settings.");
            }

            return new CosmosClientBuilder(connectionString)
                .Build();
        });

        builder.Services.AddScoped<ICommentFunctionService, CommentFunctionService>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    }
}
