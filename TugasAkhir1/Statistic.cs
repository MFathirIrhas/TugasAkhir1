using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using System.Drawing;
using System.Drawing.Imaging;

namespace TugasAkhir1
{
    public class Statistic
    {
        public static double Mean(int[] d)
        {
            double mu = d.Average();
            return mu;
        }

        public static double Variance(List<double> d)
        {
            double mean = d.Average();
            double sum = d.Select(val => (val - mean) * (val - mean)).Sum();
            double variance = sum / d.Count;
            return variance;
        }

        public static double[,] Covariance(double[,] d) //Calculate Covariance Matrix of WD-HMM Vector
        {
            var cov = Accord.Math.Matrix.Multiply(d, Accord.Math.Matrix.Transpose(d));
            return cov;
        }

        public static double Det(double[,] d)
        {
            var det = Accord.Math.Matrix.Determinant(d);
            return det;
        }

        public static double Akurasi(Bitmap bmp1, Bitmap bmp2)
        {
            List<int> akurasi = new List<int>();
            //Bitmap tr = new Bitmap(transformedImage.Image);
            for (int i = 0; i < bmp1.Height; i++)
            {
                for (int j = 0; j < bmp1.Width; j++)
                {
                    Color c1 = bmp1.GetPixel(j, i);
                    Color c2 = bmp2.GetPixel(j, i);
                    if (c1.R == c2.R)
                    {
                        akurasi.Add(1);
                    }
                    else
                    {
                        akurasi.Add(0);
                    }

                }
            }
            double sumup = akurasi.Sum();
            double ak = (sumup / (double)akurasi.Count) * 100;
            return ak;
        }
        
        
        ///Performance calculation
        ///MSE
        ///PSNR
        ///BER

        public static double MSE(Bitmap original, Bitmap transformed)
        {
            double[,] originalM = ImageProcessing.ConvertToMatrix(original);
            double[,] transformedM = ImageProcessing.ConvertToMatrix(transformed);

            double size = original.Width * original.Height;
            double sum=0;
            for (int i = 0; i < original.Height; i++)
            {
                for (int j = 0; j < original.Width; j++)
                {
                    sum += Math.Pow(originalM[i,j]-transformedM[i,j],2);
                }
            }

            double mse = sum / size;
            return mse;
        }

        public static double PSNR(Bitmap transformed,double mse)
        {
            double[,] transformedM = ImageProcessing.ConvertToMatrix(transformed);
            double max = transformedM.Cast<double>().Max();
            double psnr = 10 * (Math.Log10(max / Math.Sqrt(mse)));
            return psnr;
        }

        public static double BER(Bitmap bmp1, Bitmap bmp2)
        {
            List<int> check = new List<int>();
            for (int i = 0; i < bmp1.Height; i++)
            {
                for (int j = 0; j < bmp1.Width; j++)
                {
                    Color c1 = bmp1.GetPixel(j, i);
                    Color c2 = bmp2.GetPixel(j, i);
                    if (c1.R == c2.R)
                    {
                        check.Add(1);
                    }
                    else
                    {
                        check.Add(0);
                    }

                }
            }
            double sumup = check.Sum();
            double ber = (sumup / (double)check.Count) * 100;
            return ber;
        }
    }
}
