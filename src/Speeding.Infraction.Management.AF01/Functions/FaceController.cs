using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Speeding.Infraction.Management.AF01.ConfigOptions;
using Speeding.Infraction.Management.AF01.Constants;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using Speeding.Infraction.Management.AF01.Helpers;
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

            var correlationId = LoggingHelper.GetCorrelationId(inputEventData);

            #region Logging

            logger.LogInformation(
                new EventId((int)LoggingConstants.EventId.DetectAndBlurFacesStarted),
                LoggingConstants.Template,
                LoggingConstants.EventId.DetectAndBlurFacesStarted.ToString(),
                correlationId,
                LoggingConstants.ProcessingFunction.DetectAndBlurFaces.ToString(),
                LoggingConstants.ProcessStatus.Started,
                "Execution Started"
                );
           
            #endregion

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
                            blobName: BlobHelper.GetBlobNameWithExtension(inputEventData.TicketNumber)
                        )
                        .ConfigureAwait(false);
                }

                outputEventData.CustomEvent = CustomEvent.FaceDetectionAndBlurringCompleted.ToString();

                #region Logging

                logger.LogInformation(
                    new EventId((int)LoggingConstants.EventId.DetectAndBlurFacesFinished),
                    LoggingConstants.Template,
                    LoggingConstants.EventId.DetectAndBlurFacesFinished.ToString(),
                    correlationId,
                    LoggingConstants.ProcessingFunction.DetectAndBlurFaces.ToString(),
                    LoggingConstants.ProcessStatus.Finished,
                    "Execution Finished"
                    );

                #endregion


            }
            catch (Exception ex)
            {
                outputEventData.CustomEvent = CustomEvent.Exceptioned.ToString();

                #region Logging

                logger.LogError(
                    new EventId((int)LoggingConstants.EventId.DetectAndBlurFacesFinished),
                    LoggingConstants.Template,
                    LoggingConstants.EventId.DetectAndBlurFacesFinished.ToString(),
                    correlationId,
                    LoggingConstants.ProcessingFunction.DetectAndBlurFaces.ToString(),
                    LoggingConstants.ProcessStatus.Failed,
                    $"Execution Failed. Reason: {ex.Message}"
                    );

                #endregion
            }


            await _eventhandler.PublishEventToTopicAsync(outputEventData)
                .ConfigureAwait(false);
            
            
        }

       


    }
}
