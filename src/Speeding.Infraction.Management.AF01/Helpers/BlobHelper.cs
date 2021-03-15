using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Speeding.Infraction.Management.AF01.Helpers
{
    public static class BlobHelper
    {
        public static string GetBlobName(string blobUrl)
        {

            return Path.GetFileNameWithoutExtension(blobUrl);

        }

        public static string GetBlobNameWithExtension(string blobName)
        {

            return $"{blobName}.jpeg";

        }
    }
}
