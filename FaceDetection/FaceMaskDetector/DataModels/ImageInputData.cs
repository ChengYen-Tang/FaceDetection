using Microsoft.ML.Transforms.Image;
using System.Drawing;

namespace FaceDetection.FaceMaskDetector.DataModels
{
    public class ImageInputData
    {
        [ImageType(299, 299)]
        public byte[] Image { get; set; }
    }
}
