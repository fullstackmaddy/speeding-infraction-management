using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Functions
{
    public class FaceController
    {
        private readonly object _faceHandler;
        private readonly IBlobHandler _blobHandler;

        public FaceController(IFaceHandler faceHandler,
            IBlobHandler blobHandler)
        {
            _faceHandler = faceHandler ??
                throw new ArgumentNullException(nameof(faceHandler));

            _blobHandler = blobHandler ??
                throw new ArgumentNullException(nameof(blobHandler));

        }

        [FunctionName("DetectFaces")]
        public async Task DetectFaces(
            [EventGridTrigger] EventGridEvent eventGridEvent,
                ILogger logger
            )
        {
            
        }
    }
}
