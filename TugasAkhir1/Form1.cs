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

        private void button5_Click(object sender, EventArgs e) //Generate bit sequence
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
            Console.WriteLine("testing");
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
                watermarkImage.Image = dwt.TransformDWT(false, true, 2, decomposedImage); 
            }
            
        }

        
    }
}
