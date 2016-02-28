using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;


namespace TugasAkhir1
{
    /**
     * This class was used to do some matrix manipulation
     * 1. ConvertToVectorMatrix Class
     * 2. ConvertToBinaryVectorMatrix Class
     * 3. 1/3 Convolution Code
     * 4. Direct Spread Spectrum
     * 5. Interleaving
     * 
     * */
    public class Matrix
    {
        //Convert to 1 Dimensional Matrix fill with black and white value
        public List<int> ConvertToVectorMatrix(Bitmap bmp)
        {
            List<int> m = new List<int>();
            int width = bmp.Width;
            int height = bmp.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    int p = (c.R+c.G+c.B)/3;
                    m.Add(p);
                }
            }

            return m;
        }

        //Convert to 0 or 1 bit sequence , 1 denote to white space and 0 denote to black space
        public List<int> ConvertToBinaryVectorMatrix(List<int> vm)
        {
            List<int> bvm = new List<int>();
            foreach (int i in vm)
            {
                if (i >= 250)
                {
                    int b = 1;
                    bvm.Add(b);
                }
                else if (i <= 0)
                {
                    int b = 0;
                    bvm.Add(b);
                }
            }

            return bvm;    
        }

        /**
         * 1/3 Convolutional Code
         * k = 1 , Number of input each time process
         * n = 3 , number of output each time after process
         * m = input data
         * Generator Polynomial use:
         *      g1 = { 1, 1, 1, 1, 0, 1, 1, 1 }
         *      g2 = { 1, 1, 0, 1, 1, 0, 0, 1 }
         *      g3 = { 1, 0, 0, 1, 0, 1, 0, 1 }
         **/
        public List<int> ConvolutionCode(List<int> m)
        {
            List<int> mc = new List<int>(); //Output list
            int elm = 0;
            int v = 3; //1/3 rate with 3 output at a time.
            //memory register
            int[] reg = new int[m.Count];
            //Generator Polynomial
            int[,] g = new int[,] { { 1, 1, 1, 1, 0, 1, 1, 1 },   //g0 
                                    { 1, 1, 0, 1, 1, 0, 0, 1 },   //g1
                                    { 1, 0, 0, 1, 0, 1, 0, 1 } }; //g2

            for (int n = 0; n < m.Count; n++)
            {
                for (int i = 0; i < v/*3*/; i++)
                {
                    for(int j = 0; j < 8; j++)
                    {
                        int l = n - j;
                        if (l < 0)
                        {
                            elm += g[i,j] * 0; //x[n] = 0 , if n = negative
                        }
                        else
                        {
                            elm += g[i, j] * m[l];
                        }
                    }
                    int item = elm % 2;
                    mc.Add(item);
                }
            }
            
            return mc;
        }


    }
}
