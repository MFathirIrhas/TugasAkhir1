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

        #region Variances
        // CODE HERE
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

        #region covariances
        
        #endregion

        //end
    }
}
