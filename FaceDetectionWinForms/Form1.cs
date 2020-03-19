using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using FaceDetection;

namespace FaceDetectionWinForms
{
    public partial class Form1 : Form
    {
        private VideoCapture capture = null;
        private readonly FaceDetector faceDetection = null;

        public Form1()
        {
            InitializeComponent();
            faceDetection = new FaceDetector();
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
                    foreach (Rectangle face in facesRectangle)
                    {
                        CvInvoke.Rectangle(frame, face, new Bgr(Color.Green).MCvScalar, 2);
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
