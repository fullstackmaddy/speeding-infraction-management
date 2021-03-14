using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Handlers.Interfaces
{
    public interface IFaceHandler
    {
        /// <summary>
        /// Detect all the faces in the image specifed using an url
        /// </summary>
        /// <param name="url">Url of the image</param>
        /// <returns>List of all the detected faces</returns>
        public Task<IEnumerable<DetectedFace>> DetectFacesWithUrlAsync(string url);

        /// <summary>
        /// Detect all the faces in an image specified using a stream
        /// </summary>
        /// <param name="imageStream">Stream containing the image</param>
        /// <returns>List of all the detected faces</returns>
        public Task<IEnumerable<DetectedFace>> DetectFacesWithStreamAsync(Stream imageStream);

        /// <summary>
        /// Blur faces defined by the detected faces list
        /// </summary>
        /// <param name="imageBytes">The byte array containing the image</param>
        /// <param name="detectedFaces">List of the detected faces in the image</param>
        /// <returns>Processed stream containing image with blurred faces</returns>
        public Task<byte[]> BlurFacesAsync(byte[] imageBytes, List<DetectedFace> detectedFaces);
    }
}
