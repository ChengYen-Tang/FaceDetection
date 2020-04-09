using Newtonsoft.Json;
using System;

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

	public class UploadCustomerImageModel
	{
		[JsonConverter(typeof(Base64FileJsonConverter))]
		public byte[] ImageData { get; set; }
	}
}
