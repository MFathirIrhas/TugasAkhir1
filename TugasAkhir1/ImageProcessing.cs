﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TugasAkhir1
{
    public class ImageProcessing
    {
        public Bitmap ConvertToBinary(Bitmap bmp)
        {
            Bitmap b = new Bitmap(bmp.Width,bmp.Height);
            int width = bmp.Width;
            int height = bmp.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    double p = (c.R + c.G + c.B) / 3;
                    if (p >= 128)
                    {
                        Color newC = Color.FromArgb(255, 255, 255);
                        b.SetPixel(x, y, newC);
                    }
                    else if (p < 128)
                    {
                        Color newC = Color.FromArgb(0, 0, 0);
                        b.SetPixel(x, y, newC);
                    }
                }
            }

            return b;
        }


    }
}
