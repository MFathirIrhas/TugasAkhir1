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


        #region Get the hidden states(Temporary)
        //2 Hidden State, High=1 and Low=2
        public static List<int> GetHiddenStates(List<double> coeff)
        {
            var hs = new List<int>();
            double threshold = Threshold(coeff).Item2;
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
        #endregion
        public static Tuple<double,double,double> Threshold(List<double> coeff)
        {
            double mean;
            int counter = 0;
            //Mean
            mean = coeff.Average();

            //Mode
            var groups = coeff.GroupBy(v => v);
            int maxCount = groups.Max(g => g.Count());
            double mode = groups.First(g => g.Count() == maxCount).Key;

            //Max
            double max = coeff.Max();

            return new Tuple<double, double, double>(mean, mode, max);
        }

        ///Calculating the threshold for state variable of Wavelet Coefficients
        /// Wavelet Coefficients decomposition tend to result in more low value of wavelet coefficients than high value of wavelet coefficients.
        public static double Threshold2(double[,] coeffs)
        {
            List<double> wc = new List<double>();
            for(int i = 0; i < coeffs.GetLength(0); i++)
            {
                for(int j = 0; j < coeffs.GetLength(1); j++)
                {
                    wc.Add(coeffs[i, j]);
                }
            }

            var groups = wc.GroupBy(v => v);
            int maxCount = groups.Max(g => g.Count());
            double mode = groups.First(g => g.Count() == maxCount).Key;

            double mean = wc.Average();
            return mode;
        }

        #region Treshold3 Exclude LL2 
        public static double Threshold3(double[,] coeffs)
        {
            List<double> wc = new List<double>();
            for (int i = 0; i < coeffs.GetLength(0) / 4; i++)
            {
                for (int j = coeffs.GetLength(1) / 4; i < coeffs.GetLength(1); i++)
                {
                    wc.Add(coeffs[i, j]);
                }
            }

            for (int k=coeffs.GetLength(0)/4;k < coeffs.GetLength(0); k++)
            {
                for(int l=0; l < coeffs.GetLength(1);l++)
                {
                    wc.Add(coeffs[k, l]);
                }
            }

            var groups = wc.GroupBy(v => v);
            int maxCount = groups.Max(g => g.Count());
            double mode = groups.First(g => g.Count() == maxCount).Key;

            double mean = wc.Average();
            return mode;
        }
        #endregion

        /// Determining the hidden state value
        /// 2 Hidden state values: 1 -> High wavelet coefficients and 2 -> Low wavelet coefficients
        /// Convert Matrix of wavelet coefficients into matrix of hidden values filled with 1 or 2 according to the threshold.
        public static double[,] GetHiddenStateValue(double[,] coeffs)
        {
            double threshold = Threshold2(coeffs);
            double[,] HiddenStates = new double[coeffs.GetLength(0), coeffs.GetLength(1)];
            for(int i = 0; i < coeffs.GetLength(0); i++)
            {
                for(int j = 0; j < coeffs.GetLength(1); j++)
                {
                    if(coeffs[i,j] > threshold)
                    {
                        HiddenStates[i, j] = 1;
                    }
                    else if(coeffs[i,j] <= threshold)
                    {
                        HiddenStates[i, j] = 2;
                    }
                }
            }

            return HiddenStates;
        }

        #region StateProbability including the LL subband
        /// <summary>
        /// Calculating state probability of each state of each wavelet coefficient.
        /// Scale 2 = Coarsest scale(smaller one)
        /// Scale 1 = Finest Scale(bigger one)
        /// </summary>
        /// <param name="hiddenstates"></param>
        /// <param name="scale"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double StateProbability(double[,] hiddenstates,int scale, int m)
        {
            List<int> count11 = new List<int>();
            List<int> count12 = new List<int>();
            List<int> count21 = new List<int>();
            List<int> count22 = new List<int>();

            double prob = new double();
            if (scale == 2)
            {
                if (m == 1)
                {
                    for(int i = 0; i < hiddenstates.GetLength(0) / 2; i++)
                    {
                        for(int j = 0; j < hiddenstates.GetLength(1) / 2; j++)
                        {
                            if (hiddenstates[i, j] == 1)
                            {
                                count21.Add(1);
                            }else
                            {
                                count21.Add(0);
                            }
                        }
                    }
                    prob = (double)count21.Sum() / (double)count21.Count;

                }
                else if(m == 2)
                {
                    for (int i = 0; i < hiddenstates.GetLength(0) / 2; i++)
                    {
                        for (int j = 0; j < hiddenstates.GetLength(1) / 2; j++)
                        {
                            if (hiddenstates[i, j] == 2)
                            {
                                count22.Add(1);
                            }
                            else
                            {
                                count22.Add(0);
                            }
                        }
                    }
                    prob = (double)count22.Sum() / (double)count22.Count;

                }
            }
            else if (scale == 1)
            {
                if (m == 1)
                {
                    for (int i = 0; i < hiddenstates.GetLength(0); i++)
                    {
                        for (int j = 0; j < hiddenstates.GetLength(1); j++)
                        {
                            if (hiddenstates[i, j] == 1)
                            {
                                count11.Add(1);
                            }
                            else
                            {
                                count11.Add(0);
                            }
                        }
                    }
                    prob = (double)count11.Sum() / (double)count11.Count;

                }
                else if (m == 2)
                {
                    for (int i = 0; i < hiddenstates.GetLength(0); i++)
                    {
                        for (int j = 0; j < hiddenstates.GetLength(1); j++)
                        {
                            if (hiddenstates[i, j] == 2)
                            {
                                count12.Add(1);
                            }
                            else
                            {
                                count12.Add(0);
                            }
                        }
                    }
                    prob = (double)count12.Sum() / (double)count12.Count;

                }
            }

            return prob; //return the probability of hidden states.
        }
        #endregion

        #region StateProbability excluding the LL subband
        public static double StateProbability2(double[,] hiddenstates, int scale, int m)
        {
            List<int> count11 = new List<int>();
            List<int> count12 = new List<int>();
            List<int> count21 = new List<int>();
            List<int> count22 = new List<int>();

            #region Convert scale to 1 dimension excluding LL the most top left subband
            //Convert scale 2 to 1 dimension
            List<double> Scale2coefss = new List<double>();
            // LH
            for(int i = 0; i < hiddenstates.GetLength(0) / 4; i++)
            {
                for(int j = hiddenstates.GetLength(1) / 4; j < hiddenstates.GetLength(1) / 2; j++)
                {
                    Scale2coefss.Add(hiddenstates[i, j]);
                }
            }

            // HH
            for (int i = hiddenstates.GetLength(0) / 4; i < hiddenstates.GetLength(0) / 2; i++)
            {
                for (int j = hiddenstates.GetLength(1) / 4; j < hiddenstates.GetLength(1) / 2; j++)
                {
                    Scale2coefss.Add(hiddenstates[i, j]);
                }
            }

            // HL
            for (int i = hiddenstates.GetLength(0) / 4; i < hiddenstates.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < hiddenstates.GetLength(1) / 4; j++)
                {
                    Scale2coefss.Add(hiddenstates[i, j]);
                }
            }

            //Convert scale 1 to 1 dimension
            List<double> Scale1coefss = new List<double>();
            // LH
            for (int i = 0; i < hiddenstates.GetLength(0) / 2; i++)
            {
                for (int j = hiddenstates.GetLength(1) / 2; j < hiddenstates.GetLength(1); j++)
                {
                    Scale1coefss.Add(hiddenstates[i, j]);
                }
            }

            // HH
            for (int i = hiddenstates.GetLength(0) / 2; i < hiddenstates.GetLength(0); i++)
            {
                for (int j = hiddenstates.GetLength(1) / 2; j < hiddenstates.GetLength(1); j++)
                {
                    Scale1coefss.Add(hiddenstates[i, j]);
                }
            }

            // HL
            for (int i = hiddenstates.GetLength(0) / 2; i < hiddenstates.GetLength(0); i++)
            {
                for (int j = 0; j < hiddenstates.GetLength(1) / 2; j++)
                {
                    Scale1coefss.Add(hiddenstates[i, j]);
                }
            }
            #endregion



            double prob = new double();
            if (scale == 2)
            {
                if (m == 1)
                {
                    for(int i = 0; i < Scale2coefss.Count; i++)
                    {
                        if (Scale2coefss[i]== 1)
                        {
                            count21.Add(1);
                        }
                        else
                        {
                            count21.Add(0);
                        }
                    }
                    prob = (double)count21.Sum() / (double)Scale2coefss.Count;
                }
                else if (m == 2)
                {
                    for (int i = 0; i < Scale2coefss.Count; i++)
                    {
                        if (Scale2coefss[i] == 2)
                        {
                            count22.Add(1);
                        }
                        else
                        {
                            count22.Add(0);
                        }
                    }
                    prob = (double)count22.Sum() / (double)Scale2coefss.Count;
                }
            }
            else if (scale == 1)
            {
                if (m == 1)
                {
                    for (int i = 0; i < Scale1coefss.Count; i++)
                    {
                        if (Scale1coefss[i] == 1)
                        {
                            count11.Add(1);
                        }
                        else
                        {
                            count11.Add(0);
                        }
                    }
                    prob = (double)count11.Sum() / (double)Scale1coefss.Count;
                }
                else if (m == 2)
                {
                    for (int i = 0; i < Scale1coefss.Count; i++)
                    {
                        if (Scale1coefss[i] == 2)
                        {
                            count12.Add(1);
                        }
                        else
                        {
                            count12.Add(0);
                        }
                    }
                    prob = (double)count12.Sum() / (double)Scale1coefss.Count;
                }
            }

            return prob; //return the probability of hidden states.
        }
        #endregion

        /// <summary>
        /// Calculate the probability of hidden states in coarsest scale where parents node reside.
        /// P(Sj->1=m), j=1 and m=1,2
        /// </summary>
        /// <param name="coeffs"></param>
        /// <returns></returns>
        public static double[] RootStateProbability(double[,] coeffs)
        {
            double[,] hiddenstates = GetHiddenStateValue(coeffs);
            double[] pmf = new double[2];
            double pmf1 = StateProbability2(hiddenstates, 2, 1); //PMF for parent's state m = 1
            double pmf2 = StateProbability2(hiddenstates, 2, 2); //PMF for parent's state m = 2
            pmf[0] = pmf1;
            pmf[1] = pmf2;
            return pmf;
        }


        /// <summary>
        /// Calculate likelihood probability of child node in state n given the parent node in state m.
        /// P(n|m) , n = child node and m = parent node. 
        /// </summary>
        /// <param name="coeffs">Wavelet Coefficients</param>
        /// <param name="pos">Position of child node for each parent -> pos ={1,2,3,4}</param>
        /// <returns></returns>
        //public static double TransitionProbability(double[,] coeffs, int pos)
        //{
        //    double[,] hiddenstates = GetHiddenStateValue(coeffs);
        //    // Calculate number of parent node with state 1 or 2
        //    double rootpmf1 = StateProbability(hiddenstates, 2, 1);
        //    double rootpmf2 = StateProbability(hiddenstates, 2, 2);


        //    if (pos == 1)
        //    {
        //        for (int i = 0; i < coeffs.GetLength(0); i += 2)
        //        {
        //            for (int j = 0; j < coeffs.GetLength(1); j += 2)
        //            {

        //            }
        //        }
        //    }
            
        //}
        
        //end
    }
}
