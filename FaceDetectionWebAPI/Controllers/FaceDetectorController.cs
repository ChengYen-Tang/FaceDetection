using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FaceDetection;
using FaceDetection.FaceMaskDetector;
using FaceDetectionWebAPI.models;
using Microsoft.AspNetCore.Http;
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
        public async Task<string> FindFaceAsync(byte[] imageData)
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

            return JsonConvert.SerializeObject(faces);
        }
    }
}