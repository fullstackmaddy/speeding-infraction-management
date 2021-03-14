using System;
using System.Collections.Generic;
using System.Text;

namespace Speeding.Infraction.Management.AF01.ConfigOptions
{
    public class BlobOptions
    {
        public string BlurredImageContainerName { get; set; }

        public string CompensationContainerName { get; set; }
        public string UploadContentType { get; set; }
    }
}
