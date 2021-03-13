using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Speeding.Infraction.Management.AF01.Handlers.Implementations;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;

[assembly: FunctionsStartup(typeof(Speeding.Infraction.Management.AF01.Startup))]
namespace Speeding.Infraction.Management.AF01
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
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

            builder.Services.AddTransient<IBlobHandler, BlobHandler>();
            builder.Services.AddTransient<IDmvDbHandler, CosmosDmvDbHandler>();
            builder.Services.AddTransient<IFaceHandler, FaceHandler>();
            builder.Services.AddTransient<IComputerVisionHandler, ComputerVisionHandler>();
        }
    }
}
