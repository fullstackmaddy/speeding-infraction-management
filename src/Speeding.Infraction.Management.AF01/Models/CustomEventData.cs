using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Speeding.Infraction.Management.AF01.Models
{
    public class CustomEventData
    {
        [JsonProperty(PropertyName = "ticketNumber")]
        public string TicketNumber { get; set; }
    }
}
