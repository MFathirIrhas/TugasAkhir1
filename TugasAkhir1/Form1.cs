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
            ofd.InitialDirectory = @"C:\Users\Fathir Irhas\Pictures";
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
            //Console.WriteLine("testing");
            ImageProcessing ip = new ImageProcessing();
            Matrix m = new Matrix();
            Bitmap bmp = new Bitmap(hostImage.Image);
            //transformedImage.Image = ip.ConvertToBinary(bmp);
            List<int> a = m.ConvertToVectorMatrix(ip.ConvertToBinary(bmp));
            List<int> b = m.ConvertToBinaryVectorMatrix(a);
            List<int> c = m.ConvolutionCode(b);
            List<int> d = m.DSSS(c);
            List<int> ee = m.Interleaving(d);
            MessageBox.Show("Jumlah elemen setelah convolution code adalah: " + a.Count + ", " + b.Count + ","+c.Count +","+d.Count+","+ee.Count,"Success", MessageBoxButtons.OK);
            //int x = 5;
            //int y = x % 2;
            //MessageBox.Show("Hasil "+x +"Mod 2 adalah: "+y,"hahha",MessageBoxButtons.OK);
            //List<int> g0 = new List<int> { 1, 1, 1, 1, 0, 1, 1, 1 };
            //List<int> g1 = new List<int> { 1, 1, 0, 1, 1, 0, 0, 1 };
            //List<int> g2 = new List<int> { 1, 0, 0, 1, 0, 1, 0, 1 };
            //int[,] g = new int[,] { { 1, 1, 1, 1, 0, 1, 1, 1 }, { 1, 1, 0, 1, 1, 0, 0, 1 }, { 1, 0, 0, 1, 0, 1, 0, 1 } };
            //int[] t = new int[5];
            //t[0] = 3;
            //int a = 2;
            //int b = 4;
            //int c = a - b;
            //int d = 4 + c;
            //List<List<int>> g = new List<List<int>>();
            //g.Add(g0);
            //g.Add(g1);
            //g.Add(g2);
            //MessageBox.Show("Sekian " + g[0,4], "Adalah", MessageBoxButtons.OK);
            //MessageBox.Show("Sekian " + d, "Adalah", MessageBoxButtons.OK);
            
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
