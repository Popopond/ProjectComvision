using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectComvision
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void countegg(Mat image)
        {
            // แปลงภาพเป็นภาพ grayscale
            Mat grayImage = new Mat();
            CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

            // ใช้ Sobel edge detection เพื่อหาขอบ
            Mat sobelX = new Mat();
            Mat sobelY = new Mat();
            CvInvoke.Sobel(grayImage, sobelX, DepthType.Cv16S, 1, 0); // หาขอบในแนวแกน X
            CvInvoke.Sobel(grayImage, sobelY, DepthType.Cv16S, 0, 1); // หาขอบในแนวแกน Y

            // นำผลลัพธ์ของ Sobel X และ Sobel Y มาบวกกันเพื่อหาขอบทั้งหมด
            Mat sobelEdges = new Mat();
            CvInvoke.AddWeighted(sobelX, 0.5, sobelY, 0.5, 0, sobelEdges);

            // ใช้ Threshold เพื่อทำให้ขอบที่ตรวจจับได้ชัดเจนขึ้น
            Mat thresholdedEdges = new Mat();
            CvInvoke.Threshold(sobelEdges, thresholdedEdges, 100, 255, ThresholdType.Binary);

            // ทำการ dilation เพื่อเชื่อมต่อขอบที่เป็นจุดติดกัน
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Dilate(thresholdedEdges, thresholdedEdges, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0, 0, 0));

            // แปลงภาพ thresholdedEdges เป็นภาพ CV_8UC1 ก่อนค้นหา contours
            Mat thresholdedEdges8UC1 = new Mat();
            thresholdedEdges.ConvertTo(thresholdedEdges8UC1, DepthType.Cv8U);

            // หา Contours จากขอบที่ได้
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(thresholdedEdges8UC1, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

            // นับจำนวนไข่ไก่
            int eggCount = 0;
            for (int i = 0; i < contours.Size; i++)
            {
                double contourArea = CvInvoke.ContourArea(contours[i]);
                if (contourArea > 100) // กรองขอบที่เล็กเกินไป
                {
                    eggCount++;
                    CvInvoke.DrawContours(image, contours, i, new MCvScalar(0, 255, 0), 2);
                }
            }

            // แสดงจำนวนไข่บนภาพ
            CvInvoke.PutText(image, $"Egg Count: {eggCount}", new Point(10, 30), FontFace.HersheyComplex, 1, new MCvScalar(0, 255, 0), 2);

            // แสดงภาพที่ประมวลผลแล้ว
            pictureBox2.Image = image.ToImage<Bgr, byte>().ToBitmap();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // ตั้งค่าของ OpenFileDialog
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            openFileDialog1.Title = "Select an Image File";

            // เมื่อผู้ใช้เลือกไฟล์ภาพและกด OK
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // อ่านที่อยู่ของไฟล์ภาพ
                string selectedFileName = openFileDialog1.FileName;

                // โหลดภาพและแสดงผลลัพธ์
                Mat image = CvInvoke.Imread(selectedFileName, ImreadModes.Color);
                pictureBox1.Image = image.ToImage<Bgr, byte>().ToBitmap();

                // ทำการแปลงเป็น comic
                countegg(image);
            }

        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }

    }
}