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
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Distributions.Fitting;


namespace TugasAkhir1
{
    public class Extract
    {
        //#region Detection using Manual Parameter Estimation (NOT USED)
        ///// <summary>
        ///// Detect Watermark using Log Likelihood 
        ///// </summary>
        ///// <param name="coeffs">Wavelet Coefficients of watermarked image</param>
        ///// <param name="rootpmf">Parent nodes pmf</param>
        ///// <param name="transition">Parent-Child Transition State Probability</param>
        ///// <param name="variances">Variances</param>
        ///// <returns></returns>
        //public static double[][] DetectWatermark(double[,] coeffs, Image watermarkedImage, double[] rootpmf, double[,] transition, double[,] variances)
        //{
        //    #region HMM Initialization
        //    // Create the transition matrix A
        //    double[,] transitions = transition;


        //    // Create the vector of emission densities B
        //    //m = 1
        //    double v1 = variances[0, 0];
        //    double v2 = variances[0, 1];
        //    double v3 = variances[0, 2];
        //    double v4 = variances[0, 3];
        //    double v5 = variances[0, 4];

        //    // m = 2
        //    double v6 = variances[1, 0];
        //    double v7 = variances[1, 1];
        //    double v8 = variances[1, 2];
        //    double v9 = variances[1, 3];
        //    double v10 = variances[1, 4];

        //    GeneralDiscreteDistribution[] emissions =
        //    {
        //        new GeneralDiscreteDistribution(v1,v2,v3,v4,v5),
        //        new GeneralDiscreteDistribution(v6,v7,v8,v9,v10)
        //    };

        //    // Create the initial probabilities pi
        //    double[] initial = rootpmf;



        //    // Create a new hidden Markov model with discrete probabilities
        //    var hmm = new HiddenMarkovModel<GeneralDiscreteDistribution>(transitions, emissions, initial);
        //    #endregion

        //    /// detectedWatermark will be divide by 3, each will be taken as much as tree/segmented Watermark before embedding
        //    /// Segmented Watermark for android watermark segmented become 6480 tree each subband. Each subband will be taken 6480 tree
        //    /// So total row of tree in detected watermark will be 6480 * 3.
        //    /// in each 6480, the same tree in the same index will be concat become 15 digit watermark, and will be reverse to 5 according to mapping rule.
        //    double[][] detectedWatermark = new double[49152][]; //49152 total of tree, will be known in input

        //    double[][] listOfTrees = GetSequenceParameter(coeffs, watermarkedImage).Item1;
        //    double[][] watermarkPermutation = GetSequenceParameter(coeffs, watermarkedImage).Item2;
        //    double[,] hvs = GetSequenceParameter(coeffs, watermarkedImage).Item3;

        //    for (int i = 0; i < detectedWatermark.GetLength(0); i++) //Looping each tree = 49152
        //    {
        //        List<double> listOfLikelihood = new List<double>();
        //        double[,] V = CalculateVi(i, watermarkPermutation, hvs);
        //        for (int j = 0; j < watermarkPermutation.GetLength(0); j++) //Looping each permutation of watermark possibility in a tree = 32
        //        {
        //            double[] T = new double[5];
        //            for (int k = 0; k < 5; k++) // Looping each node in tree (5 nodes)
        //            {
        //                T[k] = (double)listOfTrees[i][k] - (double)V[j, k];
        //            }

        //            double loglikelihood = hmm.Evaluate(T); //Calculate loglikelihood P(W|hmm)
        //            listOfLikelihood.Add(loglikelihood);
        //        }

        //        int maxindex = MaxValueIndex(listOfLikelihood); // Look for maximum value in list of loglikelihood for detection result
        //        detectedWatermark[i] = watermarkPermutation[maxindex];
        //    }


        //    return detectedWatermark;
        //}
        //#endregion


