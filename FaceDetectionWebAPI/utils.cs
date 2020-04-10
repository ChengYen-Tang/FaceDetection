using Emgu.CV;
using FaceDetection;
using FaceDetection.FaceMaskDetector;
using FaceDetectionWebAPI.models;
using Newtonsoft.Json;
using System;
using System.Drawing;
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
		public async static Task<FaceModel> FaceAnalysisAsync(FaceMaskDetector faceMaskDetector, byte[] imageData, FaceRectangle faceRectangle, bool mask)
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
