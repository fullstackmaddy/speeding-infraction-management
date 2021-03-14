using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
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
        private readonly IEventHandler _eventHandler;

        public TicketController(IDmvDbHandler dmvDbHandler, 
            IBlobHandler blobHandler,
            IEventHandler eventHandler)
        {
            _dmvDbHandler = dmvDbHandler ??
                throw new ArgumentNullException(nameof(dmvDbHandler));

            _eventHandler = eventHandler ??
                throw new ArgumentNullException(nameof(eventHandler));
        }

        [FunctionName("CreateSpeedingTicket")]
        public async Task CreateTicket(
                [EventGridTrigger] EventGridEvent eventGridEvent,
                ILogger logger
            )
        {
            CustomEventData inputEventData =
                        ((JObject)eventGridEvent.Data).ToObject<CustomEventData>();


            try
            {
                

                await _dmvDbHandler
                    .CreateSpeedingTicketAsync(
                        ticketNumber: inputEventData.TicketNumber,
                        vehicleRegistrationNumber: inputEventData.VehicleRegistrationNumber,
                        district: inputEventData.DistrictOfInfraction
                    )
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                CustomEventData customEventData = new CustomEventData
                {
                    ImageUrl = inputEventData.ImageUrl,
                    TicketNumber = inputEventData.TicketNumber,
                    CustomEvent = CustomEvent.Exceptioned
                };

                await _eventHandler.PublishEventToTopicAsync(customEventData)
                .ConfigureAwait(false);
            }

        }




        public string GetBlobName(string blobUrl)
        {

            return Path.GetFileNameWithoutExtension(blobUrl);

        }
    }
}
