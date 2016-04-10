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
     * 4. Direct Sequence Spread Spectrum
     * 5. Interleaving Sequence
     * 6. Segment
     * */
    public class Matrix
    {
        //Global Variables
        int[] pnseed = new int[4];

        #region 1. ConvertToVectorMatrix
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
        #endregion

        #region 2. ConvertToBinaryVectorMatrix
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
        #endregion

        #region 3. 1/3 Convolution Code
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
        #endregion

        #region 4. Direct Sequence Spread Spectrum
        /* *
         * Direct Sequence Spread Spectrum
         * convert the input binary sequence into sequence with values 1 or 0
         * 1. Input bit is a binary bit from 1/3 Convolutional Code
         * 2. The input bit is generated using a PN Sequence
         * 3. PN Sequence is generated using a key
         * 4. The result will be a sequence contain values 1 and -1 with length = input bit length * PN Sequence length
         * */
        public List<int> DSSS(List<int> mc)
        {
            List<int> dsss = new List<int>();
            int pnlength = mc.Count * 4;
            //List<int> PNSeq = PNSeqGenerate(pnlength); //Pseudonoise sequence generated randomly using pseudorandom sequence
            string pn_seed = "1000";
            string pn_mask = "1010";
            int pn_length = pnlength;
            List<int> PNSeq = PNSeqLFSR(pn_seed, pn_mask, pn_length);

            int k = 0;
            for (int i = 0; i < mc.Count; i++) //Looping for input data, result of 1/3 convolution code
            {
                while(k < (i+1)*4) // Looping for PN sequence, each bit in input data is attach to 4 PN bit sequence
                {
                    if (mc[i] == 0)
                    {
                        int xor = mc[i] ^ PNSeq[k];
                        dsss.Add(xor);
                    }
                    else 
                    {
                        int xor = mc[i] ^ PNSeq[k];
                        dsss.Add(xor);
                    }
                    k = k + 1;
                }       
            }
            
            return dsss;
        }

        //Generate PNSequence using LFSR(Linear Feedback Shift Register)
        public List<int> PNSeqLFSR(string seed, string mask, int length)
        {
            List<int> pnseq = new List<int>();

            //Initialize shift register with the pn_seed
            for (int i = 0; i < 4; i++)
            {
                pnseed[i] = (int)Char.GetNumericValue(seed[i]);
            }

            int[] key = pnseed; //key = sr

            for (int i = 0; i < length; i++)
            {
                int new_bit = 0;
                for (int j = 0; j < 4; j++)
                {
                    if ((int)Char.GetNumericValue(mask[j]) == 1)
                        new_bit = new_bit ^ key[j];
                }

                pnseq.Add(key[4 - 1]);
                key = Roll(key);
                key[0] = new_bit;
            }

            return pnseq;
        }

        //Shift pnseed to the right
        public int[] Roll(int[] key)
        {
            int[] ShiftedKey = new int[key.Length];
            for (int i = 0; i < key.Length; i++)
            {
                ShiftedKey[(i + 1) % ShiftedKey.Length] = key[i];
            }

            return ShiftedKey;
        }

        public List<int> PNSeqGenerate(int l)
        {
            List<int> pn = new List<int>();
            Random rnd = new Random();
            for (int i = 0; i < l; i++)
            {
                pn.Add(rnd.Next(2));
            }
            
            return pn;
        }
        #endregion

        #region 5. Interleaving Sequence
        public List<int> Interleaving(List<int> dsss)
        {
            List<int> il = new List<int>();
            var ds3 = dsss;
            List<int> ySeq = GenerateRandomBinarySeq(ds3.Count);
            //dsss.AddRange(ySeq);
            //var InterLength = dsss.Count * 2;
            //int k1 = 0;
            //int k2 = InterLength / 2;
            while (ds3.Count > 0 && ySeq.Count > 0)
            {
                if (ds3.Count > 0)
                {
                    il.Add(ds3[0]);
                    ds3.RemoveAt(0);
                }

                if (ySeq.Count > 0)
                {
                    il.Add(ySeq[0]);
                    ySeq.RemoveAt(0);
                }
            }

            return il;
        }

        public List<int> GenerateRandomBinarySeq(int length) //generate random binary sequence for Interleaving sequence
        {
            var randBin = new List<int>();
            Random rand = new Random();

            for (int i = 0; i < length; i++)
            {
                if (rand.Next() % 2 == 0)
                {
                    randBin.Add(0);
                }
                else
                {
                    randBin.Add(1);
                }
            }

            randBin.Sort();
            return randBin;
        }
        #endregion

        #region 6. Segmentation
        //Group the Interleaved sequence into M-bit segments. M = 15.
        public List<List<int>> Segment(List<int> Interleaved)
        {
            List<List<int>> Tree = new List<List<int>>();
            List<int> tree_th = new List<int>();
        
            //Get total number of trees
            double t = Interleaved.Count / 15; //15 didapat dari jumlah node dalam 1 pohon HMM, 3 parents dan 12 anak-nya untuk setiap scale.
            int nSize =(int)Math.Floor(t);

            for (int i = 0; i < Interleaved.Count; i += nSize)
            {
                Tree.Add(Interleaved.GetRange(i, Math.Min(nSize, Interleaved.Count - i)));
            }

            return Tree;           
        }
        #endregion
    }
}
