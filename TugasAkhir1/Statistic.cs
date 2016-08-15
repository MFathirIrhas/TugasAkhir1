using System;
using System.Collections;
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
        
        public static double Mean2(List<double> subband)
        {
            double mean = subband.Average();
            return mean;
        }

        public static double Mode2(List<double> subband)
        {
            var groups = subband.GroupBy(v => v);
            int maxCount = groups.Max(g => g.Count());
            double mode = groups.First(g => g.Count() == maxCount).Key;
            return mode;
        }
        
        #region Performance calculation
        ///MSE
        ///PSNR
        ///BER

        /*
         * Mean Square Error = Differences between Original Image and and Watermarked Image
         * Peak Signal Noise Ratio = Similarity between Original Image and Watermarked Image
         * Bit Error Rate = Percentage of different bit between original image and watermarked image,
         *      also between pre-embed watermark and extracted watermark.
         */
        public static double MSE(Bitmap original, Bitmap transformed)
        {
            double[,] originalM = ImageProcessing.ConvertToMatrix2(original).Item2;
            double[,] transformedM = ImageProcessing.ConvertToMatrix2(transformed).Item2;

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
            double[,] transformedM = ImageProcessing.ConvertToMatrix2(transformed).Item2;
            double max = transformedM.Cast<double>().Max();
            double psnr = 10 * (Math.Log10(Math.Pow(max,2) / Math.Sqrt(mse)));
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
                    if (c1.G == c2.G)
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
            double error = check.Count - sumup;
            double ber = (error / (double)check.Count) * 100;
            return ber;
        }
        #endregion


        #region Round all elements in double 2 dimensional array
        public static double[,] RoundAll(double[,] coeffs)
        {
            double[,] round = new double[coeffs.GetLength(0),coeffs.GetLength(0)];
            for(int i = 0; i < round.GetLength(0); i++)
            {
                for(int j = 0; j < round.GetLength(1); j++)
                {
                    round[i, j] = Math.Round(coeffs[i, j]);
                }
            }

            return round;
        }
        #endregion


        #region Calculate Variance of Image
        public static double VarianceOfImage(Bitmap bmp)
        {
            List<int> m = new List<int>();
            int width = bmp.Width;
            int height = bmp.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    int p = c.G;//(c.R + c.G + c.B) / 3;
                    m.Add(p);
                }
            }

            int[] pixels = m.ToArray();
            double Variance = Accord.Statistics.Tools.Variance(pixels);
            return Variance;
        }
        #endregion

        public static int[,] ExtractGreen(Bitmap bmp)
        {
            int[,] m = new int[bmp.Height,bmp.Width];
            int width = bmp.Width;
            int height = bmp.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    int p = c.G;//(c.R + c.G + c.B) / 3;
                    m[y, x] = p;
                }
            }
            return m;
        }

        public static int MaxPixel(Bitmap bmp)
        {
            List<int> m = new List<int>();
            int width = bmp.Width;
            int height = bmp.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    int p = c.G;//(c.R + c.G + c.B) / 3;
                    m.Add(p);
                }
            }

            int[] pixels = m.ToArray();
            int maxpixel = pixels.Max();
            return maxpixel;
        }


        #region Gaussian Random Variable
        public static float NextGaussian()
        {
            Random r = new Random();
            float v1, v2, s;
            do
            {
                v1 = 2.0f * (float)r.Next(0, 1) - 1.0f;
                v2 = 2.0f * (float)r.Next(0, 1) - 1.0f;
                s = v1 * v1 + v2 * v2;
            } while (s >= 1.0f || s == 0f);

            s = (float)Math.Sqrt((-2.0f * Math.Log(s)) / s);

            return v1 * s;
        }

        public static float NextGaussian(float mean, float standard_deviation)
        {
            return mean + NextGaussian() * standard_deviation;
        }

        public static float NextGaussian(float mean, float standard_deviation, float min, float max)
        {
            float x;
            do
            {
                x = NextGaussian(mean, standard_deviation);
            } while (x < min || x > max);

            return x;
        }
        #endregion



        public static double GaussianRandom(double mean, double std)
        {
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = (int)(Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2)); //random normal(0,1)
            double randNormal = mean + std * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }
    }
}
