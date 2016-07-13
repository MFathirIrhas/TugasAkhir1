﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Utilities.Random;
using Utilities.Media.Image.ExtensionMethods;
using AForge.Imaging.Filters;
using AForge.Imaging.ColorReduction;




namespace TugasAkhir1
{
    public static class ImageAttack
    {
        #region HISTOGRAM EQUILIZATION
        //Histogram Equilization
        public static Bitmap HistEq(Bitmap b, double blackPointPercent = 0.02, double whitePointPercent = 0.01)
        {
            BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Bitmap resultImage = new Bitmap(b.Width, b.Height);
            BitmapData destData = resultImage.LockBits(new Rectangle(0, 0, resultImage.Width, resultImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int stride = bmpData.Stride;
            IntPtr srcScan0 = bmpData.Scan0;
            IntPtr destScan0 = destData.Scan0;
            var freq = new int[256];

            unsafe
            {
                byte* src = (byte*)srcScan0;
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        ++freq[src[y * stride + x * 4]];
                    }
                }

                int numPixels = b.Width * b.Height;
                int minI = 0;
                var blackPixels = numPixels * blackPointPercent;
                int accum = 0;

                while (minI < 255)
                {
                    accum += freq[minI];
                    if (accum > blackPixels) break;
                    ++minI;
                }

                int maxI = 255;
                var whitePixels = numPixels * whitePointPercent;
                accum = 0;

                while (maxI > 0)
                {
                    accum += freq[maxI];
                    if (accum > whitePixels) break;
                    --maxI;
                }
                double spread = 255d / (maxI - minI);
                byte* dst = (byte*)destScan0;
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        int i = y * stride + x * 4;

                        byte val = (byte)Clamp(Math.Round((src[i] - minI) * spread), 0, 255);
                        dst[i] = val;
                        dst[i + 1] = val;
                        dst[i + 2] = val;
                        dst[i + 3] = 255;
                    }
                }
            }

            b.UnlockBits(bmpData);
            resultImage.UnlockBits(destData);

