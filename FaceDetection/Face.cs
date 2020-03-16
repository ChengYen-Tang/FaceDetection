using Emgu.CV.Dnn;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceDetection
{
    public abstract class Face
    {
        protected Net model;

        protected Face()
        {

        }
    }
}
