using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
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
        private readonly IBlobHandler _blobHandler;

        public TicketController(IDmvDbHandler dmvDbHandler, IBlobHandler blobHandler)
        {
            _dmvDbHandler = dmvDbHandler ??
                throw new ArgumentNullException(nameof(dmvDbHandler));

            _blobHandler = blobHandler ??
                throw new ArgumentNullException(nameof(blobHandler));

        }


        public async Task CreateTicket(
                [EventGridTrigger] EventGridEvent eventGridEvent,
                ILogger logger
            )
        {
            StorageBlobCreatedEventData blobCreatedEventData =
               ((JObject)eventGridEvent.Data).ToObject<StorageBlobCreatedEventData>();

            string blobName = GetBlobName(blobCreatedEventData.Url);

            try
            {
                var blobMetatadata = await _blobHandler
                    .GetBlobMetadataAsync(blobCreatedEventData.Url)
                    .ConfigureAwait(false);

               
            }
            catch (Exception ex)
            { 
            }
        }




        public string GetBlobName(string blobUrl)
        {

            return Path.GetFileNameWithoutExtension(blobUrl);

        }
    }
}
