using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Speeding.Infraction.Management.AF01.Models
{
    public class BlobInfo
    {
        public IDictionary<string, string> Metadata { get; set; }

        public Stream Content { get; set; }

        public string ContentType { get; set; }


    }
}
