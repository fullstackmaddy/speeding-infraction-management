using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;

namespace Speeding.Infraction.Management.AF01.Handlers.Implementations
{
    public class AzureBlobHandler : IBlobHandler
    {
        #region Properties
        private readonly BlobServiceClient _blobServiceClient;
        #endregion

        #region Constructors

        public AzureBlobHandler(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient
                ?? throw new ArgumentNullException(nameof(blobServiceClient));

        }

        public async Task<byte[]> DownloadBlobAsync(string blobUrl)
        {
            var blobUriBuilder = new BlobUriBuilder(
                new Uri(blobUrl));

            var blobContainerClient = _blobServiceClient
                .GetBlobContainerClient(blobUriBuilder.BlobContainerName);

            var blobClient = blobContainerClient.GetBlobClient(blobUriBuilder.BlobName);

            BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

            using (MemoryStream ms = new MemoryStream())
            {
                await blobDownloadInfo.Content.CopyToAsync(ms)
                    .ConfigureAwait(false);

                return ms.ToArray();
            }

        }


        #endregion

        public async Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobUrl)
        {
            var blobUriBuilder = new BlobUriBuilder(
                new Uri(blobUrl));

            var blobContainerClient = _blobServiceClient
                .GetBlobContainerClient(blobUriBuilder.BlobContainerName);

            var blobClient = blobContainerClient.GetBlobClient(blobUriBuilder.BlobName);

            BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

            BlobProperties blobProperties = await blobClient
                .GetPropertiesAsync()
                .ConfigureAwait(false);

            return blobProperties.Metadata;
        }

        public async Task UploadStreamAsBlobAsync(string containerName, Stream stream, string contentType, string blobName)
        {
           
            var blobContainerClient = _blobServiceClient
                .GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient
                .GetBlobClient(blobName);

            var blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = contentType;

            var blobContentInfo = await blobClient
                .UploadAsync(stream, blobHttpHeaders)
                .ConfigureAwait(false);

            
        }
    }
}
