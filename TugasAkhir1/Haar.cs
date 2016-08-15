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
    public class Haar
    {
        //Haar Discrete Wavelet Transform
        //Matrix transform = 
        /// [ 1/sqrt(2)  1/sqrt(2) ] 
        /// [ 1/sqrt(2) -1/sqrt(2) ]
        //1.4142135623730950488016887242097 = Math.Sqrt(2)
        //0.70710678118654752440084436210485 = 1/Math.Sqrt(2)

        //For Low Pass Filter
        private const double s0 = 0.70710678118654752440084436210485;//1/1.4142135623730950488016887242097;//  0.5;
        private const double s1 = 0.70710678118654752440084436210485;//1/1.4142135623730950488016887242097;//  0.5;

        //For High Pass Filter
        private const double w0 = -0.70710678118654752440084436210485;//1/1.4142135623730950488016887242097;//  0.5; 
        private const double w1 =  0.70710678118654752440084436210485;//-1/1.4142135623730950488016887242097;// -0.5;

        public static void Forward1D(double[] data)
        {
            double[] temp = new double[data.Length];
            int h = data.Length >> 1;
            for (int i = 0; i < h; i++)
            {
                int k = (i << 1);
                temp[i] = data[k] * s0 + data[k + 1] * s1;
                temp[i + h] = data[k] * w0 + data[k + 1] * w1;
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = temp[i];
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
            double[] temp = new double[data.Length];

            int h = data.Length >> 1;
            for (int i = 0; i < h; i++)
            {
                int k = (i << 1);
                temp[k] = (data[i] * s0 + data[i + h] * w0);// w0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
                temp[k + 1] = (data[i] * s1 + data[i + h] * w1);// s0; //Changed After Matrix Transformation Changed to 1/sqrt(2)
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = temp[i];
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

        public static Tuple<double[,],double[,],double[,],Bitmap> TransformDWT(bool Forward, bool Safe, int Levels, Bitmap OriginalImage/*, Bitmap TransformedImage*/)
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
            return new Tuple<double[,], double[,], double[,], Bitmap>(Red,Green,Blue,bmp2);
        }


        #region FOR GRAYSCALE and COLOR HOST IMAGE
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

        public static double[,] GetWaveletCoeff(Bitmap bmp, int level)
        {
            double[,] p = new double[bmp.Height, bmp.Width];
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    p[y, x] = c.R;
                }
            }
            double[,] coeffs = WaveletCoeff(p, true, level);
            return coeffs;
        }

        public static List<double> WaveletCoeff1D(double[,] pixels, bool forward, int level)
        {
            double[,] p = pixels;
            List<double> WC1D = new List<double>();
            if (forward == true)
            {
                Forward2D(p, level); //Forward Haar DWT
            }
            else
            {
                Inverse2D(p, level); //Inverse Haar DWT
            }

            for(int i = 0; i < p.GetLength(0); i++)
            {
                for(int j = 0; j < p.GetLength(1); j++)
                {
                    WC1D.Add(p[i, j]);
                }
            }
            return WC1D;
        }

        public static double[,] InverseWaveletCoeff(Bitmap bmp, int level)
        {
            double[,] p = new double[bmp.Height, bmp.Width];
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    p[y, x] = c.R;
                }
            }
            double[,] coeffs = WaveletCoeff(p, true, level);
            double[,] InvertedCoeffs = WaveletCoeff(coeffs, false, level);
            return InvertedCoeffs;
        }
        #endregion


        //Convert 2 dimension watermark matrix into 1 dimension watermark.
        public static List<double> ConvertTo1DCoeff(double[,] wc)
        {
            List<double> WaveletCoefficient1D = new List<double>();
            for(int i = 0; i < wc.GetLength(0); i++)
            {
                for(int j = 0; j < wc.GetLength(1); j++)
                {
                    WaveletCoefficient1D.Add(wc[i, j]);
                }
            }

            return WaveletCoefficient1D;
        }

        /// Convert Scale 2 to 1 dimension
        /// 1 dimension array of double
        public static double[] Scale2To1DCoeff(double[,] coeffs) //only LH,HH,HL in Scale 2
        {
            int size = ((coeffs.GetLength(0) / 4) * (coeffs.GetLength(1) / 4)) * 3;
            List<double> Scale2Coeffs = new List<double>();
            //LH2
            for(int i = 0; i < coeffs.GetLength(0)/4; i++)
            {
                for(int j = coeffs.GetLength(1) / 4; j < coeffs.GetLength(1) / 2; j++)
                {
                    Scale2Coeffs.Add(coeffs[i, j]);
                }
            }

            //HH2
            for (int k = coeffs.GetLength(0) / 4; k < coeffs.GetLength(0) / 2; k++)
            {
                for (int l = coeffs.GetLength(1) / 4; l < coeffs.GetLength(1) / 2; l++)
                {
                    Scale2Coeffs.Add(coeffs[k, l]);
                }
            }

            //HL2
            for (int m = coeffs.GetLength(0) / 4; m < coeffs.GetLength(0) / 2; m++)
            {
                for (int n = 0; n < coeffs.GetLength(1) / 4; n++)
                {
                    Scale2Coeffs.Add(coeffs[m, n]);
                }
            }

            double[] Scale2 = Scale2Coeffs.ToArray();

            return Scale2;
        }

        /// Convert Scale 1 to 1 dimension
        /// 2 dimension array of double
        public static double[,] Scale1To1DCoeff(double[,] coeffs) //only LH,HH,HL in Scale 1
        {
            int size = (coeffs.GetLength(0) / 4) * (coeffs.GetLength(1) / 4);
            double[,] lh1 = new double[size, 4];
            double[,] hh1 = new double[size, 4];
            double[,] hl1 = new double[size, 4];
            double[,] Scale1 = new double[size * 3, 4];

            //LH1
            int c  = 0;
            for(int i = 0; i < coeffs.GetLength(0) / 2; i+=2)
            {
                for(int j = coeffs.GetLength(1) / 2; j < coeffs.GetLength(1); j+=2)
                {
                    lh1[c, 0] = coeffs[i, j];
                    lh1[c, 1] = coeffs[i, j+1];
                    lh1[c, 2] = coeffs[i+1, j];
                    lh1[c, 3] = coeffs[i+1, j+1];
                    c++;
                }
            }

            //HH1
            int c2 = 0;
            for (int k = coeffs.GetLength(0) / 2; k < coeffs.GetLength(0); k += 2)
            {
                for (int l = coeffs.GetLength(1) / 2; l < coeffs.GetLength(1); l += 2)
                {
                    hh1[c2, 0] = coeffs[k, l];
                    hh1[c2, 1] = coeffs[k, l + 1];
                    hh1[c2, 2] = coeffs[k + 1, l];
                    hh1[c2, 3] = coeffs[k + 1, l + 1];
                    c2++;
                }
            }

            //HL1
            int c3 = 0;
            for (int m = coeffs.GetLength(0) / 2; m < coeffs.GetLength(0); m += 2)
            {
                for (int n = 0; n < coeffs.GetLength(1) / 2; n += 2)
                {
                    hl1[c3, 0] = coeffs[m, n];
                    hl1[c3, 1] = coeffs[m, n + 1];
                    hl1[c3, 2] = coeffs[m + 1, n];
                    hl1[c3, 3] = coeffs[m + 1, n + 1];
                    c3++;
                }
            }

            ///Unite all 3 subband into 1 array
            //LH1
            for(int i = 0; i < lh1.GetLength(0); i++)
            {
                for(int j = 0; j < lh1.GetLength(1); j++)
                {
                    Scale1[i, j] = lh1[i, j];
                }
            }

            //HH1
            for (int i = 0; i < hh1.GetLength(0); i++)
            {
                for (int j = 0; j < hh1.GetLength(1); j++)
                {
                    Scale1[i+lh1.GetLength(0), j] = hh1[i, j];
                }
            }

            //HL1
            for (int i = 0; i < hh1.GetLength(0); i++)
            {
                for (int j = 0; j < hh1.GetLength(1); j++)
                {
                    Scale1[i + lh1.GetLength(0)+hh1.GetLength(0), j] = hh1[i, j];
                }
            }

            return Scale1;
        }

        public static double[,] ListOfCoeffs(double[,] coeffs)
        {
            double[] Scale2 = Haar.Scale2To1DCoeff(coeffs);
            double[,] Scale1 = Haar.Scale1To1DCoeff(coeffs);
            double[,] Scale = new double[Scale2.Length, 5];

            for (int i = 0; i < Scale2.Length; i++)
            {
                Scale[i, 0] = Scale2[i];
                Scale[i, 1] = Scale1[i, 0];
                Scale[i, 2] = Scale1[i, 1];
                Scale[i, 3] = Scale1[i, 2];
                Scale[i, 4] = Scale1[i, 3];

            }
            return Scale;
        }

        //END
    }
}