        //-------------------Used-----------------------
        //#region Detection using Baum-Welch Learning Paremeter Estimation(Univariate) Using 15-bit mapping
        //public static double[][] BaumWelchDetection(double[,] coeffs, Image watermarkedImage, int NumOfScale2, int NumOfTrees, List<int> PNSeq/*, double[] rootpmf, double[,] transition, double[,] variances*/)
        //{
        //    /// detectedWatermark will be divide by 3, each will be taken as much as tree/segmented Watermark before embedding
        //    /// Segmented Watermark for android watermark segmented become 6480 tree each subband. Each subband will be taken 6480 tree
        //    /// So total row of tree in detected watermark will be 6480 * 3.
        //    /// in each 6480, the same tree in the same index will be concat become 15 digit watermark, and will be reverse to 5 according to mapping rule.
        //    double[][] detectedWatermark = new double[NumOfScale2][]; //49152 total of tree, will be known in input

        //    double[][] listOfTrees = GetSequenceParameter(coeffs, watermarkedImage).Item1;
        //    // Real TreeOfWatermark where the watermark is embedded
        //    double[][] WatermarkTrees = TreeOfWatermark(listOfTrees, NumOfTrees);
        //    double[][] watermarkPermutation = GetSequenceParameter(coeffs, watermarkedImage).Item2;
        //    double[,] hvs = GetSequenceParameter(coeffs, watermarkedImage).Item3;

        //    #region Baum-Welch Estimation and Learning
        //    // Specify a initial normal distribution for the samples.
        //    NormalDistribution density = new NormalDistribution();

        //    // Creates a continuous hidden Markov Model with two states organized in a forward
        //    //  topology and an underlying univariate Normal distribution as probability density.
        //    var model = new HiddenMarkovModel<NormalDistribution>(new Ergodic(2), density);

        //    // Configure the learning algorithms to train the sequence classifier until the
        //    // difference in the average log-likelihood changes only by as little as 0.0001
        //    var teacher = new BaumWelchLearning<NormalDistribution>(model)
        //    {
        //        Tolerance = 0.001,
        //        Iterations = 0,
        //    };

        //    // Fit the model
        //    double likelihood = teacher.Run(WatermarkTrees);
        //    #endregion



        //    for (int i = 0; i < detectedWatermark.GetLength(0); i++) //Looping each tree = 49152
        //    {
        //        List<double> listOfLikelihood = new List<double>();
        //        //double[,] V = CalculateVi(i, watermarkPermutation, hvs);
        //        double[][] pattern = WatermarkPattern(i, PNSeq);
        //        double[,] V = CalculateVi2(i, pattern, hvs);
        //        for (int j = 0; j < 2/*watermarkPermutation.GetLength(0)*/; j++) //Looping each permutation of watermark possibility in a tree = 32
        //        {
        //            double[] T = new double[5];
        //            for (int k = 0; k < 5; k++) // Looping each node in tree (5 nodes)
        //            {
        //                T[k] = (double)listOfTrees[i][k] - (double)V[j, k];
        //            }

        //            double loglikelihood = model.Evaluate(T); //Calculate loglikelihood P(W|hmm)
        //            listOfLikelihood.Add(loglikelihood);
        //        }

        //        int maxindex = MaxValueIndex(listOfLikelihood); // Look for maximum value in list of loglikelihood for detection result
        //        detectedWatermark[i] = pattern[maxindex];

        //    }


        //    return detectedWatermark;
        //}


        //#endregion

        //#region Detection using Baum-Welch Learning Parameter Estimation using NORMALIZED DATA(Univariate)
        //public static double[][] NormalizedBaumWelchDetection(double[,] coeffs, Image watermarkedImage, int NumOfScale2, int NumOfTrees/*, double[] rootpmf, double[,] transition, double[,] variances*/)
        //{
        //    /// detectedWatermark will be divide by 3, each will be taken as much as tree/segmented Watermark before embedding
        //    /// Segmented Watermark for android watermark segmented become 6480 tree each subband. Each subband will be taken 6480 tree
        //    /// So total row of tree in detected watermark will be 6480 * 3.
        //    /// in each 6480, the same tree in the same index will be concat become 15 digit watermark, and will be reverse to 5 according to mapping rule.
        //    double[][] detectedWatermark = new double[NumOfScale2][]; //49152 total of tree, will be known in input

