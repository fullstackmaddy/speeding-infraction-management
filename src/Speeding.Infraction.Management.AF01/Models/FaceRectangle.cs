using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Speeding.Infraction.Management.AF01.Models
{
    public class FaceRectangle
    {
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }
        
        [JsonProperty(PropertyName = "left")]
        public int Left { get; set; }
        
        [JsonProperty(PropertyName = "top")]
        public int Top { get; set; }
    }
}
