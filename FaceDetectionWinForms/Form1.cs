using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using FaceDetection;
using FaceDetection.FaceMaskDetector;
using FaceDetection.FaceMaskDetector.DataModels;
using BitmapExtension = FaceDetection.BitmapExtension;

namespace FaceDetectionWinForms
{
    public partial class Form1 : Form
    {
        private VideoCapture capture;
        private readonly FaceDetector faceDetection;
        private readonly FaceMaskDetector faceMaskDetector;

        public Form1()
        {
            InitializeComponent();
            faceDetection = new FaceDetector();
            faceMaskDetector = new FaceMaskDetector();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            capture = new VideoCapture();
            capture.ImageGrabbed += ProcessFrame;
            capture.Start();
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (capture != null)
            {
                using (Mat frame = new Mat())
                {
                    capture.Retrieve(frame, 0);
                    IEnumerable<IReadOnlyDictionary<string, int>> faces = faceDetection.Detect(frame);
                    IEnumerable<Rectangle> facesRectangle = faces.Select(face => new Rectangle(face["top"], face["left"], face["width"], face["height"]));
                    foreach (Rectangle faceRectangle in facesRectangle)
                    {
                        Mat face = FaceDetector.GetFaceImage(frame, faceRectangle);
                        bool result = faceMaskDetector.Detect(BitmapExtension.ToBitmap(face));
                        if (result)
                            CvInvoke.Rectangle(frame, faceRectangle, new Bgr(Color.Green).MCvScalar, 2);
                        else
                            CvInvoke.Rectangle(frame, faceRectangle, new Bgr(Color.Yellow).MCvScalar, 2);
                    }

                    imageBox1.Image = frame.ToImage<Bgr, byte>();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            capture.Stop();
            capture.Dispose();
        }
    }
}
