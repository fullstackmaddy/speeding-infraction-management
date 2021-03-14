using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Speeding.Infraction.Management.AF01.Models
{
    public class SpeedingTicket
    {
        [JsonProperty(PropertyName = "ticketNumber")]
        public string TicketNumber { get; set; }

        [JsonProperty(PropertyName = "vehicleRegistrationNumber")]
        public string  VehicleResgistrationNumber{ get; set; }

        [JsonProperty(PropertyName = "district")]
        public string District { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }
    }
}
