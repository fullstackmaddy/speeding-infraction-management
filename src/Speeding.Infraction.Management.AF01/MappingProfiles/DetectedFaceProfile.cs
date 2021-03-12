using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FaceAPIModel = Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using LocalFaceModel = Speeding.Infraction.Management.AF01.Models;

namespace Speeding.Infraction.Management.AF01.MappingProfiles
{
    public class DetectedFaceProfile: Profile
    {
        public DetectedFaceProfile()
        {
            CreateMap<FaceAPIModel.DetectedFace, LocalFaceModel.DetectedFace>();

        }
    }
}
