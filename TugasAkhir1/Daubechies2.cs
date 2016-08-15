using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Accord.Math;
using Accord.Statistics;
using System.Windows.Forms;

namespace TugasAkhir1
{
    public class Daubechies2
    {
        //For Low Pass Filter
        private const double s0 = -0.1294095226;
        private const double s1 = 0.2241438680;
        private const double s2 = 0.8365163037;
        private const double s3 = 0.4829629131;

        //For High Pass Filter
        private const double w0 = -0.4829629131;
        private const double w1 = 0.8365163037;
        private const double w2 = -0.2241438680;
        private const double w3 = -0.1294095226;

        #region Test
        public static double[,] ForwardDb2Kernel(int size)
        {
            double[,] forwardkernel = new double[size,size];

            for(int i = 0; i < size; i += 2)
            {
                if (i == size - 2)
                {
                    ///low
                    forwardkernel[i, i + 0] = s0;
                    forwardkernel[i, i + 1] = s1;

                    ///high
                    forwardkernel[i + 1, i + 0] = w0;
                    forwardkernel[i + 1, i + 1] = w1;
                }
                else
                {
                    ///low
                    forwardkernel[i, i + 0] = s0;
                    forwardkernel[i, i + 1] = s1;
                    forwardkernel[i, i + 2] = s2;
                    forwardkernel[i, i + 3] = s3;

                    ///high
                    forwardkernel[i + 1, i + 0] = w0;
                    forwardkernel[i + 1, i + 1] = w1;
                    forwardkernel[i + 1, i + 2] = w2;
                    forwardkernel[i + 1, i + 3] = w3;
                }
                
            }

            return forwardkernel;
        }


        public static double[,] InverseDb2Kernel(int size)
        {
            double[,] inversekernel = new double[size, size];

            for (int i = 0; i < size; i += 2)
            {
                if (i == size - 2)
                {
                    ///low
                    inversekernel[i + 0, i] = s0;
                    inversekernel[i + 1, i] = s1;

                    ///high
                    inversekernel[i + 0, i + 1] = w0;
                    inversekernel[i + 1, i + 1] = w1;
                }
                else
                {
                    ///low
                    inversekernel[i + 0, i] = s0;
                    inversekernel[i + 1, i] = s1;
                    inversekernel[i + 2, i] = s2;
                    inversekernel[i + 3, i] = s3;

                    ///high
                    inversekernel[i + 0, i + 1] = w0;
                    inversekernel[i + 1, i + 1] = w1;
                    inversekernel[i + 2, i + 1] = w2;
                    inversekernel[i + 3, i + 1] = w3;
                }

            }

            return inversekernel;
        }

        public static double[] Forward1D_Db2(double[,] ForwardDb2Kernel, double[] input)
        {
            double[] output = Matrix.Multiply(ForwardDb2Kernel, input);
            return output;
        }


        //public static double[,] Forward2D_Db2(double[,]pixels ,double[,] kernel)
        //{
        //    double[][] rows = new double[pixels.GetLength(0)][];
        //    double[][] column = new double[pixels.GetLength(1)][];

        //    for(int i = 0; i < pixels.GetLength(0); i++)
        //    {
        //        double[] data = new double[pixels.GetLength(1)];
        //        for(int j = 0; j < pixels.GetLength(1); j++)
        //        {
        //            data[j] = pixels[i, j];       
        //        }

        //        double[] newData = Matrix.Multiply(kernel, data);
        //        rows[i] = newData;            
        //    }

        //    double[,] DataSet = SetData(rows);

        //    for(int i=0; i< pixels.GetLength(1); i++)
        //    {
        //        double[] data = new double[pixels.GetLength(0)];
        //        for (int j = 0; j < pixels.GetLength(0); j++)
        //        {
        //            data[j] = DataSet[j, i];
        //        }

        //        double[] newData2 = Matrix.Multiply(kernel, data);
        //        column[i] = newData2;
        //    }


        //}


        public static double[,] SetData(double[][] newData)
        {
            double[,] DataSet = new double[newData.GetLength(0), newData.GetLength(0)];

            for(int i = 0; i < DataSet.GetLength(0); i++)
            {
                int c = 0;
                for(int j = 0; j < DataSet.GetLength(1)/2; j++)
                {
                    DataSet[i, j] = newData[i][c];
                    c += 2;
                }
            }

            for(int k = 0; k < DataSet.GetLength(0); k++)
            {
                int d = 1;
                for(int l = DataSet.GetLength(1)/2; l<DataSet.GetLength(1); l++)
                {
                    DataSet[k, l] = newData[k][d];
                    d += 2;
                }
            }

            return DataSet;
        }
#endregion


        public static void Forward1D(double[] data)
        {
            double[] temp = new double[data.Length];
            int h = data.Length >> 1;
            for (int i = 0; i < h; i++)
            {
                int k = (i << 1);
                if(k == data.Length - 2)
                {
                    temp[i] = data[k] * s0 + data[k + 1] * s1 + data[0] * s2 + data[1] * s3;
                    temp[i + h] = data[k] * w0 + data[k + 1] * w1 + data[0] * w2 + data[1] *w3;
                }
                else
                {
                    temp[i] = data[k] * s0 + data[k + 1] * s1 + data[k + 2] * s2 + data[k + 3] * s3;
                    temp[i + h] = data[k] * w0 + data[k + 1] * w1 + data[k + 2] * w2 + data[k + 3] * w3;
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

        public static void Inverse1D(double[] data)
        {
            double[] temp = new double[data.Length];

            int h = data.Length >> 1;
            for (int i = 0; i < h; i++)
            {
                int k = (i << 1);
                if (k < 2)
                {
                    temp[k] = data[0] * s0 + data[0 + h] * w0 + data[h-1] * s2 + data[(h-1) + h] * w2;// w0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
                    temp[k + 1] = data[0] * s1 + data[0 + h] * w1 + data[h-1] * s3 + data[(h-1) + h] * w3;// s0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
                }
                else
                {
                    temp[k] = data[i-1] * s2 + data[(i-1) + h] * w2 + data[i] * s0 + data[i + h] * w0;// w0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
                    temp[k + 1] = data[i-1] * s3 + data[(i-1) + h] * w3 + data[i] * s1 + data[i + h] * w1; ;// s0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
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

    }
}