            return resultImage;
        }

        static double Clamp(double val, double min, double max)
        {
            return Math.Min(Math.Max(val, min), max);
        }
        #endregion

        #region MEAN FILTERING
        //Median Filtering
        ///Matrix Masking
        public static double[,] Mean3x3
        {
            get
            {
                return new double[,]
                { { 1, 1, 1, },
                  { 1, 1, 1, },
                  { 1, 1, 1, }, };
            }
        }

        public static double[,] Mean5x5
        {
            get
            {
                return new double[,]
                { { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1}, };
            }
        }

        public static double[,] Mean7x7
        {
            get
            {
                return new double[,]
                { { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1}, };
            }
        }

        public static Bitmap MeanFilter(Bitmap input, int pil)
        {
            Bitmap bmp = new Bitmap(input.Width, input.Height);
            if (pil == 1) //masking 3x3
            {
                bmp = ImageProcessing.ConvolutionFilter(input, Mean3x3, 1.0 / 9.0, 0);
            }
            else if (pil == 2)//masking 5x5
            {
                bmp = ImageProcessing.ConvolutionFilter(input, Mean5x5, 1.0 / 25.0, 0);
            }
            else
            {
                bmp = ImageProcessing.ConvolutionFilter(input, Mean7x7, 1.0 / 49.0, 0);
            }
            return bmp;
        }
        #endregion

        #region MEDIAN FILTERING
        public static Bitmap MedianFilter(this Bitmap sourceBitmap,int matrixSize)
        {
            BitmapData sourceData =sourceBitmap.LockBits(new Rectangle(0, 0,sourceBitmap.Width, sourceBitmap.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];

            byte[] resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
                                       pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            int filterOffset = (matrixSize - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            List<int> neighbourPixels = new List<int>();
            byte[] middlePixel;

            for (int offsetY = filterOffset; offsetY <
                sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    sourceBitmap.Width - filterOffset; offsetX++)
                {
                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;

                    neighbourPixels.Clear();

                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {

                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);

                            neighbourPixels.Add(BitConverter.ToInt32(
                                             pixelBuffer, calcOffset));
                        }
                    }

                    neighbourPixels.Sort();

                    middlePixel = BitConverter.GetBytes(
                                       neighbourPixels[filterOffset]);

                    resultBuffer[byteOffset] = middlePixel[0];
                    resultBuffer[byteOffset + 1] = middlePixel[1];
                    resultBuffer[byteOffset + 2] = middlePixel[2];
                    resultBuffer[byteOffset + 3] = middlePixel[3];
                }
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width,
                                             sourceBitmap.Height);

            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                       ImageLockMode.WriteOnly,
                       PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                                       resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }
        #endregion

        #region MODUS FILTERING
        public static double[,] Modus3x3
        {
            get
            {
                return new double[,]
                { { 1, 1, 1, },
                  { 1, 1, 1, },
                  { 1, 0, 0, }, };
            }
        }

        public static double[,] Modus5x5
        {
            get
            {
                return new double[,]
                { { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 0, 0},
                  { 1, 0, 0, 0, 0}, };
            }
        }

        public static double[,] Modus7x7
        {
            get
            {
                return new double[,]
                { { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 0, 0},
                  { 1, 1, 1, 0, 0, 0, 0},
                  { 1, 1, 1, 0, 0, 0, 0}, };
            }
        }

        public static Bitmap ModusFilter(Bitmap input, int pil)
        {
            Bitmap bmp = new Bitmap(input.Width, input.Height);
            if (pil == 1) //masking 3x3
            {
                bmp = ImageProcessing.ConvolutionFilter(input, Modus3x3, 1.0 / 9.0, 0);
            }
            else if (pil == 2)//masking 5x5
            {
                bmp = ImageProcessing.ConvolutionFilter(input, Modus5x5, 1.0 / 25.0, 0);
            }
            else
            {
                bmp = ImageProcessing.ConvolutionFilter(input, Modus7x7, 1.0 / 49.0, 0);
            }
            return bmp;
        }
        #endregion

        #region JPEG COMPRESSION
        //JPEG Compression : compress the image using jpeg encoder
        public static Bitmap JpegCompress(Image source, int quality, string filename)
        {
            //SaveFileDialog sfd = new SaveFileDialog();
            //sfd.Title = "Select Where Compressed Image will be Saved";
            //sfd.InitialDirectory = @"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Saved_Image";
            //if(sfd.ShowDialog() == DialogResult.OK)
            //{
                try
                {
                    //codec info
                    ImageCodecInfo jpegCodec = null;

                    //Set quality factor for compression
                    EncoderParameter imageQualityParameter = new EncoderParameter(
                        System.Drawing.Imaging.Encoder.Quality, quality);

                    //List all codec 
                    ImageCodecInfo[] alleCodecs = ImageCodecInfo.GetImageEncoders();

                    EncoderParameters codecParameter = new EncoderParameters(1);
                    codecParameter.Param[0] = imageQualityParameter;

                    //Find and choose JPEG codec
                    for (int i = 0; i < alleCodecs.Length; i++)
                    {
                        if (alleCodecs[i].MimeType == "image/jpeg")
                        {
                            jpegCodec = alleCodecs[i];
                            break;
                        }
                    }

                    string path = @"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Saved_Image\"+filename;
                    //Save and display compressed image
                    source.Save(path, jpegCodec, codecParameter);
                    
                }
                catch(Exception e)
                {
                    throw e;
                }
            //}
            return new Bitmap(@"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Saved_Image\" + filename);
        }
        #endregion

        #region Gaussian Noise       
        public static Bitmap GaussianNoise(Image img, int intense)
        {
            Bitmap finalBmp = img as Bitmap;
            System.Random r = new System.Random();
            int width = img.Width;
            int height = img.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int def = r.Next(0, 100);
                    if (def < intense)
                    {
                        int op = r.Next(0, 1);
                        if (op == 0)
                        {
                            int num = r.Next(0, intense);
                            Color clr = finalBmp.GetPixel(x, y);
                            int R = (clr.R + clr.R + num) / 2;
                            if (R > 255) R = 255;
                            int G = (clr.G + clr.G + num) / 2;
                            if (G > 255) G = 255;
                            int B = (clr.B + clr.B + num) / 2;
                            if (B > 255) B = 255;
                            Color result = Color.FromArgb(255, R, G, B);
                            finalBmp.SetPixel(x, y, result);
                        }
                        else
                        {
                            int num = r.Next(0, intense);
                            Color clr = finalBmp.GetPixel(x, y);
                            Color result = Color.FromArgb(255, (clr.R + clr.R - num) / 2, (clr.G + clr.G - num) / 2,
                                (clr.B + clr.B - num) / 2);
                            finalBmp.SetPixel(x, y, result);
                        }
                    }
                }
            }
            return finalBmp;
        }

        public static Bitmap SaltAndPepper(Bitmap bmp, int intensity)
        {
            SaltAndPepperNoise filter = new SaltAndPepperNoise(intensity);
            filter.ApplyInPlace(bmp);
            return bmp;
        }
        #endregion

        #region Median Cut
        public static Bitmap MedianCut(Bitmap bmp, int intensity)
        {
            ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
            IColorQuantizer quantizer = new MedianCutQuantizer();
            Bitmap newImage = ciq.ReduceColors(bmp, intensity);
            return newImage;
        }
        #endregion



    }
}
