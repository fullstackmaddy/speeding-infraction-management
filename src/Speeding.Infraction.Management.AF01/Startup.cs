using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Speeding.Infraction.Management.AF01.ConfigOptions;
using Speeding.Infraction.Management.AF01.Handlers.Implementations;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;

[assembly: FunctionsStartup(typeof(Speeding.Infraction.Management.AF01.Startup))]
namespace Speeding.Infraction.Management.AF01
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<CosmosDbOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("CosmosDbOptions").Bind(settings);
                });

            builder.Services.AddOptions<EventGridOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("EventGridOptions").Bind(settings);
                });

            builder.Services.AddSingleton<IDocumentClient>(
                    x => new DocumentClient(
                            new Uri(
                                    Environment.GetEnvironmentVariable("DmvDbUri")
                                ),
                            Environment.GetEnvironmentVariable("DmvDbAuthKey")
                        )
                );

            builder.Services.AddSingleton<IComputerVisionClient>(
                    x => new ComputerVisionClient(
                            new Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials(
                                    Environment.GetEnvironmentVariable("ComputerVisionSubscriptionKey")
                                )

                        )
                    {
                        Endpoint = Environment.GetEnvironmentVariable("ComputerVisionEndpoint")
                    }
                );

            builder.Services.AddSingleton<IFaceClient>(
                    x => new FaceClient(
                            new Microsoft.Azure.CognitiveServices.Vision.Face.ApiKeyServiceClientCredentials(
                                    Environment.GetEnvironmentVariable("FaceApiSubscriptionKey")
                                )
                        )
                    {
                        Endpoint = Environment.GetEnvironmentVariable("FaceApiEndpoint")
                    }
                );

            builder.Services.AddSingleton<BlobServiceClient>(
                    new BlobServiceClient(
                            Environment.GetEnvironmentVariable("BlobStorageConnectionKey")
                        )
                );
            builder.Services.AddSingleton<IEventGridClient>(
                    new EventGridClient(
                            new TopicCredentials(
                                Environment.GetEnvironmentVariable("EventGridTopicSasKey")
                            )
                        )
                );

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddTransient<IBlobHandler, AzureBlobHandler>();
            builder.Services.AddTransient<IDmvDbHandler, CosmosDmvDbHandler>();
            builder.Services.AddTransient<IFaceHandler, FaceHandler>();
            builder.Services.AddTransient<IComputerVisionHandler, ComputerVisionHandler>();
            builder.Services.AddTransient<IEventHandler, EventGridHandler>();
            
        }
    }
}
