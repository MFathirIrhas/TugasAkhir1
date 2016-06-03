using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace TugasAkhir1
{
    /* *
     * This class was used to do some Image processing
     * 1. ConvertToBinary
     * 2. ConvertToGray
     * 3. ConvertToBitmap
     * 4. ConvertToMatrix
     * 5. ConvertToMatrix2
     * 6. ConvertToBitmap
     * 7. ConvertToBitmap2
     * 8. HVS System : Using laplace detection
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

        public static Bitmap ConvertToGray(Bitmap b)
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
        public static Bitmap ConvertToBitmap2(double[,] IMatrixR, double[,] ICoeffs, double[,] IMatrixB)
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
        public static double[,] HVS(Bitmap EdgyImage)
        {
            double[,] IMatrix = ConvertToMatrix(EdgyImage);
            double[,] HVSValues = new double[IMatrix.GetLength(0), IMatrix.GetLength(1)];
            for (int i = 0; i < IMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < IMatrix.GetLength(1); j++)
                {
                    if (IMatrix[i, j] <= 51)
                    {
                        IMatrix[i, j] = 0.1;
                    }
                    else if (IMatrix[i, j] > 51 && IMatrix[i, j] <= 102)
                    {
                        IMatrix[i,j] = 0.2;
                    }
                    else if (IMatrix[i, j] > 102 && IMatrix[i, j] <= 153)
                    {
                        IMatrix[i, j] = 0.3;
                    }
                    else if (IMatrix[i, j] > 153 && IMatrix[i, j] <= 204)
                    {
                        IMatrix[i, j] = 0.4;
                    }
                    else if (IMatrix[i, j] > 204 && IMatrix[i, j] <= 255)
                    {
                        IMatrix[i, j] = 0.5;
                    }
                }
            }

            return IMatrix;
        }
        public static Bitmap LaplaceEdge(Bitmap DecomposedImage)
        {
            Bitmap GrayDecomposedImage = ConvertToGray(DecomposedImage);
            Bitmap EdgyImage = ConvolutionFilter(GrayDecomposedImage, LaplaceMask, 1.0 / 1.0, 0);
            return EdgyImage;
        }

        public static double[,] LaplaceMask
        {
            get
            {
                return new double[,]  
                { { 0, 1, 0, }, 
                  { 1,-4, 1, }, 
                  { 0, 1, 0, }, };
            }
        }

        public static Bitmap ConvolutionFilter(Bitmap sourceBitmap, double[,] filterMatrix, double factor = 1, int bias = 0)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;

            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;

                    byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                            blue += (double)(pixelBuffer[calcOffset]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];

                            green += (double)(pixelBuffer[calcOffset + 1]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];

                            red += (double)(pixelBuffer[calcOffset + 2]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                        }
                    }

                    blue = factor * blue + bias;
                    green = factor * green + bias;
                    red = factor * red + bias;

                    blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));

                    green = (green > 255 ? 255 : (green < 0 ? 0 : green));

                    red = (red > 255 ? 255 : (red < 0 ? 0 : red));

                    resultBuffer[byteOffset] = (byte)(blue);
                    resultBuffer[byteOffset + 1] = (byte)(green);
                    resultBuffer[byteOffset + 2] = (byte)(red);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                     resultBitmap.Width, resultBitmap.Height),
                                                      ImageLockMode.WriteOnly,
                                                 PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

    }
}
