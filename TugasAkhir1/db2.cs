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
    class db2
    {
        //For Low Pass Filter
        private const double s0 = -0.1294095226;
        private const double s1 =  0.2241438680;
        private const double s2 =  0.8365163037;
        private const double s3 =  0.4829629131;

        //For High Pass Filter
        private const double w0 = -0.4829629131; 
        private const double w1 =  0.8365163037;
        private const double w2 = -0.2241438680;
        private const double w3 = -0.1294095226;

        public static void Forward1D(double[] data)
        {
            int i, j;
            int n = data.Length >> 1;
            int half = n >> 1;

            double[] tmp = new double[n];

            i = 0;
            for (j = 0; j < n - 3; j = j + 2)
            {
                tmp[i] = data[j] * s0 + data[j + 1] * s1 + data[j + 2] * s2 + data[j + 3] * s3;
                tmp[i + half] = data[j] * w0 + data[j + 1] * w1 + data[j + 2] * w2 + data[j + 3] * w3;
                i++;
            }

            tmp[i] = data[n - 2] * s0 + data[n - 1] * s1 + data[0] * s2 + data[1] * s3;
            tmp[i + half] = data[n - 2] * s0 + data[n - 1] * s1 + data[0] * s2 + data[1] * s3;

            for (i = 0; i < n; i++)
            {
                data[i] = tmp[i];
            }
        }

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

        public static void Inverse1D(double[] data)
        {
            int i, j;
            int n = data.Length >> 1;
            int half = n >> 1;
            int halfPls1 = half + 1;

            double[] tmp = new double[n];

            //      last smooth val  last coef.  first smooth  first coef
            tmp[0] = data[half - 1] * s0 + data[n - 1] * s1 + data[0] * s2 + data[half] * s3;
            tmp[1] = data[half - 1] * w0 + data[n - 1] * w1 + data[0] * w2 + data[half] * w3;
            j = 2;
            for (i = 0; i < half - 1; i++)
            {
                //     smooth val     coef. val       smooth val     coef. val
                tmp[j++] = data[i] * s0 + data[i + half] * s1 + data[i + 1] * s2 + data[i + halfPls1] * s3;
                tmp[j++] = data[i] * w0 + data[i + half] * w1 + data[i + 1] * w2 + data[i + halfPls1] * w3;
            }
            for (i = 0; i < n; i++)
            {
                data[i] = tmp[i];
            }
        }

        #region buggy Inverse2d
        //public void Inverse2D(double[,] data, int level)
        //{
        //    int rows = data.GetLength(0);
        //    int cols = data.GetLength(1);

        //    double[] col = new double[rows];
        //    double[] row = new double[cols];

        //    for (int l = 0; l < level; l++)
        //    {
        //        for (int j = 0; j < cols; j++)
        //        {
        //            for (int i = 0; i < row.Length; i++)
        //                col[i] = data[i, j];

        //            Inverse1D(col);

        //            for (int i = 0; i < col.Length; i++)
        //                data[i, j] = col[i];
        //        }

        //        for (int i = 0; i < rows; i++)
        //        {
        //            for (int j = 0; j < row.Length; j++)
        //                row[j] = data[i, j];

        //            Inverse1D(row);

        //            for (int j = 0; j < row.Length; j++)
        //                data[i, j] = row[j];
        //        }
        //    }
        //}
        #endregion

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
