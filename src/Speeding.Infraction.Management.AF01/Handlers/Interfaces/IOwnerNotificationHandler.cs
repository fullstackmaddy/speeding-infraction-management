using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Handlers.Interfaces
{
    public interface IOwnerNotificationHandler
    {
        /// <summary>
        /// Notify the Owner of the Vehicle
        /// </summary>
        /// <param name="ownerNotificationMessage">Information of the vehicle Owner</param>
        /// <returns></returns>
        public Task NotifyOwnerAsync(OwnerNotificationMessage ownerNotificationMessage);
    }
}