        //    double[][] listOfTrees = GetSequenceParameter(coeffs, watermarkedImage).Item1;
        //    double[][] normalizedListOfTrees = NormalizeData(listOfTrees);
        //    // Real TreeOfWatermark where the watermark is embedded
        //    double[][] WatermarkTrees = TreeOfWatermark(normalizedListOfTrees, NumOfTrees);
        //    double[][] watermarkPermutation = GetSequenceParameter(coeffs, watermarkedImage).Item2;
        //    double[,] hvs = GetSequenceParameter(coeffs, watermarkedImage).Item3;

        //    #region Baum-Welch Estimation and Learning
        //    // Specify a initial normal distribution for the samples.
        //    NormalDistribution density = new NormalDistribution();

        //    // Creates a continuous hidden Markov Model with two states organized in a forward
        //    //  topology and an underlying univariate Normal distribution as probability density.
        //    var model = new HiddenMarkovModel<NormalDistribution>(new Ergodic(2), density);

        //    // Configure the learning algorithms to train the sequence classifier until the
        //    // difference in the average log-likelihood changes only by as little as 0.0001
        //    var teacher = new BaumWelchLearning<NormalDistribution>(model)
        //    {
        //        Tolerance = 0.001,
        //        Iterations = 0,
        //    };

        //    // Fit the model
        //    double likelihood = teacher.Run(WatermarkTrees);
        //    #endregion



        //    for (int i = 0; i < detectedWatermark.GetLength(0); i++) //Looping each tree = 49152
        //    {
        //        List<double> listOfLikelihood = new List<double>();
        //        double[,] V = CalculateVi(i, watermarkPermutation, hvs);
        //        for (int j = 0; j < watermarkPermutation.GetLength(0); j++) //Looping each permutation of watermark possibility in a tree = 32
        //        {
        //            double[] T = new double[5];
        //            double[] nT = new double[5];
        //            for (int k = 0; k < 5; k++) // Looping each node in tree (5 nodes)
        //            {
        //                T[k] = (double)listOfTrees[i][k] - (double)V[j, k];

        //            }

        //            for (int l = 0; l < 5; l++)
        //            {
        //                nT[l] = (double)T[l] / (double)T.Max();
        //            }
        //            double loglikelihood = model.Evaluate(nT); //Calculate loglikelihood P(W|hmm)
        //            listOfLikelihood.Add(loglikelihood);
        //        }

        //        int maxindex = MaxValueIndex(listOfLikelihood); // Look for maximum value in list of loglikelihood for detection result
        //        detectedWatermark[i] = watermarkPermutation[maxindex];
        //    }


        //    return detectedWatermark;
        //}
        //#endregion

        ////#region Detection using Baum-Welch Parameter Estimation(Multivariate)
        ////public static double[][] MultivariateBaumWelchDetection(double[,] coeffs, Image watermarkedImage, int NumOfScale2, int NumOfTrees/*, double[] rootpmf, double[,] transition, double[,] variances*/)
        ////{
        ////    // Create sequences of vector-valued observations. In the
        ////    // sequence below, a single observation is composed of two
        ////    // coordinate values, such as (x, y). There seems to be two
        ////    // states, one for (x,y) values less than (5,5) and another
        ////    // for higher values. The states seems to be switched on
        ////    // every observation.
        ////    double[][] detectedWatermark = new double[NumOfScale2][]; //49152 total of tree, will be known in input

        ////    double[][] listOfTrees = GetSequenceParameter(coeffs, watermarkedImage).Item1;
        ////    // Real TreeOfWatermark where the watermark is embedded
        ////    double[][] WatermarkTrees = TreeOfWatermark(listOfTrees, NumOfTrees);
        ////    double[][][] VectorCoeffs = ConvertToVectorCoeff(WatermarkTrees);

        ////    double[][] watermarkPermutation = GetSequenceParameter(coeffs, watermarkedImage).Item2;
        ////    double[,] hvs = GetSequenceParameter(coeffs, watermarkedImage).Item3;


        ////    // Specify a initial normal distribution for the samples.
        ////    var density = new MultivariateNormalDistribution(dimension: 2);

