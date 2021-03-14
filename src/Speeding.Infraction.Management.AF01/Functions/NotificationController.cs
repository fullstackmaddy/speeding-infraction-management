using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Collections.Generic;
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

        [FunctionName("NotifyRegisteredOwners")]
        public async Task NotifyRegisteredOwners(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            ILogger logger
            )
        {
            CustomEventData inputEventData =
                       ((JObject)eventGridEvent.Data).ToObject<CustomEventData>();


            try
            {
                var registeredOwnerInfo = await _dmvDbHandler
                        .GetOwnerInformationAsync(
                        vehicleRegistrationNumber: inputEventData.VehicleRegistrationNumber)
                        .ConfigureAwait(false);

                var infractionImage = await _blobHandler
                    .DownloadBlobAsync(inputEventData.ImageUrl)
                    .ConfigureAwait(false);


                var attachments = new List<OwnerNotificationMessageAttachment>
            {
                new OwnerNotificationMessageAttachment
                {
                    Name = inputEventData.TicketNumber,
                    Content = Convert.ToBase64String(infractionImage),
                    ContentType = "image/jpeg"
                }
            };

                var notificationMessage =
                    CreateNotificationMessage
                    (
                        vehicleOwnerInfo: registeredOwnerInfo,
                        attachments: attachments,
                        ticketNumber: inputEventData.TicketNumber,
                        infractionDate: inputEventData.DateOfInfraction,
                        infractionDistrict: inputEventData.DistrictOfInfraction
                     );

                await _ownerNotificationHandler
                    .NotifyOwnerAsync(notificationMessage)
                    .ConfigureAwait(false);
            }
            catch (Exception)
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


        private OwnerNotificationMessage CreateNotificationMessage(
                VehicleOwnerInfo vehicleOwnerInfo,
                List<OwnerNotificationMessageAttachment> attachments,
                string infractionDate,
                string infractionDistrict,
                string ticketNumber

            )
        {

            throw new NotImplementedException();
        }
    }
}
