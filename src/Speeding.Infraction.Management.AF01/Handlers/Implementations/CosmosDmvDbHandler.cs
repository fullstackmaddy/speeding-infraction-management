using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Speeding.Infraction.Management.AF01.Models;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using Speeding.Infraction.Management.AF01.ConfigOptions;
using Microsoft.Extensions.Options;
using System.Linq;

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
        public async Task<VehicleOwnerInfo> GetOwnerInformationAsync(string vehicleRegistrationNumber)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(
                    databaseId: _options.DatabseId,
                    collectionId: _options.OwnersCollection
                );


            var documentquery = await Task.Factory.StartNew(
               () => _documentClient.CreateDocumentQuery<VehicleOwnerInfo>(
                   collectionUri,
                   new FeedOptions
                   {
                       MaxItemCount = 1,
                       EnableCrossPartitionQuery = true
                   }
               )
               .Where(x => x.VehicleRegistrationNumber == vehicleRegistrationNumber))
               .ConfigureAwait(false);

            return documentquery.ToList()[0];

        }

        public async Task CreateSpeedingTicketAsync(string ticketNumber, string vehicleRegistrationNumber, string district, string date)
        {
            

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(
                    databaseId: _options.DatabseId,
                    collectionId: _options.InfractionsCollection
                );

            var document = await _documentClient.CreateDocumentAsync(
                    documentCollectionUri : collectionUri,
                    new {id = Guid.NewGuid().ToString(),
                        ticketNumber = ticketNumber,
                        vehicleRegistrationNumber = vehicleRegistrationNumber,
                        district = district,
                        date = date
                        },

                    new RequestOptions
                    {
                        PartitionKey = new PartitionKey(district)
                    }
                );

        }

        public async Task<SpeedingTicket> GetSpeedingTicketInfoAsync(string ticketNumber)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(
                    databaseId: _options.DatabseId,
                    collectionId: _options.InfractionsCollection
                );

            var documentquery = await Task.Factory.StartNew(
                () =>_documentClient.CreateDocumentQuery<SpeedingTicket>(
                    collectionUri,
                    new FeedOptions
                    {
                        MaxItemCount = 1,
                        EnableCrossPartitionQuery = true
                    }
                )
                .Where(x => x.TicketNumber == ticketNumber))
                .ConfigureAwait(false);
            
            return documentquery.ToList()[0];
        }
        #endregion
    }

}