        ////    // Creates a continuous hidden Markov Model with two states organized in a forward
        ////    //  topology and an underlying univariate Normal distribution as probability density.
        ////    var model = new HiddenMarkovModel<MultivariateNormalDistribution>(new Forward(2), density);

        ////    // Configure the learning algorithms to train the sequence classifier until the
        ////    // difference in the average log-likelihood changes only by as little as 0.0001
        ////    var teacher = new BaumWelchLearning<MultivariateNormalDistribution>(model)
        ////    {
        ////        Tolerance = 0.0001,
        ////        Iterations = 0,
        ////    };

        ////    // Fit the model
        ////    double logLikelihood = teacher.Run(sequences);

        ////    // See the likelihood of the sequences learned
        ////    double a1 = Math.Exp(model.Evaluate(new[] {
        ////                new double[] { 1, 2 },
        ////                new double[] { 6, 7 },
        ////                new double[] { 2, 3 }})); // 0.000208
        ////}
        ////#endregion

        //#region Detection using Baum-Welch Learning Parameter in LH ONLY
        //public static double[][] BaumWelchDetectionInLH(double[,] coeffs, Image watermarkedImage, int NumOfScale2, int NumOfTrees/*, double[] rootpmf, double[,] transition, double[,] variances*/)
        //{
        //    /// detectedWatermark will be divide by 3, each will be taken as much as tree/segmented Watermark before embedding
        //    /// Segmented Watermark for android watermark segmented become 6480 tree each subband. Each subband will be taken 6480 tree
        //    /// So total row of tree in detected watermark will be 6480 * 3.
        //    /// in each 6480, the same tree in the same index will be concat become 15 digit watermark, and will be reverse to 5 according to mapping rule.
        //    /*double[][] detectedWatermark = new double[NumOfScale2][];*/ //49152 total of tree, will be known in input

        //    // Convert Matrix of Watermark into List of 5-nodes tree get from LH,HH,and HL.
        //    double[][] listOfTrees = GetSequenceParameter(coeffs, watermarkedImage).Item1; 

        //    // Get the trees of watermark where the actual watermark was embedded
        //    double[][] WatermarkTrees = TreeOfWatermark2(listOfTrees, NumOfTrees, "lh");

            
        //    double[][] watermarkPermutation = GetSequenceParameter(coeffs, watermarkedImage).Item2;
        //    double[,] hvs = GetSequenceParameter(coeffs, watermarkedImage).Item3;

        //    double[][] detectedWatermark = new double[WatermarkTrees.GetLength(0)][];

        //    #region Baum-Welch Estimation and Learning
        //    // Specify a initial normal distribution for the samples.
        //    NormalDistribution density = new NormalDistribution();

        //    // Creates a continuous hidden Markov Model with two states organized in a forward
        //    //  topology and an underlying univariate Normal distribution as probability density.
        //    var model = new HiddenMarkovModel<NormalDistribution>(new Ergodic(2), density);

        //    // Configure the learning algorithms to train the sequence classifier until the
        //    // difference in the average log-likelihood changes only by as little as 0.0001
        //    var teacher = new BaumWelchLearning<NormalDistribution>(model)
        //    {
        //        Tolerance = 0.001,
        //        Iterations = 0,
        //    };

        //    // Fit the model
        //    double likelihood = teacher.Run(WatermarkTrees);
        //    #endregion



        //    for (int i = 0; i < detectedWatermark.GetLength(0); i++) //Looping each tree = 49152
        //    {
        //        List<double> listOfLikelihood = new List<double>();
        //        double[,] V = CalculateVi(i, watermarkPermutation, hvs);
        //        for (int j = 0; j < watermarkPermutation.GetLength(0); j++) //Looping each permutation of watermark possibility in a tree = 32
        //        {
        //            double[] T = new double[5];
        //            for (int k = 0; k < 5; k++) // Looping each node in tree (5 nodes)
        //            {
        //                T[k] = (double)WatermarkTrees[i][k] - (double)V[j, k];
        //            }

        //            double loglikelihood = model.Evaluate(T); //Calculate loglikelihood P(W|hmm)
        //            listOfLikelihood.Add(loglikelihood);
        //        }

