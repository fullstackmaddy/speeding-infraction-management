using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Options;
using Speeding.Infraction.Management.AF01.ConfigOptions;
using Speeding.Infraction.Management.AF01.Constants;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Speeding.Infraction.Management.AF01.Handlers.Implementations
{
    public class EventGridHandler : IEventHandler
    {
        private readonly IEventGridClient _eventGridClient;
        private readonly EventGridOptions _eventGridOptions;
        public EventGridHandler(IEventGridClient eventGridClient, IOptions<EventGridOptions> settings)
        {
            _eventGridClient = eventGridClient ??
                throw new ArgumentNullException(nameof(eventGridClient));

            _eventGridOptions = settings.Value;
        }
        public async Task PublishEventToTopicAsync(CustomEventData customEventData)
        {
            List<EventGridEvent> eventGridEvents = new List<EventGridEvent>
            {
                CreateEventInstance(customEventData)
            };

            await _eventGridClient
                .PublishEventsAsync(
                topicHostname: _eventGridOptions.TopicHostName,
                eventGridEvents
                )
                .ConfigureAwait(false);

        }

        private EventGridEvent CreateEventInstance(CustomEventData customEventData)
        {
            return new EventGridEvent
            {
                DataVersion = "1.0",
                EventTime = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                EventType = customEventData.CustomEvent.ToString(),
                Subject = "speeding.infraction.management.customevent",
                Data = customEventData
            };

        }

        
    }
}
