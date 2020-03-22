using Microsoft.ML.Data;

namespace FaceDetection.DataModels
{
    public class ImageLabelPredictions
    {
        //TODO: Change to fixed output column name for TensorFlow model
        [ColumnName("loss:0")]
        public float[] PredictedLabels { get; set; }
    }
}
