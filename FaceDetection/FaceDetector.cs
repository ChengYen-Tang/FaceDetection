using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FaceDetection
{
    public class FaceDetector : IDisposable
    {
        private readonly Net detectorModel;

        /// <summary>
        /// 初始化臉部偵測物件
        /// </summary>
        public FaceDetector()
        {
            string programPath = AppDomain.CurrentDomain.BaseDirectory;
            string modelsPath = Path.Combine(programPath, "models");
            string detectionPbPath = Path.Combine(modelsPath, "opencv_face_detector_uint8.pb");
            string detectionPbtxtPath = Path.Combine(modelsPath, "opencv_face_detector.pbtxt");

            detectorModel = DnnInvoke.ReadNetFromTensorflow(detectionPbPath, detectionPbtxtPath);
        }

        /// <summary>
        /// 偵測圖片中的臉部 (非同步)
        /// </summary>
        /// <param name="imageData"> 圖片 </param>
        /// <returns> 臉部座標 (X, Y, 寬度, 高度) </returns>
        public async Task<IEnumerable<IReadOnlyDictionary<string, int>>> DetectAsync(byte[] imageData)
        {
            return await Task.Run(() => Detect(imageData));
        }

        /// <summary>
        /// 偵測圖片中的臉部 (非同步)
        /// </summary>
        /// <param name="image"> 圖片 </param>
        /// <returns> 臉部座標 (X, Y, 寬度, 高度) </returns>
        public async Task<IEnumerable<IReadOnlyDictionary<string, int>>> DetectAsync(Mat image)
        {
            return await Task.Run(() => Detect(image));
        }

        /// <summary>
        /// 偵測圖片中的臉部
        /// </summary>
        /// <param name="imageData"> 圖片 </param>
        /// <returns> 臉部座標 (X, Y, 寬度, 高度) </returns>
        public IEnumerable<IReadOnlyDictionary<string, int>> Detect(byte[] imageData)
        {
            Mat mat = new Mat();
            CvInvoke.Imdecode(imageData, ImreadModes.Color, mat);
            return Detect(mat);
        }

        /// <summary>
        /// 偵測圖片中的臉部
        /// </summary>
        /// <param name="image"> 圖片 </param>
        /// <returns> 臉部座標 (X, Y, 寬度, 高度) </returns>
        public IEnumerable<IReadOnlyDictionary<string, int>> Detect(Mat image)
        {
            Mat inputBlob = DnnInvoke.BlobFromImage(image, 1.0, new Size(300, 300), new MCvScalar(104.0, 117.0, 123.0), true, false);
            detectorModel.SetInput(inputBlob, "data");
            Mat detection = detectorModel.Forward("detection_out");

            // 神經網路輸出形狀 (1, 1, {face count}, 7)
            // 第四項內容是人臉偵測結果的值共七個[0, 1, 2, 3, 4, 5, 6, 7]
            // 其中第三個值(2的位置)：人臉偵測的信任分數，越高表示越像人臉
            // 第四~七的值(3,4,5,6的位置)分別代表人臉的左上角(x, y)到右下角(x, y) 位置的比例
            int resultFaceCount = detection.SizeOfDimension[2];
            int resultFaceInfoLength = detection.SizeOfDimension[3];

            float[] facesInfo = new float[resultFaceCount * resultFaceInfoLength];
            Marshal.Copy(detection.DataPointer, facesInfo, 0, facesInfo.Length);

            for (int faceIndex = 0; faceIndex < resultFaceCount; faceIndex++)
            {
                float faceConfidence = facesInfo[faceIndex * resultFaceInfoLength + 2];
                int x1 = Convert.ToInt32(facesInfo[faceIndex * resultFaceInfoLength + 3] * image.Width);
                int y1 = Convert.ToInt32(facesInfo[faceIndex * resultFaceInfoLength + 4] * image.Height);
                int x2 = Convert.ToInt32(facesInfo[faceIndex * resultFaceInfoLength + 5] * image.Width);
                int y2 = Convert.ToInt32(facesInfo[faceIndex * resultFaceInfoLength + 6] * image.Height);

                if (faceConfidence > 0.7 &&
                    x1 < image.Width && x2 <= image.Width &&
                    y1 < image.Height && y2 <= image.Height)
                {
                    Dictionary<string, int> face = new Dictionary<string, int>
                    {
                        { "top", x1 },
                        { "left", y1 },
                        { "width", x2 - x1 },
                        { "height", y2 - y1 },
                        { "x1", x1 },
                        { "x2", x2 },
                        { "y1", y1 },
                        { "y2", y2 }
                    };

                    yield return face;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    detectorModel.Dispose();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        ~FaceDetector()
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
