using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Speeding.Infraction.Management.AF01.Constants;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using Speeding.Infraction.Management.AF01.Helpers;
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

            CustomEventData outputEventData = new CustomEventData
            {
                ImageUrl = inputEventData.ImageUrl,
                TicketNumber = inputEventData.TicketNumber,
                DistrictOfInfraction = inputEventData.DistrictOfInfraction,
                DateOfInfraction = inputEventData.DateOfInfraction
            };

            var correlationId = LoggingHelper.GetCorrelationId(inputEventData);

            #region Logging

            logger.LogInformation(
                new EventId((int)LoggingConstants.EventId.CreateSpeedingTicketStarted),
                LoggingConstants.Template,
                LoggingConstants.EventId.CreateSpeedingTicketStarted.ToString(),
                correlationId,
                LoggingConstants.ProcessingFunction.CreateSpeedingTicket.ToString(),
                LoggingConstants.ProcessStatus.Started.ToString(),
                "Execution Started"
                );

            #endregion


            try
            {
                

                await _dmvDbHandler
                    .CreateSpeedingTicketAsync(
                        ticketNumber: inputEventData.TicketNumber,
                        vehicleRegistrationNumber: inputEventData.VehicleRegistrationNumber,
                        district: inputEventData.DistrictOfInfraction,
                        date: inputEventData.DateOfInfraction
                    )
                    .ConfigureAwait(false);

                outputEventData.CustomEvent = CustomEvent.SpeedingTicketCreated.ToString();

                #region Logging

                logger.LogInformation(
                    new EventId((int)LoggingConstants.EventId.CreateSpeedingTicketFinished),
                    LoggingConstants.Template,
                    LoggingConstants.EventId.CreateSpeedingTicketFinished.ToString(),
                    correlationId,
                    LoggingConstants.ProcessingFunction.CreateSpeedingTicket.ToString(),
                    LoggingConstants.ProcessStatus.Finished.ToString(),
                    "Execution Finished"
                    );

                #endregion
            }
            catch (Exception ex)
            {

                outputEventData.CustomEvent = CustomEvent.Exceptioned.ToString();

                #region Logging

                logger.LogError(
                    new EventId((int)LoggingConstants.EventId.CreateSpeedingTicketFinished),
                    LoggingConstants.Template,
                    LoggingConstants.EventId.CreateSpeedingTicketFinished.ToString(),
                    correlationId,
                    LoggingConstants.ProcessingFunction.CreateSpeedingTicket.ToString(),
                    LoggingConstants.ProcessStatus.Failed.ToString(),
                    $"Execution Failed. Reason: {ex.Message}"
                    );

                #endregion

               

            }

            await _eventHandler.PublishEventToTopicAsync(outputEventData)
               .ConfigureAwait(false);

        }

    }
}
