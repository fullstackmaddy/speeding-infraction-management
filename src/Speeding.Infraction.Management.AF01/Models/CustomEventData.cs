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

        [JsonProperty(PropertyName = "imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "customEvent")]
        public CustomEvent CustomEvent { get; set; }

        [JsonProperty(PropertyName = "vehicleRegistrationNumber")]
        public string VehicleRegistrationNumber { get; set; }

        [JsonProperty(PropertyName = "districtOfInfraction")]
        public string DistrictOfInfraction { get; set; }
    }
}
