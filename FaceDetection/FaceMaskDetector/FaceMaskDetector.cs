using FaceDetection.FaceMaskDetector.DataModels;
using Microsoft.ML;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FaceDetection.FaceMaskDetector
{
    public class FaceMaskDetector : IDisposable
    {
        private readonly MLContext mlContext;
        private readonly PredictionEngine<ImageInputData, ImageLabelPredictions> predictionEngine;
        private readonly float probabilityThreshold;

        /// <summary>
        /// 初始化口罩偵測物件
        /// </summary>
        public FaceMaskDetector(string modelName = "Resnet50_V2.zip", float probabilityThreshold = 50)
        {
            string programPath = AppDomain.CurrentDomain.BaseDirectory;
            string modelPath = Path.Combine(programPath, "models", modelName);
            
            mlContext = new MLContext();
            predictionEngine = SetupPredictionEngine(modelPath);
            this.probabilityThreshold = probabilityThreshold;
        }

        /// <summary>
        /// 偵測是否有戴口罩 (非同步)
        /// </summary>
        /// <param name="image"></param>
        public async Task<bool> DetectAsync(Bitmap image)
        {
            return await Task.Run(() => Detect(image));
        }

        /// <summary>
        /// 偵測是否有戴口罩
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool Detect(Bitmap image)
        {
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            image.Dispose();

            var prediction = predictionEngine.Predict(new ImageInputData { Image = memoryStream.ToArray() });
            Debug.WriteLine("{0}, {1}", prediction.Score[0], prediction.Score[1]);
            return prediction.Score[1] / prediction.Score.Sum() > probabilityThreshold/100 ? true : false;
        }

        /// <summary>
        /// 建置模型
        /// </summary>
        /// <param name="tensorFlowModelFilePath"> 模型路徑 </param>
        /// <returns></returns>
        private PredictionEngine<ImageInputData, ImageLabelPredictions> SetupPredictionEngine(string modelFilePath)
        {
            // Load the model
            var loadedModel = mlContext.Model.Load(modelFilePath, out _);

            // Create prediction engine to try a single prediction (input = ImageData, output = ImagePrediction)
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ImageInputData, ImageLabelPredictions>(loadedModel);

            return predictionEngine;
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
                    predictionEngine.Dispose();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        ~FaceMaskDetector()
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
