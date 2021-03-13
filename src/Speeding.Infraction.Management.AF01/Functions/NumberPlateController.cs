using Microsoft.Azure.EventGrid.Models;
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

        public NumberPlateController(IComputerVisionHandler computerVisionHandler)
        {
            _computerVisionHandler = computerVisionHandler ??
                throw new ArgumentNullException(nameof(computerVisionHandler));

        }


        [FunctionName("ExtractRegistrationNumber")]
        [return:EventGrid(TopicEndpointUri = "MyEventGridTopicUriSetting", TopicKeySetting = "MyEventGridTopicKeySetting")]
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

            EventGridEvent outputEvent = new EventGridEvent
            {
                DataVersion = "1.0",
                EventTime = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
            };

            if (!string.IsNullOrWhiteSpace(registrationNumber))
            {
                outputEvent.Subject = string.Format(CustomEventDataConstants.Subject,
                    CustomEvent.NumberExtractionCompleted.ToString());
                outputEvent.EventType = CustomEvent.NumberExtractionCompleted.ToString();
                outputEvent.Data = new CustomEventData
                {
                    TicketNumber = blobName
                };
  
                    
            }
            else
            {
                outputEvent.Subject = string.Format(CustomEventDataConstants.Subject,
                    CustomEvent.Exceptioned.ToString());
                outputEvent.EventType = CustomEvent.Exceptioned.ToString();
            }
                                                                                                                                                                                                                                                             
        }

        public string GetBlobName(string blobUrl)
        {

            return Path.GetFileNameWithoutExtension(blobUrl);

        }
    }
}
