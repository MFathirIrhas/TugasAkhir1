using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Accord.Statistics;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;

namespace TugasAkhir1
{
    public class Extract
    {
        #region Detection using Manual Parameter Estimation
        /// <summary>
        /// Detect Watermark using Log Likelihood 
        /// </summary>
        /// <param name="coeffs">Wavelet Coefficients of watermarked image</param>
        /// <param name="rootpmf">Parent nodes pmf</param>
        /// <param name="transition">Parent-Child Transition State Probability</param>
        /// <param name="variances">Variances</param>
        /// <returns></returns>
        public static double[][] DetectWatermark(double[,] coeffs,Image watermarkedImage,double[] rootpmf, double[,] transition, double[,] variances)
        {
            #region HMM Initialization
            // Create the transition matrix A
            double[,] transitions = transition;


            // Create the vector of emission densities B
            //m = 1
            double v1 = variances[0, 0];
            double v2 = variances[0, 1];
            double v3 = variances[0, 2];
            double v4 = variances[0, 3];
            double v5 = variances[0, 4];

            // m = 2
            double v6 = variances[1, 0];
            double v7 = variances[1, 1];
            double v8 = variances[1, 2];
            double v9 = variances[1, 3];
            double v10= variances[1, 4];

            GeneralDiscreteDistribution[] emissions =
            {
                new GeneralDiscreteDistribution(v1,v2,v3,v4,v5),
                new GeneralDiscreteDistribution(v6,v7,v8,v9,v10)
            };

            // Create the initial probabilities pi
            double[] initial = rootpmf;

            

            // Create a new hidden Markov model with discrete probabilities
            var hmm = new HiddenMarkovModel<GeneralDiscreteDistribution>(transitions, emissions, initial);
            #endregion

            /// detectedWatermark will be divide by 3, each will be taken as much as tree/segmented Watermark before embedding
            /// Segmented Watermark for android watermark segmented become 6480 tree each subband. Each subband will be taken 6480 tree
            /// So total row of tree in detected watermark will be 6480 * 3.
            /// in each 6480, the same tree in the same index will be concat become 15 digit watermark, and will be reverse to 5 according to mapping rule.
            double[][] detectedWatermark = new double[49152][]; //49152 total of tree, will be known in input

            double[][] listOfTrees = GetSequenceParameter(coeffs, watermarkedImage).Item1;
            double[][] watermarkPermutation = GetSequenceParameter(coeffs, watermarkedImage).Item2;
            double[,] hvs = GetSequenceParameter(coeffs, watermarkedImage).Item3;

            for(int i = 0; i < detectedWatermark.GetLength(0); i++) //Looping each tree = 49152
            {
                List<double> listOfLikelihood = new List<double>();
                double[,] V = CalculateVi(i, watermarkPermutation, hvs);
                for (int j = 0; j < watermarkPermutation.GetLength(0); j++) //Looping each permutation of watermark possibility in a tree = 32
                {
                    double[] T= new double[5];
                    for(int k = 0; k < 5; k++) // Looping each node in tree (5 nodes)
                    {
                        T[k] = (double)listOfTrees[i][k] - (double)V[j, k];
                    }

                    double loglikelihood = hmm.Evaluate(T); //Calculate loglikelihood P(W|hmm)
                    listOfLikelihood.Add(loglikelihood);
                }

                int maxindex = MaxValueIndex(listOfLikelihood); // Look for maximum value in list of loglikelihood for detection result
                detectedWatermark[i] = watermarkPermutation[maxindex];
            }


            return detectedWatermark;
        }
        #endregion

