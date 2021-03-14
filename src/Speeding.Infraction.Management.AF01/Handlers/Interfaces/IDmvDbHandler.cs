using System.Threading.Tasks;
using Speeding.Infraction.Management.AF01.Models;
namespace Speeding.Infraction.Management.AF01.Handlers.Interfaces
{
    public interface IDmvDbHandler
    {
        /// <summary>
        /// Get the information of the registered owner for the vehicle using the vehicle registration number
        /// </summary>
        /// <param name="vehicleRegistrationNumber">Vehicle registration number</param>
        /// <returns>Owner of the registered vehicle</returns>
        public Task<VehicleOwnerInfo> GetOwnerInformationAsync(string vehicleRegistrationNumber);

        /// <summary>
        /// Create a speeding infraction ticket against a vehicle registration number
        /// </summary>
        /// <param name="ticketNumber">Ticket number</param>
        /// <param name="vehicleRegistrationNumber">Vehicle registration number</param>
        /// <param name="district">The district where the infraction occured</param>
        /// <returns></returns>
        public Task CreateSpeedingTicketAsync(string ticketNumber, string vehicleRegistrationNumber, string district);


    }
}