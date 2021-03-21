using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Handlers.Interfaces
{
    public interface IEventHandler
    {
        public Task PublishEventToTopicAsync(CustomEventData customEventData);
    }
}