        #region Detection using Baum-Welch Learning Paremeter Estimation
        public static double[][] BaumWelchDetection(double[,] coeffs, Image watermarkedImage, int NumOfTrees /*, double[] rootpmf, double[,] transition, double[,] variances*/)
        {
            /// detectedWatermark will be divide by 3, each will be taken as much as tree/segmented Watermark before embedding
            /// Segmented Watermark for android watermark segmented become 6480 tree each subband. Each subband will be taken 6480 tree
            /// So total row of tree in detected watermark will be 6480 * 3.
            /// in each 6480, the same tree in the same index will be concat become 15 digit watermark, and will be reverse to 5 according to mapping rule.
            double[][] detectedWatermark = new double[49152][]; //49152 total of tree, will be known in input

            double[][] listOfTrees = GetSequenceParameter(coeffs, watermarkedImage).Item1;
            // Real TreeOfWatermark where the watermark is embedded
            double[][] WatermarkTrees = TreeOfWatermark(listOfTrees, NumOfTrees);
            double[][] watermarkPermutation = GetSequenceParameter(coeffs, watermarkedImage).Item2;
            double[,] hvs = GetSequenceParameter(coeffs, watermarkedImage).Item3;

            #region Baum-Welch Estimation and Learning
            // Specify a initial normal distribution for the samples.
            NormalDistribution density = new NormalDistribution();

            // Creates a continuous hidden Markov Model with two states organized in a forward
            //  topology and an underlying univariate Normal distribution as probability density.
            var model = new HiddenMarkovModel<NormalDistribution>(new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<NormalDistribution>(model)
            {
                Tolerance = 0.001,
                Iterations = 0,
            };

            // Fit the model
            double likelihood = teacher.Run(WatermarkTrees);
            #endregion

            

            for (int i = 0; i < detectedWatermark.GetLength(0); i++) //Looping each tree = 49152
            {
                List<double> listOfLikelihood = new List<double>();
                double[,] V = CalculateVi(i, watermarkPermutation, hvs);
                for (int j = 0; j < watermarkPermutation.GetLength(0); j++) //Looping each permutation of watermark possibility in a tree = 32
                {
                    double[] T = new double[5];
                    for (int k = 0; k < 5; k++) // Looping each node in tree (5 nodes)
                    {
                        T[k] = (double)listOfTrees[i][k] - (double)V[j, k];
                    }

                    double loglikelihood = model.Evaluate(T); //Calculate loglikelihood P(W|hmm)
                    listOfLikelihood.Add(loglikelihood);
                }

                int maxindex = MaxValueIndex(listOfLikelihood); // Look for maximum value in list of loglikelihood for detection result
                detectedWatermark[i] = watermarkPermutation[maxindex];
            }


            return detectedWatermark;
        }


        #endregion

        #region Process Watermark using Manual HMM Model Estimation
        /// Get the tree where watermark inserted
        /// Total tree per subband : 6480
        public static double[][] TreeOfWatermark(double[][] detectedWatermark, int NumOfTree) // NumOfTree = 6480
        {             
            int subbandsize = detectedWatermark.GetLength(0) / 3;
            double[][] LH = new double[subbandsize][];
            double[][] HH = new double[subbandsize][];
            double[][] HL = new double[subbandsize][];
            int totalTree = NumOfTree * 3;
            double[][] tree = new double[totalTree][];

            // LH
            for (int i = 0; i < subbandsize; i++)
            {
                LH[i] = detectedWatermark[i];
            }

            // HH
            for (int j = 0; j < subbandsize; j++)
            {
                HH[j] = detectedWatermark[j + subbandsize];
            }

            // HL
            for(int k = 0; k < subbandsize; k++)
            {
                HL[k] = detectedWatermark[k + subbandsize + subbandsize];
            }

            //-----------------------------------
            // LH
            for(int l=0; l < NumOfTree; l++)
            {
                tree[l] = LH[l];
            }

            // HH
            for(int m = 0; m < NumOfTree; m++)
            {
                tree[m+ NumOfTree] = HH[m];
            }

            // HL
            for(int n = 0; n < NumOfTree; n++)
            {
                tree[n + NumOfTree + NumOfTree] = HL[n];
            }

            return tree;
        }

        public static double[][] CombineTrees(double[][] tree)
        {
            int size = tree.GetLength(0) / 3;
            double[][] CombinedTrees = new double[size][];

            double[][] LH = new double[size][];
            double[][] HH = new double[size][];
            double[][] HL = new double[size][];
            //int totalTree = NumOfTree * 3;

            // LH
            for (int i = 0; i < size; i++)
            {
                LH[i] = tree[i];
            }

            // HH
            for (int j = 0; j < size; j++)
            {
                HH[j] = tree[j + size];
            }

            // HL
            for (int k = 0; k < size; k++)
            {
                HL[k] = tree[k + size+ size];
            }

            //-----------------------------------
            // Combining tree into 15 node mapped watermark
            for(int i = 0; i < size; i++)
            {
                CombinedTrees[i] = CombineArray(LH[i], HH[i], HL[i]);
            }

            return CombinedTrees;
        }

        public static double[] CombineArray(double[] lh, double[] hh, double[] hl)
        {
            var z = new double[lh.Length + hh.Length + hl.Length];
            lh.CopyTo(z, 0);
            hh.CopyTo(z, lh.Length);
            hl.CopyTo(z, lh.Length + hh.Length);
            return z;
        }
        #endregion

        /// Inverse mapping mapped tree
        /// Convert 15 node tree to 5 node tree based on mapping strategy
        /// Extract Each triangle, circle, and rectangle pattern of mapped watermark
        public static Tuple<double[,],double[,],double[,]> InverseMapping(double[][] combinedTrees)
        {
            double[,] triangle = new double[combinedTrees.GetLength(0),5];
            double[,] circle = new double[combinedTrees.GetLength(0), 5];
            double[,] rectangle = new double[combinedTrees.GetLength(0), 5];
            double[,] square = new double[combinedTrees.GetLength(0), 5];

            for(int i = 0; i < combinedTrees.GetLength(0); i++)
            {
                triangle[i, 0] = combinedTrees[i][0];
                triangle[i, 1] = combinedTrees[i][5];
                triangle[i, 2] = combinedTrees[i][10];
                triangle[i, 3] = combinedTrees[i][1];
                triangle[i, 4] = combinedTrees[i][6];
            }

            for(int j = 0; j < combinedTrees.GetLength(0); j++)
            {
                circle[j, 0] = combinedTrees[j][8];
                circle[j, 1] = combinedTrees[j][13];
                circle[j, 2] = combinedTrees[j][4];
                circle[j, 3] = combinedTrees[j][9];
                circle[j, 4] = combinedTrees[j][14];
            }

            for(int k = 0; k < combinedTrees.GetLength(0); k++)
            {
                rectangle[k, 0] = combinedTrees[k][11];
                rectangle[k, 1] = combinedTrees[k][2];
                rectangle[k, 2] = combinedTrees[k][7];
                rectangle[k, 3] = combinedTrees[k][12];
                rectangle[k, 4] = combinedTrees[k][3];
            }

            // reverse rectangle
            for(int l = 0; l < rectangle.GetLength(0); l++)
            {
                for(int m = 0; m < rectangle.GetLength(1); m++)
                {
                    if(rectangle[l,m] == 0)
                    {
                        square[l, m] = 1;
                    }else
                    {
                        square[l, m] = 0;
                    }
                }
            }

            // These return values should be same.
            return new Tuple<double[,], double[,], double[,]>(triangle, circle, square);
        }

        /// Get All Possible Watermark for 1 tree(5 node) , get all HVS, and v value
        public static Tuple<double[][],double[][],double[,]> GetSequenceParameter(double[,] coeffs,Image watermarkedImage)
        {
            
            // Trees of wavelet coefficients
            double[][] listOfTree = HMM.ConvertToNestedArray(coeffs); // item1

            #region All Posible watermark in one tree, 32
            // All Posible watermark in one tree, 32
            double[][] w = new double[32][];
            w[0] = new double[] { 0, 0, 0, 0, 0 };
            w[1] = new double[] { 0, 0, 0, 0, 1 };
            w[2] = new double[] { 0, 0, 0, 1, 0 };
            w[3] = new double[] { 0, 0, 0, 1, 1 };
            w[4] = new double[] { 0, 0, 1, 0, 0 };
            w[5] = new double[] { 0, 0, 1, 0, 1 };
            w[6] = new double[] { 0, 0, 1, 1, 0 };
            w[7] = new double[] { 0, 0, 1, 1, 1 };

            w[8] = new double[] { 0, 1, 0, 0, 0 };
            w[9] = new double[] { 0, 1, 0, 0, 1 };
            w[10] = new double[] { 0, 1, 0, 1, 0 };
            w[11] = new double[] { 0, 1, 0, 1, 1 };
            w[12] = new double[] { 0, 1, 1, 0, 0 };
            w[13] = new double[] { 0, 1, 1, 0, 1 };
            w[14] = new double[] { 0, 1, 1, 1, 0 };
            w[15] = new double[] { 0, 1, 1, 1, 1 };

            w[16] = new double[] { 1, 0, 0, 0, 0 };
            w[17] = new double[] { 1, 0, 0, 0, 1 };
            w[18] = new double[] { 1, 0, 0, 1, 0 };
            w[19] = new double[] { 1, 0, 0, 1, 1 };
            w[20] = new double[] { 1, 0, 1, 0, 0 };
            w[21] = new double[] { 1, 0, 1, 0, 1 };
            w[22] = new double[] { 1, 0, 1, 1, 0 };
            w[23] = new double[] { 1, 0, 1, 1, 1 };

            w[24] = new double[] { 1, 1, 0, 0, 0 };
            w[25] = new double[] { 1, 1, 0, 0, 1 };
            w[26] = new double[] { 1, 1, 0, 1, 0 };
            w[27] = new double[] { 1, 1, 0, 1, 1 };
            w[28] = new double[] { 1, 1, 1, 0, 0 };
            w[29] = new double[] { 1, 1, 1, 0, 1 };
            w[30] = new double[] { 1, 1, 1, 1, 0 };
            w[31] = new double[] { 1, 1, 1, 1, 1 };
            #endregion // item2

            // HVS
            Bitmap decomposedImage = DWT.TransformDWT(true, false, 2, new Bitmap(watermarkedImage));
            Bitmap edgyImage = ImageProcessing.LaplaceEdge(decomposedImage);
            double[,] hvs = ImageProcessing.HVS(edgyImage);
            double[,] listOfHvs = DWT.ListOfCoeffs(hvs); // item3

            return new Tuple<double[][], double[][], double[,]>(listOfTree, w, listOfHvs);
        }

        public static double[,] CalculateVi(int k,double[][] w, double[,] listOfHVS)
        {
            double[,] Vi = new double[32,5];
            for(int i = 0; i < Vi.GetLength(0); i++)
            {
                for(int j = 0; j < Vi.GetLength(1); j++)
                {
                    Vi[i, j] = w[i][j] * listOfHVS[k, j] * 0.3;
                }
            }
            return Vi;
        }

        public static int MaxValueIndex(List<double> loglikelihood)
        {
            int indexMax= !loglikelihood.Any() ? -1 : loglikelihood
                                .Select((value, index) => new { Value = value, Index = index })
                                .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                                .Index;
            return indexMax;
        }


    }
}
