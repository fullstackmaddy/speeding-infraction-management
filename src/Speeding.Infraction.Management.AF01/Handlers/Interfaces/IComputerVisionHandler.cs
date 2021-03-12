using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Handlers.Interfaces
{
    public interface IComputerVisionHandler
    {
        /// <summary>
        /// Extract registration number from image specified using its url
        /// </summary>
        /// <param name="imageUrl">Url of the image</param>
        /// <returns>Extracted registration number</returns>
        public Task<string> ExtractRegistrationNumberWithUrlAsync(string imageUrl);

        /// <summary>
        /// Extract registration number from image specified using the stream
        /// </summary>
        /// <param name="imageStream">Stream containing the image</param>
        /// <returns>Extracted  registration number</returns>
        public Task<string> ExtractRegistrationNumberWithStreamAsync(Stream imageStream);
       
    }
}
