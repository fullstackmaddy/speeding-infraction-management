using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using FaceAPIModel = Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using LocalFaceModel = Speeding.Infraction.Management.AF01.Models;

namespace Speeding.Infraction.Management.AF01.MappingProfiles
{
    public class FaceRectangleProfile: Profile
    {
        public FaceRectangleProfile()
        {
            CreateMap<FaceAPIModel.FaceRectangle, LocalFaceModel.FaceRectangle>();
        }
    }
}
