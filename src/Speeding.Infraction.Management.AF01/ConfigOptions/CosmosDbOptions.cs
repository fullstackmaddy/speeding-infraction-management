using System;
using System.Collections.Generic;
using System.Text;

namespace Speeding.Infraction.Management.AF01.ConfigOptions
{
    public class CosmosDbOptions
    {
        public string DatabseId { get; set; }

        public string OwnersCollection { get; set; }

        public string InfractionsCollection { get; set; }
    }
}
