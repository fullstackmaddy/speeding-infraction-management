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
    public class NotificationController
    {
        private readonly IDmvDbHandler _dmvDbHandler;
        private readonly IBlobHandler _blobHandler;
        private readonly IOwnerNotificationHandler _ownerNotificationHandler;
        private readonly IEventHandler _eventHandler;

        public NotificationController(IDmvDbHandler dmvDbHandler,
            IBlobHandler blobHandler,
            IOwnerNotificationHandler ownerNotificationHandler,
            IEventHandler eventHandler)
        {
            _dmvDbHandler = dmvDbHandler ??
                throw new ArgumentNullException(nameof(dmvDbHandler));

            _blobHandler = blobHandler ??
                throw new ArgumentNullException(nameof(blobHandler));

            _ownerNotificationHandler = ownerNotificationHandler ??
                throw new ArgumentNullException(nameof(ownerNotificationHandler));

            _eventHandler = eventHandler ??
                throw new ArgumentNullException(nameof(eventHandler));
        }

        [FunctionName("NotifyRegisteredOwner")]
        public async Task NotifyRegisteredOwner(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            ILogger logger
            )
        {
            StorageBlobCreatedEventData blobCreatedEventData =
              ((JObject)eventGridEvent.Data).ToObject<StorageBlobCreatedEventData>();

            string blobName = BlobHelper.GetBlobName(blobCreatedEventData.Url);

            #region Logging

            logger.LogInformation(
                new EventId((int)LoggingConstants.EventId.NotifyVehicleOwnerStarted),
                LoggingConstants.Template,
                LoggingConstants.EventId.NotifyVehicleOwnerStarted.ToString(),
                blobName,
                LoggingConstants.ProcessingFunction.NotifyRegisteredOwner.ToString(),
                LoggingConstants.ProcessStatus.Started.ToString(),
                "Execution Started"
                );

            #endregion


            try
            {
                var speedingTicket = await _dmvDbHandler
                    .GetSpeedingTicketInfoAsync(blobName)
                    .ConfigureAwait(false);

                var registeredOwnerInfo = await _dmvDbHandler
                        .GetOwnerInformationAsync(
                        vehicleRegistrationNumber: speedingTicket.VehicleResgistrationNumber)
                        .ConfigureAwait(false);

                var infractionImage = await _blobHandler
                    .DownloadBlobAsync(blobCreatedEventData.Url)
                    .ConfigureAwait(false);


                var attachments = new List<OwnerNotificationMessageAttachment>
            {
                new OwnerNotificationMessageAttachment
                {
                    Name = speedingTicket.TicketNumber,
                    Content = Convert.ToBase64String(infractionImage),
                    ContentType = "image/jpeg"
                }
            };

                var notificationMessage =
                    CreateNotificationMessage
                    (
                        vehicleOwnerInfo: registeredOwnerInfo,
                        attachments: attachments,
                        ticketNumber: speedingTicket.TicketNumber,
                        infractionDate: speedingTicket.Date ,
                        infractionDistrict: speedingTicket.District
                     );

                await _ownerNotificationHandler
                    .NotifyOwnerAsync(notificationMessage)
                    .ConfigureAwait(false);

                #region Logging

                logger.LogInformation(
                    new EventId((int)LoggingConstants.EventId.NotifyVehicleOwnerFinished),
                    LoggingConstants.Template,
                    LoggingConstants.EventId.NotifyVehicleOwnerFinished.ToString(),
                    blobName,
                    LoggingConstants.ProcessingFunction.NotifyRegisteredOwner.ToString(),
                    LoggingConstants.ProcessStatus.Finished.ToString(),
                    "Execution Finished"
                    );

                #endregion
            }
            catch (Exception ex)
            {

                CustomEventData customEventData = new CustomEventData
                {
                    ImageUrl = blobCreatedEventData.Url,
                    
                    CustomEvent = CustomEvent.Exceptioned.ToString()
                };

                await _eventHandler.PublishEventToTopicAsync(customEventData)
                .ConfigureAwait(false);

                #region Logging

                logger.LogError(
                    new EventId((int)LoggingConstants.EventId.NotifyVehicleOwnerFinished),
                    LoggingConstants.Template,
                    LoggingConstants.EventId.NotifyVehicleOwnerFinished.ToString(),
                    blobName,
                    LoggingConstants.ProcessingFunction.NotifyRegisteredOwner.ToString(),
                    LoggingConstants.ProcessStatus.Failed.ToString(),
                    $"Execution Failed. Reason: {ex.Message}"
                    );

                #endregion
            }

        }


        private OwnerNotificationMessage CreateNotificationMessage(
                VehicleOwnerInfo vehicleOwnerInfo,
                List<OwnerNotificationMessageAttachment> attachments,
                string infractionDate,
                string infractionDistrict,
                string ticketNumber

            )
        {

            return new OwnerNotificationMessage
            {
                Attachments = attachments,
                InfractionDate = infractionDate,
                InfractionDistrict = infractionDistrict,
                TicketNumber = ticketNumber,
                VehicleOwnerInfo = vehicleOwnerInfo
            };
        }
    }
}
