using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Handlers.Implementations
{
    public class ComputerVisionHandler : IComputerVisionHandler
    {
        public Task<string> ExtractRegistrationNumberWithStreamAsync(Stream imageStream)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExtractRegistrationNumberWithUrlAsync(string imageUrl)
        {
            throw new NotImplementedException();
        }
    }
}
