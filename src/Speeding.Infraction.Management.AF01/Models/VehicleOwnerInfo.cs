using Newtonsoft.Json;

namespace Speeding.Infraction.Management.AF01.Models
{
    public class VehicleOwnerInfo
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "salutation")]
        public string Salutation { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "district")]
        public string District { get; set; }

        [JsonProperty(PropertyName = "vehicleRegistrationNumber")]
        public string VehicleRegistrationNumber { get; set; }

        public string GetFullName()
        {
            return $"{this.Salutation}.{this.LastName},{this.FirstName}";
            
        }
    }
}