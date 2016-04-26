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
                        coeff.Add(p);
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
                        coeff.Add(p);
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
                        coeff.Add(p);
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
                        coeff.Add(p);
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
                        coeff.Add(p);
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
                        coeff.Add(p);
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
                        coeff.Add(p);
                    }
                }
            }

            return coeff;
        }


    }
}
