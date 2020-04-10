using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FaceDetectionWebAPI.models
{
    public class FaceRectangle
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int Y1 { get; set; }
        public int Y2 { get; set; }
    }

    public class FaceAttributes
    {
        public bool IsMask { get; set; }
        public bool IsFever { get; set; }
    }

    public class FaceModel
    { 
        public FaceRectangle FaceRectangle { get; set; }
        public FaceAttributes FaceAttributes { get; set; }
    }

    public class UploadCustomerImageModel
    {
        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] ImageData { get; set; }
    }
}
