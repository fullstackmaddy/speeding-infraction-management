using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using FaceAPIModel = Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using LocalFaceModel = Speeding.Infraction.Management.AF01.Models;

using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using System.IO;
using Speeding.Infraction.Management.AF01.Models;
using ImageProcessorCore;

namespace Speeding.Infraction.Management.AF01.Handlers.Implementations
{
    public class FaceHandler : IFaceHandler
    {
        #region Properties
        private readonly IFaceClient _faceClient;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors
        public FaceHandler(IFaceClient faceClient, IMapper mapper)
        {
            _faceClient = faceClient
                ?? throw new ArgumentNullException(nameof(faceClient));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Stream> BlurFacesAsync(Stream imageStream, List<LocalFaceModel.DetectedFace> detectedFaces)
        {
            Image<Color, uint> image = await Task
                .Factory.StartNew(() => BlurFaces(imageStream, detectedFaces))
                .ConfigureAwait(false);

            Stream processedImage = new MemoryStream();
            image.Save(processedImage);

            return processedImage;

        }

        private static Image<Color, uint> BlurFaces(Stream imageStream, List<DetectedFace> detectedFaces)
        {
            var image = new Image<Color, uint>(imageStream);

            foreach (var detectedFace in detectedFaces)
            {
                var rectangle = new Rectangle
                {
                    Height = detectedFace.FaceRectangle.Height,
                    Width = detectedFace.FaceRectangle.Width,
                    X = detectedFace.FaceRectangle.Left,
                    Y = detectedFace.FaceRectangle.Top
                };

                image = image.BoxBlur(detectedFace.FaceRectangle.Height, rectangle);

            }

            return image;
        }
        #endregion


        public async Task<IEnumerable<LocalFaceModel.DetectedFace>> DetectFacesWithUrlAsync(string url)
        {
            var detectedFaces = await _faceClient
                .Face.DetectWithUrlAsync(
                    url,
                    recognitionModel: FaceAPIModel.RecognitionModel.Recognition03
                )
                .ConfigureAwait(false);

            return _mapper.Map<List<LocalFaceModel.DetectedFace>>
                (
                    detectedFaces.ToList()
                );
            
        }

        public async Task<IEnumerable<DetectedFace>> DetectFacesWithStreamAsync(Stream imageStream)
        {
            var detectedFaces = await _faceClient
                 .Face.DetectWithStreamAsync(
                imageStream,
                recognitionModel: FaceAPIModel.RecognitionModel.Recognition03
                )
                 .ConfigureAwait(false);

            return _mapper.Map<List<LocalFaceModel.DetectedFace>>
                (
                    detectedFaces.ToList()
                );
        }
    }
}
