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
        
        // Convert inversed list of extracted watermark back into bitmap
        public static Bitmap ConvertListToWatermark(List<int> InverseDSSS, int height, int widht)
        {
            int size = (int)Math.Sqrt(InverseDSSS.Count);
            int[,] pixel = new int[height,widht];
            Bitmap bmp = new Bitmap(pixel.GetLength(0), pixel.GetLength(1));
            int c = 0;
            for(int i = 0; i < bmp.Height; i++)
            {
                for(int j = 0; j < bmp.Width; j++)
                {
                    if(InverseDSSS[c] == 1)
                    {
                        pixel[i, j] = 254;
                        c++;
                    }else
                    {
                        pixel[i, j] = 0;
                        c++;
                    }
                }
            }

            
            for(int y = 0; y < bmp.Height; y++)
            {
                for(int x = 0; x < bmp.Width; x++)
                {
                    Color clr = Color.FromArgb(pixel[y, x], pixel[y, x], pixel[y, x]);
                    bmp.SetPixel(x, y, clr);
                }
            }

            return bmp;
        }

        public static Bitmap ConvertListToWatermark2(double[] InverseDSSS, int height, int widht)
        {
            int size = (int)Math.Sqrt(InverseDSSS.Length);
            int[,] pixel = new int[height, widht];
            Bitmap bmp = new Bitmap(pixel.GetLength(0), pixel.GetLength(1));
            int c = 0;
            for (int i = 0; i < pixel.GetLength(0); i++)
            {
                for (int j = 0; j < pixel.GetLength(1); j++)
                {
                    if (InverseDSSS[c] == 1)
                    {
                        pixel[i, j] = 254;
                        c++;
                    }
                    else
                    {
                        pixel[i, j] = 0;
                        c++;
                    }
                }
            }


            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color clr = Color.FromArgb(pixel[y, x], pixel[y, x], pixel[y, x]);
                    bmp.SetPixel(x, y, clr);
                }
            }

            return bmp;
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
                    IMatrix[i, j] = c.G;
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
                        Color c = Color.FromArgb((int)IMatrixR[i, j], 0, (int)IMatrixB[i, j]);
                        bmp.SetPixel(j, i, c);
                    }
                    else if (ICoeffs[i, j] > 255)
                    {
                        Color c = Color.FromArgb((int)IMatrixR[i, j], 255, (int)IMatrixB[i, j]);
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
                        HVSValues[i, j] = 2;//0.1;
                    }
                    else if (IMatrix[i, j] > 51 && IMatrix[i, j] <= 102)
                    {
                        HVSValues[i, j] = 4;//0.2;
                    }
                    else if (IMatrix[i, j] > 102 && IMatrix[i, j] <= 153)
                    {
                        HVSValues[i, j] = 6;//0.3;
                    }
                    else if (IMatrix[i, j] > 153 && IMatrix[i, j] <= 204)
                    {
                        HVSValues[i, j] = 8;//0.4;
                    }
                    else if (IMatrix[i, j] > 204 && IMatrix[i, j] <= 255)
                    {
                        HVSValues[i, j] = 10;//0.5;
                    }
                }
            }

            return HVSValues;
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

        #region Convert To 8 bit 
        public static System.Drawing.Bitmap CopyToBpp(System.Drawing.Bitmap b, int bpp)
        {
            if (bpp != 1 && bpp != 8) throw new System.ArgumentException("1 or 8", "bpp");

            // Plan: built into Windows GDI is the ability to convert
            // bitmaps from one format to another. Most of the time, this
            // job is actually done by the graphics hardware accelerator card
            // and so is extremely fast. The rest of the time, the job is done by
            // very fast native code.
            // We will call into this GDI functionality from C#. Our plan:
            // (1) Convert our Bitmap into a GDI hbitmap (ie. copy unmanaged->managed)
            // (2) Create a GDI monochrome hbitmap
            // (3) Use GDI "BitBlt" function to copy from hbitmap into monochrome (as above)
            // (4) Convert the monochrone hbitmap into a Bitmap (ie. copy unmanaged->managed)

            int w = b.Width, h = b.Height;
            IntPtr hbm = b.GetHbitmap(); // this is step (1)
                                         //
                                         // Step (2): create the monochrome bitmap.
                                         // "BITMAPINFO" is an interop-struct which we define below.
                                         // In GDI terms, it's a BITMAPHEADERINFO followed by an array of two RGBQUADs
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.biSize = 40;  // the size of the BITMAPHEADERINFO struct
            bmi.biWidth = w;
            bmi.biHeight = h;
            bmi.biPlanes = 1; // "planes" are confusing. We always use just 1. Read MSDN for more info.
            bmi.biBitCount = (short)bpp; // ie. 1bpp or 8bpp
            bmi.biCompression = BI_RGB; // ie. the pixels in our RGBQUAD table are stored as RGBs, not palette indexes
            bmi.biSizeImage = (uint)(((w + 7) & 0xFFFFFFF8) * h / 8);
            bmi.biXPelsPerMeter = 1000000; // not really important
            bmi.biYPelsPerMeter = 1000000; // not really important
                                           // Now for the colour table.
            uint ncols = (uint)1 << bpp; // 2 colours for 1bpp; 256 colours for 8bpp
            bmi.biClrUsed = ncols;
            bmi.biClrImportant = ncols;
            bmi.cols = new uint[256]; // The structure always has fixed size 256, even if we end up using fewer colours
            if (bpp == 1) { bmi.cols[0] = MAKERGB(0, 0, 0); bmi.cols[1] = MAKERGB(255, 255, 255); }
            else { for (int i = 0; i < ncols; i++) bmi.cols[i] = MAKERGB(i, i, i); }
            // For 8bpp we've created an palette with just greyscale colours.
            // You can set up any palette you want here. Here are some possibilities:
            // greyscale: for (int i=0; i<256; i++) bmi.cols[i]=MAKERGB(i,i,i);
            // rainbow: bmi.biClrUsed=216; bmi.biClrImportant=216; int[] colv=new int[6]{0,51,102,153,204,255};
            //          for (int i=0; i<216; i++) bmi.cols[i]=MAKERGB(colv[i/36],colv[(i/6)%6],colv[i%6]);
            // optimal: a difficult topic: http://en.wikipedia.org/wiki/Color_quantization
            // 
            // Now create the indexed bitmap "hbm0"
            IntPtr bits0; // not used for our purposes. It returns a pointer to the raw bits that make up the bitmap.
            IntPtr hbm0 = CreateDIBSection(IntPtr.Zero, ref bmi, DIB_RGB_COLORS, out bits0, IntPtr.Zero, 0);
            //
            // Step (3): use GDI's BitBlt function to copy from original hbitmap into monocrhome bitmap
            // GDI programming is kind of confusing... nb. The GDI equivalent of "Graphics" is called a "DC".
            IntPtr sdc = GetDC(IntPtr.Zero);       // First we obtain the DC for the screen
                                                   // Next, create a DC for the original hbitmap
            IntPtr hdc = CreateCompatibleDC(sdc); SelectObject(hdc, hbm);
            // and create a DC for the monochrome hbitmap
            IntPtr hdc0 = CreateCompatibleDC(sdc); SelectObject(hdc0, hbm0);
            // Now we can do the BitBlt:
            BitBlt(hdc0, 0, 0, w, h, hdc, 0, 0, SRCCOPY);
            // Step (4): convert this monochrome hbitmap back into a Bitmap:
            System.Drawing.Bitmap b0 = System.Drawing.Bitmap.FromHbitmap(hbm0);
            //
            // Finally some cleanup.
            DeleteDC(hdc);
            DeleteDC(hdc0);
            ReleaseDC(IntPtr.Zero, sdc);
            DeleteObject(hbm);
            DeleteObject(hbm0);
            //
            return b0;
        }

        static uint MAKERGB(int r, int g, int b)
        {
            return ((uint)(b & 255)) | ((uint)((r & 255) << 8)) | ((uint)((g & 255) << 16));
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int InvalidateRect(IntPtr hwnd, IntPtr rect, int bErase);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int DeleteDC(IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr hdcDst, int xDst, int yDst, int w, int h, IntPtr hdcSrc, int xSrc, int ySrc, int rop);
        static int SRCCOPY = 0x00CC0020;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);
        static uint BI_RGB = 0;
        static uint DIB_RGB_COLORS = 0;
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public uint biSize;
            public int biWidth, biHeight;
            public short biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] cols;
        }
        #endregion


    }
}
