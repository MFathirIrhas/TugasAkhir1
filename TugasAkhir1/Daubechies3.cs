using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace TugasAkhir1
{
    class Daubechies3
    {
        //For Low Pass Filter
        private const double s0 =  0.0352262919;
        private const double s1 = -0.0854412739;
        private const double s2 = -0.1350110200;
        private const double s3 =  0.4598775021;
        private const double s4 =  0.8068915093;
        private const double s5 =  0.3326705530;


        //For High Pass Filter
        private const double w0 = -0.3326705530;
        private const double w1 =  0.8068915093;
        private const double w2 = -0.4598775021;
        private const double w3 = -0.1350110200;
        private const double w4 =  0.0854412739;
        private const double w5 =  0.0352262919;

        /* Matrix Transformation for 
         * Forward Transformation Daubechies 3
         * 
         * s0 s1 s2 s3 s4 s5 0  0
         * w0 w1 w2 w3 w4 w5 0  0
         * 0  0  s0 s1 s2 s3 s4 s5 
         * 0  0  w0 w1 w2 w3 w4 w5
         * s4 s5 0  0  s0 s1 s2 s3
         * w4 w5 0  0  w0 w1 w2 w3
         * s2 s3 s4 s5 0  0  s0 s1
         * s2 s3 s4 w5 0  0  w0 w1
         * 
         */ 
        public static void Forward1D(double[] data)
        {
            double[] temp = new double[data.Length];
            int h = data.Length >> 1;
            for (int i = 0; i < h; i++)
            {
                int k = (i << 1);
                if (k == data.Length - 4)
                {
                    temp[i] = data[k] * s0 + data[k + 1] * s1 + data[k + 2] * s2 + data[k + 3] * s3 + data[0] * s4 + data[1] * s5;
                    temp[i + h] = data[k] * w0 + data[k + 1] * w1 + data[k + 2] * w2 + data[k + 3] * w3 + data[0] * w4 + data[1] * w5;
                }
                else if(k == data.Length - 2)
                {
                    temp[i] = data[k] * s0 + data[k + 1] * s1 + data[0] * s2 + data[1] * s3 + data[2] * s4 + data[3] * s5;
                    temp[i + h] = data[k] * w0 + data[k + 1] * w1 + data[0] * w2 + data[1] * w3 + data[2] * w4 + data[3] * w5;
                }
                else
                {
                    temp[i] = data[k] * s0 + data[k + 1] * s1 + data[k + 2] * s2 + data[k + 3] * s3 + data[k + 4] * s4 + data[k + 5] * s5;
                    temp[i + h] = data[k] * w0 + data[k + 1] * w1 + data[k + 2] * w2 + data[k + 3] * w3 + data[k + 4] * w4 + data[k + 5] * w5;
                }
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = temp[i];
        }

        #region Forward2D
        public static void Forward2D(double[,] data, int level)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            double[] row;
            double[] col;

            for (int k = 0; k < level; k++)
            {
                int lev = 1 << k;
                int levCols = cols / lev;
                int levRows = rows / lev;
                row = new double[levCols];

                for (int i = 0; i < levRows; i++)
                {
                    for (int j = 0; j < row.Length; j++)
                        row[j] = data[i, j];

                    Forward1D(row);

                    for (int j = 0; j < row.Length; j++)
                        data[i, j] = row[j];
                }

                col = new double[levRows];

                for (int j = 0; j < levCols; j++)
                {
                    for (int i = 0; i < col.Length; i++)
                        col[i] = data[i, j];

                    Forward1D(col);

                    for (int i = 0; i < col.Length; i++)
                        data[i, j] = col[i];
                }
            }
        }
        #endregion

        /* Matrix Transformation for 
         * Daubechies 3 Inverse Transform
         * 
         * s0 w0 0  0  s4 w4 s2 w2  
         * s1 w1 0  0  s5 w5 s3 w3 
         * s2 w2 s0 w0 0  0  s4 w4
         * s3 w3 s1 w1 0  0  s5 w5
         * s4 w4 s2 w2 s0 w0 0  0 
         * s5 w5 s3 w3 s1 w1 0  0 
         * 0  0  s4 s4 s2 w2 s0 w0
         * 0  0  s5 w5 s3 w3 s1 w1 
         */
        public static void Inverse1D(double[] data)
        {
            double[] temp = new double[data.Length];

            int h = data.Length >> 1;
            for (int i = 0; i < h; i++)
            {
                int k = (i << 1);
                if (k < 2)
                {
                    temp[k] = data[0] * s0 + data[0 + h] * w0 + data[h - 2] * s4 + data[(h - 2) + h] * w4 + data[h-1] * s2 + data[(h-1) + h] * w2;// w0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
                    temp[k + 1] = data[0] * s1 + data[0 + h] * w1 + data[h - 2] * s5 + data[(h - 2) + h] * w5 + data[h - 1] * s3 + data[(h - 1) + h] * w3;// s0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
                }
                else if(k >= 2 && k < 4)
                {
                    temp[k] = data[0] * s2 + data[0 + h] * w2 + data[1] * s0 + data[1 + h] * w0 + data[h - 1] * s4 + data[(h - 1) + h] * w4;// w0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
                    temp[k + 1] = data[0] * s3 + data[0 + h] * w3 + data[1] * s1 + data[1 + h] * w1 + data[h - 1] * s5 + data[(h - 1) + h] * w5;// s0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
                }
                else
                {
                    temp[k] = data[i - 2] * s4 + data[(i - 2) + h] * w4 + data[i - 1] * s2 + data[(i - 1) + h] * w2 + data[i] * s0 + data[i + h] * w0;// w0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
                    temp[k + 1] = data[i - 2] * s5 + data[(i - 2) + h] * w5 + data[i - 1] * s3 + data[(i - 1) + h] * w3 + data[i] * s1 + data[i + h] * w1;// s0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
                }

            }

            for (int i = 0; i < data.Length; i++)
                data[i] = temp[i];
        }



        public static void Inverse2D(double[,] data, int iterations)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            double[] col;
            double[] row;

            for (int k = iterations - 1; k >= 0; k--)
            {
                int lev = 1 << k;

                int levCols = cols / lev;
                int levRows = rows / lev;

                col = new double[levRows];
                for (int j = 0; j < levCols; j++)
                {
                    for (int i = 0; i < col.Length; i++)
                        col[i] = data[i, j];

                    Inverse1D(col);

                    for (int i = 0; i < col.Length; i++)
                        data[i, j] = col[i];
                }

                row = new double[levCols];
                for (int i = 0; i < levRows; i++)
                {
                    for (int j = 0; j < row.Length; j++)
                        row[j] = data[i, j];

                    Inverse1D(row);

                    for (int j = 0; j < row.Length; j++)
                        data[i, j] = row[j];
                }
            }
        }

        public static double Scale(double fromMin, double fromMax, double toMin, double toMax, double x)
        {
            if (fromMax - fromMin == 0) return 0;
            double value = (toMax - toMin) * (x - fromMin) / (fromMax - fromMin) + toMin;
            if (value > toMax)
            {
                value = toMax;
            }
            if (value < toMin)
            {
                value = toMin;
            }
            return value;
        }

        public static Tuple<double[,], double[,], double[,], Bitmap> TransformDWT(bool Forward, bool Safe, int Levels, Bitmap OriginalImage/*, Bitmap TransformedImage*/)
        {

            //Bitmap bmp = Forward ? OriginalImage : TransformedImage;
            Bitmap bmp = OriginalImage;
            Bitmap bmp2;
            //int Iterations = 0;
            //int.TryParse(Levels, out Iterations);

            int maxScale = (int)(Math.Log(bmp.Width < bmp.Height ? bmp.Width : bmp.Height) / Math.Log(2));
            if (Levels < 1 || Levels > maxScale)
            {
                MessageBox.Show("Iteration must be Integer from 1 to " + maxScale);

                //return;
            }

            int time = Environment.TickCount;

            double[,] Red = new double[bmp.Width, bmp.Height];
            double[,] Green = new double[bmp.Width, bmp.Height];
            double[,] Blue = new double[bmp.Width, bmp.Height];

            int PixelSize = 3;
            BitmapData bmData = null;

            if (Safe)
            {
                Color c;

                for (int j = 0; j < bmp.Height; j++)
                {
                    for (int i = 0; i < bmp.Width; i++)
                    {
                        c = bmp.GetPixel(i, j);
                        Red[i, j] = (double)Scale(0, 255, -1, 1, c.R);
                        Green[i, j] = (double)Scale(0, 255, -1, 1, c.G);
                        Blue[i, j] = (double)Scale(0, 255, -1, 1, c.B);
                    }
                }
            }
            else
            {
                bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    for (int j = 0; j < bmData.Height; j++)
                    {
                        byte* row = (byte*)bmData.Scan0 + (j * bmData.Stride);
                        for (int i = 0; i < bmData.Width; i++)
                        {
                            Red[i, j] = (double)Scale(0, 255, -1, 1, row[i * PixelSize + 2]);
                            Green[i, j] = (double)Scale(0, 255, -1, 1, row[i * PixelSize + 1]);
                            Blue[i, j] = (double)Scale(0, 255, -1, 1, row[i * PixelSize]);
                        }
                    }
                }
            }

            if (Forward)
            {
                Forward2D(Red, Levels);
                Forward2D(Green, Levels);
                Forward2D(Blue, Levels);
            }
            else
            {
                Inverse2D(Red, Levels);
                Inverse2D(Green, Levels);
                Inverse2D(Blue, Levels);
            }

            if (Safe)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    for (int i = 0; i < bmp.Width; i++)
                    {
                        bmp.SetPixel(i, j, Color.FromArgb((int)Scale(-1, 1, 0, 255, Red[i, j]), (int)Scale(-1, 1, 0, 255, Green[i, j]), (int)Scale(-1, 1, 0, 255, Blue[i, j])));
                    }
                }
            }
            else
            {
                unsafe
                {
                    for (int j = 0; j < bmData.Height; j++)
                    {
                        byte* row = (byte*)bmData.Scan0 + (j * bmData.Stride);
                        for (int i = 0; i < bmData.Width; i++)
                        {
                            row[i * PixelSize + 2] = (byte)Scale(-1, 1, 0, 255, Red[i, j]);
                            row[i * PixelSize + 1] = (byte)Scale(-1, 1, 0, 255, Green[i, j]);
                            row[i * PixelSize] = (byte)Scale(-1, 1, 0, 255, Blue[i, j]);
                        }
                    }
                }

                bmp.UnlockBits(bmData);
            }

            //if (Forward)
            //{
            //    TransformedImage = new Bitmap(bmp);
            //}

            bmp2 = bmp;
            return new Tuple<double[,], double[,], double[,], Bitmap>(Red, Green, Blue, bmp2);
        }

        public static double[,] WaveletCoeff(double[,] pixels, bool forward, int level)
        {
            double[,] p = pixels;
            if (forward == true)
            {
                Forward2D(p, level); //Forward Haar DWT
            }
            else
            {
                Inverse2D(p, level); //Inverse Haar DWT
            }

            return p;
        }

        //END
    }
}
