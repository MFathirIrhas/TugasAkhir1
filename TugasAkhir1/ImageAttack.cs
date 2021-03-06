﻿extern alias aforgemath;
extern alias af;
using System;
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
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging.ColorReduction;
using AForge.Math.Random;
using Utilities;
using Utilities.Media.Image;
using Accord.Statistics.Distributions.Univariate;



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

        public static Bitmap HistogramEquilization(Bitmap bmp)
        {
            // create filter
            HistogramEqualization filter = new HistogramEqualization();
            // process image
            filter.ApplyInPlace(bmp);
            return bmp;

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

        #region Mean Filtering Aforge
        public static Bitmap Mean(Bitmap bmp)
        {
            // create filter
            Mean filter = new Mean();
            // apply the filter
            filter.ApplyInPlace(bmp);
            return bmp;
        }
        #endregion

        #region MEDIAN FILTERING
        public static Bitmap MedianFilter(this Bitmap sourceBitmap,int matrixSize)
        {
            BitmapData sourceData =sourceBitmap.LockBits(new Rectangle(0, 0,sourceBitmap.Width, sourceBitmap.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

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

        #region Median Filtering Aforge
        public static Bitmap Median(Bitmap bmp)
        {
            // create filter
            Median filter = new Median();
            // apply the filter
            filter.ApplyInPlace(bmp);
            return bmp;
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

        #region JPEG Compression2
        private static ImageCodecInfo GetEncoderInfo(string mime_type)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i <= encoders.Length; i++)
            {
                if (encoders[i].MimeType == mime_type) return encoders[i];
            }
            return null;
        }
        private static void SaveJpg(Image image, string file_name, long compression)
        {
            try
            {
                EncoderParameters encoder_params = new EncoderParameters(1);
                encoder_params.Param[0] = new EncoderParameter(
                    System.Drawing.Imaging.Encoder.Quality, compression);

                ImageCodecInfo image_codec_info = GetEncoderInfo("image/jpeg");
                File.Delete(file_name);
                image.Save(file_name, image_codec_info, encoder_params);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file '" + file_name +
                    "'\nTry a different file name.\n" + ex.Message,
                    "Save Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static Bitmap LoadBitmapUnlocked(string file_name)
        {
            using (Bitmap bm = new Bitmap(file_name))
            {
                return new Bitmap(bm);
            }
        }

        public  static Bitmap JPEGCompress2(Image img, string level)
        {
            

            // Free the PictureBox's current image.
            

            // Save the image with the selected compression level.
            long compression = long.Parse(level);
            string file_name = Application.StartupPath + "\\__temp.jpg";
            SaveJpg(img, file_name, compression);

            // Display the result without locking the file.
            Image ResultImage = LoadBitmapUnlocked(file_name);

            return new Bitmap(ResultImage);
            // See how big the file is.
            //FileInfo file_info = new FileInfo(file_name);
            
        }
        #endregion

        #region Noise       
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

        #region Noise 2
        public static Bitmap AddNoise(Bitmap OriginalImage, int Amount)
        {
            Bitmap NewBitmap = new Bitmap(OriginalImage.Width, OriginalImage.Height);
            BitmapData NewData = NewBitmap.LockImage();//Image.LockImage(NewBitmap);
            BitmapData OldData = OriginalImage.LockImage();//Image.LockImage(OriginalImage);
            int NewPixelSize = NewData.GetPixelSize();//Image.GetPixelSize(NewData);
            int OldPixelSize = OldData.GetPixelSize();//Image.GetPixelSize(OldData);
            Utilities.Random.Random TempRandom = new Utilities.Random.Random();
            for (int x = 0; x < NewBitmap.Width; ++x)
            {
                for (int y = 0; y < NewBitmap.Height; ++y)
                {
                    Color CurrentPixel = OldData.GetPixel(x, y, OldPixelSize);//Image.GetPixel(OldData, x, y, OldPixelSize);
                    int R = CurrentPixel.R + TempRandom.Next(-Amount, Amount + 1);
                    int G = CurrentPixel.G + TempRandom.Next(-Amount, Amount + 1);
                    int B = CurrentPixel.B + TempRandom.Next(-Amount, Amount + 1);
                    R = R > 255 ? 255 : R;
                    R = R < 0 ? 0 : R;
                    G = G > 255 ? 255 : G;
                    G = G < 0 ? 0 : G;
                    B = B > 255 ? 255 : B;
                    B = B < 0 ? 0 : B;
                    Color TempValue = Color.FromArgb(R, G, B);
                    NewData.SetPixel(x, y, TempValue, NewPixelSize);//Image.SetPixel(NewData, x, y, TempValue, NewPixelSize);
                }
            }
            NewBitmap.UnlockImage(NewData);//Image.UnlockImage(NewBitmap, NewData);
            OriginalImage.UnlockImage(OldData);//Image.UnlockImage(OriginalImage, OldData);
            return NewBitmap;
        }

        public static Bitmap AddNoise2(Bitmap OriginalImage, int Amount)
        {
            Bitmap NewBitmap = new Bitmap(OriginalImage.Width, OriginalImage.Height);
            BitmapData NewData = NewBitmap.LockImage();//Image.LockImage(NewBitmap);
            BitmapData OldData = OriginalImage.LockImage();//Image.LockImage(OriginalImage);
            int NewPixelSize = NewData.GetPixelSize();//Image.GetPixelSize(NewData);
            int OldPixelSize = OldData.GetPixelSize();//Image.GetPixelSize(OldData);
            Utilities.Random.Random TempRandom = new Utilities.Random.Random();
            for (int x = 0; x < NewBitmap.Width; ++x)
            {
                for (int y = 0; y < NewBitmap.Height; ++y)
                {
                    Color CurrentPixel = OldData.GetPixel(x, y, OldPixelSize);//Image.GetPixel(OldData, x, y, OldPixelSize);
                    int noise = (int)AWGN() * 255;
                    int R = CurrentPixel.R + noise;//(int)Statistic.GaussianRandom(0, 1);//(int)Statistic.NextGaussian(0f, 1f, -Amount, Amount + 1);//TempRandom.Next(-Amount, Amount + 1);
                    int G = CurrentPixel.G + noise;//(int)Statistic.GaussianRandom(0, 1);//(int)Statistic.NextGaussian(0f, 1f, -Amount, Amount + 1);//TempRandom.Next(-Amount, Amount + 1);
                    int B = CurrentPixel.B + noise;//(int)Statistic.GaussianRandom(0, 1);//(int)Statistic.NextGaussian(0f, 1f, -Amount, Amount + 1);//TempRandom.Next(-Amount, Amount + 1);
                    R = R > 255 ? 255 : R;
                    R = R < 0 ? 0 : R;
                    G = G > 255 ? 255 : G;
                    G = G < 0 ? 0 : G;
                    B = B > 255 ? 255 : B;
                    B = B < 0 ? 0 : B;
                    Color TempValue = Color.FromArgb(R, G, B);
                    NewData.SetPixel(x, y, TempValue, NewPixelSize);//Image.SetPixel(NewData, x, y, TempValue, NewPixelSize);
                }
            }
            NewBitmap.UnlockImage(NewData);//Image.UnlockImage(NewBitmap, NewData);
            OriginalImage.UnlockImage(OldData);//Image.UnlockImage(OriginalImage, OldData);
            return NewBitmap;
        }

        public static double AWGN()
        {
            const double PI = 3.1415926536;
           
            double temp1 = 0;
            double temp2 = 0;
            double result;
            int p;

            System.Random r = new System.Random();
            p = 1;

            while (p > 0)
            {
                temp2 = (r.Next(0, 255) / (double)255);

                if (temp2 == 0)
                {
                    p = 1;
                }
                else
                {
                    p = -1;
                }
            }

            temp1 = Math.Cos((2.0 * (double)PI) * r.Next(0, 255) / ((double)255));
            result = Math.Sqrt(-2.0 * Math.Log(temp2)) * temp1;
            return result;
        }

        public static Bitmap AWGNNoise(Bitmap bmp, int intensity)
        {
            Bitmap img = new Bitmap(bmp.Width, bmp.Height);
            var normal = new NormalDistribution(mean: 0, stdDev: 1);
            double[] noise = normal.Generate(img.Width*img.Height);
            int l = 0;
            for (int i = 0; i < bmp.Height; i++)
            {
                for(int j = 0; j < bmp.Width; j++)
                {
                    Color c = bmp.GetPixel(j, i);
                    int c1 = c.R + ((int)Math.Ceiling(noise[l]) * intensity);//(int)noise + intensity;//(int)Math.Sqrt(noise() * 255) + intensity;
                    int c2 = c.G + ((int)Math.Ceiling(noise[l]) * intensity);//(int)Math.Sqrt(noise() * 255) + intensity;
                    int c3 = c.B + ((int)Math.Ceiling(noise[l]) * intensity);//(int)Math.Sqrt(noise() * 255) + intensity;
                    c1 = c1 > 255 ? 255 : c1;
                    c1 = c1 < 0 ? 0 : c1;
                    c2 = c2 > 255 ? 255 : c2;
                    c2 = c2 < 0 ? 0 : c2;
                    c3 = c3 > 255 ? 255 : c3;
                    c3 = c3 < 0 ? 0 : c3;
                    Color newC = Color.FromArgb(c1, c2, c3);
                    img.SetPixel(j, i, newC);
                    l++;
                }
            }
            return img;
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

        #region Sharpen
        public static Bitmap Sharpen(Bitmap bmp)
        {
            Sharpen filter = new Sharpen();
            filter.ApplyInPlace(bmp);
            return bmp;
        }
        #endregion

        #region Gaussian Sharpen
        public static Bitmap GaussianSharpen(Bitmap bmp, int value, int kernel)
        {
            GaussianSharpen filter = new GaussianSharpen(value, kernel);
            filter.ApplyInPlace(bmp);
            return bmp;
        }
        #endregion

        #region Blur
        public static Bitmap Blur(Bitmap bmp)
        {
            // create filter
            Blur filter = new Blur();
            // apply the filter
            filter.ApplyInPlace(bmp);
            return bmp;
        }
        #endregion

        #region Gaussian Blur
        public static Bitmap GaussianBlur(Bitmap bmp, int value)
        {
            // create filter with kernel size equal to 11
            // and Gaussia sigma value equal to 4.0
            GaussianBlur filter = new GaussianBlur(value, 11);
            // apply the filter
            filter.ApplyInPlace(bmp);
            return bmp;
        }
        #endregion

        #region Burkes Color Dithering
        public static Bitmap BurkesColorDithering(Bitmap bmp, int value)
        {
            // create color image quantization routine
            ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
            // create 8 colors table
            Color[] colorTable = ciq.CalculatePalette(bmp, value);
            // create dithering routine
            BurkesColorDithering dithering = new BurkesColorDithering();
            dithering.ColorTable = colorTable;
            // apply the dithering routine
            Bitmap newImage = dithering.Apply(bmp);
            return newImage;
        }
        #endregion

        #region FloydSteinberg Color Dithering
        public static Bitmap FloydSteinbergColorDithering(Bitmap bmp, int value)
        {
            // create color image quantization routine
            ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
            // create 16 colors table
            Color[] colorTable = ciq.CalculatePalette(bmp, value);
            // create dithering routine
            FloydSteinbergColorDithering dithering = new FloydSteinbergColorDithering();
            dithering.ColorTable = colorTable;
            // apply the dithering routine
            Bitmap newImage = dithering.Apply(bmp);
            return newImage;
        }
        #endregion

        #region JarvisJudiceNinke Color Dithering
        public static Bitmap JarvisJudiceNinkeColorDithering(Bitmap bmp, int value)
        {
            // create color image quantization routine
            ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
            // create 32 colors table
            Color[] colorTable = ciq.CalculatePalette(bmp, value);
            // create dithering routine
            JarvisJudiceNinkeColorDithering dithering = new JarvisJudiceNinkeColorDithering();
            dithering.ColorTable = colorTable;
            // apply the dithering routine
            Bitmap newImage = dithering.Apply(bmp);
            return newImage;
        }
        #endregion

        #region Sierra Color Dithering
        public static Bitmap SierraColorDithering(Bitmap bmp)
        {
            // create dithering routine (use default color table)
            SierraColorDithering dithering = new SierraColorDithering();
            // apply the dithering routine
            Bitmap newImage = dithering.Apply(bmp);
            return newImage;
        }
        #endregion

        #region Stucki Color Dithering
        public static Bitmap StuckiColorDithering(Bitmap bmp, int value)
        {
            // create color image quantization routine
            ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
            // create 64 colors table
            Color[] colorTable = ciq.CalculatePalette(bmp, value);
            // create dithering routine
            StuckiColorDithering dithering = new StuckiColorDithering();
            dithering.ColorTable = colorTable;
            // apply the dithering routine
            Bitmap newImage = dithering.Apply(bmp);
            return newImage;
        }
        #endregion

        #region Ordered Color Dithering
        public static Bitmap OrderedColorDithering(Bitmap bmp, int value)
        {
            // create color image quantization routine
            ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
            // create 256 colors table
            Color[] colorTable = ciq.CalculatePalette(bmp, value);
            // create dithering routine
            OrderedColorDithering dithering = new OrderedColorDithering();
            dithering.ColorTable = colorTable;
            // apply the dithering routine
            Bitmap newImage = dithering.Apply(bmp);
            return newImage;
        }
        #endregion

        #region Jitter Attack
        public static Bitmap Jitter(Bitmap bmp, int value)
        {
            // create filter
            Jitter filter = new Jitter(value);
            // apply the filter
            filter.ApplyInPlace(bmp);
            return bmp;
        }
        #endregion

        #region ResizeBilinear
        public static Bitmap ResizeBilinear(Bitmap bmp, int width, int height)
        {
            // create filter
            ResizeBilinear filter = new ResizeBilinear(width, height);
            // apply the filter
            Bitmap newImage = filter.Apply(bmp);
            return newImage;
        }
        #endregion

        #region Rotate
        public static Bitmap RotateBilinear(Bitmap bmp, int degree)
        {
            // create filter - rotate for 30 degrees keeping original image size
            RotateBilinear filter = new RotateBilinear(degree, true);
            // apply the filter
            Bitmap newImage = filter.Apply(bmp);
            return newImage;
        }

        public static Bitmap RotateBicubic(Bitmap bmp, int degree)
        {
            // create filter - rotate for 30 degrees keeping original image size
            RotateBicubic filter = new RotateBicubic(degree, true);
            // apply the filter
            Bitmap newImage = filter.Apply(bmp);
            return bmp;
        }

        public static Bitmap RotateImg(Bitmap bmp, float angle, Color bkColor)
        {
            angle = angle % 360;
            if (angle > 180)
                angle -= 360;

            System.Drawing.Imaging.PixelFormat pf = default(System.Drawing.Imaging.PixelFormat);
            if (bkColor == Color.Transparent)
            {
                pf = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            float sin = (float)Math.Abs(Math.Sin(angle * Math.PI / 180.0)); // this function takes radians
            float cos = (float)Math.Abs(Math.Cos(angle * Math.PI / 180.0)); // this one too
            float newImgWidth = sin * bmp.Height + cos * bmp.Width;
            float newImgHeight = sin * bmp.Width + cos * bmp.Height;
            float originX = 0f;
            float originY = 0f;

            if (angle > 0)
            {
                if (angle <= 90)
                    originX = sin * bmp.Height;
                else
                {
                    originX = newImgWidth;
                    originY = newImgHeight - sin * bmp.Width;
                }
            }
            else
            {
                if (angle >= -90)
                    originY = sin * bmp.Width;
                else
                {
                    originX = newImgWidth - sin * bmp.Height;
                    originY = newImgHeight;
                }
            }

            Bitmap newImg = new Bitmap((int)newImgWidth, (int)newImgHeight, pf);
            Graphics g = Graphics.FromImage(newImg);
            g.Clear(bkColor);
            g.TranslateTransform(originX, originY); // offset the origin to our calculated values
            g.RotateTransform(angle); // set up rotate
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(bmp, 0, 0); // draw the image at 0, 0
            g.Dispose();

            return newImg;
        }
        #endregion

        #region Crop Image
        public static Bitmap Crop(Bitmap bmp, int x, int y, int x2, int y2)
        {
            // create filter
            Crop filter = new Crop(new Rectangle(x, y, x2, y2));
            // apply the filter
            Bitmap newImage = filter.Apply(bmp);
            return newImage;
        }
        #endregion

        #region Brightness
        public static Bitmap SetBrightness(Bitmap bmp,int brightness)
        {
            Bitmap temp = (Bitmap)bmp;
            Bitmap bmap = (Bitmap)temp.Clone();
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    int cR = c.R + brightness;
                    int cG = c.G + brightness;
                    int cB = c.B + brightness;

                    if (cR < 0) cR = 1;
                    if (cR > 255) cR = 255;

                    if (cG < 0) cG = 1;
                    if (cG > 255) cG = 255;

                    if (cB < 0) cB = 1;
                    if (cB > 255) cB = 255;

                    bmap.SetPixel(i, j,
                    Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            //_currentBitmap = (Bitmap)bmap.Clone();
            return bmap;

            
        }
        #endregion

        #region Contrast
        public static Bitmap SetContrast(Bitmap bmp,double contrast)
        {
            Bitmap temp = (Bitmap)bmp;
            Bitmap bmap = (Bitmap)temp.Clone();
            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    double pR = c.R / 255.0;
                    pR -= 0.5;
                    pR *= contrast;
                    pR += 0.5;
                    pR *= 255;
                    if (pR < 0) pR = 0;
                    if (pR > 255) pR = 255;

                    double pG = c.G / 255.0;
                    pG -= 0.5;
                    pG *= contrast;
                    pG += 0.5;
                    pG *= 255;
                    if (pG < 0) pG = 0;
                    if (pG > 255) pG = 255;

                    double pB = c.B / 255.0;
                    pB -= 0.5;
                    pB *= contrast;
                    pB += 0.5;
                    pB *= 255;
                    if (pB < 0) pB = 0;
                    if (pB > 255) pB = 255;

                    bmap.SetPixel(i, j,
        Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                }
            }
            //_currentBitmap = (Bitmap)bmap.Clone();
            return bmap;
        }
        #endregion

        #region Gamma
        public static Bitmap SetGamma(Bitmap bmp,double red, double green, double blue)
        {
            Bitmap temp = (Bitmap)bmp;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            byte[] redGamma = CreateGammaArray(red);
            byte[] greenGamma = CreateGammaArray(green);
            byte[] blueGamma = CreateGammaArray(blue);
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    bmap.SetPixel(i, j, Color.FromArgb(redGamma[c.R],
                       greenGamma[c.G], blueGamma[c.B]));
                }
            }
            //_currentBitmap = (Bitmap)bmap.Clone();
            return bmap;
        }

        private static byte[] CreateGammaArray(double color)
        {
            byte[] gammaArray = new byte[256];
            for (int i = 0; i < 256; ++i)
            {
                gammaArray[i] = (byte)Math.Min(255,(int)((255.0 * Math.Pow(i / 255.0, 1.0 / color)) + 0.5));
            }
            return gammaArray;
        }
        #endregion

        #region Gamma2
        public static Bitmap AdjustGamma(Bitmap OriginalImage, float Value)
        {
            Bitmap NewBitmap = new Bitmap(OriginalImage.Width, OriginalImage.Height);
            BitmapData NewData = NewBitmap.LockImage();//Image.LockImage(NewBitmap);
            BitmapData OldData = OriginalImage.LockImage();//Image.LockImage(OriginalImage);
            int NewPixelSize = NewData.GetPixelSize();//Image.GetPixelSize(NewData);
            int OldPixelSize = OldData.GetPixelSize();//Image.GetPixelSize(OldData);

            int[] RedRamp = new int[256];
            int[] GreenRamp = new int[256];
            int[] BlueRamp = new int[256];
            for (int x = 0; x < 256; ++x)
            {
                RedRamp[x] = Clamp((int)((255.0 * System.Math.Pow(x / 255.0, 1.0 / Value)) + 0.5), 255, 0);
                GreenRamp[x] = Clamp((int)((255.0 * System.Math.Pow(x / 255.0, 1.0 / Value)) + 0.5), 255, 0);
                BlueRamp[x] = Clamp((int)((255.0 * System.Math.Pow(x / 255.0, 1.0 / Value)) + 0.5), 255, 0);
            }

            for (int x = 0; x < NewBitmap.Width; ++x)
            {
                for (int y = 0; y < NewBitmap.Height; ++y)
                {
                    Color Pixel = OldData.GetPixel(x, y, OldPixelSize);//Image.GetPixel(OldData, x, y, OldPixelSize);
                    int Red = RedRamp[Pixel.R];
                    int Green = GreenRamp[Pixel.G];
                    int Blue = BlueRamp[Pixel.B];
                    NewData.SetPixel(x, y, Color.FromArgb(Red, Green, Blue), NewPixelSize);//Image.SetPixel(NewData, x, y, Color.FromArgb(Red, Green, Blue), NewPixelSize);
                }
            }

            NewBitmap.UnlockImage(NewData);//Image.UnlockImage(NewBitmap, NewData);
            OriginalImage.UnlockImage(OldData);//Image.UnlockImage(OriginalImage, OldData);
            return NewBitmap;
        }

        private static int Clamp(int Value, int Max, int Min)
        {
            Value = Value > Max? Max : Value;
            Value = Value<Min? Min : Value;
            return Value;
        }
        #endregion

        #region Adaptive Smoothing
        public static Bitmap AdaptiveSmoothing(Bitmap bmp)
        {
            // create filter
            AdaptiveSmoothing filter = new AdaptiveSmoothing();
            // apply the filter
            filter.ApplyInPlace(bmp);
            return bmp;
        }
        #endregion

        #region Erosion
        public static Bitmap Erosion(Bitmap bmp)
        {
            // create filter
            Erosion filter = new Erosion();
            // apply the filter
            filter.Apply(bmp);
            return bmp;
        }
        #endregion
    }
}
