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

namespace TugasAkhir1
{
    public class Extract
    {
        /// <summary>
        /// Detect Watermark using Log Likelihood 
        /// </summary>
        /// <param name="coeffs">Wavelet Coefficients of watermarked image</param>
        /// <param name="rootpmf">Parent nodes pmf</param>
        /// <param name="transition">Parent-Child Transition State Probability</param>
        /// <param name="variances">Variances</param>
        /// <returns></returns>
        public static double[] DetectWatermark(double[,] coeffs,Image watermarkedImage,double[] rootpmf, double[,] transition, double[,] variances)
        {
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

            // After that, one could, for example, query the probability
            // of a sequence occurring. We will consider the sequence
            double[] sequence = new double[] { 0, 1, 2 };

            // And now we will evaluate its likelihood
            double logLikelihood = hmm.Evaluate(sequence);

            // At this point, the log-likelihood of the sequence
            // occurring within the model is -3.3928721329161653.

            // We can also get the Viterbi path of the sequence
            int[] path = hmm.Decode(sequence, out logLikelihood);

            // At this point, the state path will be 1-0-0 and the
            // log-likelihood will be -4.3095199438871337

            /// Detection of Watermark
            double[,] trees = HMM.ExtractTrees(coeffs);
            Bitmap decomposedImage = DWT.TransformDWT(true, false, 2, new Bitmap(watermarkedImage));
            Bitmap edgyImage = ImageProcessing.LaplaceEdge(decomposedImage);
            double[,] hvs = ImageProcessing.HVS(edgyImage);
            double[,] listOfHvs = HMM.ExtractTrees(hvs);
            double embedStrength = 0.3;
        }
    }
}
