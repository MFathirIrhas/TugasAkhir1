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
        //Get wavelet coefficients for training data in HMM Model
        public static List<int> GetWaveletCoeff(Bitmap bmp,int level, string subband)
        {
            List<int> coeff = new List<int>();
            Color c;
            if (level == 1 && subband == "LL")
            {
                for (int y = 0; y < bmp.Height / 4; y++)
                {
                    for (int x = 0; x < bmp.Width / 4; x++)
                    {
                        c = bmp.GetPixel(x, y);
                        int p = c.R;
                        coeff.Add(p);// -127);
                    }
                }
            }
            else if(level == 1 && subband == "HL")
            {
                for (int y = 0; y < bmp.Height / 4; y++)
                {
                    for (int x = bmp.Width/4; x < bmp.Width / 2; x++)
                    {
                        c = bmp.GetPixel(x, y);
                        int p = c.R;
                        coeff.Add(p);//  - 127);
                    }
                }
            }
            else if(level==1 && subband=="LH")
            {
                for (int y = bmp.Width/4; y < bmp.Height / 2; y++)
                {
                    for (int x = 0; x < bmp.Width / 4; x++)
                    {
                        c = bmp.GetPixel(x, y);
                        int p = c.R;
                        coeff.Add(p);//  - 127);
                    }
                }
            }
            else if (level == 1 && subband == "HH")
            {
                for (int y = bmp.Height/4; y < bmp.Height / 2; y++)
                {
                    for (int x = bmp.Height/4; x < bmp.Width / 2; x++)
                    {
                        c = bmp.GetPixel(x, y);
                        int p = c.R;
                        coeff.Add(p);//  - 127);
                    }
                }
            }
            else if (level == 2 && subband == "HL")
            {
                for (int y = 0; y < bmp.Height / 2; y++)
                {
                    for (int x = bmp.Height/2; x < bmp.Width; x++)
                    {
                        c = bmp.GetPixel(x, y);
                        int p = c.R;
                        coeff.Add(p);//  - 127);
                    }
                }
            }
            else if (level == 2 && subband == "LH")
            {
                for (int y = bmp.Height/2; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width / 2; x++)
                    {
                        c = bmp.GetPixel(x, y);
                        int p = c.R;
                        coeff.Add(p);//  - 127);
                    }
                }
            }
            else if (level == 2 && subband == "HH")
            {
                for (int y = bmp.Height/2; y < bmp.Height; y++)
                {
                    for (int x = bmp.Width/2; x < bmp.Width; x++)
                    {
                        c = bmp.GetPixel(x, y);
                        int p = c.R;
                        coeff.Add(p);//  - 127);
                    }
                }
            }
            else if (level == 0 && subband == "ALL")
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0 ; x < bmp.Width; x++)
                    {
                        c = bmp.GetPixel(x, y);
                        int p = c.R;
                        coeff.Add(p);// - 127);
                    }
                }
            }
            
            return coeff;
        }

        //2 Hidden State, High=1 and Low=2
        public static List<int> GetHiddenStates(List<int> coeff)
        {
            var hs = new List<int>();
            int threshold = 127;
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
        public static double Threshold(List<int> coeff)
        {
            double mean;
            int counter = 0;
            mean = coeff.Average();
            var groups = coeff.GroupBy(v => v);
            int maxCount = groups.Max(g => g.Count());
            int mode = groups.First(g => g.Count() == maxCount).Key;
            int max = coeff.Max();
            //for (int i = 0; i < coeff.Count; i++)
            //{
            //    counter += coeff[i];
            //}
            //d = counter / coeff.Count;
            return mean;
        }
        
    }
}
