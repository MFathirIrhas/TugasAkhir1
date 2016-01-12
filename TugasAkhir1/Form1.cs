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

namespace TugasAkhir1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.hostImage.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Picture";
            ofd.InitialDirectory = @"C:\Users\Fathir Irhas\Pictures";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileLocationTxt.Text = ofd.FileName;
                hostImage.Image = new Bitmap(ofd.FileName);
            }
        }
    }
}
