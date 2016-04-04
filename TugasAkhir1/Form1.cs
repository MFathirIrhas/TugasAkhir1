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
                DWT dwt = new DWT();
                OriginalImage = new Bitmap(hostImage.Image);
                //DWTImage = new Bitmap(transformImage.Image);
                transformedImage.Image = dwt.TransformDWT(true, false, 2, OriginalImage);
            }
            else
            {
                MessageBox.Show("Load Image First", "Incomplete Procedure Detected", MessageBoxButtons.OK);
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
            Bitmap hostImg =new Bitmap(hostImage.Image);

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
            time.Stop();
            var elapsedTime = time.ElapsedMilliseconds/1000;

            
            #region End of GUI Processing
            StatusPanel.BackColor = Color.LightSkyBlue;
            StatusTxt.BackColor = Color.LightSkyBlue;
            StatusTxt.Text = "Ready";
            StatusTxt.ForeColor = Color.DarkGreen;
            label4.BackColor = Color.LightSkyBlue;
            label5.BackColor = Color.LightSkyBlue;
            TimeExecTxt.BackColor = Color.LightSkyBlue;
            TimeExecTxt.Text = " "+ elapsedTime.ToString() +" Second(s)";
            int totalOriginalImage = hostImg.Height * hostImg.Width;
            totalWatermarkTxt.Text = totalOriginalImage.ToString();
            totalScrambledTxt.Text = ee.Count.ToString();
            label7.BackColor = Color.LightSkyBlue;
            label8.BackColor = Color.LightSkyBlue;
            totalScrambledTxt.BackColor = Color.LightSkyBlue;
            totalWatermarkTxt.BackColor = Color.LightSkyBlue;
            #endregion 
            
            //Write Result of Scrambling to Txt.
            TextWriter tw = new StreamWriter("Scrambled_Data.txt");
            tw.WriteLine("Total Scrambled Data: " + ee.Count);
            foreach (int i in ee)
                tw.WriteLine(i);
            tw.Close();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (transformedImage.Image == null)
            {
                MessageBox.Show("There is no Transform image yet","Incomplete Procedure Detected",MessageBoxButtons.OK);
            }
            else
            {
                DWT dwt = new DWT();
                Bitmap decomposedImage = new Bitmap(transformedImage.Image);
                transformedImage.Image = dwt.TransformDWT(false, true, 2, decomposedImage); 
            }
            
        }

        
    }
}
