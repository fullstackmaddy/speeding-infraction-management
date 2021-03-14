using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Speeding.Infraction.Management.AF01.ConfigOptions;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Functions
{
    public class FaceController
    {
        private readonly IFaceHandler _faceHandler;
        private readonly IBlobHandler _blobHandler;
        private readonly IEventHandler _eventhandler;
        private readonly BlobOptions _options;

        public FaceController(IFaceHandler faceHandler,
            IBlobHandler blobHandler,
            IEventHandler eventHandler,
            IOptions<BlobOptions> settings)
        {
            _faceHandler = faceHandler ??
                throw new ArgumentNullException(nameof(faceHandler));

            _blobHandler = blobHandler ??
                throw new ArgumentNullException(nameof(blobHandler));

            _eventhandler = eventHandler ??
                throw new ArgumentNullException(nameof(eventHandler));

            _options = settings.Value;

        }

        [FunctionName("DetectAndBlurFaces")]
        public async Task DetectAndBlurFaces(
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

            try
            {
               

                var detectedFaces = await _faceHandler
                    .DetectFacesWithUrlAsync(inputEventData.ImageUrl)
                    .ConfigureAwait(false);

                var imageBytes = await _blobHandler
                    .DownloadBlobAsync(inputEventData.ImageUrl)
                    .ConfigureAwait(false);

                var blurredImageBytes = await _faceHandler
                    .BlurFacesAsync(imageBytes, detectedFaces.ToList());

                using (MemoryStream ms = new MemoryStream(blurredImageBytes))
                {
                    await _blobHandler.UploadStreamAsBlobAsync(
                            containerName: _options.BlurredImageContainerName,
                            stream: ms,
                            contentType: _options.UploadContentType,
                            blobName: inputEventData.TicketNumber
                        )
                        .ConfigureAwait(false);
                }

                outputEventData.CustomEvent = CustomEvent.FaceDetectionAndBlurringCompleted;
            }
            catch (Exception)
            {
                outputEventData.CustomEvent = CustomEvent.Exceptioned;
                
            }


            await _eventhandler.PublishEventToTopicAsync(outputEventData)
                .ConfigureAwait(false);
            
            
        }
    }
}
