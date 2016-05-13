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
        
        
    }
}
