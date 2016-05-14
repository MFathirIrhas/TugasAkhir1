using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Imaging;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;


namespace TugasAkhir1
{
    public partial class Form1 : Form
    {
        public Bitmap OriginalImage { get; set; }
        public Bitmap DWTImage { get; set; }

        //Global Variables
        public double[,] Wavelet_Coefficients;
        public List<int> Scrambled_Watermark = new List<int>();
        public double[,] Embedded_Wavelet_Coefficients; 
        public double[,] Inversed_Wavelet_Coefficients;


        Stopwatch time = new Stopwatch();
        public Form1()
        {
            InitializeComponent();
            this.hostImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.transformedImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.watermarkImage.SizeMode = PictureBoxSizeMode.Zoom;
        }


        public void GUIStart(string status)
        {
            StatusPanel.BackColor = Color.Gray;
            StatusTxt.BackColor = Color.Gray;
            StatusTxt.Text = status;
            StatusTxt.ForeColor = Color.Red;
            StatusTxt.BackColor = Color.Gray;
            label4.BackColor = Color.Gray;
            label5.BackColor = Color.Gray;
            TimeExecTxt.BackColor = Color.Gray;
            PSNRlbl.BackColor = Color.Gray;
            BERlbl.BackColor = Color.Gray;
            BERValue.BackColor = Color.Gray;
            PSNRValue.BackColor = Color.Gray;
            TimeExecTxt.Text = "0";
            BERValue.Text = "0";
            PSNRValue.Text = "0";
            StatusTxt.Refresh();
            label4.Refresh();
            label5.Refresh();
            PSNRlbl.Refresh();
            BERlbl.Refresh();
            PSNRValue.Refresh();
            BERValue.Refresh();
            TimeExecTxt.Refresh();


            time = Stopwatch.StartNew();
        }

        public void GUIEnd(string status,double mse, double psnr, double ber )
        {
            time.Stop();
            var elapsedTime = time.ElapsedMilliseconds / 1000;

            StatusPanel.BackColor = Color.LightSkyBlue;
            StatusTxt.BackColor = Color.LightSkyBlue;
            StatusTxt.Text = status;
            StatusTxt.ForeColor = Color.DarkGreen;
            label4.BackColor = Color.LightSkyBlue;
            label5.BackColor = Color.LightSkyBlue;
            TimeExecTxt.BackColor = Color.LightSkyBlue;
            TimeExecTxt.Text = " " + elapsedTime.ToString() + " Second(s)";
            PSNRValue.Text = String.Format("{0:0.00}", psnr) ;//psnr.ToString();
            BERValue.Text = String.Format("{0:0.00}", ber);//ber.ToString();
            MSEValue.Text = String.Format("{0:0.00}", mse);//mse.ToString();
            PSNRlbl.BackColor = Color.LightSkyBlue;
            BERlbl.BackColor = Color.LightSkyBlue;
            BERValue.BackColor = Color.LightSkyBlue;
            PSNRValue.BackColor = Color.LightSkyBlue;
        }

        //Sementara
        public void Print(string s)
        {
            MessageBox.Show("Result: " + s, "Succeed", MessageBoxButtons.OK);
        }
        private void button1_Click(object sender, EventArgs e) //Open Original Image
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Picture to Watermark";
            ofd.InitialDirectory = @"F:\College\Semester 8\TA2";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //Bitmap bmp = new Bitmap(ofd.FileName);
                //int w = bmp.Width;
                //int h = bmp.Height;
                //double x1 = Math.Log(w,2);
                //double x2 = Math.Log(h,2);
                ////Cek if dimension is 2^n
                //if (x1 % 1 == 0 && x2 % 1 == 0)
                //{
                //    HostImageLocationTxt.Text = ofd.FileName;
                //    hostImage.Image = new Bitmap(ofd.FileName);
                //}
                //else
                //{
                //    MessageBox.Show("Image dimension must have 2^n x 2^m pixels, \n n and m might be same, might be not.", "Input Error", MessageBoxButtons.OK);
                //}

