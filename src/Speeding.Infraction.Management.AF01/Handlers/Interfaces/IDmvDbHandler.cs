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
        /// <param name="district">The district where the vehicle is registered</param>
        /// <returns>Owner of the registered vehicle</returns>
        public Task<VehicleOwner> GetOwnerInformationAsync(string vehicleRegistrationNumber, string district);

        /// <summary>
        /// Create a speeding infraction ticket against a vehicle registration number
        /// </summary>
        /// <param name="ticketNumber">Ticket number</param>
        /// <param name="vehicleRegistrationNumber">Vehicle registration number</param>
        /// <param name="district">The district where the infraction occured</param>
        /// <returns></returns>
        public Task<bool> CreateSpeedingTicketAsync(string ticketNumber, string vehicleRegistrationNumber, string district);


    }
}