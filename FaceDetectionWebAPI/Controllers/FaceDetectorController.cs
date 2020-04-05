using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Emgu.CV;
using FaceDetection;
using FaceDetection.FaceMaskDetector;
using FaceDetectionWebAPI.models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FaceDetectionWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceDetectorController : ControllerBase
    {
        private FaceDetector faceDetector;
        private FaceMaskDetector faceMaskDetector;

        public FaceDetectorController(FaceDetector faceDetector, FaceMaskDetector faceMaskDetector)
        {
            this.faceDetector = faceDetector;
            this.faceMaskDetector = faceMaskDetector;
        }

        [HttpPost]
        public async Task<string> FaceDetection(byte[] imageData)
        {
            IEnumerable<FaceRectangle> faces = (await faceDetector.DetectAsync(imageData))
                .Select(face => new FaceRectangle { 
                    Height = face["height"], 
                    Left = face["left"],
                    Top = face["top"],
                    Width = face["width"],
                    X1 = face["x1"],
                    X2 = face["x2"],
                    Y1 = face["y1"],
                    Y2 = face["y2"]
                });

            return JsonConvert.SerializeObject(faces.Select(face => new FaceModel { FaceRectangle = face, FaceAttributes = new FaceAttributes() }));
        }

        [HttpPost]
        public async Task<string> MaskDetection(byte[] imageData)
        {
            IEnumerable<FaceRectangle> faces = (await faceDetector.DetectAsync(imageData))
                .Select(face => new FaceRectangle
                {
                    Height = face["height"],
                    Left = face["left"],
                    Top = face["top"],
                    Width = face["width"],
                    X1 = face["x1"],
                    X2 = face["x2"],
                    Y1 = face["y1"],
                    Y2 = face["y2"]
                });


            List<FaceModel> faceModels = new List<FaceModel>();
            foreach (var face in faces)
                faceModels.Add(await FaceAnalysisAsync(imageData, face, true));

            return JsonConvert.SerializeObject(faceModels);
        }

        private async Task<FaceModel> FaceAnalysisAsync(byte[] imageData, FaceRectangle faceRectangle, bool mask)
        {
            FaceModel faceModel = new FaceModel { FaceRectangle = faceRectangle, FaceAttributes = new FaceAttributes() };
            if (mask)
            {
                Rectangle rectangle = new Rectangle(faceRectangle.Top, faceRectangle.Left, faceRectangle.Width, faceRectangle.Height);
                Mat face = FaceDetector.GetFaceImage(imageData, rectangle);
                faceModel.FaceAttributes.IsMask = await faceMaskDetector.DetectAsync(face.ToBitmap());
            }

            return faceModel;
        }
    }
}