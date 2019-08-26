﻿using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmguCVTest
{
    public partial class Form1 : Form
    {
        
        private VideoCapture capture = new VideoCapture(0, VideoCapture.API.DShow);
        private ImageViewer viewer = new ImageViewer();
        private CascadeClassifier cascadeClassifier;
        private Rectangle _myFaceRect;
        private Image<Gray, Byte> _myFace = null;
        private bool saveToFile = false;


        private Timer timer1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cascadeClassifier = new CascadeClassifier(Application.StartupPath + "\\cascades\\haarcascade_frontalface_default.xml");

            timer1 = new Timer();
            timer1.Tick += new EventHandler(processSnap);
            timer1.Interval = 200; // in miliseconds
            timer1.Start();

            //Application.Idle += new EventHandler(delegate (object send, EventArgs er)
            //{
            //    //viewer.Image = capture.QueryFrame();
            //    //using(Mat mat = capture.QueryFrame())
            //    //{
            //    //    if(mat != null)
            //    //    {
            //    //        Image<Gray, byte> grayFrame = mat.ToImage<Gray, byte>();


            //    //    }
            //    //}

               

            //});

            //viewer.ShowDialog();
        }

        private void btnCaptureClick(object sender, EventArgs e)
        {
            saveToFile = true;
        }


        private void processSnap(object sender, EventArgs e)
        {
            using (Image<Bgr, Byte> imageFrame = capture.QueryFrame().ToImage<Bgr, Byte>())
            {
                if (imageFrame != null)
                {
                    var grayFrame = imageFrame.Convert<Gray, byte>();
                    var faces = cascadeClassifier.DetectMultiScale(grayFrame, 1.1, 10, Size.Empty);

                    if (faces.Length > 0)
                    {
                        _myFaceRect = faces[0];
                        _myFace = grayFrame;

                        int lastHeight = 0;
                        int lastWidth = 0;

                        foreach (var face in faces)
                        {
                            if (lastHeight < face.Height || lastWidth < face.Width)
                            {
                                lastWidth = face.Width;
                                lastHeight = face.Height;
                                _myFaceRect = face;

                            }
                        }
                        if (saveToFile)
                        {
                            //int label = (int)_myFace[].Intensity;
                            //var temp = _myFace.
                            //VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                            //Mat m = new Mat();
                            //CvInvoke.FindContours(imageFrame, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                            if (_myFaceRect != null)
                            {
                                var temp = imageFrame;
                                saveToFile = !saveToFile;
                                //Rectangle bbox = CvInvoke.BoundingRectangle(contours[0]);
                                var cropFaceRect = new Rectangle(_myFaceRect.Location.X - 50, _myFaceRect.Location.Y - 50, _myFaceRect.Width + 100, _myFaceRect.Height + 100);
                                temp.ROI = cropFaceRect;

                                var img = temp.Copy();
                                temp.ROI = Rectangle.Empty;

                                picBox.Image = img.Bitmap;
                                Bitmap bImage = img.Bitmap;  // Your Bitmap Image
                                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                                bImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                byte[] byteImage = ms.ToArray();
                                string base64String = Convert.ToBase64String(byteImage);

                                txtBase64.Text = base64String;
                            }


                            //imageFrame.Save( @"C:\faces\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");
                            //saveToFile = !saveToFile;
                            //MessageBox.Show("Captured");
                        }

                        imageFrame.Draw(_myFaceRect, new Bgr(Color.BurlyWood), 3);

                    }


                }
                //viewer.Image = imageFrame;
                //viewer.ShowDialog();
                imageViewer.Image = imageFrame;
                //bool show = false;
                //if (!show)
                //{
                //    show = !show;
                //    this.Visible = false;
                //    viewer.ShowDialog();
                //}
            }
        }
    }
}