                HostImageLocationTxt.Text = ofd.FileName;
                hostImage.Image = new Bitmap(ofd.FileName);
            }
        }

        private void button10_Click(object sender, EventArgs e) //Save Transformed/Watermarked Image
        {
            if (transformedImage.Image != null)
            {
                Bitmap bmp = new Bitmap(transformedImage.Image);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Select Save Location";
                sfd.InitialDirectory = @"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Saved_Image";
                sfd.AddExtension = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    switch (Path.GetExtension(sfd.FileName).ToUpper())
                    {
                        case ".BMP":
                            bmp.Save(sfd.FileName, ImageFormat.Bmp);
                            break;
                        case ".gif":
                            bmp.Save(sfd.FileName, ImageFormat.Gif);
                            break;
                        case ".JPG":
                            bmp.Save(sfd.FileName, ImageFormat.Jpeg);
                            break;
                        case ".JPEG":
                            bmp.Save(sfd.FileName,ImageFormat.Jpeg);
                            break;
                        case ".PNG":
                            bmp.Save(sfd.FileName, ImageFormat.Png);
                            break;
                        case ".png":
                            bmp.Save(sfd.FileName, ImageFormat.Png);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e) //Open Watermark Image
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Picture as Watermark";
            ofd.InitialDirectory = @"F:\College\Semester 8\TA2";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                WatermarkImageLocationTxt.Text = ofd.FileName;
                watermarkImage.Image = new Bitmap(ofd.FileName);
            }
        }

        /// <summary>
        /// Forward DWT Transform and Extract the Wavelet Coefficients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) //Forward Transform
        {
            if (hostImage.Image != null)
            {
                GUIStart("Processing...!");
                ///For Visualization
                OriginalImage = new Bitmap(hostImage.Image);
                transformedImage.Image = DWT.TransformDWT(true, false, 2, OriginalImage);

                ///For Wavelet Coefficients Extraction
                Bitmap b = new Bitmap(hostImage.Image);
                double[,] IMatrix = ImageProcessing.ConvertToMatrix(b);
                double[,] ArrayImage = IMatrix;
                Wavelet_Coefficients = DWT.WaveletCoeff(ArrayImage, true, 2);
                GUIEnd("FDWT Succeed!", 0, 0, 0);
            }
            else
            {
                MessageBox.Show("Load Image First", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }    
        }

        

        

        /// <summary>
        /// Scramble the watermark bits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e/*,double[,] coeffs*/) //Generate bit sequence contain 0 and 1
        {
            if (watermarkImage.Image == null)
            {
                MessageBox.Show("Load Watermark Image First!", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }
            else
            {
                GUIStart("Processing......!");
                //double[,] coefficients = coeffs;
                Bitmap bmp = new Bitmap(watermarkImage.Image);
                Scramble m = new Scramble();
                Bitmap b = ImageProcessing.ConvertToBinary(bmp);
                List<int> VectorImage = Scramble.ConvertToVectorMatrix(b); //Include integer values between 255 or 0
                List<int> BinaryVectorImage = Scramble.ConvertToBinaryVectorMatrix(VectorImage); //Include integer values between 1 or 0
                List<int> scrambled_Watermark = Scramble.DSSS(BinaryVectorImage);
                Scrambled_Watermark = scrambled_Watermark;
                GUIEnd("Scramble Succeed!", 0, 0, 0);
                MessageBox.Show("Watermark is Succeed. \n Original Watermark: " + BinaryVectorImage.Count + "\n Scrambled Watermark: " + scrambled_Watermark.Count, "Succeed", MessageBoxButtons.OK);  
            }
            

            
        }

        private void button6_Click(object sender, EventArgs e) //Testing button
        {
            Bitmap hostImg = new Bitmap(hostImage.Image);

            #region GUI Processing
            StatusPanel.BackColor = Color.Gray;
            StatusTxt.BackColor = Color.Gray;
            StatusTxt.Text = "Processing...";
            StatusTxt.ForeColor = Color.Red;
            StatusTxt.BackColor = Color.Gray;
            label4.BackColor = Color.Gray;
            label5.BackColor = Color.Gray;
            TimeExecTxt.BackColor = Color.Gray;
            PSNRlbl.BackColor = Color.Gray;
            BERlbl.BackColor = Color.Gray;
            BERValue.BackColor = Color.Gray;
            PSNRValue.BackColor = Color.Gray;
            TimeExecTxt.Text = "0";
            BERValue.Text = "0";
            PSNRValue.Text = "0";
            StatusTxt.Refresh();
            label4.Refresh();
            label5.Refresh();
            PSNRlbl.Refresh();
            BERlbl.Refresh();
            PSNRValue.Refresh();
            BERValue.Refresh();
            TimeExecTxt.Refresh();
            #endregion


            var time = Stopwatch.StartNew();
            ImageProcessing ip = new ImageProcessing();
            Bitmap bmp = new Bitmap(hostImage.Image);
            //transformedImage.Image = ip.ConvertToBinary(bmp);
            List<int> a = Scramble.ConvertToVectorMatrix(ImageProcessing.ConvertToBinary(bmp));
            List<int> b = Scramble.ConvertToBinaryVectorMatrix(a);
            List<int> c = Scramble.ConvolutionCode(b);
            List<int> d = Scramble.DSSS(c);
            List<int> ee = Scramble.Interleaving(d);
            List<List<int>> totaloftree = Scramble.Segment(ee);
            time.Stop();
            var elapsedTime = time.ElapsedMilliseconds / 1000;


            #region End of GUI Processing
            StatusPanel.BackColor = Color.LightSkyBlue;
            StatusTxt.BackColor = Color.LightSkyBlue;
            StatusTxt.Text = "Ready";
            StatusTxt.ForeColor = Color.DarkGreen;
            label4.BackColor = Color.LightSkyBlue;
            label5.BackColor = Color.LightSkyBlue;
            TimeExecTxt.BackColor = Color.LightSkyBlue;
            TimeExecTxt.Text = " " + elapsedTime.ToString() + " Second(s)";
            int totalOriginalImage = hostImg.Height * hostImg.Width;
            PSNRValue.Text = totalOriginalImage.ToString();
            BERValue.Text = totaloftree[13247].Count.ToString();
            PSNRlbl.BackColor = Color.LightSkyBlue;
            BERlbl.BackColor = Color.LightSkyBlue;
            BERValue.BackColor = Color.LightSkyBlue;
            PSNRValue.BackColor = Color.LightSkyBlue;
            #endregion

            //Write Result of Scrambling to Txt.
            TextWriter tw = new StreamWriter("Scrambled_Data.txt");
            tw.WriteLine("Total Scrambled Data: " + ee.Count);
            foreach (int i in totaloftree[13247])
                tw.WriteLine(i);
            tw.Close();

            //Bitmap bmp = new Bitmap(transformedImage.Image);
            //var p = bmp.GetPixel(400, 400);
            //var cR = p.R;
            //var cG = p.G;
            //var cB = p.B;
            //MessageBox.Show("Red: "+cR+" Green: "+cG+" Blue: "+cB,"Result",MessageBoxButtons.OK);            
        }

        

        private void button9_Click(object sender, EventArgs e)
        {
            double akurasi = Statistic.Akurasi(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
            Print(akurasi.ToString());

        }

        

       

        private void button8_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Embed the Watermark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e/*, double[,] coeffs, List<int> scrambled_Watermark*/) //Embed Watermark
        {
            if (hostImage.Image == null)
            {
                MessageBox.Show("Load Host Image Image First!", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }
            else if (watermarkImage.Image == null)
            {
                MessageBox.Show("Load Watermark Image First!", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }
            else
            {
                GUIStart("Processing.....!");
                List<List<int>> Segmented = Scramble.Segment(Scrambled_Watermark);
                double[,] MappedWatermark = Scramble.Mapping(Segmented);
                double[,] EmbeddedWatermark = HMM.Embedding(Wavelet_Coefficients,MappedWatermark);
                Embedded_Wavelet_Coefficients = EmbeddedWatermark;
                GUIEnd("Embedding Succeed!", 0, 0, 0);
                MessageBox.Show("Embedding Succeed!", "Embedding Process", MessageBoxButtons.OK);
            }
        }


        /// <summary>
        /// Inverse DWT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e/*, double[,] Embedded_Coefficients*/) //Inverse Transform
        {
            if (transformedImage.Image == null)
            {
                MessageBox.Show("There is no Transformed image yet", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }
            else
            {
                GUIStart("Processing.....!");
                //MessageBox.Show("Embedded Wavelet Coefficients: "+Embedded_Wavelet_Coefficients[0,0],"blabal",MessageBoxButtons.OK);
                double[,] InverseDWT = DWT.WaveletCoeff(Embedded_Wavelet_Coefficients, false, 2);
                transformedImage.Image = ImageProcessing.ConvertToBitmap(InverseDWT);
                double mse = Statistic.MSE(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
                double psnr = Statistic.PSNR(new Bitmap(transformedImage.Image), mse);
                double ber = Statistic.BER(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
                GUIEnd("IDWT Succeed!",mse,psnr,ber);
            }

        }
      
    }
}
