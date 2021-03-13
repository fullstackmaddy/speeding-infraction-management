using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Speeding.Infraction.Management.AF01.Models;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using Speeding.Infraction.Management.AF01.ConfigOptions;
using Microsoft.Extensions.Options;

namespace Speeding.Infraction.Management.AF01.Handlers.Implementations
{
    public class CosmosDmvDbHandler: IDmvDbHandler
    {
        #region Properties
        private readonly IDocumentClient _documentClient;
        private readonly CosmosDbOptions _options;
        #endregion

        #region Constructors
        public CosmosDmvDbHandler(IDocumentClient documentClient, IOptions<CosmosDbOptions> settings)
        {
            _documentClient = documentClient ??
                throw new ArgumentNullException(nameof(documentClient));

            _options = settings.Value ??
                throw new ArgumentNullException(nameof(settings));
            
        }
        #endregion

        #region PublicMethods
        public async Task<VehicleOwner> GetOwnerInformationAsync(string vehicleRegistrationNumber, string district)
        {
            Uri documentUri = UriFactory.CreateDocumentUri(
                    databaseId: _options.DatabseId,
                    collectionId: _options.OwnersCollection,
                    documentId: vehicleRegistrationNumber
                );

            var vehicleOwner = await _documentClient.ReadDocumentAsync<VehicleOwner>(
                    documentUri: documentUri,
                    new RequestOptions {PartitionKey = new PartitionKey(district) }
                )
                .ConfigureAwait(false);
            
            return vehicleOwner;

        }

        public async Task<bool> CreateSpeedingTicket(string ticketNumber, string vehicleRegistrationNumber, string district)
        {
            bool retVal = true;

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(
                    databaseId: _options.DatabseId,
                    collectionId: _options.InfractionsCollection
                );

            var document = await _documentClient.CreateDocumentAsync(
                    documentCollectionUri : collectionUri,
                    new {id = ticketNumber,
                        vehicleRegistrationNumber = vehicleRegistrationNumber,
                        district = district},

                    new RequestOptions
                    {
                        PartitionKey = new PartitionKey(district)
                    }
                );

            if(document == null)
                retVal = false;
            
            return retVal;

        }
        #endregion
    }

}