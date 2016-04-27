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

        public Form1()
        {
            InitializeComponent();
            this.hostImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.transformedImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.watermarkImage.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void button1_Click(object sender, EventArgs e) //Open Original Image
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Picture to Watermark";
            ofd.InitialDirectory = @"F:\College\Semester 8\TA2";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
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
            ofd.InitialDirectory = @"C:\Users\Fathir Irhas\Pictures";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                WatermarkImageLocationTxt.Text = ofd.FileName;
                watermarkImage.Image = new Bitmap(ofd.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (hostImage.Image != null)
            {
                OriginalImage = new Bitmap(hostImage.Image);
                //DWTImage = new Bitmap(transformImage.Image);
                transformedImage.Image = DWT.TransformDWT(true, false, 2, OriginalImage);
            }
            else
            {
                MessageBox.Show("Load Image First", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }    
        }

        private void button5_Click(object sender, EventArgs e) //Generate bit sequence contain 0 and 1
        {
            Bitmap bmp = new Bitmap(watermarkImage.Image);
            Matrix m = new Matrix();
            ImageProcessing ip = new ImageProcessing();

            Bitmap b = ip.ConvertToBinary(bmp);
            List<int> VectorImage = m.ConvertToVectorMatrix(b); //Include integer values between 255 or 0
            List<int> BinaryVectorImage = m.ConvertToBinaryVectorMatrix(VectorImage); //Include integer values between 1 or 0
            //transformedImage.Image = b;
            MessageBox.Show("Nilai: " + BinaryVectorImage[34], "ini adalah", MessageBoxButtons.OK);
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
            label7.BackColor = Color.Gray;
            label8.BackColor = Color.Gray;
            totalScrambledTxt.BackColor = Color.Gray;
            totalWatermarkTxt.BackColor = Color.Gray;
            TimeExecTxt.Text = "0";
            totalScrambledTxt.Text = "0";
            totalWatermarkTxt.Text = "0";
            StatusTxt.Refresh();
            label4.Refresh();
            label5.Refresh();
            label7.Refresh();
            label8.Refresh();
            totalWatermarkTxt.Refresh();
            totalScrambledTxt.Refresh();
            TimeExecTxt.Refresh();
            #endregion


            var time = Stopwatch.StartNew();
            ImageProcessing ip = new ImageProcessing();
            Matrix m = new Matrix();
            Bitmap bmp = new Bitmap(hostImage.Image);
            //transformedImage.Image = ip.ConvertToBinary(bmp);
            List<int> a = m.ConvertToVectorMatrix(ip.ConvertToBinary(bmp));
            List<int> b = m.ConvertToBinaryVectorMatrix(a);
            List<int> c = m.ConvolutionCode(b);
            List<int> d = m.DSSS(c);
            List<int> ee = m.Interleaving(d);
            List<List<int>> totaloftree = m.Segment(ee);
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
            totalWatermarkTxt.Text = totalOriginalImage.ToString();
            totalScrambledTxt.Text = totaloftree[13247].Count.ToString();
            label7.BackColor = Color.LightSkyBlue;
            label8.BackColor = Color.LightSkyBlue;
            totalScrambledTxt.BackColor = Color.LightSkyBlue;
            totalWatermarkTxt.BackColor = Color.LightSkyBlue;
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (transformedImage.Image == null)
            {
                MessageBox.Show("There is no Transformed image yet","Incomplete Procedure Detected!",MessageBoxButtons.OK);
            }
            else
            {
                Bitmap decomposedImage = new Bitmap(transformedImage.Image);
                transformedImage.Image = DWT.TransformDWT(false, true, 2, decomposedImage); 
            }
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //ImageProcessing ip = new ImageProcessing();
            //Matrix m = new Matrix();
            //Bitmap bmp = new Bitmap(hostImage.Image);
            ////transformedImage.Image = ip.ConvertToBinary(bmp);
            //List<int> a = m.ConvertToVectorMatrix(ip.ConvertToBinary(bmp));
            //List<int> b = m.ConvertToBinaryVectorMatrix(a);
            //List<int> c = m.ConvolutionCode(b);
            //List<int> d = m.DSSS(c);
            //List<int> ee = m.Interleaving(d);
            //List<List<int>> ff = m.Segment(ee);

            //TextWriter tw = new StreamWriter("Segmented_Data.txt");
            ////tw.WriteLine(ff[0].ToString());
            //tw.WriteLine("Total First Segment: " + ff[0].Count);
            ////foreach (int i in ff[0])
            ////tw.WriteLine(ff[0][]);
            //for (int i = 0; i < ff[0].Count; i++)
            //{
            //    tw.WriteLine(ff[0][i]);
            //}
            //tw.Close();

            //Bitmap bmp = new Bitmap(hostImage.Image);
            //Color c, c2, c3;
            //int w = bmp.Width / 2;
            //int w2 = w / 2;
            //c = bmp.GetPixel(0, 0);//width=397 , height=367
            //c2 = bmp.GetPixel(1, 0);
            //c3 = bmp.GetPixel(w2 + 1, w2 + 1);
            //int R = c.R;
            //int R2 = c2.R;
            //int R3 = c3.R;

            //int[] f = { 3, 2, 3, 2, 3, 1, 1, 2 };
            //int[] g = { 2, 2, 2, 2, 2, 2 };
            //double m = Statistic.Mean(g);
            //double var = Statistic.Variance(g);
            //MessageBox.Show("RGB Value:" +m+","+var, "succeed", MessageBoxButtons.OK);

            //MessageBox.Show("Pixel Sudut Kiri:" + R + ",Pixel tetangga:" + R2 + ",Pixel Decomposed:" +R3 +",", "succeed", MessageBoxButtons.OK);

            
            var b = new Bitmap(hostImage.Image);
            var coeff = HMM.GetWaveletCoeff(b,0,"ALL");
            var hs = HMM.GetHiddenStates(coeff);
            var hs12 = HMM.CountHS(hs);
            int aa = hs12.Item1 + hs12.Item2;
            double prob1 = (double)hs12.Item1 / aa;
            double prob2 = (double)hs12.Item2 / aa;
            double prob11 = Math.Round(prob1, 2);
            double prob22 = Math.Round(prob2, 2);
            totalWatermarkTxt.Text = prob11.ToString();//hs12.Item1.ToString();
            totalScrambledTxt.Text = prob22.ToString();//hs12.Item2.ToString();
            double prob = prob1+prob2;
            TimeExecTxt.Text = prob.ToString();//aa.ToString();
            //double d = HMM.Threshold(coeff);
            //totalScrambledTxt.Text = d.ToString();
            TextWriter tw = new StreamWriter("Wavelet_Coefficients.txt");
            tw.WriteLine("Jumlah Wavelet-Coeff: " + coeff.Count);
            tw.WriteLine("Total pixel: " + b.Width * b.Height);
            tw.WriteLine("");
            for (int i = 0; i < coeff.Count; i++)
            {
                tw.WriteLine(coeff[i]);
            }
            tw.Close();

        }

        

        
    }
}
