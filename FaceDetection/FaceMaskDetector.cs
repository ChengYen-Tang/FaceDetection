using FaceDetection.DataModels;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace FaceDetection
{
    public class FaceMaskDetector
    {
        private readonly MLContext context;
        private readonly ITransformer model;
        private readonly float probabilityThreshold;

        /// <summary>
        /// 初始化口罩偵測物件
        /// </summary>
        public FaceMaskDetector(string modelName = "face_mask_2.pb", float probabilityThreshold = 40)
        {
            string programPath = AppDomain.CurrentDomain.BaseDirectory;
            string modelsPath = Path.Combine(programPath, "models");
            string modelPath = Path.Combine(modelsPath, modelName);
            
            context = new MLContext();
            model = SetupModel(modelPath);
            this.probabilityThreshold = probabilityThreshold;
        }

        /// <summary>
        /// 偵測是否有戴口罩
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool Detect(ImageInputData image)
        {
            var predictor = context.Model.CreatePredictionEngine<ImageInputData, ImageLabelPredictions>(model);
            var prediction = predictor.Predict(image);
            Debug.WriteLine("{0}, {1}", prediction.PredictedLabels[0], prediction.PredictedLabels[1]);
            return prediction.PredictedLabels[0] / prediction.PredictedLabels.Sum() > probabilityThreshold/100 ? true : false;
        }

        private struct ImageSettings
        {
            // 神經網路輸入大小
            public const int imageHeight = 224;
            public const int imageWidth = 224;
            public const float mean = 117;
            public const bool channelsLast = true;
        }

        private struct TensorFlowModelSettings
        {
            // input tensor name
            public const string inputTensorName = "Placeholder:0";

            // output tensor name
            public const string outputTensorName = "loss:0";
        }

        /// <summary>
        /// 建置模型
        /// </summary>
        /// <param name="tensorFlowModelFilePath"> 模型路徑 </param>
        /// <returns></returns>
        private ITransformer SetupModel(string tensorFlowModelFilePath)
        {
            var pipeline = context.Transforms.ResizeImages(outputColumnName: TensorFlowModelSettings.inputTensorName, imageWidth: ImageSettings.imageWidth, imageHeight: ImageSettings.imageHeight, inputColumnName: nameof(ImageInputData.Image))
                .Append(context.Transforms.ExtractPixels(outputColumnName: TensorFlowModelSettings.inputTensorName, interleavePixelColors: ImageSettings.channelsLast, offsetImage: ImageSettings.mean))
                .Append(context.Model.LoadTensorFlowModel(tensorFlowModelFilePath).
                ScoreTensorFlowModel(outputColumnNames: new[] { TensorFlowModelSettings.outputTensorName },
                                    inputColumnNames: new[] { TensorFlowModelSettings.inputTensorName }, addBatchDimensionInput: false));

            ITransformer mlModel = pipeline.Fit(CreateEmptyDataView());

            return mlModel;
        }

        /// <summary>
        /// 建立一個 Placeholder
        /// </summary>
        /// <returns></returns>
        private IDataView CreateEmptyDataView()
        {
            List<ImageInputData> list = new List<ImageInputData>
            {
                new ImageInputData() { Image = new Bitmap(ImageSettings.imageWidth, ImageSettings.imageHeight) }
            };

            IDataView dataView = context.Data.LoadFromEnumerable<ImageInputData>(list);
            return dataView;
        }
    }
}
