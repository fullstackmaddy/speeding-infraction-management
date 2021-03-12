using System;
using System.Collections.Generic;
using System.Text;

namespace Speeding.Infraction.Management.AF01.Models
{
    public class DetectedFace
    {
        public Guid? FaceId { get; set; }

        public FaceRectangle FaceRectangle { get; set; }

    }

    
}
