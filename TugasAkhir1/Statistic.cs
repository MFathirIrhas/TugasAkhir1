using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace TugasAkhir1
{
    public class Statistic
    {
        public static double Mean(int[] d)
        {
            double mu = d.Average();
            return mu;
        }

        public static double Variance(List<int> d)
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

        
    }
}
