using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Functions
{
    public class TicketController
    {
        private readonly IDmvDbHandler _dmvDbHandler;
        
        public TicketController(IDmvDbHandler dmvDbHandler, IBlobHandler blobHandler)
        {
            _dmvDbHandler = dmvDbHandler ??
                throw new ArgumentNullException(nameof(dmvDbHandler));

        }


        public async Task CreateTicket(
                [EventGridTrigger] EventGridEvent eventGridEvent,
                ILogger logger
            )
        {
            
            CustomEventData customEventData =
                ((JObject)eventGridEvent.Data).ToObject<CustomEventData>();

            await _dmvDbHandler
                .CreateSpeedingTicketAsync(
                    ticketNumber: customEventData.TicketNumber,
                    vehicleRegistrationNumber: customEventData.VehicleRegistrationNumber,
                    district: customEventData.DistrictOfInfraction
                )
                .ConfigureAwait(false);

        }




        public string GetBlobName(string blobUrl)
        {

            return Path.GetFileNameWithoutExtension(blobUrl);

        }
    }
}