        //        int maxindex = MaxValueIndex(listOfLikelihood); // Look for maximum value in list of loglikelihood for detection result
        //        detectedWatermark[i] = watermarkPermutation[maxindex];
        //    }


        //    return detectedWatermark;
        //}
        //#endregion

        #region Detection using Baum-Welch Learning Parameter in LH ONLY using 2 Possible Watermark pattern in each 5-nodes tree
        public static double[] BaumWelchDetectionInLH_2(double[,] coeffs, Image watermarkedImage, int NumOfScale2, int NumOfTrees, List<int> PNSeq/*, double[] rootpmf, double[,] transition, double[,] variances*/)
        {
            double[] Watermark = new double[NumOfTrees];
            /// detectedWatermark will be divide by 3, each will be taken as much as tree/segmented Watermark before embedding
            /// Segmented Watermark for android watermark segmented become 6480 tree each subband. Each subband will be taken 6480 tree
            /// So total row of tree in detected watermark will be 6480 * 3.
            /// in each 6480, the same tree in the same index will be concat become 15 digit watermark, and will be reverse to 5 according to mapping rule.
            /*double[][] detectedWatermark = new double[NumOfScale2][];*/ //49152 total of tree, will be known in input

            // Convert Matrix of Watermark into List of 5-nodes tree get from LH,HH,and HL.
            double[][] listOfTrees = GetSequenceParameter(coeffs, watermarkedImage, NumOfTrees).Item1;

            // Get the trees of watermark where the actual watermark was embedded
            double[][] WaveletTrees = TreeOfWatermark2(listOfTrees, NumOfTrees, "hh");

            /// Test
            //TextWriter tw1 = new StreamWriter("LIST_OF_WAVELET_TREES.txt");
            //tw1.WriteLine("Total Watermark: " + WaveletTrees.GetLength(0));
            //for (int i = 0; i < WaveletTrees.GetLength(0); i++)
            //{
            //    for (int j = 0; j < WaveletTrees[i].Length; j++)
            //    {
            //        tw1.Write("["+j+"]"+ WaveletTrees[i][j] + " # ");
            //    }
            //    tw1.WriteLine();
            //}
            //foreach (double i in ExtractedWatermark)
            //{
            //    tw1.WriteLine(i);
            //}
            //tw1.Close();

            double[][] watermarkPermutation = GetSequenceParameter(coeffs, watermarkedImage, NumOfTrees).Item2;
            double[,] hvs = GetSequenceParameter(coeffs, watermarkedImage, NumOfTrees).Item3;

            double[][] detectedWatermark = new double[WaveletTrees.GetLength(0)][];

            /// Train original wavelet coefficients
            //double[][] listOfTrees2 = GetSequenceParameter(realCoeffs, watermarkedImage).Item1;
            //double[][] WaveletTrees2 = TreeOfWatermark2(listOfTrees2, NumOfTrees);

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

                //FittingOptions = new NormalOptions()
                //{
                //    Regularization = 1e-5 // specify a regularization constant
                //}
            };

            // Fit the model
            double likelihood = teacher.Run(WaveletTrees);
            #endregion



            for (int i = 0; i < detectedWatermark.GetLength(0); i++) //Looping each tree = 49152
            {
                List<double> listOfLikelihood = new List<double>();
                double[][] pattern = WatermarkPattern(i, PNSeq);
                double[,] V = CalculateVi2(i, pattern, hvs);
                for (int j = 0; j < 2; j++) //Looping each permutation of watermark possibility in a tree = 32
                {
                    double[] T = new double[5];
                    for (int k = 0; k < 5; k++) // Looping each node in tree (5 nodes)
                    {
                        T[k] = (double)WaveletTrees[i][k] - (double)V[j, k];
                    }

                    double loglikelihood = model.Evaluate(T); //Calculate loglikelihood P(W|hmm)
                    listOfLikelihood.Add(loglikelihood);
                }

                int maxindex = MaxValueIndex(listOfLikelihood); // Look for maximum value in list of loglikelihood for detection result
                //detectedWatermark[i] = pattern[maxindex];
                Watermark[i] = maxindex;
            }


