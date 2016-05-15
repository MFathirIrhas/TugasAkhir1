using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TugasAkhir1
{
    /* *
     * This class was used to do some Image processing
     * 1. ConvertToBinary Class
     * 2. ConvertToBitmap
     * 3. 
     * 4. 
     * */
    public class ImageProcessing
    {
        //Convert Image to Black and White, pixel value are only 255 or 0
        public static Bitmap ConvertToBinary(Bitmap bmp)
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

        public static Bitmap CovertToGray(Bitmap b)
        {

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride; // bytes in a row 3*b.Width
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte red, green, blue;
                int nOffset = stride - b.Width * 3;
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];
                        p[0] = p[1] = p[2] = (byte)(.299 * red
                            + .587 * green + .114 * blue);
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            b.UnlockBits(bmData);
            return b;
        }

        //For Grayscale Host Image
        public static double[,] ConvertToMatrix(Bitmap bmp)
        {
            double[,] IMatrix = new double[bmp.Height, bmp.Width];
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color c = bmp.GetPixel(j,i);
                    IMatrix[i, j] = c.R;
                }
            }
            return IMatrix;
        }

        //For Color Host Image
        public static Tuple<double[,],double[,],double[,]> ConvertToMatrix2(Bitmap bmp)
        {
            double[,] IMatrixR = new double[bmp.Height,bmp.Width];
            double[,] IMatrixG = new double[bmp.Height, bmp.Width];
            double[,] IMatrixB = new double[bmp.Height, bmp.Width];

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color c = bmp.GetPixel(j, i);
                    IMatrixR[i, j] = c.R;
                    IMatrixG[i, j] = c.G;
                    IMatrixB[i, j] = c.B;
                }
            }

            return new Tuple<double[,], double[,], double[,]>(IMatrixR, IMatrixG, IMatrixB);
        }

        //For Grayscale
        public static Bitmap ConvertToBitmap(double[,] ICoeffs)
        {
            Bitmap bmp = new Bitmap(ICoeffs.GetLength(1), ICoeffs.GetLength(0));
            for (int i = 0; i < ICoeffs.GetLength(0); i++)
            {
                for (int j = 0; j < ICoeffs.GetLength(1); j++)
                {
                    if (ICoeffs[i, j] < 0)
                    {
                        Color c = Color.FromArgb(0, 0, 0);
                        bmp.SetPixel(j, i, c);
                    }
                    else if (ICoeffs[i, j] > 254)
                    {
                        Color c = Color.FromArgb(254,254,254);
                        bmp.SetPixel(j, i, c);
                    }
                    else
                    {
                        Color c = Color.FromArgb((int)ICoeffs[i, j], (int)ICoeffs[i, j], (int)ICoeffs[i, j]);
                        bmp.SetPixel(j, i, c);
                    }

                    //Color c = Color.FromArgb((int)ICoeffs[i, j], (int)ICoeffs[i, j], (int)ICoeffs[i, j]);
                    //bmp.SetPixel(j, i, c);
                }
            }
            return bmp;
        }

        //For Color Image
        public static Bitmap ConvertToBitmap2(double[,] ICoeffs, double[,] IMatrixR, double[,] IMatrixB)
        {
            Bitmap bmp = new Bitmap(ICoeffs.GetLength(1), ICoeffs.GetLength(0));
            for (int i = 0; i < ICoeffs.GetLength(0); i++)
            {
                for (int j = 0; j < ICoeffs.GetLength(1); j++)
                {
                    if (ICoeffs[i, j] < 0)
                    {
                        Color c = Color.FromArgb(0, 0, 0);
                        bmp.SetPixel(j, i, c);
                    }
                    else if (ICoeffs[i, j] > 254)
                    {
                        Color c = Color.FromArgb(254, 254, 254);
                        bmp.SetPixel(j, i, c);
                    }
                    else
                    {
                        Color c = Color.FromArgb((int)IMatrixR[i, j], (int)ICoeffs[i, j], (int)IMatrixB[i, j]);
                        bmp.SetPixel(j, i, c);
                    }

                    //Color c = Color.FromArgb((int)IMatrixR[i, j], (int)ICoeffs[i, j], (int)IMatrixB[i, j]);
                    //bmp.SetPixel(j, i, c);
                }
            }
            return bmp;
        }

        //Procedure for Human Visual System

    }
}
