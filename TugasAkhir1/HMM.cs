using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Accord.Statistics;

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

        #region Wrong Calculation of HMM Model

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

        #region StateProbability excluding the LL subband (USED)
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


        ///Calculate Parameters of HMM Model
        /// tetha = {P1, A2, variances/covariance)
        /// 1. P1 = Parent Hidden States Probability
        /// 2. A2 = Hidden States Probability 
        ///     [ P(n=1|m=1)  P(n=2|m=1) ]
        ///     [ P(n=1|m=2)  P(n=2|m=2) ]
        /// 3. Variances -> ignore cross correlation among subband
        ///    Covariances -> Consist of Wavelet Coefficients at the same scale and location
        #region Parent Hidden States Probability
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
        #endregion

        #region Parent-Child Hidden States Transition Probability
        /// <summary>
        /// Calculate likelihood probability of child node in state n given the parent node in state m.
        /// P(n|m) , n = child node and m = parent node.
        /// Create A Matrix that contain:
        ///     [P(n=1|m=1)  P(n=2|m=1)]   -> [ pos=1    pos=2 ]
        ///     [P(n=1|m=2)  P(n=2|m=2)]      [ pos=3    pos=4 ]
        /// The nature of Child Hidden States are that 1 & 3 always same, and 2 & 4 always same.(still in doubt)
        /// </summary>
        /// <param name="coeffs">Wavelet Coefficients</param>
        /// <param name="pos">Position of child node for each parent -> pos ={1,2,3,4}</param>
        /// <param name="m">Hidden state of Parent node</param>
        /// <param name="n">Hidden state of Child node</param>
        /// <returns></returns>
        public static double TransitionProbability(double[,] coeffs, /*int pos,*/ int n, int m)
        {
            double[,] hiddenstates = GetHiddenStateValue(coeffs);
            // Calculate number of parent node with state 1 or 2
            double rootpmf1 = StateProbability2(hiddenstates, 2, 1);
            double rootpmf2 = StateProbability2(hiddenstates, 2, 2);
            double NumOfRootm1 = (double)rootpmf1 * (double)(((coeffs.GetLength(0) * coeffs.GetLength(1)) / 16) * 3); //Number of parent node whose m = 1
            double NumOfRootm2 = (double)rootpmf2 * (double)(((coeffs.GetLength(0) * coeffs.GetLength(1)) / 16) * 3); //Number of parent node whose m = 2

            double[] Scale2 = DWT.Scale2To1DCoeff(hiddenstates);
            double[,] Scale1 = DWT.Scale1To1DCoeff(hiddenstates);
            double[,] Scale = new double[Scale2.Length, 5];

            for (int i = 0; i < Scale2.Length; i++)
            {
                Scale[i, 0] = Scale2[i];
                Scale[i, 1] = Scale1[i, 0];
                Scale[i, 2] = Scale1[i, 1];
                Scale[i, 3] = Scale1[i, 2];
                Scale[i, 4] = Scale1[i, 3];

            }


            ///Calculating Transition probability
            int c1 = 0;
            int c2 = 0;
            int c3 = 3;
            int c4 = 4;
            if(n==1 && m == 1)
            {
                for(int i = 0; i < Scale.GetLength(0); i++)
                {
                    for (int j = 1; j < Scale.GetLength(1); j++)
                    {
                        if (Scale[i, j] == 1 && Scale[i, 0] == 1)
                        {
                            c1++;
                        }
                    }
                }
                double transitionProb = (double)c1 / (double)NumOfRootm1;
                return transitionProb;

            }
            else if(n==2 && m==1)
            {
                for (int i = 0; i < Scale.GetLength(0); i++)
                {
                    for (int j = 1; j < Scale.GetLength(1); j++)
                    {
                        if (Scale[i, j] == 2 && Scale[i, 0] == 1)
                        {
                            c2++;
                        }
                    }
                }
                double transitionProb = (double)c2 / (double)NumOfRootm1;
                return transitionProb;
            }
            else if(n==1 && m==2)
            {
                for (int i = 0; i < Scale.GetLength(0); i++)
                {
                    for (int j = 1; j < Scale.GetLength(1); j++)
                    {
                        if (Scale[i, j] == 1 && Scale[i, 0] == 2)
                        {
                            c3++;
                        }
                    }
                }
                double transtitionProb = (double)c3 / (double)NumOfRootm2;
                return transtitionProb;
            }
            else
            {
                for (int i = 0; i < Scale.GetLength(0); i++)
                {
                    for (int j = 1; j < Scale.GetLength(1); j++)
                    {
                        if (Scale[i, j] == 2 && Scale[i, 0] == 2)
                        {
                            c4++;
                        }
                    }
                }
                double transtitionProb = (double)c4 / (double)NumOfRootm2;
                return transtitionProb;
            }
        }
        #endregion

        #region Variances (NOT SURE)
        public static double Variances(double[,] coeffs, int j, int m)
        {
            double[] j2m1 = Scale2m1(coeffs);
            double[] j2m2 = Scale2m2(coeffs);
            double[] j1m1 = Scale1m1(coeffs);
            double[] j1m2 = Scale1m2(coeffs);

            if(j==2 && m == 1)
            {
                double v = Tools.Variance(j2m1);
                return v;
            }
            else if(j==2 && m == 2)
            {
                double v = Tools.Variance(j2m2);
                return v;
            }
            else if(j==1 && m == 1)
            {
                double v = Tools.Variance(j1m1);
                return v;
            }
            else
            {
                double v = Tools.Variance(j1m2);
                return v;
            }
        }

        public static double[] Scale2m1(double[,] coeffs)
        {
            //int size = ((coeffs.GetLength(0) * coeffs.GetLength(1)) / 16) * 3;
            List<double> w = new List<double>();
            double threshold = Threshold2(coeffs);
            //LH2
            for(int i = 0; i < coeffs.GetLength(0) / 4; i++)
            {
                for(int j= coeffs.GetLength(1)/4; j < coeffs.GetLength(1) / 2; j++)
                {
                    if (coeffs[i, j] > threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }                    
                }
            }

            //HH2
            for (int i = coeffs.GetLength(0) / 4; i < coeffs.GetLength(0) / 2; i++)
            {
                for (int j = coeffs.GetLength(1) / 4; j < coeffs.GetLength(1) / 2; j++)
                {
                    if (coeffs[i, j] > threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }

            //HL2
            for (int i = coeffs.GetLength(0) / 4; i < coeffs.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < coeffs.GetLength(1) / 4; j++)
                {
                    if (coeffs[i, j] > threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }
            double[] w2 = w.ToArray();
            return w2;
        }

        public static double[] Scale2m2(double[,] coeffs)
        {
            //int size = ((coeffs.GetLength(0) * coeffs.GetLength(1)) / 16) * 3;
            List<double> w = new List<double>();
            double threshold = Threshold2(coeffs);
            //LH2
            for (int i = 0; i < coeffs.GetLength(0) / 4; i++)
            {
                for (int j = coeffs.GetLength(1) / 4; j < coeffs.GetLength(1) / 2; j++)
                {
                    if (coeffs[i, j] <= threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }

            //HH2
            for (int i = coeffs.GetLength(0) / 4; i < coeffs.GetLength(0) / 2; i++)
            {
                for (int j = coeffs.GetLength(1) / 4; j < coeffs.GetLength(1) / 2; j++)
                {
                    if (coeffs[i, j] <= threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }

            //HL2
            for (int i = coeffs.GetLength(0) / 4; i < coeffs.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < coeffs.GetLength(1) / 4; j++)
                {
                    if (coeffs[i, j] <= threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }
            double[] w2 = w.ToArray();
            return w2;
        }

        public static double[] Scale1m1(double[,] coeffs)
        {
            //int size = ((coeffs.GetLength(0) * coeffs.GetLength(1)) / 16) * 3;
            List<double> w = new List<double>();
            double threshold = Threshold2(coeffs);
            //LH2
            for (int i = 0; i < coeffs.GetLength(0) / 2; i++)
            {
                for (int j = coeffs.GetLength(1) / 2; j < coeffs.GetLength(1) ; j++)
                {
                    if (coeffs[i, j] > threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }

            //HH2
            for (int i = coeffs.GetLength(0) / 2; i < coeffs.GetLength(0); i++)
            {
                for (int j = coeffs.GetLength(1) / 2; j < coeffs.GetLength(1) ; j++)
                {
                    if (coeffs[i, j] > threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }

            //HL2
            for (int i = coeffs.GetLength(0) / 2; i < coeffs.GetLength(0) ; i++)
            {
                for (int j = 0; j < coeffs.GetLength(1) / 2; j++)
                {
                    if (coeffs[i, j] > threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }
            double[] w2 = w.ToArray();
            return w2;
        }

        public static double[] Scale1m2(double[,] coeffs)
        {
            //int size = ((coeffs.GetLength(0) * coeffs.GetLength(1)) / 16) * 3;
            List<double> w = new List<double>();
            double threshold = Threshold2(coeffs);
            //LH2
            for (int i = 0; i < coeffs.GetLength(0) / 2; i++)
            {
                for (int j = coeffs.GetLength(1) / 2; j < coeffs.GetLength(1); j++)
                {
                    if (coeffs[i, j] <= threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }

            //HH2
            for (int i = coeffs.GetLength(0) / 2; i < coeffs.GetLength(0); i++)
            {
                for (int j = coeffs.GetLength(1) / 2; j < coeffs.GetLength(1); j++)
                {
                    if (coeffs[i, j] <= threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }

            //HL2
            for (int i = coeffs.GetLength(0) / 2; i < coeffs.GetLength(0); i++)
            {
                for (int j = 0; j < coeffs.GetLength(1) / 2; j++)
                {
                    if (coeffs[i, j] <= threshold)
                    {
                        w.Add(coeffs[i, j]);
                    }
                }
            }
            double[] w2 = w.ToArray();
            return w2;
        }
        #endregion

        #region Covariances

        #endregion

        #endregion

        #region Recalculation of HMM Model (Scalar Model)
        /// <summary>
        /// EM Steps of estimating the probability
        /// </summary>
        /// <param name="coeffs"></param>
        /// <returns></returns>
        public static double[] ParentStateProbability(double[,] coeffs) //P(m)
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
        /// Child State Probability : Pi(m) = Sigma(m'): Ppi(m')*P(m|m') 
        /// </summary>
        /// <param name="coeffs"></param>
        /// <returns></returns>
        public static double[] ChildStateProbability(double[,] coeffs) //P(n)
        {
            double[] rootpmf = ParentStateProbability(coeffs);
            double[] pmf = new double[2];
            double transitionn1m1 = TransitionStateProbability(coeffs, 1, 1);
            double transitionn2m1 = TransitionStateProbability(coeffs, 2, 1);
            double transitionn1m2 = TransitionStateProbability(coeffs, 1, 2);
            double transitionn2m2 = TransitionStateProbability(coeffs, 2, 2);

            double pmf1 = (rootpmf[0] * transitionn1m1) + (rootpmf[1] * transitionn1m2);
            double pmf2 = (rootpmf[0] * transitionn2m1) + (rootpmf[1] * transitionn2m2);
            pmf[0] = pmf1;
            pmf[1] = pmf2;
            return pmf;
        }
        public static double TransitionStateProbability(double[,] coeffs, int n, int m) //P(n|m)
        {
            int size = ((coeffs.GetLength(0) * coeffs.GetLength(1)) / 16) * 3;
            double[,] hiddenstates = GetHiddenStateValue(coeffs);
            double[] transitionProb = new double[size];
            double[,] trees = ExtractTrees(hiddenstates);

            int n1m1 = 0;
            int n2m1 = 0;
            int n1m2 = 0;
            int n2m2 = 0;
            if (n == 1 && m == 1)
            {
                for (int i = 0; i < trees.GetLength(0); i++)
                {
                    n1m1 = 0;
                    for (int j = 1; j < trees.GetLength(1); j++)
                    {
                        if (trees[i, j] == 1 && trees[i, 0] == 1)
                        {
                            n1m1++;
                        }
                    }
                    transitionProb[i] = (double)n1m1 / (double)4;
                }

                double sum = transitionProb.Sum();
                double[] rootpmf = ParentStateProbability(coeffs);
                double pmf1 = rootpmf[0];
                double transitionn1m1 = (double)sum / ((double)trees.GetLength(0) * (double)pmf1);
                return transitionn1m1;

            }
            else if (n == 2 && m == 1)
            {
                for (int i = 0; i < trees.GetLength(0); i++)
                {
                    n2m1 = 0;
                    for (int j = 1; j < trees.GetLength(1); j++)
                    {
                        if (trees[i, j] == 2 && trees[i, 0] == 1)
                        {
                            n2m1++;
                        }
                    }
                    transitionProb[i] = (double)n2m1 / (double)4;
                }

                double sum = transitionProb.Sum();
                double[] rootpmf = ParentStateProbability(coeffs);
                double pmf1 = rootpmf[0];
                double transitionn2m1 = (double)sum / ((double)trees.GetLength(0) * (double)pmf1);
                return transitionn2m1;
            }
            else if (n == 1 && m == 2)
            {
                for (int i = 0; i < trees.GetLength(0); i++)
                {
                    n1m2 = 0;
                    for (int j = 1; j < trees.GetLength(1); j++)
                    {
                        if (trees[i, j] == 1 && trees[i, 0] == 2)
                        {
                            n1m2++;
                        }
                    }
                    transitionProb[i] = (double)n1m2 / (double)4;
                }

                double sum = transitionProb.Sum();
                double[] rootpmf = ParentStateProbability(coeffs);
                double pmf2 = rootpmf[1];
                double transitionn1m2 = (double)sum / ((double)trees.GetLength(0) * (double)pmf2);
                return transitionn1m2;
            }
            else
            {
                for (int i = 0; i < trees.GetLength(0); i++)
                {
                    n2m2 = 0;
                    for (int j = 1; j < trees.GetLength(1); j++)
                    {
                        if (trees[i, j] == 2 && trees[i, 0] == 2)
                        {
                            n2m2++;
                        }
                    }
                    transitionProb[i] = (double)n2m2 / (double)4;
                }

                double sum = transitionProb.Sum();
                double[] rootpmf = ParentStateProbability(coeffs);
                double pmf2 = rootpmf[1];
                double transitionn2m2 = (double)sum / ((double)trees.GetLength(0) * (double)pmf2);
                return transitionn2m2;
            }
        }

        public static double[] Means(double[,] coeffs, int m)
        {
            double[] mean = new double[5]; //5 node each tree
            double[,] treesOfcoeffs = ExtractTrees(coeffs);
            double[,] hiddenstates = GetHiddenStateValue(coeffs);
            double[,] treesOfHiddenStates = ExtractTrees(hiddenstates);

            //P(m)
            double[] rootpmf = ParentStateProbability(hiddenstates); 

            //P(n|m)
            double transitionn1m1 = TransitionProbability(hiddenstates, 1, 1); 
            double transitionn2m1 = TransitionProbability(hiddenstates, 2, 1); 
            double transitionn1m2 = TransitionProbability(hiddenstates, 1, 2); 
            double transitionn2m2 = TransitionProbability(hiddenstates, 2, 2); 

            //P(n)
            double[] childpmf = ChildStateProbability(hiddenstates);
            
            if (m == 1)
            {
                double sum  = 0;
                double sum2 = 0;
                double sum3 = 0;
                double sum4 = 0;
                double sum5 = 0;
                
                    // Parent Node
                    for(int j = 0; j < treesOfcoeffs.GetLength(0); j++) 
                    {
                        sum += treesOfcoeffs[j, 0] * rootpmf[0];
                    }
                    double mean1 = (double)sum / ((double)treesOfcoeffs.GetLength(0)*(double)rootpmf[0]);

                    // Child node 1
                    for(int k = 0; k < treesOfcoeffs.GetLength(0); k++)
                    {
                        sum2 += treesOfcoeffs[k, 1] * childpmf[0];
                    }
                    double mean2 = (double)sum2 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[0]);
                    
                    // Child node 2
                    for (int l = 0; l < treesOfcoeffs.GetLength(0); l++)
                    {
                        sum3 += treesOfcoeffs[l, 2] * childpmf[0];
                    }
                    double mean3 = (double)sum3 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[0]);

                    // Child node 3
                    for (int o = 0; o < treesOfcoeffs.GetLength(0); o++)
                    {
                        sum4 += treesOfcoeffs[o, 3] * childpmf[0];
                    }
                    double mean4 = (double)sum4 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[0]);

                    // Child node 4
                    for (int p = 0; p < treesOfcoeffs.GetLength(0); p++)
                    {
                        sum5 += treesOfcoeffs[p, 4] * childpmf[0];
                    }
                    double mean5 = (double)sum5 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[0]);

                mean[0] = mean1;
                mean[1] = mean2;
                mean[2] = mean3;
                mean[3] = mean4;
                mean[4] = mean5;

            }
            else
            {
                double sum = 0;
                double sum2 = 0;
                double sum3 = 0;
                double sum4 = 0;
                double sum5 = 0;

                // Parent Node
                for (int j = 0; j < treesOfcoeffs.GetLength(0); j++)
                {
                    sum += treesOfcoeffs[j, 0] * rootpmf[1];
                }
                double mean1 = (double)sum / ((double)treesOfcoeffs.GetLength(0) * (double)rootpmf[1]);

                // Child node 1
                for (int k = 0; k < treesOfcoeffs.GetLength(0); k++)
                {
                    sum2 += treesOfcoeffs[k, 1] * childpmf[1];
                }
                double mean2 = (double)sum2 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[1]);

                // Child node 2
                for (int l = 0; l < treesOfcoeffs.GetLength(0); l++)
                {
                    sum3 += treesOfcoeffs[l, 2] * childpmf[1];
                }
                double mean3 = (double)sum3 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[1]);

                // Child node 3
                for (int o = 0; o < treesOfcoeffs.GetLength(0); o++)
                {
                    sum4 += treesOfcoeffs[o, 3] * childpmf[1];
                }
                double mean4 = (double)sum4 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[1]);

                // Child node 4
                for (int p = 0; p < treesOfcoeffs.GetLength(0); p++)
                {
                    sum5 += treesOfcoeffs[p, 4] * childpmf[1];
                }
                double mean5 = (double)sum5 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[1]);

                mean[0] = mean1;
                mean[1] = mean2;
                mean[2] = mean3;
                mean[3] = mean4;
                mean[4] = mean5;

            }

            return mean;
        }

        public static double[] Variances(double[,] coeffs , int m)
        {
            double[] variances = new double[5]; //5 node each tree
            double[,] treesOfcoeffs = ExtractTrees(coeffs);
            double[,] hiddenstates = GetHiddenStateValue(coeffs);
            double[,] treesOfHiddenStates = ExtractTrees(hiddenstates);

            //P(m)
            double[] rootpmf = ParentStateProbability(hiddenstates);

            //P(n|m)
            double transitionn1m1 = TransitionProbability(hiddenstates, 1, 1);
            double transitionn2m1 = TransitionProbability(hiddenstates, 2, 1);
            double transitionn1m2 = TransitionProbability(hiddenstates, 1, 2);
            double transitionn2m2 = TransitionProbability(hiddenstates, 2, 2);

            //P(n)
            double[] childpmf = ChildStateProbability(hiddenstates);

            double[] mean1 = Means(coeffs, 1);
            double[] mean2 = Means(coeffs, 2);

            if (m == 1)
            {
                double sum = 0;
                double sum2 = 0;
                double sum3 = 0;
                double sum4 = 0;
                double sum5 = 0;

                // Parent Node
                for (int j = 0; j < treesOfcoeffs.GetLength(0); j++)
                {
                    sum += Math.Pow((treesOfcoeffs[j, 0]-mean1[0]),2) * rootpmf[0];
                }
                double variance1 = (double)sum / ((double)treesOfcoeffs.GetLength(0) * (double)rootpmf[0]);

                // Child node 1
                for (int k = 0; k < treesOfcoeffs.GetLength(0); k++)
                {
                    sum2 += Math.Pow((treesOfcoeffs[k, 1]-mean1[1]),2) * childpmf[0];
                }
                double variance2 = (double)sum2 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[0]);

                // Child node 2
                for (int l = 0; l < treesOfcoeffs.GetLength(0); l++)
                {
                    sum3 += Math.Pow((treesOfcoeffs[l, 2] - mean1[2]), 2) * childpmf[0];
                }
                double variance3 = (double)sum3 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[0]);

                // Child node 3
                for (int o = 0; o < treesOfcoeffs.GetLength(0); o++)
                {
                    sum4 += Math.Pow((treesOfcoeffs[o, 3] - mean1[3]), 2) * childpmf[0];
                }
                double variance4 = (double)sum4 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[0]);

                // Child node 4
                for (int p = 0; p < treesOfcoeffs.GetLength(0); p++)
                {
                    sum5 += Math.Pow((treesOfcoeffs[p, 4] - mean1[4]), 2) * childpmf[0];
                }
                double variance5 = (double)sum5 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[0]);

                variances[0] = variance1;
                variances[1] = variance2;
                variances[2] = variance3;
                variances[3] = variance4;
                variances[4] = variance5;

            }
            else
            {
                double sum = 0;
                double sum2 = 0;
                double sum3 = 0;
                double sum4 = 0;
                double sum5 = 0;

                // Parent Node
                for (int j = 0; j < treesOfcoeffs.GetLength(0); j++)
                {
                    sum += Math.Pow((treesOfcoeffs[j, 0] - mean2[0]), 2) * rootpmf[1];
                }
                double variance1 = (double)sum / ((double)treesOfcoeffs.GetLength(0) * (double)rootpmf[1]);

                // Child node 1
                for (int k = 0; k < treesOfcoeffs.GetLength(0); k++)
                {
                    sum2 += Math.Pow((treesOfcoeffs[k, 1] - mean2[1]), 2) * childpmf[1];
                }
                double variance2 = (double)sum2 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[1]);

                // Child node 2
                for (int l = 0; l < treesOfcoeffs.GetLength(0); l++)
                {
                    sum3 += Math.Pow((treesOfcoeffs[l, 2] - mean2[2]), 2) * childpmf[1];
                }
                double variance3 = (double)sum3 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[1]);

                // Child node 3
                for (int o = 0; o < treesOfcoeffs.GetLength(0); o++)
                {
                    sum4 += Math.Pow((treesOfcoeffs[o, 3] - mean2[3]), 2) * childpmf[1];
                }
                double variance4 = (double)sum4 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[1]);

                // Child node 4
                for (int p = 0; p < treesOfcoeffs.GetLength(0); p++)
                {
                    sum5 += Math.Pow((treesOfcoeffs[p, 4] - mean2[4]), 2) * childpmf[1];
                }
                double variance5 = (double)sum5 / ((double)treesOfcoeffs.GetLength(0) * (double)childpmf[1]);

                variances[0] = variance1;
                variances[1] = variance2;
                variances[2] = variance3;
                variances[3] = variance4;
                variances[4] = variance5;

            }

            return variances;
        }

        public static double[,] ExtractTrees(double[,] coeffs)
        {
            int size = ((coeffs.GetLength(0) * coeffs.GetLength(1)) / 16) * 3;
            int subsize = size / 3;
            int subsize2 = subsize * 2;
            double[,] trees = new double[size,5];
            ///Scale 2
            //LH2
            int lh2 = 0;
            for(int i = 0; i < coeffs.GetLength(0) / 4; i++)
            {
                for(int j = coeffs.GetLength(1) / 4; j < coeffs.GetLength(1) / 2; j++)
                {
                    trees[lh2, 0] = coeffs[i,j];
                }
            }

            //HH2
            int hh2 = 0;
            for (int i = coeffs.GetLength(0) / 4; i < coeffs.GetLength(0)/ 2; i++)
            {
                for(int j = coeffs.GetLength(1) / 4; j < coeffs.GetLength(1) / 2; j++)
                {
                    trees[hh2 + subsize, 0] = coeffs[i, j];
                }
            }

            //HL2
            int hl2 = 0;
            for (int i = coeffs.GetLength(0) / 4; i < coeffs.GetLength(0)/2 ; i++)
            {
                for (int j = 0; j < coeffs.GetLength(1) / 4; j++)
                {
                    trees[hh2 + subsize2, 0] = coeffs[i, j];
                }
            }

            ///Scale 1
            //LH1
            int lh1 = 0;
            for (int i = 0; i < coeffs.GetLength(0) / 2; i+=2)
            {
                for (int j = coeffs.GetLength(1) / 2; j < coeffs.GetLength(1); j+=2)
                {
                    trees[lh1, 1] = coeffs[i, j];
                    trees[lh1, 2] = coeffs[i, j + 1];
                    trees[lh1, 3] = coeffs[i + 1, j];
                    trees[lh1, 4] = coeffs[i + 1, j + 1];
                }
            }

            //HH1
            int hh1 = 0;
            for (int i = coeffs.GetLength(0) / 2; i < coeffs.GetLength(0); i += 2)
            {
                for (int j = coeffs.GetLength(1) / 2; j < coeffs.GetLength(1); j += 2)
                {
                    trees[hh1 + subsize, 1] = coeffs[i, j];
                    trees[hh1 + subsize, 2] = coeffs[i, j + 1];
                    trees[hh1 + subsize, 3] = coeffs[i + 1, j];
                    trees[hh1 + subsize, 4] = coeffs[i + 1, j + 1];
                }
            }

            //HL1
            int hl1 = 0;
            for (int i = coeffs.GetLength(0) / 2; i < coeffs.GetLength(0); i += 2)
            {
                for (int j = 0; j < coeffs.GetLength(1)/2; j += 2)
                {
                    trees[hh1 + subsize2, 1] = coeffs[i, j];
                    trees[hh1 + subsize2, 2] = coeffs[i, j + 1];
                    trees[hh1 + subsize2, 3] = coeffs[i + 1, j];
                    trees[hh1 + subsize2, 4] = coeffs[i + 1, j + 1];
                }
            }

            return trees;
        }
        #endregion

        #region INITIATE HMM PARAMETERS
        public static Tuple<double[],double[,], double[,]> CreateHMMModel(double[,] coeffs)
        {
            // Parent nodes PMF
            double[] rootpmf = ParentStateProbability(coeffs);

            // Transition State Probability
            double[,] transition = new double[2, 2];
            double transitionn1m1 = TransitionStateProbability(coeffs, 1, 1);
            double transitionn2m1 = TransitionStateProbability(coeffs, 2, 1);
            double transitionn1m2 = TransitionStateProbability(coeffs, 1, 2);
            double transitionn2m2 = TransitionStateProbability(coeffs, 2, 2);
            transition[0, 0] = transitionn1m1;
            transition[0, 1] = transitionn2m1;
            transition[1, 0] = transitionn1m2;
            transition[1, 1] = transitionn2m2;

            //Variances
            double[,] variances = new double[2, 5];
            double[] variancem1 = Variances(coeffs, 1);
            double[] variancem2 = Variances(coeffs, 2);
            for(int i = 0; i < 5; i++)
            {
                variances[0, i] = variancem1[i];
            }
            for(int j = 0; j < 5; j++)
            {
                variances[1, j] = variancem2[j];
            }

            return new Tuple<double[], double[,], double[,]>(rootpmf, transition, variances);

        }
        #endregion
        //end
    }
}