            //return detectedWatermark;
            return Watermark;
        }
        #endregion 


        /// Addition functions for HMM Training and Detection
        #region Get the tree where the watermarks were inserted according to NumOfTree in Key file
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
            for (int k = 0; k < subbandsize; k++)
            {
                HL[k] = detectedWatermark[k + subbandsize + subbandsize];
            }

            //-----------------------------------
            // LH
            for (int l = 0; l < NumOfTree; l++)
            {
                tree[l] = LH[l];
            }

            // HH
            for (int m = 0; m < NumOfTree; m++)
            {
                tree[m + NumOfTree] = HH[m];
            }

            // HL
            for (int n = 0; n < NumOfTree; n++)
            {
                tree[n + NumOfTree + NumOfTree] = HL[n];
            }

            return tree;
        }
        #endregion

        #region Combine each 5-node tree in LH,HH,and HL to form 15 digit from mapped watermark
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
                HL[k] = tree[k + size + size];
            }

            //-----------------------------------
            // Combining tree into 15 node mapped watermark
            for (int i = 0; i < size; i++)
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

        #region Inverse 15 digit sequence into real 5-segment watermarks digits according to mapping rule 
        /// Inverse mapping mapped tree
        /// Convert 15 node tree to 5 node tree based on mapping strategy
        /// Extract Each triangle, circle, and rectangle pattern of mapped watermark
        public static Tuple<double[,], double[,], double[,]> InverseMapping(double[][] combinedTrees)
        {
            double[,] triangle = new double[combinedTrees.GetLength(0), 5];
            double[,] circle = new double[combinedTrees.GetLength(0), 5];
            double[,] rectangle = new double[combinedTrees.GetLength(0), 5];
            double[,] square = new double[combinedTrees.GetLength(0), 5];

            for (int i = 0; i < combinedTrees.GetLength(0); i++)
            {
                triangle[i, 0] = combinedTrees[i][0];
                triangle[i, 1] = combinedTrees[i][5];
                triangle[i, 2] = combinedTrees[i][10];
                triangle[i, 3] = combinedTrees[i][1];
                triangle[i, 4] = combinedTrees[i][6];
            }

            for (int j = 0; j < combinedTrees.GetLength(0); j++)
            {
                circle[j, 0] = combinedTrees[j][8];
                circle[j, 1] = combinedTrees[j][13];
                circle[j, 2] = combinedTrees[j][4];
                circle[j, 3] = combinedTrees[j][9];
                circle[j, 4] = combinedTrees[j][14];
            }

            for (int k = 0; k < combinedTrees.GetLength(0); k++)
            {
                rectangle[k, 0] = combinedTrees[k][11];
                rectangle[k, 1] = combinedTrees[k][2];
                rectangle[k, 2] = combinedTrees[k][7];
                rectangle[k, 3] = combinedTrees[k][12];
                rectangle[k, 4] = combinedTrees[k][3];
            }

            // reverse rectangle
            for (int l = 0; l < rectangle.GetLength(0); l++)
            {
                for (int m = 0; m < rectangle.GetLength(1); m++)
                {
                    if (rectangle[l, m] == 0)
                    {
                        square[l, m] = 1;
                    }
                    else
                    {
                        square[l, m] = 0;
                    }
                }
            }

            // These return values should be same.
            return new Tuple<double[,], double[,], double[,]>(triangle, circle, square);
        }
        #endregion

        #region Get all watermark tree vectors, all possible watermark, and list of hvs
        /// Get All Possible Watermark for 1 tree(5 node) , get all HVS, and v value
        public static Tuple<double[][], double[][], double[,]> GetSequenceParameter(double[,] coeffs, Image watermarkedImage, int NumOfTree)
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

            //w[0] = new double[] { -1, -1, -1, -1, -1 };
            //w[1] = new double[] { -1, -1, -1, -1, 1 };
            //w[2] = new double[] { -1, -1, -1, 1, -1 };
            //w[3] = new double[] { -1, -1, -1, 1, 1 };
            //w[4] = new double[] { -1, -1, 1, -1, -1 };
            //w[5] = new double[] { -1, -1, 1, -1, 1 };
            //w[6] = new double[] { -1, -1, 1, 1, -1 };
            //w[7] = new double[] { -1, -1, 1, 1, 1 };

            //w[8] = new double[] { -1, 1, -1, -1, -1 };
            //w[9] = new double[] { -1, 1, -1, -1, 1 };
            //w[10] = new double[] { -1, 1, -1, 1, -1 };
            //w[11] = new double[] { -1, 1, -1, 1, 1 };
            //w[12] = new double[] { -1, 1, 1, -1, -1 };
            //w[13] = new double[] { -1, 1, 1, -1, 1 };
            //w[14] = new double[] { -1, 1, 1, 1, -1 };
            //w[15] = new double[] { -1, 1, 1, 1, 1 };

            //w[16] = new double[] { 1, -1, -1, -1, -1 };
            //w[17] = new double[] { 1, -1, -1, -1, 1 };
            //w[18] = new double[] { 1, -1, -1, 1, -1 };
            //w[19] = new double[] { 1, -1, -1, 1, 1 };
            //w[20] = new double[] { 1, -1, 1, -1, -1 };
            //w[21] = new double[] { 1, -1, 1, -1, 1 };
            //w[22] = new double[] { 1, -1, 1, 1, -1 };
            //w[23] = new double[] { 1, -1, 1, 1, 1 };

            //w[24] = new double[] { 1, 1, -1, -1, -1 };
            //w[25] = new double[] { 1, 1, -1, -1, 1 };
            //w[26] = new double[] { 1, 1, -1, 1, -1 };
            //w[27] = new double[] { 1, 1, -1, 1, 1 };
            //w[28] = new double[] { 1, 1, 1, -1, -1 };
            //w[29] = new double[] { 1, 1, 1, -1, 1 };
            //w[30] = new double[] { 1, 1, 1, 1, -1 };
            //w[31] = new double[] { 1, 1, 1, 1, 1 };
            #endregion // item2

            // HVS
            //Bitmap decomposedImage = DWT.TransformDWT(true, false, 2, new Bitmap(watermarkedImage));
            //Bitmap edgyImage = ImageProcessing.LaplaceEdge(decomposedImage);
            //double[,] hvs = ImageProcessing.HVS(edgyImage);
            //double[,] listOfHvs = DWT.ListOfCoeffs(hvs); // item3

            double[,] pixels = ImageProcessing.ConvertToMatrix2(new Bitmap(watermarkedImage)).Item2;
            double[,] Hvs = Embed.AdaptiveHVS(coeffs,pixels);
            double[,] listOfHvs = DWT.ListOfCoeffs(Hvs);

            //double[,] Hvs = Embed.AdaptiveHVS2(coeffs, NumOfTree);
            //double[,] listOfHvs = DWT.ListOfCoeffs(Hvs);


            return new Tuple<double[][], double[][], double[,]>(listOfTree, w, listOfHvs);
        }
        #endregion

        #region Calculate Vi value for substraction with watermarked wavelet coefficients.
        public static double[,] CalculateVi(int k, double[][] w, double[,] listOfHVS)
        {
            double[,] Vi = new double[32, 5];
            for (int i = 0; i < Vi.GetLength(0); i++)
            {
                for (int j = 0; j < Vi.GetLength(1); j++)
                {
                    Vi[i, j] = w[i][j] * listOfHVS[k, j] * 0.3;
                }
            }
            return Vi;
        }
        #endregion

        #region Calculate 2 possible watermark pattern in each tree
        public static double[][] WatermarkPattern(int k,List<int> PNSeq)
        {
            List<List<int>> Segmented = Scramble.Segment(PNSeq);
            double[][] Pattern = new double[2][];
            for(int i = 0; i < Pattern.GetLength(0); i++)
            {
                double[] row = new double[5];
                for(int j = 0; j < 5; j++)
                {
                    // i is 0 and 1
                    row[j] = i ^ Segmented[k][j];
                }
                Pattern[i] = row;
            }
            return Pattern;
        }

        public static double[,] CalculateVi2(int k, double[][] pattern, double[,] listOfHVS)
        {
            double[,] Vi = new double[2, 5];
            //double[][] pattern = WatermarkPattern(k, PNSeq);
            for (int i = 0; i < Vi.GetLength(0); i++)
            {
                for (int j = 0; j < Vi.GetLength(1); j++)
                {
                    //Vi[i, j] = pattern[i][j] * listOfHVS[k, j] * 0.3;
                    //Vi[i, j] = pattern[i][j] * listOfHVS[k, j] * 1;
                    Vi[i, j] = pattern[i][j] * listOfHVS[k, j] * 0.2;//0.0001;//0.01;//0.16111;

                    //Vi[i, j] = pattern[i][j] * 10;
                    //Vi[i, j] = pattern[i][j] * 1;
                    //Vi[i, j] = pattern[i][j] * 4;
                }
            }
            return Vi;
        }
        #endregion

        public static int MaxValueIndex(List<double> loglikelihood)
        {
            int indexMax = !loglikelihood.Any() ? -1 : loglikelihood
                                .Select((value, index) => new { Value = value, Index = index })
                                .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                                .Index;
            return indexMax;
        }

        public static double[][] NormalizeData(double[][] listOfTrees)
        {
            double[,] normalizedData = new double[listOfTrees.GetLength(0), listOfTrees[0].Length];
            for (int i = 0; i < listOfTrees.GetLength(0); i++)
            {
                for (int j = 0; j < listOfTrees[0].Length; j++)
                {
                    normalizedData[i, j] = listOfTrees[i][j] / listOfTrees[i].Max();
                }
            }

            double[][] TheNormalizedData = ConvertToJaggedArray(normalizedData);
            return TheNormalizedData;
        }

        public static double[][] ConvertToJaggedArray(double[,] array)
        {
            double[][] jaggedarray = new double[array.GetLength(0)][];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                double[] row = new double[array.GetLength(1)];
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    row[j] = array[i, j];
                }
                jaggedarray[i] = row;
            }
            return jaggedarray;
        }

        #region FOR MULTIVARIATE 
        public static double[][][] ConvertToVectorCoeff(double[][] TreeOfWatermark)
        {
            int size = TreeOfWatermark.GetLength(0) / 3;
            double[][][] VectorCoeffs = new double[size][][];


            for (int i = 0; i < VectorCoeffs.GetLength(0); i++)
            {
                double[][] vectors = new double[3][];
                vectors[0] = TreeOfWatermark[i];
                vectors[1] = TreeOfWatermark[i + size];
                vectors[2] = TreeOfWatermark[i + size + size];
                VectorCoeffs[i] = vectors;
            }

            return VectorCoeffs;
        }

        #endregion

        #region For LH, HH, or HL Trees only
        public static double[][] TreeOfWatermark2(double[][] detectedWatermark, int NumOfTree, string subband) // NumOfTree = 6480
        {
            int subbandsize = detectedWatermark.GetLength(0) / 3;
            double[][] LH = new double[subbandsize][];
            double[][] HH = new double[subbandsize][];
            double[][] HL = new double[subbandsize][];            
            int totalTree = NumOfTree;
            double[][] tree = new double[totalTree][];

            if (subband == "lh")
            {
                // LH
                for (int i = 0; i < subbandsize; i++)
                {
                    LH[i] = detectedWatermark[i];
                }

                //-----------------------------------
                // LH where watermarks are inserted
                for (int l = 0; l < NumOfTree; l++)
                {
                    tree[l] = LH[l];
                }
                return tree;
            }
            else if(subband == "hh")
            {
                // HH
                for (int j = 0; j < subbandsize; j++)
                {
                    HH[j] = detectedWatermark[j + subbandsize];
                }

                // HH
                for (int m = 0; m < NumOfTree; m++)
                {
                    tree[m] = HH[m];
                }
                return tree;
            }
            else
            {
                // HL
                for (int k = 0; k < subbandsize; k++)
                {
                    HL[k] = detectedWatermark[k + subbandsize + subbandsize];
                }

                // HL
                for (int n = 0; n < NumOfTree; n++)
                {
                    tree[n] = HL[n];
                }
                return tree;
            }           
        }
        #endregion

        //End
    }
}
