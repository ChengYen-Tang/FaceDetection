using Emgu.CV;
using FaceDetection;
using FaceDetection.FaceMaskDetector;
using FaceDetectionWebAPI.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace FaceDetectionWebAPI
{
	public class Base64FileJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(string);
		}


		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return Convert.FromBase64String(reader.Value as string);
		}

		//Because we are never writing out as Base64, we don't need this. 
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}

	public static class Utils
	{
		public static IEnumerable<FaceModel> FaceAnalysis(FaceMaskDetector faceMaskDetector, byte[] imageData, IEnumerable<FaceRectangle> faceRectangles, bool mask)
		{
			if (mask)
			{
				IEnumerable<Rectangle> rectangles = faceRectangles.Select(faceRectangle => new Rectangle(faceRectangle.Top, faceRectangle.Left, faceRectangle.Width, faceRectangle.Height));
				IEnumerable<Bitmap> faces = rectangles.Select(rectangle => FaceDetector.GetFaceImage(imageData, rectangle).ToBitmap());
				IEnumerable<bool> isMasks = faceMaskDetector.Detect(faces);

				return faceRectangles.Zip(isMasks, (faceRectangle, isMask) => new FaceModel { FaceRectangle = faceRectangle, FaceAttributes = new FaceAttributes { IsMask = isMask } });
			}

			return faceRectangles.Select(faceRectangle => new FaceModel { FaceRectangle = faceRectangle, FaceAttributes = new FaceAttributes()});
		}
	}
}
