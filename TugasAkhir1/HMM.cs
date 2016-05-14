using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace TugasAkhir1
{
    public class HMM
    {
        public static double[,] Embedding(double[,] Wavelet_coefficients,double[,] MappedWatermark)
        {
            double[,] Embedded_Watermark = new double[Wavelet_coefficients.GetLength(0), Wavelet_coefficients.GetLength(1)];
            double[,] Trained_Watermark = TrainMappedWatermark(Wavelet_coefficients, MappedWatermark);
            int embedding_Strength = 1;

            for (int i = 0; i < Wavelet_coefficients.GetLength(0); i++)
            {
                for (int j = 0; j < Wavelet_coefficients.GetLength(1); j++)
                {
                    Embedded_Watermark[i, j] = Wavelet_coefficients[i, j] + (Trained_Watermark[i, j]*embedding_Strength);
                }
            }

            return Embedded_Watermark;
        }

        public static double[,] TrainMappedWatermark(double[,] WC, double[,] MW)
        {
            
            double[,] TrainedWatermark = new double[WC.GetLength(0),WC.GetLength(1)];
            int sizeOfLvl2 = (TrainedWatermark.GetLength(0)/2) * (TrainedWatermark.GetLength(1)/2);
            ///LH1
            double[] LH1 = new double[MW.GetLength(0)];
            int countLH1 = 0;
            for (int i = 0; i < LH1.Length; i++)
            {
                LH1[i] = MW[i, 0];
            }

            double[,] LH2 = new double[MW.GetLength(0),4];
            for (int i = 0; i < LH2.GetLength(0); i++)
            {
                LH2[i,0] = MW[i,1];
                LH2[i,1] = MW[i,2];
                LH2[i,2] = MW[i,3];
                LH2[i,3] = MW[i,4];
            }

            
            ///HH1
            double[] HH1 = new double[MW.GetLength(0)];
            for (int i = 0; i < HH1.Length; i++)
            {
                HH1[i] = MW[i, 5];
            }

            double[,] HH2 = new double[MW.GetLength(0), 4];
            for (int i = 0; i < HH2.GetLength(0); i++)
            {
                HH2[i, 0] = MW[i, 6];
                HH2[i, 1] = MW[i, 7];
                HH2[i, 2] = MW[i, 8];
                HH2[i, 3] = MW[i, 9];
            }

            ///HL1 
            double[] HL1 = new double[MW.GetLength(0)];
            for (int i = 0; i < HL1.Length; i++)
            {
                HL1[i] = MW[i, 10];
            }

            double[,] HL2 = new double[MW.GetLength(0), 4];
            for (int i = 0; i < HL2.GetLength(0); i++)
            {
                HL2[i, 0] = MW[i, 11];
                HL2[i, 1] = MW[i, 12];
                HL2[i, 2] = MW[i, 13];
                HL2[i, 3] = MW[i, 14];
            }


            ///Mapping MappedWatermark through wavelet domain
            ///LH1
            int iLH1 = 0;
            for (int i = 0; i < TrainedWatermark.GetLength(0) / 4; i++)
            {
                for (int j = TrainedWatermark.GetLength(1) / 4; j < TrainedWatermark.GetLength(1) / 2; j++)
                {
                    if(iLH1<MW.GetLength(0))
                    {
                        TrainedWatermark[i, j] = LH1[iLH1];
                        iLH1++;
                    }
                    else
                    {
                        TrainedWatermark[i, j] = 0;
                    }
                    
                }
            }

            ///LH2
            int iLH2 = 0;
            for (int i = 0; i < (TrainedWatermark.GetLength(0) / 4); i += 2)
            {
                for (int j = TrainedWatermark.GetLength(1) / 2; j < TrainedWatermark.GetLength(1)*0.75; j += 2)
                {
                    TrainedWatermark[i, j] = LH2[iLH2, 0];
                    TrainedWatermark[i, j + 1] = LH2[iLH2, 1];
                    TrainedWatermark[i + 1, j] = LH2[iLH2, 2];
                    TrainedWatermark[i + 1, j + 1] = LH2[iLH2, 3];
                    iLH2++;
                }
            }


            ///HH1
            int iHH1 = 0;
            for (int i = TrainedWatermark.GetLength(0) / 4; i < TrainedWatermark.GetLength(0) / 2; i++)
            {
                for (int j = TrainedWatermark.GetLength(1) / 4; j < TrainedWatermark.GetLength(1) / 2; j++)
                {
                    if (iHH1 < MW.GetLength(0))
                    {
                        TrainedWatermark[i, j] = HH1[iHH1];
                        iHH1++;
                    }
                    else
                    {
                        TrainedWatermark[i, j] = 0;
                    }

                }
            }

            ///HH2
            int iHH2 = 0;
            for (int i = TrainedWatermark.GetLength(0) / 2; i < TrainedWatermark.GetLength(0)*0.75; i += 2)
            {
                for (int j = TrainedWatermark.GetLength(1) / 2; j < TrainedWatermark.GetLength(1)*0.75; j += 2)
                {
                    TrainedWatermark[i, j] = HH2[iHH2, 0];
                    TrainedWatermark[i, j + 1] = HH2[iHH2, 1];
                    TrainedWatermark[i + 1, j] = HH2[iHH2, 2];
                    TrainedWatermark[i + 1, j + 1] = HH2[iHH2, 3];
                    iHH2++;
                }
            }

            ///HL1
            int iHL1 = 0;
            for (int i = TrainedWatermark.GetLength(0) / 4; i < TrainedWatermark.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < TrainedWatermark.GetLength(1) / 4; j++)
                {
                    if (iHL1 < MW.GetLength(0))
                    {
                        TrainedWatermark[i, j] = HL1[iHL1];
                        iHL1++;
                    }
                    else
                    {
                        TrainedWatermark[i, j] = 0;
                    }

                }
            }

            ///HL2
            int iHL2 = 0;
            for (int i = TrainedWatermark.GetLength(0) / 2; i < TrainedWatermark.GetLength(0)*0.75; i += 2)
            {
                for (int j = 0; j < TrainedWatermark.GetLength(1)/4 / 2; j += 2)
                {
                    TrainedWatermark[i, j] = HL2[iHL2, 0];
                    TrainedWatermark[i, j + 1] = HL2[iHL2, 1];
                    TrainedWatermark[i + 1, j] = HL2[iHL2, 2];
                    TrainedWatermark[i + 1, j + 1] = HL2[iHL2, 3];
                    iHL2++;
                }
            }

            return TrainedWatermark;
        }


        #region Get the hidden states
        //2 Hidden State, High=1 and Low=2
        public static List<int> GetHiddenStates(List<int> coeff)
        {
            var hs = new List<int>();
            double threshold = Threshold(coeff);
            for (int i = 0; i < coeff.Count; i++)
            {
                if (coeff[i] > threshold)
                {
                    hs.Add(1);
                }
                else if (coeff[i] <= threshold)
                {
                    hs.Add(2);
                }
            }
            return hs;
        }

        public static Tuple<int,int> CountHS(List<int> hs)
        {
            int c;
            int hs1 = 0; 
            int hs2 = 0;
            for (int i = 0; i < hs.Count; i++)
            {
                if (hs[i] == 1)
                {
                    hs1++;
                }
                else if (hs[i] == 2)
                {
                    hs2++;
                }
            }
            return new Tuple<int, int>(hs1, hs2);
        }
        public static double Threshold(List<int> coeff)
        {
            double mean;
            int counter = 0;
            mean = coeff.Average();
            var groups = coeff.GroupBy(v => v);
            int maxCount = groups.Max(g => g.Count());
            int mode = groups.First(g => g.Count() == maxCount).Key;
            int max = coeff.Max();
            
            return mode;
        }
        #endregion


        #region Calculate 2-State Gaussian Mixture Model
        //public static double GaussianMixtureModel(Bitmap bmp, int level, string subband ,double prob1, double prob2, int Wi)
        //{
        //    double gmm;
        //    double gauss1 = Gaussian(bmp,Wi,level,subband);
        //    double gauss2 = Gaussian(bmp,Wi,level,subband);

        //    gmm = (prob1 * gauss1) + (prob2 * gauss2);

        //    return gmm;
        //}


        //public static double Gaussian(Bitmap bmp,int Wi ,int level, string subband )
        //{
        //    //List<int> Wn = GetWaveletCoeff(bmp,level,subband);
        //    var var = Statistic.Variance(Wn);
        //    double gauss = (1/Math.Sqrt(2*Math.PI*var))* Math.Exp(Math.Pow(-Wi,2)/2*var); 
        //    return gauss;
        //}
        #endregion


         
    }
}
