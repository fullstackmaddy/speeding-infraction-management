using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Handlers.Interfaces
{
    public interface IBlobHandler
    {
        /// <summary>
        /// Retrieve the metadata associated with a blob
        /// </summary>
        /// <param name="blobUrl">Url of the blob</param>
        /// <returns>Key value pairs of the metadata</returns>
        public Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobUrl);

        /// <summary>
        /// Upload the stream as a blob to a container
        /// </summary>
        /// <param name="containerName">Name of the container where the stream is to be uploaded</param>
        /// <param name="stream">Actual Stream representing object to be uploaded</param>
        /// <param name="contentType">Content type of the object</param>
        /// <param name="blobName">Name with which the blob is to be created</param>
        /// <returns></returns>
        public Task UploadStreamAsBlobAsync(string containerName, Stream stream, string contentType,
            string blobName);

        /// <summary>
        /// Download Blob contents and its metadata using blob url
        /// </summary>
        /// <param name="blobUrl">Url of the blob</param>
        /// <returns>Blob information</returns>
        public Task<byte[]> DownloadBlobAsync(string blobUrl);

        /// <summary>
        /// Copy the blob from one container to another in same storage account using the url of the source blob
        /// </summary>
        /// <param name="sourceBlobUrl">Url of the source blob</param>
        /// <param name="targetContainerName">Destination container name</param>
        /// <returns></returns>
        public Task CopyBlobAcrossContainerWithUrlsAsync(string sourceBlobUrl, string targetContainerName);


    }
}
