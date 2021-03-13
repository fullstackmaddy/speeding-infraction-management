using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Handlers.Implementations
{
    public class ComputerVisionHandler : IComputerVisionHandler
    {
        #region Properties
        private readonly IComputerVisionClient _computerVisionClient;
        private const string textLanguage = "en";
        #endregion

        #region Constructors
        public ComputerVisionHandler(IComputerVisionClient computerVisionClient)
        {
            _computerVisionClient = computerVisionClient
                ?? throw new ArgumentNullException(nameof(computerVisionClient));

        }
        #endregion

        #region PublicMethods
        public async Task<string> ExtractRegistrationNumberWithStreamAsync(Stream imageStream)
        {
            var textHeaders = await _computerVisionClient
                .ReadInStreamAsync(
                    image: imageStream,
                    language: textLanguage
                )
                .ConfigureAwait(false);

            string operationLocation = textHeaders.OperationLocation;

            var readResult = await GetReadResultsAsync(operationLocation)
                .ConfigureAwait(false);

            string registrationNumber =
                await Task.Factory.StartNew(
                        () => ParseReadResult(readResult)
                        )
                .ConfigureAwait(false);

            return registrationNumber;
        }

        public async Task<string> ExtractRegistrationNumberWithUrlAsync(string imageUrl)
        {
            var textHeaders = await _computerVisionClient
                .ReadAsync(
                    url: imageUrl,
                    language: textLanguage
                )
                .ConfigureAwait(false);

            var operationLocation = textHeaders.OperationLocation;

            var readResult = await GetReadResultsAsync(operationLocation)
                .ConfigureAwait(false);

            string registrationNumber =
                await Task.Factory.StartNew(
                        () => ParseReadResult(readResult)
                    )
                .ConfigureAwait(false);

            return registrationNumber;
        }

        public async Task<ReadOperationResult> GetReadResultsAsync(string operationLocation)
        {
            var numberofCharsInOperationId = 36;

            var operationId = operationLocation
                .Substring(operationLocation.Length - numberofCharsInOperationId);

            ReadOperationResult result;
            Thread.Sleep(2000);

            do
            {
                result = await _computerVisionClient
                    .GetReadResultAsync(
                        operationId: Guid.Parse(operationId)
                    )
                    .ConfigureAwait(false);

            }
            while (
                (
                    result.Status == OperationStatusCodes.Running ||
                    result.Status == OperationStatusCodes.NotStarted
                )
            );

            return result;
        }

        public string ParseReadResult(ReadOperationResult result)
        {
            var detectedTexts = result.AnalyzeResult.ReadResults;

            if (detectedTexts.Count != 1)
                return string.Empty;

            if (detectedTexts[0].Lines.Count != 1)
                return string.Empty;

            return detectedTexts[0].Lines[0].Text;
            
        }

        #endregion
    }
}
