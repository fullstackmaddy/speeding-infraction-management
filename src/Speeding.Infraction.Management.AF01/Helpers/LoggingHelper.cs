using Microsoft.Extensions.Logging;
using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Speeding.Infraction.Management.AF01.Helpers
{
    public static class LoggingHelper
    {
        public static string GetCorrelationId(CustomEventData customEventData)
        {
            return !string.IsNullOrWhiteSpace(customEventData.TicketNumber) ?
                customEventData.TicketNumber :
                BlobHelper.GetBlobName(customEventData.ImageUrl);
        }

        
    }
}
