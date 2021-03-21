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
    public class NumberPlateController
    {
        private readonly IComputerVisionHandler _computerVisionHandler;
        private readonly IEventHandler _eventHandler;
        private readonly IBlobHandler _blobHandler;

        public NumberPlateController(IComputerVisionHandler computerVisionHandler,
            IEventHandler eventHandler,
            IBlobHandler blobHandler)
        {
            _computerVisionHandler = computerVisionHandler ??
                throw new ArgumentNullException(nameof(computerVisionHandler));

            _eventHandler = eventHandler ??
                throw new ArgumentNullException(nameof(eventHandler));

            _blobHandler = blobHandler ??
                throw new ArgumentNullException(nameof(blobHandler));
        }


        [FunctionName("ExtractRegistrationNumber")]
        public async Task ExtractRegistrationNumber(
                [EventGridTrigger] EventGridEvent eventGridEvent,
                ILogger logger
            )
        {
            StorageBlobCreatedEventData blobCreatedEventData =
              ((JObject)eventGridEvent.Data).ToObject<StorageBlobCreatedEventData>();

            string blobName = BlobHelper.GetBlobName(blobCreatedEventData.Url);


            #region Logging

            logger.LogInformation(
                new EventId((int)LoggingConstants.EventId.ExtractRegistrationNumberStarted),
                LoggingConstants.Template,
                LoggingConstants.EventId.ExtractRegistrationNumberStarted.ToString(),
                blobName,
                LoggingConstants.ProcessingFunction.ExtractRegistrationNumber.ToString(),
                LoggingConstants.ProcessStatus.Started.ToString(),
                "Execution Started"
                );

            #endregion

            CustomEventData customEventData = new CustomEventData
            {
                ImageUrl = blobCreatedEventData.Url,
                TicketNumber = blobName,
            };

            try
            {
                string registrationNumber = await _computerVisionHandler
                        .ExtractRegistrationNumberWithUrlAsync(blobCreatedEventData.Url)
                        .ConfigureAwait(false);

                var metadata = await _blobHandler
                    .GetBlobMetadataAsync(blobCreatedEventData.Url);


                customEventData.DistrictOfInfraction = metadata["districtofinfraction"];
                customEventData.DateOfInfraction = eventGridEvent.EventTime.ToString("dd-MM-yyyy");

                if (!string.IsNullOrWhiteSpace(registrationNumber))
                {
                    customEventData.VehicleRegistrationNumber = registrationNumber;
                    customEventData.CustomEvent = CustomEvent.NumberExtractionCompleted.ToString();

                    #region Logging

                    logger.LogInformation(
                        new EventId((int)LoggingConstants.EventId.ExtractRegistrationNumberFinished),
                        LoggingConstants.Template,
                        LoggingConstants.EventId.ExtractRegistrationNumberFinished.ToString(),
                        blobName,
                        LoggingConstants.ProcessingFunction.ExtractRegistrationNumber.ToString(),
                        LoggingConstants.ProcessStatus.Finished.ToString(),
                        "Execution Finished"
                        );

                    #endregion

                }
                else
                {
                    customEventData.CustomEvent = CustomEvent.Exceptioned.ToString();

                    #region Logging

                    logger.LogError(
                        new EventId((int)LoggingConstants.EventId.ExtractRegistrationNumberFinished),
                        LoggingConstants.Template,
                        LoggingConstants.EventId.ExtractRegistrationNumberFinished.ToString(),
                        blobName,
                        LoggingConstants.ProcessingFunction.ExtractRegistrationNumber.ToString(),
                        LoggingConstants.ProcessStatus.Failed.ToString(),
                        "Execution Failed. Reason: Failed to extract number plate from the image"
                        );

                    #endregion
                }
            }
            catch (Exception ex)
            {

                customEventData.CustomEvent = CustomEvent.Exceptioned.ToString();

                #region Logging

                logger.LogError(
                    new EventId((int)LoggingConstants.EventId.ExtractRegistrationNumberFinished),
                    LoggingConstants.Template,
                    LoggingConstants.EventId.ExtractRegistrationNumberFinished.ToString(),
                    blobName,
                    LoggingConstants.ProcessingFunction.ExtractRegistrationNumber.ToString(),
                    LoggingConstants.ProcessStatus.Failed.ToString(),
                    $"Execution Failed. Reason: {ex.Message}"
                    );

                #endregion
            }

            await _eventHandler
                .PublishEventToTopicAsync(customEventData)
                .ConfigureAwait(false);
                                                                                                                                                                                                                                                             
        }

    }
}
