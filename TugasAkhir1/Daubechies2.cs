using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Accord.Math;
using Accord.Statistics;

namespace TugasAkhir1
{
    public class Daubechies2
    {
        //For Low Pass Filter
        private const double s0 = -0.1294095226;
        private const double s1 = 0.2241438680;
        private const double s2 = 0.8365163037;
        private const double s3 = 0.4829629131;

        //For High Pass Filter
        private const double w0 = -0.4829629131;
        private const double w1 = 0.8365163037;
        private const double w2 = -0.2241438680;
        private const double w3 = -0.1294095226;

        public static double[,] Db2Kernel(int size)
        {
            double[,] kernel = new double[size,size];

            for(int i = 0; i < size; i += 2)
            {
                if (i == size - 2)
                {
                    ///low
                    kernel[i, i + 0] = s0;
                    kernel[i, i + 1] = s1;

                    ///high
                    kernel[i + 1, i + 0] = w0;
                    kernel[i + 1, i + 1] = w1;
                }
                else
                {
                    ///low
                    kernel[i, i + 0] = s0;
                    kernel[i, i + 1] = s1;
                    kernel[i, i + 2] = s2;
                    kernel[i, i + 3] = s3;

                    ///high
                    kernel[i + 1, i + 0] = w0;
                    kernel[i + 1, i + 1] = w1;
                    kernel[i + 1, i + 2] = w2;
                    kernel[i + 1, i + 3] = w3;
                }
                
            }

            return kernel;
        }


    }
}
