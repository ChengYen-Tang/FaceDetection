using FaceDetection.FaceMaskDetector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace FaceDetectionTest
{
    [TestClass]
    public class FaceMaskDetectorTest : IDisposable
    {
        private FaceMaskDetector faceMaskDetector;
        private readonly string imageFolder;
        public FaceMaskDetectorTest()
        {
            faceMaskDetector = new FaceMaskDetector();
            string programPath = AppDomain.CurrentDomain.BaseDirectory;
            imageFolder = Path.Combine(programPath, "Image", "FaceMaskDetector");
        }

        [TestMethod]
        public void FaceTest()
        {
            string faceFolder = Path.Combine(imageFolder, "Face");

            foreach(var facePath in Directory.GetFiles(faceFolder, "*.jpg"))
            {
                Bitmap image = new Bitmap(facePath);

                var isMask = faceMaskDetector.Detect(image);
                Assert.IsFalse(isMask);
            }
        }

        [TestMethod]
        public void FaceMultiplePredictTest()
        {
            string faceFolder = Path.Combine(imageFolder, "Face");

            IEnumerable<Bitmap> images = Directory.GetFiles(faceFolder, "*.jpg").Select(faceMaskPath => new Bitmap(faceMaskPath));

            var isMasks = faceMaskDetector.Detect(images);

            Assert.AreEqual(isMasks.Count(), images.Count());

            foreach (var isMask in isMasks)
                Assert.IsFalse(isMask);
        }

        [TestMethod]
        public void FaceMaskTest()
        {
            string faceMaskFolder = Path.Combine(imageFolder, "FaceMask");

            foreach (var faceMaskPath in Directory.GetFiles(faceMaskFolder, "*.jpg"))
            {
                Bitmap image = new Bitmap(faceMaskPath);

                var isMask = faceMaskDetector.Detect(image);
                Assert.IsTrue(isMask);
            }
        }

        [TestMethod]
        public void FaceMaskMultiplePredictTest()
        {
            string faceMaskFolder = Path.Combine(imageFolder, "FaceMask");

            IEnumerable<Bitmap> images = Directory.GetFiles(faceMaskFolder, "*.jpg").Select(faceMaskPath => new Bitmap(faceMaskPath));

            var isMasks = faceMaskDetector.Detect(images);

            Assert.AreEqual(isMasks.Count(), images.Count());

            foreach (var isMask in isMasks)
                Assert.IsTrue(isMask);
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
                    faceMaskDetector.Dispose();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        ~FaceMaskDetectorTest()
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
