using FaceDetection;
using FaceDetection.FaceMaskDetector;
using FaceDetectionWebAPI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FaceDetectionWebAPI.models;
using Newtonsoft.Json.Linq;
using FaceDetectionWebAPI;

namespace FaceDetectionTest
{
    [TestClass]
    public class FaceDetectionWebAPITest : IDisposable
    {
        private FaceDetector faceDetector;
        private FaceMaskDetector faceMaskDetector;
        private FaceDetectorController controller;
        private readonly string imageFolder;

        public FaceDetectionWebAPITest()
        {
            faceDetector = new FaceDetector();
            faceMaskDetector = new FaceMaskDetector();
            controller = new FaceDetectorController(faceDetector, faceMaskDetector);
            string programPath = AppDomain.CurrentDomain.BaseDirectory;
            imageFolder = Path.Combine(programPath, "Image", "FaceDetector");
        }

        [TestMethod]
        public async Task NoFacePictureTestAsync()
        {
            string imagePath = Path.Combine(imageFolder, "Taiwan.png");
            Bitmap image = new Bitmap(imagePath);

            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            image.Dispose();

            var facesJson = await controller.FaceDetection(new UploadCustomerImageModel { ImageData = memoryStream.ToArray() });
            var faces = JsonConvert.DeserializeObject<IEnumerable<FaceModel>>(facesJson);
            Assert.AreEqual(faces.ToArray().Length, 0);
        }

        [TestMethod]
        public async Task TwoFacePictureTestAsync()
        {
            string imagePath = Path.Combine(imageFolder, "13_Interview_Interview_2_People_Visible_13_274.jpg");
            Bitmap image = new Bitmap(imagePath);

            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            image.Dispose();

            var facesJson = await controller.FaceDetection(new UploadCustomerImageModel { ImageData = memoryStream.ToArray() });
            var faces = JsonConvert.DeserializeObject<IEnumerable<FaceModel>>(facesJson);
            Assert.AreEqual(faces.ToArray().Length, 2);
        }

        [TestMethod]
        public async Task FaceMaskPictureTestAsync()
        {
            string imagePath = Path.Combine(imageFolder, "FaceMask.jpg");
            Bitmap image = new Bitmap(imagePath);

            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            image.Dispose();

            var facesJson = await controller.FaceDetection(new UploadCustomerImageModel { ImageData = memoryStream.ToArray() });
            var faces = JsonConvert.DeserializeObject<IEnumerable<FaceModel>>(facesJson);
            Assert.AreEqual(faces.ToArray().Length, 2);
        }

        [TestMethod]
        public async Task FaceMaskTestAsync()
        {
            string imagePath = Path.Combine(imageFolder, "FaceMask.jpg");
            Bitmap image = new Bitmap(imagePath);

            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            image.Dispose();

            var facesJson = await controller.MaskDetection(new UploadCustomerImageModel { ImageData = memoryStream.ToArray() });
            var faces = JsonConvert.DeserializeObject<IEnumerable<FaceModel>>(facesJson);
            foreach (var face in faces)
                Assert.IsTrue(face.FaceAttributes.IsMask);
        }

        [TestMethod]
        public async Task NoMaskTestAsync()
        {
            string imagePath = Path.Combine(imageFolder, "13_Interview_Interview_2_People_Visible_13_274.jpg");
            Bitmap image = new Bitmap(imagePath);

            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            image.Dispose();

            var facesJson = await controller.MaskDetection(new UploadCustomerImageModel { ImageData = memoryStream.ToArray() });
            var faces = JsonConvert.DeserializeObject<IEnumerable<FaceModel>>(facesJson);
            foreach (var face in faces)
                Assert.IsFalse(face.FaceAttributes.IsMask);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。
                    faceDetector.Dispose();
                    faceMaskDetector.Dispose();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        ~FaceDetectionWebAPITest()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
