using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Accord.Statistics;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Distributions.Fitting;

namespace TugasAkhir1
{
    public class Embed
    {
        public static double[,] Embedding(double[,] Wavelet_coefficients, double[,] MappedWatermark, double[,] HVSValues)
        {
            double[,] Embedded_Watermark = new double[Wavelet_coefficients.GetLength(0), Wavelet_coefficients.GetLength(1)];
            //double[,] Trained_Watermark = TrainMappedWatermark(Wavelet_coefficients, MappedWatermark);
            double[,] Trained_Watermark = TrainMappedWatermark2(Wavelet_coefficients, MappedWatermark,"hh");
            double embedding_Strength =0.2;//0.0001;//0.01;//0.16111;

            for (int i = 0; i < Wavelet_coefficients.GetLength(0); i++)
            {
                for (int j = 0; j < Wavelet_coefficients.GetLength(1); j++)
                {
                    Embedded_Watermark[i, j] = Wavelet_coefficients[i, j] + (Trained_Watermark[i, j] * embedding_Strength * HVSValues[i, j]);
                    //Embedded_Watermark[i, j] = Wavelet_coefficients[i, j] + (Trained_Watermark[i, j] * embedding_Strength);
                }
            }

            return Embedded_Watermark;
        }

        #region Train Watermark from Mapped Watermark
        /// <summary>
        /// Convert list the Mapped watermark(each row is 15 bit) into matrix with the same size with wavelet coefficients
        /// </summary>
        /// <param name="WC"></param>
        /// <param name="MW"></param>
        /// <returns></returns>
        public static double[,] TrainMappedWatermark(double[,] WC, double[,] MW)
        {
            double[,] TrainedWatermark = new double[WC.GetLength(0), WC.GetLength(1)];
            int sizeOfLvl2 = (TrainedWatermark.GetLength(0) / 2) * (TrainedWatermark.GetLength(1) / 2);
            ///LH1
            double[] LH1 = new double[MW.GetLength(0)];
            int countLH1 = 0;
            for (int i = 0; i < LH1.Length; i++)
            {
                LH1[i] = MW[i, 0];
            }

            double[,] LH2 = new double[MW.GetLength(0), 4];
            for (int i = 0; i < LH2.GetLength(0); i++)
            {
                LH2[i, 0] = MW[i, 1];
                LH2[i, 1] = MW[i, 2];
                LH2[i, 2] = MW[i, 3];
                LH2[i, 3] = MW[i, 4];
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
                    if (iLH1 < MW.GetLength(0)) //Check if counter smaller than total of tree provided compared to all coefficients in the subband.
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
            //int iLH2 = 0;
            //for (int i = 0; i < (TrainedWatermark.GetLength(0) / 4); i += 2)
            //{
            //    for (int j = TrainedWatermark.GetLength(1) / 2; j < TrainedWatermark.GetLength(1) * 0.75; j += 2)
            //    {
            //        TrainedWatermark[i, j] = LH2[iLH2, 0];
            //        TrainedWatermark[i, j + 1] = LH2[iLH2, 1];
            //        TrainedWatermark[i + 1, j] = LH2[iLH2, 2];
            //        TrainedWatermark[i + 1, j + 1] = LH2[iLH2, 3];
            //        iLH2++;
            //    }
            //}
            int iLH2 = 0;
            for (int i = 0; i < (TrainedWatermark.GetLength(0) / 2); i += 2)
            {
                for (int j = TrainedWatermark.GetLength(1) / 2; j < TrainedWatermark.GetLength(1); j += 2)
                {
                    if (iLH2 < MW.GetLength(0))
                    {
                        TrainedWatermark[i, j] = LH2[iLH2, 0];
                        TrainedWatermark[i, j + 1] = LH2[iLH2, 1];
                        TrainedWatermark[i + 1, j] = LH2[iLH2, 2];
                        TrainedWatermark[i + 1, j + 1] = LH2[iLH2, 3];
                        iLH2++;
                    }
                    else
                    {
                        TrainedWatermark[i, j] = 0;
                        TrainedWatermark[i, j + 1] = 0;
                        TrainedWatermark[i + 1, j] = 0;
                        TrainedWatermark[i + 1, j + 1] = 0;
                    }

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
            //int iHH2 = 0;
            //for (int i = TrainedWatermark.GetLength(0) / 2; i < TrainedWatermark.GetLength(0) * 0.75; i += 2)
            //{
            //    for (int j = TrainedWatermark.GetLength(1) / 2; j < TrainedWatermark.GetLength(1) * 0.75; j += 2)
            //    {
            //        TrainedWatermark[i, j] = HH2[iHH2, 0];
            //        TrainedWatermark[i, j + 1] = HH2[iHH2, 1];
            //        TrainedWatermark[i + 1, j] = HH2[iHH2, 2];
            //        TrainedWatermark[i + 1, j + 1] = HH2[iHH2, 3];
            //        iHH2++;
            //    }
            //}
            int iHH2 = 0;
            for (int i = TrainedWatermark.GetLength(0) / 2; i < TrainedWatermark.GetLength(0); i += 2)
            {
                for (int j = TrainedWatermark.GetLength(1) / 2; j < TrainedWatermark.GetLength(1); j += 2)
                {
                    if (iHH2 < MW.GetLength(0))
                    {
                        TrainedWatermark[i, j] = HH2[iHH2, 0];
                        TrainedWatermark[i, j + 1] = HH2[iHH2, 1];
                        TrainedWatermark[i + 1, j] = HH2[iHH2, 2];
                        TrainedWatermark[i + 1, j + 1] = HH2[iHH2, 3];
                        iHH2++;
                    }
                    else
                    {
                        TrainedWatermark[i, j] = 0;
                        TrainedWatermark[i, j + 1] = 0;
                        TrainedWatermark[i + 1, j] = 0;
                        TrainedWatermark[i + 1, j + 1] = 0;
                    }

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
            //int iHL2 = 0;
            //for (int i = TrainedWatermark.GetLength(0) / 2; i < TrainedWatermark.GetLength(0) * 0.75; i += 2)
            //{
            //    for (int j = 0; j < TrainedWatermark.GetLength(1) / 4 / 2; j += 2)
            //    {
            //        TrainedWatermark[i, j] = HL2[iHL2, 0];
            //        TrainedWatermark[i, j + 1] = HL2[iHL2, 1];
            //        TrainedWatermark[i + 1, j] = HL2[iHL2, 2];
            //        TrainedWatermark[i + 1, j + 1] = HL2[iHL2, 3];
            //        iHL2++;
            //    }
            //}
            int iHL2 = 0;
            for (int i = TrainedWatermark.GetLength(0) / 2; i < TrainedWatermark.GetLength(0); i += 2)
            {
                for (int j = 0; j < TrainedWatermark.GetLength(1) / 2; j += 2)
                {
                    if (iHL2 < MW.GetLength(0))
                    {
                        TrainedWatermark[i, j] = HL2[iHL2, 0];
                        TrainedWatermark[i, j + 1] = HL2[iHL2, 1];
                        TrainedWatermark[i + 1, j] = HL2[iHL2, 2];
                        TrainedWatermark[i + 1, j + 1] = HL2[iHL2, 3];
                        iHL2++;
                    }
                    else
                    {
                        TrainedWatermark[i, j] = 0;
                        TrainedWatermark[i, j + 1] = 0;
                        TrainedWatermark[i + 1, j] = 0;
                        TrainedWatermark[i + 1, j + 1] = 0;
                    }

                }
            }

            return TrainedWatermark;
        }
        #endregion


        #region Train Watermark without mapping watermark
        /// <summary>
        /// Convert segmented watermark without mapping strategy
        /// </summary>
        /// <param name="WC"></param>
        /// <param name="MW"></param>
        /// <returns></returns>
        public static double[,] TrainMappedWatermark2(double[,] WC, double[,] MW, string subband)
        {
            double[,] TrainedWatermark = new double[WC.GetLength(0), WC.GetLength(1)];
            int sizeOfLvl2 = (TrainedWatermark.GetLength(0) / 2) * (TrainedWatermark.GetLength(1) / 2);
            if(subband == "lh")
            {
                ///LH1
                double[] LH1 = new double[MW.GetLength(0)];
                int countLH1 = 0;
                for (int i = 0; i < LH1.Length; i++)
                {
                    LH1[i] = MW[i, 0];
                }

                double[,] LH2 = new double[MW.GetLength(0), 4];
                for (int i = 0; i < LH2.GetLength(0); i++)
                {
                    LH2[i, 0] = MW[i, 1];
                    LH2[i, 1] = MW[i, 2];
                    LH2[i, 2] = MW[i, 3];
                    LH2[i, 3] = MW[i, 4];
                }

                ///Mapping MappedWatermark through wavelet domain
                ///LH1
                int iLH1 = 0;
                for (int i = 0; i < TrainedWatermark.GetLength(0) / 4; i++)
                {
                    for (int j = TrainedWatermark.GetLength(1) / 4; j < TrainedWatermark.GetLength(1) / 2; j++)
                    {
                        if (iLH1 < MW.GetLength(0)) //Check if counter smaller than total of tree provided compared to all coefficients in the subband.
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
                //int iLH2 = 0;
                //for (int i = 0; i < (TrainedWatermark.GetLength(0) / 4); i += 2)
                //{
                //    for (int j = TrainedWatermark.GetLength(1) / 2; j < TrainedWatermark.GetLength(1) * 0.75; j += 2)
                //    {
                //        TrainedWatermark[i, j] = LH2[iLH2, 0];
                //        TrainedWatermark[i, j + 1] = LH2[iLH2, 1];
                //        TrainedWatermark[i + 1, j] = LH2[iLH2, 2];
                //        TrainedWatermark[i + 1, j + 1] = LH2[iLH2, 3];
                //        iLH2++;
                //    }
                //}
                int iLH2 = 0;
                for (int i = 0; i < (TrainedWatermark.GetLength(0) / 2); i += 2)
                {
                    for (int j = TrainedWatermark.GetLength(1) / 2; j < TrainedWatermark.GetLength(1); j += 2)
                    {
                        if (iLH2 < MW.GetLength(0))
                        {
                            TrainedWatermark[i, j] = LH2[iLH2, 0];
                            TrainedWatermark[i, j + 1] = LH2[iLH2, 1];
                            TrainedWatermark[i + 1, j] = LH2[iLH2, 2];
                            TrainedWatermark[i + 1, j + 1] = LH2[iLH2, 3];
                            iLH2++;
                        }
                        else
                        {
                            TrainedWatermark[i, j] = 0;
                            TrainedWatermark[i, j + 1] = 0;
                            TrainedWatermark[i + 1, j] = 0;
                            TrainedWatermark[i + 1, j + 1] = 0;
                        }

                    }
                }

                return TrainedWatermark;
            }
            else if( subband == "hh")
            {
                ///HH1
                double[] HH1 = new double[MW.GetLength(0)];
                for (int i = 0; i < HH1.Length; i++)
                {
                    HH1[i] = MW[i, 0];
                }

                double[,] HH2 = new double[MW.GetLength(0), 4];
                for (int i = 0; i < HH2.GetLength(0); i++)
                {
                    HH2[i, 0] = MW[i, 1];
                    HH2[i, 1] = MW[i, 2];
                    HH2[i, 2] = MW[i, 3];
                    HH2[i, 3] = MW[i, 4];
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
                //int iHH2 = 0;
                //for (int i = TrainedWatermark.GetLength(0) / 2; i < TrainedWatermark.GetLength(0) * 0.75; i += 2)
                //{
                //    for (int j = TrainedWatermark.GetLength(1) / 2; j < TrainedWatermark.GetLength(1) * 0.75; j += 2)
                //    {
                //        TrainedWatermark[i, j] = HH2[iHH2, 0];
                //        TrainedWatermark[i, j + 1] = HH2[iHH2, 1];
                //        TrainedWatermark[i + 1, j] = HH2[iHH2, 2];
                //        TrainedWatermark[i + 1, j + 1] = HH2[iHH2, 3];
                //        iHH2++;
                //    }
                //}
                int iHH2 = 0;
                for (int i = TrainedWatermark.GetLength(0) / 2; i < TrainedWatermark.GetLength(0); i += 2)
                {
                    for (int j = TrainedWatermark.GetLength(1) / 2; j < TrainedWatermark.GetLength(1); j += 2)
                    {
                        if (iHH2 < MW.GetLength(0))
                        {
                            TrainedWatermark[i, j] = HH2[iHH2, 0];
                            TrainedWatermark[i, j + 1] = HH2[iHH2, 1];
                            TrainedWatermark[i + 1, j] = HH2[iHH2, 2];
                            TrainedWatermark[i + 1, j + 1] = HH2[iHH2, 3];
                            iHH2++;
                        }
                        else
                        {
                            TrainedWatermark[i, j] = 0;
                            TrainedWatermark[i, j + 1] = 0;
                            TrainedWatermark[i + 1, j] = 0;
                            TrainedWatermark[i + 1, j + 1] = 0;
                        }

                    }
                }

                return TrainedWatermark;
            }
            else
            {
                ///HL1 
                double[] HL1 = new double[MW.GetLength(0)];
                for (int i = 0; i < HL1.Length; i++)
                {
                    HL1[i] = MW[i, 0];
                }

                double[,] HL2 = new double[MW.GetLength(0), 4];
                for (int i = 0; i < HL2.GetLength(0); i++)
                {
                    HL2[i, 0] = MW[i, 1];
                    HL2[i, 1] = MW[i, 2];
                    HL2[i, 2] = MW[i, 3];
                    HL2[i, 3] = MW[i, 4];
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
                //int iHL2 = 0;
                //for (int i = TrainedWatermark.GetLength(0) / 2; i < TrainedWatermark.GetLength(0) * 0.75; i += 2)
                //{
                //    for (int j = 0; j < TrainedWatermark.GetLength(1) / 4 / 2; j += 2)
                //    {
                //        TrainedWatermark[i, j] = HL2[iHL2, 0];
                //        TrainedWatermark[i, j + 1] = HL2[iHL2, 1];
                //        TrainedWatermark[i + 1, j] = HL2[iHL2, 2];
                //        TrainedWatermark[i + 1, j + 1] = HL2[iHL2, 3];
                //        iHL2++;
                //    }
                //}
                int iHL2 = 0;
                for (int i = TrainedWatermark.GetLength(0) / 2; i < TrainedWatermark.GetLength(0); i += 2)
                {
                    for (int j = 0; j < TrainedWatermark.GetLength(1) / 2; j += 2)
                    {
                        if (iHL2 < MW.GetLength(0))
                        {
                            TrainedWatermark[i, j] = HL2[iHL2, 0];
                            TrainedWatermark[i, j + 1] = HL2[iHL2, 1];
                            TrainedWatermark[i + 1, j] = HL2[iHL2, 2];
                            TrainedWatermark[i + 1, j + 1] = HL2[iHL2, 3];
                            iHL2++;
                        }
                        else
                        {
                            TrainedWatermark[i, j] = 0;
                            TrainedWatermark[i, j + 1] = 0;
                            TrainedWatermark[i + 1, j] = 0;
                            TrainedWatermark[i + 1, j + 1] = 0;
                        }

                    }
                }

                return TrainedWatermark;
            }
                       
        }
        #endregion


        #region Adaptive HVS calculation
        /// <summary>
        /// 3 factors affect eye sensitivity:
        /// - Frequency band
        /// - Luminance
        /// - Texture
        /// </summary>
        /// <param name="coeffs"></param>
        /// <returns></returns>
        public static double[,] AdaptiveHVS(double[,] coeffs, double[,] pixels)
        {
            double[,] hvs = new double[pixels.GetLength(0), pixels.GetLength(1)];

            double[,] frequency = new double[pixels.GetLength(0), pixels.GetLength(1)];
            double[,] luminance = new double[pixels.GetLength(0), pixels.GetLength(1)];
            double[,] texture   = new double[pixels.GetLength(0), pixels.GetLength(1)];

            #region Frequency
            /// LL 2
            for(int i = 0; i < frequency.GetLength(0)/4; i++)
            {
                for(int j = 0; j < frequency.GetLength(1)/4; j++)
                {
                    frequency[i, j] = 1 * 0.32;
                }
            }

            /// LH 2 
            for (int i = 0; i < frequency.GetLength(0) / 4; i++)
            {
                for (int j = frequency.GetLength(1) / 4; j < frequency.GetLength(1) / 2; j++)
                {
                    frequency[i, j] = 1 * 0.32;
                }
            }

            /// HH 2
            for (int i = frequency.GetLength(0) / 4; i < frequency.GetLength(0) / 2; i++)
            {
                for (int j = frequency.GetLength(1) / 4; j < frequency.GetLength(1) / 2; j++)
                {
                    frequency[i, j] = Math.Sqrt(2) * 0.32;
                }
            }

            /// HL 2
            for (int i = frequency.GetLength(0) / 4; i < frequency.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < frequency.GetLength(1) / 4; j++)
                {
                    frequency[i, j] = 1 * 0.32;
                }
            }

            /// LH 1 
            for (int i = 0; i < frequency.GetLength(0) / 2; i++)
            {
                for (int j = frequency.GetLength(1) / 2; j < frequency.GetLength(1); j++)
                {
                    frequency[i, j] = 1 * 1;
                }
            }

            /// HH 2
            for (int i = frequency.GetLength(0) / 2; i < frequency.GetLength(0); i++)
            {
                for (int j = frequency.GetLength(1) / 2; j < frequency.GetLength(1); j++)
                {
                    frequency[i, j] = Math.Sqrt(2) * 1;
                }
            }

            /// HL 2
            for (int i = frequency.GetLength(0) / 2; i < frequency.GetLength(0); i++)
            {
                for (int j = 0; j < frequency.GetLength(1)/2; j++)
                {
                    frequency[i, j] = 1 * 1;
                }
            }
            #endregion

            #region Luminance
            /// LL 2
            for (int i = 0; i < luminance.GetLength(0) / 4; i++)
            {
                for (int j = 0; j < luminance.GetLength(1) / 4; j++)
                {
                    if(coeffs[i,j] < 0.5)
                    {
                        luminance[i, j] = 1 - pixels[i, j];
                    }else
                    {
                        luminance[i, j] = pixels[i, j];
                    }
                }
            }

            /// LH 2 
            for (int i = 0; i < luminance.GetLength(0) / 4; i++)
            {
                for (int j = luminance.GetLength(1) / 4; j < luminance.GetLength(1) / 2; j++)
                {
                    if(pixels[i,j-(luminance.GetLength(1) / 4)] < 0.5)
                    {
                        luminance[i, j] = 1 - pixels[i, j - (luminance.GetLength(1) / 4)];
                    }else
                    {
                        luminance[i, j] = pixels[i, j - (luminance.GetLength(1) / 4)];
                    }
                }
            }

            /// HH 2
            for (int i = luminance.GetLength(0) / 4; i < luminance.GetLength(0) / 2; i++)
            {
                for (int j = luminance.GetLength(1) / 4; j < luminance.GetLength(1) / 2; j++)
                {
                    if (pixels[i - (luminance.GetLength(0) / 4), j - (luminance.GetLength(1) / 4)] < 0.5)
                    {
                        luminance[i, j] = 1 - pixels[i - (luminance.GetLength(0) / 4), j - (luminance.GetLength(1) / 4)];
                    }
                    else
                    {
                        luminance[i, j] = pixels[i - (luminance.GetLength(0) / 4), j - (luminance.GetLength(1) / 4)];
                    }
                }
            }

            /// HL 2
            for (int i = luminance.GetLength(0) / 4; i < luminance.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < luminance.GetLength(1) / 4; j++)
                {
                    if (pixels[i - (luminance.GetLength(0) / 4), j] < 0.5)
                    {
                        luminance[i, j] = 1 - pixels[i - (luminance.GetLength(0) / 4), j];
                    }
                    else
                    {
                        luminance[i, j] = pixels[i - (luminance.GetLength(0) / 4), j];
                    }
                }
            }

            /// LH 1 
            for (int i = 0; i < luminance.GetLength(0) / 2; i++)
            {
                for (int j = luminance.GetLength(1) / 2; j < luminance.GetLength(1); j++)
                {
                    if(pixels[i,j-(luminance.GetLength(1) / 2)] < 0.5)
                    {
                        luminance[i, j] = 1 - pixels[i, j - (luminance.GetLength(1) / 2)];
                    }else
                    {
                        luminance[i, j] = pixels[i, j - (luminance.GetLength(1) / 2)];
                    }
                }
            }

            /// HH 2
            for (int i = luminance.GetLength(0) / 2; i < luminance.GetLength(0); i++)
            {
                for (int j = luminance.GetLength(1) / 2; j < luminance.GetLength(1); j++)
                {
                    if (pixels[i-(luminance.GetLength(0) / 2), j - (luminance.GetLength(1) / 2)] < 0.5)
                    {
                        luminance[i, j] = 1 - pixels[i - (luminance.GetLength(0) / 2), j - (luminance.GetLength(1) / 2)];
                    }
                    else
                    {
                        luminance[i, j] = pixels[i - (luminance.GetLength(0) / 2), j - (luminance.GetLength(1) / 2)];
                    }
                }
            }

            /// HL 2
            for (int i = luminance.GetLength(0) / 2; i < luminance.GetLength(0); i++)
            {
                for (int j = 0; j < luminance.GetLength(1) / 2; j++)
                {
                    if (pixels[i - (luminance.GetLength(0) / 2), j] < 0.5)
                    {
                        luminance[i, j] = 1 - pixels[i - (luminance.GetLength(0) / 2), j];
                    }
                    else
                    {
                        luminance[i, j] = pixels[i - (luminance.GetLength(0) / 2), j];
                    }
                }
            }

            /// final calculation of luminance
            for(int k = 0; k < luminance.GetLength(0); k++)
            {
                for(int l = 0;l < luminance.GetLength(1); l++)
                {
                    luminance[k, l] = 1 + luminance[k, l];
                }
            }
            #endregion

            #region texture
            ///Extract low-low subband
            ///level 2
            List<double> approx2 = new List<double>();
            for(int i = 0; i < texture.GetLength(0) / 4; i++)
            {
                for(int j = 0; j < texture.GetLength(1) / 4; j++)
                {
                    approx2.Add(coeffs[i, j]);
                }
            }
            double[] vectorApprox2 = approx2.ToArray();
            double varApprox2 = Accord.Statistics.Tools.Variance(vectorApprox2);

            List<double> approx1 = new List<double>();
            for (int i = 0; i < texture.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < texture.GetLength(1) / 2; j++)
                {
                    approx1.Add(coeffs[i, j]);
                }
            }
            double[] vectorApprox1 = approx1.ToArray();
            double varApprox1 = Accord.Statistics.Tools.Variance(vectorApprox1);

            /// LL 2
            for (int i = 0; i < texture.GetLength(0) / 4; i++)
            {
                for (int j = 0; j < texture.GetLength(1) / 4; j++)
                {
                    texture[i, j] = (((Math.Pow(pixels[i, j + (texture.GetLength(1) / 4)], 2) + Math.Pow(pixels[i + (texture.GetLength(0) / 4), j + (texture.GetLength(1) / 4)], 2) + Math.Pow(pixels[i + (texture.GetLength(0) / 4), j], 2))) / 3) * varApprox2;
                }
            }

            /// LH 2 
            for (int i = 0; i < texture.GetLength(0) / 4; i++)
            {
                for (int j = texture.GetLength(1) / 4; j < texture.GetLength(1) / 2; j++)
                {
                    texture[i, j] = (((Math.Pow(pixels[i, j], 2) + Math.Pow(pixels[i + (texture.GetLength(0) / 4), j], 2) + Math.Pow(pixels[i + (texture.GetLength(0) / 4), j - (texture.GetLength(1) / 4)], 2))) / 3) * varApprox2;
                    //texture[i, j] = ((Math.Pow(pixels[i, j], 2)) / 3) * varApprox2;
                }
            }

            /// HH 2
            for (int i = texture.GetLength(0) / 4; i < texture.GetLength(0) / 2; i++)
            {
                for (int j = texture.GetLength(1) / 4; j < texture.GetLength(1) / 2; j++)
                {
                    texture[i, j] = (((Math.Pow(pixels[i - (texture.GetLength(0) / 4), j], 2) + Math.Pow(pixels[i, j], 2) + Math.Pow(pixels[i, j - (texture.GetLength(1) / 4)], 2))) / 3) * varApprox2;
                    //texture[i, j] = (Math.Pow(coeffs[i, j], 2) / 3) * varApprox2;
                }
            }

            /// HL 2
            for (int i = texture.GetLength(0) / 4; i < texture.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < texture.GetLength(1) / 4; j++)
                {
                    texture[i, j] = (((Math.Pow(pixels[i - (texture.GetLength(0) / 4), j + (texture.GetLength(1) / 4)], 2) + Math.Pow(pixels[i, j + (texture.GetLength(1) / 4)], 2) + Math.Pow(pixels[i, j], 2))) / 3) * varApprox2;
                }
            }

            /// LH 1 
            for (int i = 0; i < texture.GetLength(0) / 2; i++)
            {
                for (int j = texture.GetLength(1) / 2; j < texture.GetLength(1); j++)
                {
                    texture[i, j] = (((Math.Pow(pixels[i, j], 2) + Math.Pow(pixels[i + (texture.GetLength(0) / 2), j], 2) + Math.Pow(pixels[i + (texture.GetLength(0) / 2), j-(texture.GetLength(1) / 2)], 2))) / 3) * varApprox1;
                }
            }

            /// HH 2
            for (int i = texture.GetLength(0) / 2; i < texture.GetLength(0); i++)
            {
                for (int j = texture.GetLength(1) / 2; j < texture.GetLength(1); j++)
                {
                    texture[i, j] = (((Math.Pow(pixels[i-(texture.GetLength(0) / 2), j], 2) + Math.Pow(pixels[i, j], 2) + Math.Pow(pixels[i, j - (texture.GetLength(1) / 2)], 2))) / 3) * varApprox1;
                }
            }

            /// HL 2
            for (int i = texture.GetLength(0) / 2; i < texture.GetLength(0); i++)
            {
                for (int j = 0; j < texture.GetLength(1) / 2; j++)
                {
                    texture[i, j] = (((Math.Pow(pixels[i - (texture.GetLength(0) / 2), j + (texture.GetLength(1) / 2)], 2) + Math.Pow(pixels[i, j + (texture.GetLength(1) / 2)], 2) + Math.Pow(pixels[i, j], 2))) / 3) * varApprox1;
                }
            }
            #endregion

            #region Final Calculation of HVS
            for(int m = 0; m < hvs.GetLength(0); m++)
            {
                for(int n = 0;n < hvs.GetLength(1); n++)
                {
                    hvs[m, n] = frequency[m, n] * Math.Pow(luminance[m, n],0.2) * Math.Pow(texture[m, n],0.2);  
                }
            }
            #endregion

            //List<double> list = new List<double>();
            //for (int o = 0; o < hvs.GetLength(0); o++)
            //{
            //    for (int p = 0; p < hvs.GetLength(1); p++)
            //    {
            //        list.Add(hvs[o, p]);
            //    }
            //}

            //for (int q = 0; q < hvs.GetLength(0); q++)
            //{
            //    for (int r = 0; r < hvs.GetLength(1); r++)
            //    {
            //        hvs[q, r] = hvs[q, r] + list.Min();
            //    }
            //}

            return hvs;
        }


        #endregion

        #region Proposed HVS Calculation
        public static double[,] AdaptiveHVS2(double[,] coeffs, int NumOfTree)
        {
            double[,] HH2 = new double[coeffs.GetLength(0) / 4, coeffs.GetLength(1) / 4];
            double[,] HH1 = new double[coeffs.GetLength(0) / 2, coeffs.GetLength(1) / 2];
            double[,] hvs = new double[coeffs.GetLength(0), coeffs.GetLength(1)];
            for(int i = 0; i < HH2.GetLength(0); i++)
            {
                for(int j = 0; j < HH2.GetLength(1); j++)
                {
                    HH2[i, j] = ((coeffs[i, j] * VarianceOfHH2(coeffs,NumOfTree))/4) + 0.01;
                }
            }

            
            for(int k = hvs.GetLength(0)/4; k < hvs.GetLength(0)/2; k++)
            {
                for(int l = hvs.GetLength(1)/4; l < hvs.GetLength(1)/2; l++)
                {
                    hvs[k, l] = HH2[k - (hvs.GetLength(0) / 4), l - (hvs.GetLength(1) / 4)];
                }
            }

            for(int m = hvs.GetLength(0) / 2; m < hvs.GetLength(0); m++)
            {
                for(int n = hvs.GetLength(1) / 2; n < hvs.GetLength(1); n++)
                {
                    hvs[m, n] = 0.01;//HH2[m - (hvs.GetLength(0) / 2), n - (hvs.GetLength(1) / 2)];
                }
            }

            return hvs;
        }

        public static double VarianceOfHH2(double[,] coeffs,int NumOfTree)
        {
            List<double> list = new List<double>();
            List<double> returnList = new List<double>();
            for (int i = coeffs.GetLength(0) / 4; i < coeffs.GetLength(0) / 2; i++)
            {
                for(int j=coeffs.GetLength(1) / 4; j < coeffs.GetLength(1) / 2; j++)
                {
                    list.Add(coeffs[i, j]);
                }
            }

            for(int k = 0; k < NumOfTree; k++)
            {
                returnList.Add(list[k]);
            }

            double[] listHH2 = returnList.ToArray();
            double varianceHH2 = Tools.Variance(listHH2);
            return varianceHH2;
        }

        public static double VarianceOfLH2(double[,] coeffs, int NumOfTree)
        {
            List<double> list = new List<double>();
            List<double> returnList = new List<double>();
            for (int i = 0; i < coeffs.GetLength(0) / 4; i++)
            {
                for (int j = coeffs.GetLength(1) / 4; j < coeffs.GetLength(1) / 2; j++)
                {
                    list.Add(coeffs[i, j]);
                }
            }

            for (int k = 0; k < NumOfTree; k++)
            {
                returnList.Add(list[k]);
            }

            double[] listLH2 = returnList.ToArray();
            double varianceLH2 = Tools.Variance(listLH2);
            return varianceLH2;
        }

        double VarianceOfHH1(double[,] coeffs, int NumOfTree)
        {
            List<double> list = new List<double>();
            List<double> returnList = new List<double>();
            for (int i = coeffs.GetLength(0) / 2; i < coeffs.GetLength(0); i++)
            {
                for (int j = coeffs.GetLength(1) / 2; j < coeffs.GetLength(1); j++)
                {
                    list.Add(coeffs[i, j]);
                }
            }

            for (int k = 0; k < NumOfTree*2; k++)
            {
                returnList.Add(list[k]);
            }

            double[] listHH1 = returnList.ToArray();
            double varianceHH1 = Tools.Variance(listHH1);
            return varianceHH1;
        }
        #endregion

        //public static double[,] HVS2(double[,] coeffs, int NumOfTree)
        //{
        //    double[,] HVSValues = new double[coeffs.GetLength(0), coeffs.GetLength(1)];
        //    for (int i = 0; i < coeffs.GetLength(0) / 4; i++)
        //    {
        //        for (int j = coeffs.GetLength(1) / 4; j < coeffs.GetLength(1) / 2; j++)
        //        {
        //            HVSValues[i, j] = (coeffs[i, j - (coeffs.GetLength(1) / 4)] / 255) * (coeffs[i, j] / 255) * VarianceOfHH2(coeffs, NumOfTree);
        //        }
        //    }

        //    for(int l = 0; l < coeffs.GetLength(0) / 2; l+=2)
        //    {
        //        for(int m= coeffs.GetLength(1)/2; m< coeffs.GetLength(1); m += 2)
        //        {
        //            HVSValues[l,m] = (coeffs[l,m-()])
        //        }
        //    }
        //}
        //End
    }
}
