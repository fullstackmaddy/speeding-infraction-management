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
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Functions
{
    public class ExceptionController
    {
        private IBlobHandler _blobHandler;
        private readonly BlobOptions _options;

        public ExceptionController(IBlobHandler blobHandler, 
            IOptions<BlobOptions> settings)
        {
            _blobHandler = blobHandler ??
                throw new ArgumentNullException(nameof(blobHandler));

            _options = settings.Value;

        }

        [FunctionName("ManageExeceptions")]
        public async Task ManageExceptions(
                [EventGridTrigger] EventGridEvent eventGridEvent,
                ILogger logger
            )
        {
            CustomEventData inputEventData =
                       ((JObject)eventGridEvent.Data).ToObject<CustomEventData>();

            var correlationId = LoggingHelper.GetCorrelationId(inputEventData);

            #region Logging
            logger.LogInformation(
                new EventId((int)LoggingConstants.EventId.ManageExeceptionsStarted),
                LoggingConstants.Template,
                LoggingConstants.EventId.ManageExeceptionsStarted.ToString(),
                correlationId,
                LoggingConstants.ProcessingFunction.ManageExeceptions.ToString(),
                LoggingConstants.ProcessStatus.Started.ToString(),
                "Execution started"
                );
            #endregion

            await _blobHandler
                .CopyBlobAcrossContainerWithUrlsAsync(inputEventData.ImageUrl, _options.CompensationContainerName)
                .ConfigureAwait(false);

            #region Logging

            logger.LogInformation(
                new EventId((int)LoggingConstants.EventId.ManageExeceptionsFinished),
                LoggingConstants.Template,
                LoggingConstants.EventId.ManageExeceptionsFinished.ToString(),
                correlationId,
                LoggingConstants.ProcessingFunction.ManageExeceptions.ToString(),
                LoggingConstants.ProcessStatus.Finished.ToString(),
                "Execution Finished"
                );
            #endregion
        }
    }
}
