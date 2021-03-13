﻿using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Speeding.Infraction.Management.AF01.Constants;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
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

            string blobName = GetBlobName(blobCreatedEventData.Url);

            string registrationNumber = await _computerVisionHandler
                .ExtractRegistrationNumberWithUrlAsync(blobCreatedEventData.Url)
                .ConfigureAwait(false);

            var metadata = await _blobHandler
                .GetBlobMetadataAsync(blobCreatedEventData.Url);

            CustomEventData customEventData = new CustomEventData()
            {
                ImageUrl = blobCreatedEventData.Url,
                TicketNumber = blobName,
                DistrictOfInfraction = metadata["districtofinfraction"]
                
            };

            if (!string.IsNullOrWhiteSpace(registrationNumber))
            {
                customEventData.VehicleRegistrationNumber = registrationNumber;
                customEventData.CustomEvent = CustomEvent.NumberExtractionCompleted;
            }
            else
            {
                customEventData.CustomEvent = CustomEvent.Exceptioned;
            }

            await _eventHandler
                .PublishEventToTopicAsync(customEventData)
                .ConfigureAwait(false);
                                                                                                                                                                                                                                                             
        }

        public string GetBlobName(string blobUrl)
        {

            return Path.GetFileNameWithoutExtension(blobUrl);

        }
    }
}