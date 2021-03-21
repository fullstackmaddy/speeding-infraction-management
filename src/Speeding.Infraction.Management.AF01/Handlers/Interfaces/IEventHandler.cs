using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Handlers.Interfaces
{
    public interface IEventHandler
    {
        /// <summary>
        /// Publish a custom event to the event sink
        /// </summary>
        /// <param name="customEventData">Customn Event Data</param>
        /// <returns></returns>
        public Task PublishEventToTopicAsync(CustomEventData customEventData);
    }
}
