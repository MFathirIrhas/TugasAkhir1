using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Imaging;
#region
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using Accord.Statistics;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Distributions.Fitting;
using AForge.Imaging.Filters;
#endregion

namespace TugasAkhir1
{
    public partial class Form1 : Form
    {
        public Bitmap OriginalImage { get; set; }
        public Bitmap DWTImage { get; set; }

        public string file_name;
        //Global Variables
        public double[,] Wavelet_Coefficients;
        public double[,] Red_Coeffs;
        public double[,] Green_Coeffs;
        public double[,] Blue_Coeffs;

        public double[,] Watermarked_Wavelet_Coefficients;
        public double[,] RedWatermarked_Wavelet_Coefficients;
        public double[,] GreenWatermarked_Wavelet_Coefficients;
        public double[,] BlueWatermarked_Wavelet_Coefficients;

        public List<int> Scrambled_Watermark = new List<int>();
        public double[,] Embedded_Wavelet_Coefficients;
        public double[,] RedEmbedded_Wavelet_Coefficients;
        public double[,] GreenEmbedded_Wavelet_Coefficients;
        public double[,] BlueEmbedded_Wavelet_Coefficients;

        public double[,] Inversed_Wavelet_Coefficients;
        public double[,] IMatrixR;
        public double[,] IMatrixG;
        public double[,] IMatrixB;

        public Bitmap WatermarkedImage;

        //HMM Parameters
        public double[] rootpmf = new double[2];
        public double[,] transition = new double[2, 2];
        public double[,] variances = new double[2, 5];

        //HMM Detection
        double[][] detectedWatermark;
        double[][] treeOfWatermark;
        double[][] CombinedTree;
        double[] ExtractedWatermark;
        double[] RedExtractedWatermark;
        double[] GreenExtractedWatermark;
        double[] BlueExtractedWatermark;

        //Inverse mapping
        double[,] InversedMappingTriangle;
        double[,] InversedMappingCircle;
        double[,] InversedMappingSquare;

        //Merge segmented watermark for each pattern.
        List<double> ScrambledWatermarkfromTriangle;
        List<double> ScrambledWatermarkfromCircle;
        List<double> ScrambledWatermarkfromSquare;

        List<double> ScrambledWatermark;

        // Extraction Key
        string KeyFileName;


        /// Addition Mapped Watermark
        double[,] Mapped_Watermark;
        List<int> Real_Watermark;
        double[,] EMBEDDED_WATERMARK_For_Extraction;
        
        
        Stopwatch time = new Stopwatch();

        #region DRAW ON WATERMARKED IMAGE
        bool draw = false;

        int pX = -1;
        int pY = -1;

        Bitmap drawing;
        Bitmap Transformed_Image;
        #endregion


        public Form1()
        {
            InitializeComponent();
            this.hostImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.transformedImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.panel12.AutoScroll = true;
            this.panel13.AutoScroll = true;
            this.watermarkImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.extractedImageRed.SizeMode = PictureBoxSizeMode.Zoom;
            this.extractedImageGreen.SizeMode = PictureBoxSizeMode.Zoom;
            this.extractedImageBlue.SizeMode = PictureBoxSizeMode.Zoom;
            

            //GUI Initialize
            HostImageLocationTxt.Text = "Browse Image to be inserted watermark";
            HostImageLocationTxt.ForeColor = Color.LightGray;
            WatermarkImageLocationTxt.Text = "Browse Watermark Image";
            WatermarkImageLocationTxt.ForeColor = Color.LightGray;
            //WatermarkedImageTxt.Text = "Browse Watermarked Image to be Attacked";
            //WatermarkedImageTxt.ForeColor = Color.LightGray;

            ///Draw On Image
            //drawing = new Bitmap(transformedImage.Width, transformedImage.Height, transformedImage.CreateGraphics());
            //Graphics.FromImage(drawing).Clear(Color.Transparent);

        }


        public void GUIStart(string status)
        {
            StatusPanel.BackColor = Color.Gray;
            StatusTxt.BackColor = Color.Gray;
            StatusTxt.Text = status;
            StatusTxt.ForeColor = Color.Red;
            StatusTxt.BackColor = Color.Gray;
            label4.BackColor = Color.Gray;
            label5.BackColor = Color.Gray;
            TimeExecTxt.BackColor = Color.Gray;
            //PSNRlbl.BackColor = Color.Gray;
            //MSElbl.BackColor = Color.Gray;
            //BERlbl.BackColor = Color.Gray;
            //BERValue.BackColor = Color.Gray;
            //PSNRValue.BackColor = Color.Gray;
            TimeExecTxt.Text = "0";
            //BERValue.Text = "0";
            //PSNRValue.Text = "0";
            StatusTxt.Refresh();
            label4.Refresh();
            label5.Refresh();
            PSNRlbl.Refresh();
            //BERlbl.Refresh();
            PSNRValue.Refresh();
            //BERValue.Refresh();
            TimeExecTxt.Refresh();


            time = Stopwatch.StartNew();
        }

        public void GUIEnd(string status,double mse, double psnr, double ber )
        {
            time.Stop();
            var elapsedTime = time.ElapsedMilliseconds / 1000;

            StatusPanel.BackColor = Color.LightSkyBlue;
            StatusTxt.BackColor = Color.LightSkyBlue;
            StatusTxt.Text = status;
            StatusTxt.ForeColor = Color.DarkGreen;
            label4.BackColor = Color.LightSkyBlue;
            label5.BackColor = Color.LightSkyBlue;
            TimeExecTxt.BackColor = Color.LightSkyBlue;
            TimeExecTxt.Text = " " + elapsedTime.ToString() + " Second(s)";
            PSNRValue.Text = String.Format("{0:0.00}", psnr) ;//psnr.ToString();
            //BERValue.Text = String.Format("{0:0.00}", ber);//ber.ToString();
            MSEValue.Text = String.Format("{0:0.00}", mse);//mse.ToString();
            //MSElbl.BackColor = Color.LightSkyBlue;
            //PSNRlbl.BackColor = Color.LightSkyBlue;
            //BERlbl.BackColor = Color.LightSkyBlue;
            //BERValue.BackColor = Color.LightSkyBlue;
            //PSNRValue.BackColor = Color.LightSkyBlue;
        }

        //Sementara
        public void Print(string s)
        {
            MessageBox.Show("Result: " + s, "Succeed", MessageBoxButtons.OK);
        }
        private void button1_Click(object sender, EventArgs e) //Open Original Image
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Picture to Watermark";
            ofd.InitialDirectory = @"F:\College\Semester 8\TA2";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ///Check Image Dimension before processing
                //Bitmap bmp = new Bitmap(ofd.FileName);
                //int w = bmp.Width;
                //int h = bmp.Height;
                //double x1 = Math.Log(w,2);
                //double x2 = Math.Log(h,2);
                ////Cek if dimension is 2^n
                //if (x1 % 1 == 0 && x2 % 1 == 0)
                //{
                //    HostImageLocationTxt.Text = ofd.FileName;
                //    hostImage.Image = new Bitmap(ofd.FileName);
                //}
                //else
                //{
                //    MessageBox.Show("Image dimension must have 2^n x 2^m pixels, \n n and m might be same, might be not.", "Input Error", MessageBoxButtons.OK);
                //}

                HostImageLocationTxt.Text = ofd.FileName;
                hostImage.Image = new Bitmap(ofd.FileName);

                file_name = ofd.SafeFileName;
            }
        }

        public static Bitmap Create24bpp(Image image)
        {
            Bitmap b = new Bitmap(image);
            Bitmap bmp = new Bitmap(b.Width, b.Height,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics gr = Graphics.FromImage(bmp))
                gr.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height));
            return bmp;
        }

        private void button10_Click(object sender, EventArgs e) //Save Transformed/Watermarked Image
        {
            if (transformedImage.Image != null)
            {
                //Bitmap bmp = new Bitmap(transformedImage.Image);
                ///Bitmap bmp = Create24bpp(transformedImage.Image); ////Resave image using 24 bit format.
                Bitmap bmp = drawing;

                //Bitmap first = new Bitmap(transformedImage.Image);
                //Bitmap second = drawing;
                ////Bitmap result = new Bitmap(first.Width, first.Height);
                ////fix :
                //Bitmap result = new Bitmap(Math.Max(first.Width, second.Width), Math.Max(first.Height, second.Height));
                //Graphics g = Graphics.FromImage(result);
                //g.DrawImageUnscaled(first, 0, 0);
                //g.DrawImageUnscaled(second, 0, 0);
                //transformedImage.Image = result;

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Select Save Location";
                sfd.InitialDirectory = @"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Saved_Image";
                sfd.AddExtension = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    switch (Path.GetExtension(sfd.FileName).ToUpper())
                    {
                        case ".BMP":
                            bmp.Save(sfd.FileName, ImageFormat.Bmp);
                            break;
                        case ".gif":
                            bmp.Save(sfd.FileName, ImageFormat.Gif);
                            break;
                        case ".JPG":
                            bmp.Save(sfd.FileName, ImageFormat.Jpeg);
                            break;
                        case ".JPEG":
                            bmp.Save(sfd.FileName,ImageFormat.Jpeg);
                            break;
                        case ".PNG":
                            bmp.Save(sfd.FileName, ImageFormat.Png);
                            break;
                        case ".png":
                            bmp.Save(sfd.FileName, ImageFormat.Png);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e) //Open Watermark Image
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Picture as Watermark";
            ofd.InitialDirectory = @"F:\College\Semester 8\TA2";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                WatermarkImageLocationTxt.Text = ofd.FileName;
                watermarkImage.Image = new Bitmap(ofd.FileName);
            }
        }


        private void button12_Click_1(object sender, EventArgs e) //Open Watermarked Image
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Picture as Watermark";
            ofd.InitialDirectory = @"F:\College\Semester 8\TA2";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                //WatermarkedImageTxt.Text = ofd.FileName;
                transformedImage.Image = new Bitmap(ofd.FileName);
                WatermarkedImage = new Bitmap(transformedImage.Image);
                Transformed_Image = new Bitmap(transformedImage.Image);
                drawing = new Bitmap(transformedImage.Image);


                //Enable button
                //histeqBtn.Enabled = true;
                //meanFilterBtn.Enabled = true;
                //medianFilterBtn.Enabled = true;
                //modusFilterBtn.Enabled = true;
                //jpegencoderBtn.Enabled = true;
                //noiseBtn.Enabled = true;

                //histeqBtn.BackColor = Color.DeepSkyBlue;
                //meanFilterBtn.BackColor = Color.DeepSkyBlue;
                //medianFilterBtn.BackColor = Color.DeepSkyBlue;
                //modusFilterBtn.BackColor = Color.DeepSkyBlue;
                //jpegencoderBtn.BackColor = Color.DeepSkyBlue;
                //noiseBtn.BackColor = Color.DeepSkyBlue;


            }
        }


        /// <summary>
        /// Forward DWT Transform and Extract the Wavelet Coefficients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) //Forward Transform
        {
            if(dwtTypeValue.Text == "Haar")
            {
                if (hostImage.Image != null)
                {
                    GUIStart("Processing...!");

                    //int level = Convert.ToInt32(HVSValue.Text);
                    ///For Visualization
                    OriginalImage = new Bitmap(hostImage.Image);
                    transformedImage.Image = Haar.TransformDWT(true, false, 2, OriginalImage).Item4;

                    ///For Wavelet Coefficients Extraction
                    Bitmap b = new Bitmap(hostImage.Image);
                    IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
                    IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
                    IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;
                    //double[,] IMatrix = ImageProcessing.ConvertToMatrix(b);
                    //Test
                    //MessageBox.Show("Red: " + IMatrixR[0, 0].ToString() + ", Green: " + IMatrixG[0, 0].ToString() + ", Blue: " + IMatrixB[0, 0].ToString(), "Values of RGB");

                    double[,] ArrayImage = IMatrixG; //Embedding in Green 
                    //Wavelet_Coefficients = Haar.WaveletCoeff(ArrayImage, true, level);
                    Red_Coeffs = Haar.WaveletCoeff(IMatrixR, true, 2);
                    Green_Coeffs = Haar.WaveletCoeff(IMatrixG, true, 2);
                    Blue_Coeffs = Haar.WaveletCoeff(IMatrixB, true, 2);
                    resultLbl.Text = "Decomposed Host Image";
                    GUIEnd("FDWT Succeed!", 0, 0, 0);

                    /// Test 
                    //int c = 1;
                    //TextWriter tw2 = new StreamWriter("GreenPixels.txt");
                    //tw2.WriteLine("Total Watermark ");
                    //for (int i = 0; i < IMatrixG.GetLength(0); i++)
                    //{
                    //    for (int j = 0; j < IMatrixG.GetLength(1); j++)
                    //    {
                    //        tw2.Write("[" + c + "]" + IMatrixG[i, j] + " - ");
                    //        c++;
                    //    }
                    //    tw2.WriteLine();
                    //}
                    //tw2.Close();

                }
                else
                {
                    MessageBox.Show("Load Image First", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
                }
            }
            else if(dwtTypeValue.Text == "Db2")
            {
                if (hostImage.Image != null)
                {
                    GUIStart("Processing...!");

                    //int level = Convert.ToInt32(HVSValue.Text);
                    ///For Visualization
                    OriginalImage = new Bitmap(hostImage.Image);
                    transformedImage.Image = Daubechies2.TransformDWT(true, false, 2, OriginalImage).Item4;

                    ///For Wavelet Coefficients Extraction
                    Bitmap b = new Bitmap(hostImage.Image);
                    IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
                    IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
                    IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;
                    //double[,] IMatrix = ImageProcessing.ConvertToMatrix(b);
                    //Test
                    //MessageBox.Show("Red: " + IMatrixR[0, 0].ToString() + ", Green: " + IMatrixG[0, 0].ToString() + ", Blue: " + IMatrixB[0, 0].ToString(), "Values of RGB");

                    double[,] ArrayImage = IMatrixG; //Embedding in Green 
                    //Wavelet_Coefficients = Haar.WaveletCoeff(ArrayImage, true, level);
                    Red_Coeffs = Daubechies2.WaveletCoeff(IMatrixR, true, 2);
                    Green_Coeffs = Daubechies2.WaveletCoeff(IMatrixG, true, 2);
                    Blue_Coeffs = Daubechies2.WaveletCoeff(IMatrixB, true, 2);
                    resultLbl.Text = "Decomposed Host Image";
                    GUIEnd("FDWT Succeed!", 0, 0, 0);

                    /// Test 
                    //int c = 1;
                    //TextWriter tw2 = new StreamWriter("GreenPixels.txt");
                    //tw2.WriteLine("Total Watermark ");
                    //for (int i = 0; i < IMatrixG.GetLength(0); i++)
                    //{
                    //    for (int j = 0; j < IMatrixG.GetLength(1); j++)
                    //    {
                    //        tw2.Write("[" + c + "]" + IMatrixG[i, j] + " - ");
                    //        c++;
                    //    }
                    //    tw2.WriteLine();
                    //}
                    //tw2.Close();

                }
                else
                {
                    MessageBox.Show("Load Image First", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
                }
            }
            else
            {
                if (hostImage.Image != null)
                {
                    GUIStart("Processing...!");

                    //int level = Convert.ToInt32(HVSValue.Text);
                    ///For Visualization
                    OriginalImage = new Bitmap(hostImage.Image);
                    transformedImage.Image = Daubechies3.TransformDWT(true, false, 2, OriginalImage).Item4;

                    ///For Wavelet Coefficients Extraction
                    Bitmap b = new Bitmap(hostImage.Image);
                    IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
                    IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
                    IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;
                    //double[,] IMatrix = ImageProcessing.ConvertToMatrix(b);
                    //Test
                    //MessageBox.Show("Red: " + IMatrixR[0, 0].ToString() + ", Green: " + IMatrixG[0, 0].ToString() + ", Blue: " + IMatrixB[0, 0].ToString(), "Values of RGB");

                    double[,] ArrayImage = IMatrixG; //Embedding in Green 
                    //Wavelet_Coefficients = Haar.WaveletCoeff(ArrayImage, true, level);
                    Red_Coeffs = Daubechies3.WaveletCoeff(IMatrixR, true, 2);
                    Green_Coeffs = Daubechies3.WaveletCoeff(IMatrixG, true, 2);
                    Blue_Coeffs = Daubechies3.WaveletCoeff(IMatrixB, true, 2);
                    resultLbl.Text = "Decomposed Host Image";
                    GUIEnd("FDWT Succeed!", 0, 0, 0);

                    /// Test 
                    //int c = 1;
                    //TextWriter tw2 = new StreamWriter("GreenPixels.txt");
                    //tw2.WriteLine("Total Watermark ");
                    //for (int i = 0; i < IMatrixG.GetLength(0); i++)
                    //{
                    //    for (int j = 0; j < IMatrixG.GetLength(1); j++)
                    //    {
                    //        tw2.Write("[" + c + "]" + IMatrixG[i, j] + " - ");
                    //        c++;
                    //    }
                    //    tw2.WriteLine();
                    //}
                    //tw2.Close();

                }
                else
                {
                    MessageBox.Show("Load Image First", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
                }
            }
            
        }

        

        

        /// <summary>
        /// Scramble the watermark bits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e/*,double[,] coeffs*/) //Generate bit sequence contain 0 and 1
        {
            if (watermarkImage.Image == null)
            {
                MessageBox.Show("Load Watermark Image First!", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }
            else
            {
                if(seedValue.Text.Length > 5 || seedValue.Text == null)
                {
                    MessageBox.Show("Maximum length of Key is 5 or cannot be empty");
                }else
                {
                    GUIStart("Processing......!");
                    //double[,] coefficients = coeffs;
                    Bitmap bmp = new Bitmap(watermarkImage.Image);
                    Scramble m = new Scramble();
                    Bitmap b = ImageProcessing.ConvertToBinary(bmp);
                    List<int> VectorImage = Scramble.ConvertToVectorMatrix(b); //Include integer values between 255 or 0
                    List<int> BinaryVectorImage = Scramble.ConvertToBinaryVectorMatrix(VectorImage); //Include integer values between 1 or 0
                    Real_Watermark = BinaryVectorImage;

                    int pnlength = BinaryVectorImage.Count * 5;
                    string pn_seed = seedValue.Text;//"10000";
                    string pn_mask = "10100";
                    int pn_length = pnlength;
                    List<int> PNSeq = Scramble.PNSeqLFSR(pn_seed, pn_mask, pn_length);
                    List<int> scrambled_Watermark = Scramble.DSSS(BinaryVectorImage, PNSeq);

                    /// Save into .txt file (PN Sequence and size)
                    Bitmap hostbmp = new Bitmap(hostImage.Image);
                    string hostfilename = HostImageLocationTxt.Text;
                    string watermarkfilename = WatermarkImageLocationTxt.Text;
                    string hostName = Path.GetFileNameWithoutExtension(hostfilename);
                    string watermarkName = Path.GetFileNameWithoutExtension(watermarkfilename);
                    string name = hostName + "_" + watermarkName + "_Key.txt";
                    int NumOfTrees = pnlength / 5; // Restult in total of tree from segmented watermark , i.e 6480
                                                   //TextWriter tw = new StreamWriter(name);
                    using (TextWriter tw = File.CreateText(@"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Key\" + name))
                    {
                        tw.WriteLine("Size of Host Image:");
                        tw.WriteLine(hostbmp.Height);
                        tw.WriteLine(hostbmp.Width);
                        tw.WriteLine("Size of Watermark: ");
                        tw.WriteLine(bmp.Height);
                        tw.WriteLine(bmp.Width);
                        tw.WriteLine("Number of Trees: ");
                        tw.WriteLine(NumOfTrees);
                        tw.WriteLine("PN Sequence: ");
                        foreach (int i in PNSeq)
                        {
                            tw.WriteLine(i);
                        }

                        tw.Close();
                    }

                    //List<int> oneminusone = Scramble.ConvertTo1minus1(scrambled_Watermark);
                    Scrambled_Watermark = scrambled_Watermark;
                    //Scrambled_Watermark = oneminusone;
                    GUIEnd("Scramble Succeed!", 0, 0, 0);
                    //MessageBox.Show("Watermark is Succeed. File Was Saved \n Original Watermark: " + BinaryVectorImage.Count + "\n Scrambled Watermark: " + scrambled_Watermark.Count + "\n PN Sequence: " + PNSeq.Count, "Succeed", MessageBoxButtons.OK);

                    //TextWriter tw1 = new StreamWriter("REAL_WATERMARK.txt");
                    //tw1.WriteLine("Total Real Watermark: " + BinaryVectorImage.Count);
                    //foreach (int i in BinaryVectorImage)
                    //    tw1.WriteLine(i);
                    //tw1.Close();

                    //TextWriter tw2 = new StreamWriter("SCRAMBLED_WATERMARK.txt");
                    //tw2.WriteLine("Total Scrambled Watermark: " + Scrambled_Watermark.Count);
                    //foreach (int i in Scrambled_Watermark)
                    //    tw2.WriteLine(i);
                    //tw2.Close();

                    //TextWriter tw3 = new StreamWriter("PN_Sequence.txt");
                    //tw3.WriteLine("Total PN Sequence Watermark: " + PNSeq.Count);
                    //foreach (int i in PNSeq)
                    //    tw3.WriteLine(i);
                    //tw3.Close();
                }
            }        
        }

        public static Bitmap Create8bpp(Image image)
        {
            Bitmap b = new Bitmap(image);
            Bitmap bmp = ImageProcessing.CopyToBpp(b, 8);
            return bmp;          
        }
        #region Save Extracted Watermark
        private void button6_Click(object sender, EventArgs e) //Save Watermark
        {
            if (extractedImageRed.Image != null)
            {
                Bitmap bmp = new Bitmap(extractedImageRed.Image);
                //Bitmap bmp = Create8bpp(watermarkImage.Image);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Select Save Location";
                sfd.InitialDirectory = @"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Saved_Image";
                sfd.AddExtension = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    switch (Path.GetExtension(sfd.FileName).ToUpper())
                    {
                        case ".BMP":
                            bmp.Save(sfd.FileName, ImageFormat.Bmp);
                            break;
                        case ".gif":
                            bmp.Save(sfd.FileName, ImageFormat.Gif);
                            break;
                        case ".JPG":
                            bmp.Save(sfd.FileName, ImageFormat.Jpeg);
                            break;
                        case ".JPEG":
                            bmp.Save(sfd.FileName, ImageFormat.Jpeg);
                            break;
                        case ".PNG":
                            bmp.Save(sfd.FileName, ImageFormat.Png);
                            break;
                        case ".png":
                            bmp.Save(sfd.FileName, ImageFormat.Png);
                            break;
                        default:
                            break;
                    }
                }
            }else
            {
                MessageBox.Show("No Watermark Detected!", "Error", MessageBoxButtons.OK);
            }
        }
        #endregion

        private void button8_Click(object sender, EventArgs e) /// Do Embedding Process at once
        {
            #region FDWT
            GUIStart("Processing...!");
            ///For Visualization
            OriginalImage = new Bitmap(hostImage.Image);
            transformedImage.Image = Haar.TransformDWT(true, false, 2, OriginalImage).Item4;

            ///For Wavelet Coefficients Extraction
            Bitmap b = new Bitmap(hostImage.Image);
            IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
            IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
            IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;
            //double[,] IMatrix = ImageProcessing.ConvertToMatrix(b);
            //Test
            //MessageBox.Show("Red: " + IMatrixR[0, 0].ToString() + ", Green: " + IMatrixG[0, 0].ToString() + ", Blue: " + IMatrixB[0, 0].ToString(), "Values of RGB");

            double[,] ArrayImage = IMatrixG; //Embedding in Green 
            Wavelet_Coefficients = Haar.WaveletCoeff(ArrayImage, true, 2);
            #endregion

            #region Scrambling
            Bitmap bmp = new Bitmap(watermarkImage.Image);
            Scramble m = new Scramble();
            Bitmap b2 = ImageProcessing.ConvertToBinary(bmp);
            List<int> VectorImage = Scramble.ConvertToVectorMatrix(b2); //Include integer values between 255 or 0
            List<int> BinaryVectorImage = Scramble.ConvertToBinaryVectorMatrix(VectorImage); //Include integer values between 1 or 0
            Real_Watermark = BinaryVectorImage;

            int pnlength = BinaryVectorImage.Count * 5;
            string pn_seed = "10000";
            string pn_mask = "10100";
            int pn_length = pnlength;
            List<int> PNSeq = Scramble.PNSeqLFSR(pn_seed, pn_mask, pn_length);
            List<int> scrambled_Watermark = Scramble.DSSS(BinaryVectorImage, PNSeq);

            /// Save into .txt file (PN Sequence and size)
            Bitmap hostbmp = new Bitmap(hostImage.Image);
            string hostfilename = HostImageLocationTxt.Text;
            string watermarkfilename = WatermarkImageLocationTxt.Text;
            string hostName = Path.GetFileNameWithoutExtension(hostfilename);
            string watermarkName = Path.GetFileNameWithoutExtension(watermarkfilename);
            string name = hostName + "_" + watermarkName + "_Key.txt";
            int NumOfTrees = pnlength / 5; // Restult in total of tree from segmented watermark , i.e 6480
                                           //TextWriter tw = new StreamWriter(name);
            using (TextWriter tw = File.CreateText(@"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Key\" + name))
            {
                tw.WriteLine("Size of Host Image:");
                tw.WriteLine(hostbmp.Height);
                tw.WriteLine(hostbmp.Width);
                tw.WriteLine("Size of Watermark: ");
                tw.WriteLine(bmp.Height);
                tw.WriteLine(bmp.Width);
                tw.WriteLine("Number of Trees: ");
                tw.WriteLine(NumOfTrees);
                tw.WriteLine("PN Sequence: ");
                foreach (int i in PNSeq)
                {
                    tw.WriteLine(i);
                }

                tw.Close();
            }

            //List<int> oneminusone = Scramble.ConvertTo1minus1(scrambled_Watermark);
            Scrambled_Watermark = scrambled_Watermark;
            #endregion

            #region embed
            List<List<int>> Segmented = Scramble.Segment(Scrambled_Watermark);
            //double[,] MappedWatermark = Scramble.Mapping(Segmented); // Use mapping into 15 bit each 5 segment of scrambled watermark
            double[,] MappedWatermark = Scramble.Mapping2(Segmented);  // Don't Use mapping 
            Mapped_Watermark = MappedWatermark;

            double[,] HVSValues = new double[Wavelet_Coefficients.GetLength(0), Wavelet_Coefficients.GetLength(1)];
            if (transformedImage.Image == null)
            {
                MessageBox.Show("Do Forward Transform First!", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }
            else
            {
                Bitmap EdgyImage = ImageProcessing.LaplaceEdge(new Bitmap(transformedImage.Image));
                HVSValues = ImageProcessing.HVS(EdgyImage);
            }

            double[,] AdaptiveHVS = new double[Wavelet_Coefficients.GetLength(0), Wavelet_Coefficients.GetLength(1)];
            double[,] pixels = ImageProcessing.ConvertToMatrix2(new Bitmap(transformedImage.Image)).Item2;
            Bitmap EdgeImage = ImageProcessing.LaplaceEdge(new Bitmap(transformedImage.Image));
            double[,] pixels2 = ImageProcessing.ConvertToMatrix2(EdgeImage).Item2;
            int NumOfTree = Scrambled_Watermark.Count / 5;
            AdaptiveHVS = Embed.AdaptiveHVS(Wavelet_Coefficients, pixels, pixels2, NumOfTree);

            //double[,] AdaptiveHVS2 = new double[Wavelet_Coefficients.GetLength(0), Wavelet_Coefficients.GetLength(1)];
            //int NumOfTree = (Scrambled_Watermark.Count / 5);
            //AdaptiveHVS2 = Embed.AdaptiveHVS2(Wavelet_Coefficients,NumOfTree);

            string subband = subbandValue.Text;
            double embed_constant = Convert.ToDouble(embedConstantValue.Text);
            double[,] EmbeddedWatermark = Embed.Embedding(Wavelet_Coefficients, MappedWatermark, AdaptiveHVS, subband, embed_constant);
            Embedded_Wavelet_Coefficients = EmbeddedWatermark;
            #endregion

            #region IDWT
            double[,] InverseDWT = Haar.WaveletCoeff(Embedded_Wavelet_Coefficients, false, 2);

            /// Round all elements in InverseDWT
            //double[,] RoundedInversedDWT = Statistic.RoundAll(InverseDWT);

            //transformedImage.Image = ImageProcessing.ConvertToBitmap(InverseDWT);
            transformedImage.Image = ImageProcessing.ConvertToBitmap2(IMatrixR, InverseDWT, IMatrixB);
            double mse = Statistic.MSE(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
            double psnr = Statistic.PSNR(new Bitmap(transformedImage.Image), mse);
            double ber = Statistic.BER(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
            GUIEnd("IDWT Succeed!", mse, psnr, ber);

            /// Test
            //double[,] WC = DWT.WaveletCoeff(InverseDWT, true, 2);
            /// Test
            //int h = 1;
            //TextWriter tw1 = new StreamWriter("InversedDWT.txt");
            //tw1.WriteLine("Total Watermark: " + InverseDWT.GetLength(0));
            //for (int i = 0; i < InverseDWT.GetLength(0); i++)
            //{
            //    for (int j = 0; j < InverseDWT.GetLength(1); j++)
            //    {
            //        tw1.Write("[" + h + "]" + InverseDWT[i, j] + " # ");
            //        h++;
            //    }
            //    tw1.WriteLine();
            //}
            ////foreach (double i in ExtractedWatermark)
            ////{
            ////    tw1.WriteLine(i);
            ////}
            //tw1.Close();

            ///Activate Attack Button
            //histeqBtn.Enabled = true;
            //histeqBtn.BackColor = Color.DeepSkyBlue;
            //meanFilterBtn.Enabled = true;
            //meanFilterBtn.BackColor = Color.DeepSkyBlue;
            //medianFilterBtn.Enabled = true;
            //medianFilterBtn.BackColor = Color.DeepSkyBlue;
            //modusFilterBtn.Enabled = true;
            //modusFilterBtn.BackColor = Color.DeepSkyBlue;
            //jpegencoderBtn.Enabled = true;
            //jpegencoderBtn.BackColor = Color.DeepSkyBlue;

            resultLbl.Text = "Watermarked Host Image";
            WatermarkedImage = new Bitmap(transformedImage.Image);
            Transformed_Image = new Bitmap(transformedImage.Image);


            double psnrvalue = Math.Round(psnr, 2);
            //double bervalue = Math.Round(ber, 2);
            double msevalue = Math.Round(mse, 2);
            psnrtxt.Text += ">" + file_name + "\n" + "PSNR:" + psnrvalue +"dB"+ "\n" + "\n" + "Key is saved!" + "\n" + "-----------" + "\n";

            GUIEnd("IDWT Succeed!", mse, psnr, ber);
            #endregion
        }

        private void button18_Click(object sender, EventArgs e) /// Extract Watermark At Once
        {
            if(dwtTypeValue2.Text == "Haar")
            {
                GUIStart("Extracting Watermark...");

                #region Training HMM and Detection
                /// Detection            
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a key accordingly";
                ofd.InitialDirectory = @"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Key";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    StreamReader objstream = new StreamReader(ofd.FileName);
                    string[] lines = objstream.ReadToEnd().Split(new char[] { '\n' });
                    int hostheight = Convert.ToInt32(lines[1]);
                    int hostwidth = Convert.ToInt32(lines[2]);
                    int NumOfTrees = Convert.ToInt32(lines[7]);
                    KeyFileName = ofd.FileName;

                    /// Get PNSeq
                    List<int> PNSeq = new List<int>();
                    for (int i = 9; i < lines.Length - 1; i++)
                    {
                        PNSeq.Add(Convert.ToInt32(lines[i]));
                    }

                    GUIStart("Training HMM Model...");
                    //transformedImage.Image = DWT.TransformDWT(true, false, 2, OriginalImage);

                    ///For Wavelet Coefficients Extraction
                    Bitmap b = new Bitmap(transformedImage.Image);
                    IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
                    IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
                    IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;

                    double[,] ArrayImage = IMatrixG; //Embedding in Green 
                                                     //Watermarked_Wavelet_Coefficients = Haar.WaveletCoeff(ArrayImage, true, 2);
                    RedWatermarked_Wavelet_Coefficients = Haar.WaveletCoeff(IMatrixR, true, 2);
                    GreenWatermarked_Wavelet_Coefficients = Haar.WaveletCoeff(IMatrixG, true, 2);
                    BlueWatermarked_Wavelet_Coefficients = Haar.WaveletCoeff(IMatrixB, true, 2);

                    int NumOfScale2 = ((hostheight * hostwidth) / 16) * 3;


                    Image decomposed = Haar.TransformDWT(true, false, 2, new Bitmap(transformedImage.Image)).Item4;
                    //RedExtractedWatermark = Extract.BaumWelchDetectionRGB(RedWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq , "red"/*, rootpmf, transition, variances*/);
                    //GreenExtractedWatermark = Extract.BaumWelchDetectionRGB(GreenWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "green" /*, rootpmf, transition, variances*/);
                    //BlueExtractedWatermark = Extract.BaumWelchDetectionRGB(BlueWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "blue" /*, rootpmf, transition, variances*/);
                    string subband = subbandValue2.Text;
                    double embed_constant = Convert.ToDouble(embedConstantValue2.Text);
                    if (HVSValue.Text == "Xie Model")
                    {
                        RedExtractedWatermark = Extract.BaumWelchDetectionInLH_2(RedWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Red", subband, embed_constant);
                        GreenExtractedWatermark = Extract.BaumWelchDetectionInLH_2(GreenWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "green", subband, embed_constant);
                        BlueExtractedWatermark = Extract.BaumWelchDetectionInLH_2(BlueWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Blue", subband, embed_constant);
                    }
                    else
                    {
                        RedExtractedWatermark = Extract.BaumWelchDetectionInLH_22(RedWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Red", subband, embed_constant);
                        GreenExtractedWatermark = Extract.BaumWelchDetectionInLH_22(GreenWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "green", subband, embed_constant);
                        BlueExtractedWatermark = Extract.BaumWelchDetectionInLH_22(BlueWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Blue", subband, embed_constant);
                    }

                }
                else
                {
                    MessageBox.Show("Select Key accordingly first!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
                }
                #endregion

                #region Extraction
                if (KeyFileName != null)
                {
                    GUIStart("Extracting Watermark...");

                    List<int> PNSeq = new List<int>();

                    StreamReader objstream = new StreamReader(KeyFileName);
                    string[] lines = objstream.ReadToEnd().Split(new char[] { '\n' });
                    int height = Convert.ToInt32(lines[4]);
                    int width = Convert.ToInt32(lines[5]);
                    int NumOfTrees = Convert.ToInt32(lines[7]);
                    for (int i = 9; i < lines.Length - 1; i++)
                    {
                        PNSeq.Add(Convert.ToInt32(lines[i]));
                    }

                    #region Extracting Image in Red
                    ///Not using 15 bit mapping
                    Bitmap redbmp = ImageProcessing.ConvertListToWatermark2(RedExtractedWatermark, height, width);
                    //watermarkImage.Image = bmp;
                    //Bitmap bmp = new Bitmap(watermarkImage.Image);               
                    ConservativeSmoothing filter = new ConservativeSmoothing();
                    filter.ApplyInPlace(redbmp);
                    extractedImageRed.Image = redbmp;
                    #endregion

                    #region Extraction Image in Green
                    ///Not using 15 bit mapping
                    Bitmap greenbmp = ImageProcessing.ConvertListToWatermark2(GreenExtractedWatermark, height, width);
                    //watermarkImage.Image = bmp;
                    //Bitmap bmp = new Bitmap(watermarkImage.Image);               
                    ConservativeSmoothing filter2 = new ConservativeSmoothing();
                    filter2.ApplyInPlace(greenbmp);
                    extractedImageGreen.Image = greenbmp;
                    #endregion

                    #region Extraction Image in Blue
                    ///Not using 15 bit mapping
                    Bitmap bluebmp = ImageProcessing.ConvertListToWatermark2(BlueExtractedWatermark, height, width);
                    //watermarkImage.Image = bmp;
                    //Bitmap bmp = new Bitmap(watermarkImage.Image);               
                    ConservativeSmoothing filter3 = new ConservativeSmoothing();
                    filter3.ApplyInPlace(greenbmp);
                    extractedImageBlue.Image = bluebmp;
                    #endregion

                    if (watermarkImage.Image != null)
                    {
                        #region Red BER Calculation
                        /// Test
                        int counter = 0;
                        for (int i = 0; i < RedExtractedWatermark.Length; i++)
                        {
                            if (RedExtractedWatermark[i] == Real_Watermark[i])
                            {
                                counter++;
                            }
                        }
                        double akurasi = ((double)counter / (double)RedExtractedWatermark.Length) * 100;
                        double BER = 100 - akurasi;
                        bertxt.Text += "> " + Math.Round(BER, 2) + " %" + "\n";
                        //double BER = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImageGreen.Image));
                        RedextractedBERtxt.Text = Math.Round(BER, 2).ToString();
                        //MessageBox.Show("Akurasi: " + BER, "Succeed!", MessageBoxButtons.OK);
                        #endregion

                        #region Green BER Calculation
                        /// Test
                        int counter2 = 0;
                        for (int i = 0; i < GreenExtractedWatermark.Length; i++)
                        {
                            if (GreenExtractedWatermark[i] == Real_Watermark[i])
                            {
                                counter2++;
                            }
                        }
                        double akurasi2 = ((double)counter2 / (double)GreenExtractedWatermark.Length) * 100;
                        double BER2 = 100 - akurasi2;
                        bertxt.Text += "> " + Math.Round(BER2, 2) + " %" + "\n";
                        //double BER = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImageGreen.Image));
                        GreenextractedBERtxt.Text = Math.Round(BER2, 2).ToString();
                        //MessageBox.Show("Akurasi: " + BER, "Succeed!", MessageBoxButtons.OK);
                        #endregion

                        #region Blue BER Calculation
                        /// Test
                        int counter3 = 0;
                        for (int i = 0; i < BlueExtractedWatermark.Length; i++)
                        {
                            if (BlueExtractedWatermark[i] == Real_Watermark[i])
                            {
                                counter3++;
                            }
                        }
                        double akurasi3 = ((double)counter3 / (double)BlueExtractedWatermark.Length) * 100;
                        double BER3 = 100 - akurasi3;
                        bertxt.Text += "> " + Math.Round(BER3, 2) + " %" + "\n";
                        //double BER = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImageGreen.Image));
                        BlueextractedBERtxt.Text = Math.Round(BER3, 2).ToString();
                        //MessageBox.Show("Akurasi: " + BER, "Succeed!", MessageBoxButtons.OK);
                        #endregion

                    }

                    GUIEnd("Watermark Extracted!", 0, 0, 0);
                }
                else
                {
                    MessageBox.Show("Train and Detect Watermark first!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
                }
                #endregion

                GUIEnd("Watermark Extracted!", 0, 0, 0);
            }
            else if(dwtTypeValue2.Text == "Db2")
            {
                GUIStart("Extracting Watermark...");

                #region Training HMM and Detection
                /// Detection            
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a key accordingly";
                ofd.InitialDirectory = @"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Key";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    StreamReader objstream = new StreamReader(ofd.FileName);
                    string[] lines = objstream.ReadToEnd().Split(new char[] { '\n' });
                    int hostheight = Convert.ToInt32(lines[1]);
                    int hostwidth = Convert.ToInt32(lines[2]);
                    int NumOfTrees = Convert.ToInt32(lines[7]);
                    KeyFileName = ofd.FileName;

                    /// Get PNSeq
                    List<int> PNSeq = new List<int>();
                    for (int i = 9; i < lines.Length - 1; i++)
                    {
                        PNSeq.Add(Convert.ToInt32(lines[i]));
                    }

                    GUIStart("Training HMM Model...");
                    //transformedImage.Image = DWT.TransformDWT(true, false, 2, OriginalImage);

                    ///For Wavelet Coefficients Extraction
                    Bitmap b = new Bitmap(transformedImage.Image);
                    IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
                    IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
                    IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;

                    double[,] ArrayImage = IMatrixG; //Embedding in Green 
                                                     //Watermarked_Wavelet_Coefficients = Haar.WaveletCoeff(ArrayImage, true, 2);
                    RedWatermarked_Wavelet_Coefficients = Daubechies2.WaveletCoeff(IMatrixR, true, 2);
                    GreenWatermarked_Wavelet_Coefficients = Daubechies2.WaveletCoeff(IMatrixG, true, 2);
                    BlueWatermarked_Wavelet_Coefficients = Daubechies2.WaveletCoeff(IMatrixB, true, 2);

                    int NumOfScale2 = ((hostheight * hostwidth) / 16) * 3;


                    Image decomposed = Haar.TransformDWT(true, false, 2, new Bitmap(transformedImage.Image)).Item4;
                    //RedExtractedWatermark = Extract.BaumWelchDetectionRGB(RedWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq , "red"/*, rootpmf, transition, variances*/);
                    //GreenExtractedWatermark = Extract.BaumWelchDetectionRGB(GreenWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "green" /*, rootpmf, transition, variances*/);
                    //BlueExtractedWatermark = Extract.BaumWelchDetectionRGB(BlueWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "blue" /*, rootpmf, transition, variances*/);
                    string subband = subbandValue2.Text;
                    double embed_constant = Convert.ToDouble(embedConstantValue2.Text);
                    if (HVSValue.Text == "Xie Model")
                    {
                        RedExtractedWatermark = Extract.BaumWelchDetectionInLH_2(RedWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Red", subband, embed_constant);
                        GreenExtractedWatermark = Extract.BaumWelchDetectionInLH_2(GreenWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "green", subband, embed_constant);
                        BlueExtractedWatermark = Extract.BaumWelchDetectionInLH_2(BlueWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Blue", subband, embed_constant);
                    }
                    else
                    {
                        RedExtractedWatermark = Extract.BaumWelchDetectionInLH_22(RedWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Red", subband, embed_constant);
                        GreenExtractedWatermark = Extract.BaumWelchDetectionInLH_22(GreenWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "green", subband, embed_constant);
                        BlueExtractedWatermark = Extract.BaumWelchDetectionInLH_22(BlueWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Blue", subband, embed_constant);
                    }

                }
                else
                {
                    MessageBox.Show("Select Key accordingly first!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
                }
                #endregion

                #region Extraction
                if (KeyFileName != null)
                {
                    GUIStart("Extracting Watermark...");

                    List<int> PNSeq = new List<int>();

                    StreamReader objstream = new StreamReader(KeyFileName);
                    string[] lines = objstream.ReadToEnd().Split(new char[] { '\n' });
                    int height = Convert.ToInt32(lines[4]);
                    int width = Convert.ToInt32(lines[5]);
                    int NumOfTrees = Convert.ToInt32(lines[7]);
                    for (int i = 9; i < lines.Length - 1; i++)
                    {
                        PNSeq.Add(Convert.ToInt32(lines[i]));
                    }

                    #region Extracting Image in Red
                    ///Not using 15 bit mapping
                    Bitmap redbmp = ImageProcessing.ConvertListToWatermark2(RedExtractedWatermark, height, width);
                    //watermarkImage.Image = bmp;
                    //Bitmap bmp = new Bitmap(watermarkImage.Image);               
                    ConservativeSmoothing filter = new ConservativeSmoothing();
                    filter.ApplyInPlace(redbmp);
                    extractedImageRed.Image = redbmp;
                    #endregion

                    #region Extraction Image in Green
                    ///Not using 15 bit mapping
                    Bitmap greenbmp = ImageProcessing.ConvertListToWatermark2(GreenExtractedWatermark, height, width);
                    //watermarkImage.Image = bmp;
                    //Bitmap bmp = new Bitmap(watermarkImage.Image);               
                    ConservativeSmoothing filter2 = new ConservativeSmoothing();
                    filter2.ApplyInPlace(greenbmp);
                    extractedImageGreen.Image = greenbmp;
                    #endregion

                    #region Extraction Image in Blue
                    ///Not using 15 bit mapping
                    Bitmap bluebmp = ImageProcessing.ConvertListToWatermark2(BlueExtractedWatermark, height, width);
                    //watermarkImage.Image = bmp;
                    //Bitmap bmp = new Bitmap(watermarkImage.Image);               
                    ConservativeSmoothing filter3 = new ConservativeSmoothing();
                    filter3.ApplyInPlace(greenbmp);
                    extractedImageBlue.Image = bluebmp;
                    #endregion

                    if (watermarkImage.Image != null)
                    {
                        #region Red BER Calculation
                        /// Test
                        int counter = 0;
                        for (int i = 0; i < RedExtractedWatermark.Length; i++)
                        {
                            if (RedExtractedWatermark[i] == Real_Watermark[i])
                            {
                                counter++;
                            }
                        }
                        double akurasi = ((double)counter / (double)RedExtractedWatermark.Length) * 100;
                        double BER = 100 - akurasi;
                        bertxt.Text += "> " + Math.Round(BER, 2) + " %" + "\n";
                        //double BER = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImageGreen.Image));
                        RedextractedBERtxt.Text = Math.Round(BER, 2).ToString();
                        //MessageBox.Show("Akurasi: " + BER, "Succeed!", MessageBoxButtons.OK);
                        #endregion

                        #region Green BER Calculation
                        /// Test
                        int counter2 = 0;
                        for (int i = 0; i < GreenExtractedWatermark.Length; i++)
                        {
                            if (GreenExtractedWatermark[i] == Real_Watermark[i])
                            {
                                counter2++;
                            }
                        }
                        double akurasi2 = ((double)counter2 / (double)GreenExtractedWatermark.Length) * 100;
                        double BER2 = 100 - akurasi2;
                        bertxt.Text += "> " + Math.Round(BER2, 2) + " %" + "\n";
                        //double BER = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImageGreen.Image));
                        GreenextractedBERtxt.Text = Math.Round(BER2, 2).ToString();
                        //MessageBox.Show("Akurasi: " + BER, "Succeed!", MessageBoxButtons.OK);
                        #endregion

                        #region Blue BER Calculation
                        /// Test
                        int counter3 = 0;
                        for (int i = 0; i < BlueExtractedWatermark.Length; i++)
                        {
                            if (BlueExtractedWatermark[i] == Real_Watermark[i])
                            {
                                counter3++;
                            }
                        }
                        double akurasi3 = ((double)counter3 / (double)BlueExtractedWatermark.Length) * 100;
                        double BER3 = 100 - akurasi3;
                        bertxt.Text += "> " + Math.Round(BER3, 2) + " %" + "\n";
                        //double BER = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImageGreen.Image));
                        BlueextractedBERtxt.Text = Math.Round(BER3, 2).ToString();
                        //MessageBox.Show("Akurasi: " + BER, "Succeed!", MessageBoxButtons.OK);
                        #endregion

                    }

                    GUIEnd("Watermark Extracted!", 0, 0, 0);
                }
                else
                {
                    MessageBox.Show("Train and Detect Watermark first!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
                }
                #endregion

                GUIEnd("Watermark Extracted!", 0, 0, 0);
            }
            else
            {
                GUIStart("Extracting Watermark...");

                #region Training HMM and Detection
                /// Detection            
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a key accordingly";
                ofd.InitialDirectory = @"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Key";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    StreamReader objstream = new StreamReader(ofd.FileName);
                    string[] lines = objstream.ReadToEnd().Split(new char[] { '\n' });
                    int hostheight = Convert.ToInt32(lines[1]);
                    int hostwidth = Convert.ToInt32(lines[2]);
                    int NumOfTrees = Convert.ToInt32(lines[7]);
                    KeyFileName = ofd.FileName;

                    /// Get PNSeq
                    List<int> PNSeq = new List<int>();
                    for (int i = 9; i < lines.Length - 1; i++)
                    {
                        PNSeq.Add(Convert.ToInt32(lines[i]));
                    }

                    GUIStart("Training HMM Model...");
                    //transformedImage.Image = DWT.TransformDWT(true, false, 2, OriginalImage);

                    ///For Wavelet Coefficients Extraction
                    Bitmap b = new Bitmap(transformedImage.Image);
                    IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
                    IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
                    IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;

                    double[,] ArrayImage = IMatrixG; //Embedding in Green 
                                                     //Watermarked_Wavelet_Coefficients = Haar.WaveletCoeff(ArrayImage, true, 2);
                    RedWatermarked_Wavelet_Coefficients = Daubechies3.WaveletCoeff(IMatrixR, true, 2);
                    GreenWatermarked_Wavelet_Coefficients = Daubechies3.WaveletCoeff(IMatrixG, true, 2);
                    BlueWatermarked_Wavelet_Coefficients = Daubechies3.WaveletCoeff(IMatrixB, true, 2);

                    int NumOfScale2 = ((hostheight * hostwidth) / 16) * 3;


                    Image decomposed = Haar.TransformDWT(true, false, 2, new Bitmap(transformedImage.Image)).Item4;
                    //RedExtractedWatermark = Extract.BaumWelchDetectionRGB(RedWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq , "red"/*, rootpmf, transition, variances*/);
                    //GreenExtractedWatermark = Extract.BaumWelchDetectionRGB(GreenWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "green" /*, rootpmf, transition, variances*/);
                    //BlueExtractedWatermark = Extract.BaumWelchDetectionRGB(BlueWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "blue" /*, rootpmf, transition, variances*/);
                    string subband = subbandValue2.Text;
                    double embed_constant = Convert.ToDouble(embedConstantValue2.Text);
                    if (HVSValue.Text == "Xie Model")
                    {
                        RedExtractedWatermark = Extract.BaumWelchDetectionInLH_2(RedWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Red", subband, embed_constant);
                        GreenExtractedWatermark = Extract.BaumWelchDetectionInLH_2(GreenWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "green", subband, embed_constant);
                        BlueExtractedWatermark = Extract.BaumWelchDetectionInLH_2(BlueWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Blue", subband, embed_constant);
                    }
                    else
                    {
                        RedExtractedWatermark = Extract.BaumWelchDetectionInLH_22(RedWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Red", subband, embed_constant);
                        GreenExtractedWatermark = Extract.BaumWelchDetectionInLH_22(GreenWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "green", subband, embed_constant);
                        BlueExtractedWatermark = Extract.BaumWelchDetectionInLH_22(BlueWatermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq, "Blue", subband, embed_constant);
                    }

                }
                else
                {
                    MessageBox.Show("Select Key accordingly first!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
                }
                #endregion

                #region Extraction
                if (KeyFileName != null)
                {
                    GUIStart("Extracting Watermark...");

                    List<int> PNSeq = new List<int>();

                    StreamReader objstream = new StreamReader(KeyFileName);
                    string[] lines = objstream.ReadToEnd().Split(new char[] { '\n' });
                    int height = Convert.ToInt32(lines[4]);
                    int width = Convert.ToInt32(lines[5]);
                    int NumOfTrees = Convert.ToInt32(lines[7]);
                    for (int i = 9; i < lines.Length - 1; i++)
                    {
                        PNSeq.Add(Convert.ToInt32(lines[i]));
                    }

                    #region Extracting Image in Red
                    ///Not using 15 bit mapping
                    Bitmap redbmp = ImageProcessing.ConvertListToWatermark2(RedExtractedWatermark, height, width);
                    //watermarkImage.Image = bmp;
                    //Bitmap bmp = new Bitmap(watermarkImage.Image);               
                    ConservativeSmoothing filter = new ConservativeSmoothing();
                    filter.ApplyInPlace(redbmp);
                    extractedImageRed.Image = redbmp;
                    #endregion

                    #region Extraction Image in Green
                    ///Not using 15 bit mapping
                    Bitmap greenbmp = ImageProcessing.ConvertListToWatermark2(GreenExtractedWatermark, height, width);
                    //watermarkImage.Image = bmp;
                    //Bitmap bmp = new Bitmap(watermarkImage.Image);               
                    ConservativeSmoothing filter2 = new ConservativeSmoothing();
                    filter2.ApplyInPlace(greenbmp);
                    extractedImageGreen.Image = greenbmp;
                    #endregion

                    #region Extraction Image in Blue
                    ///Not using 15 bit mapping
                    Bitmap bluebmp = ImageProcessing.ConvertListToWatermark2(BlueExtractedWatermark, height, width);
                    //watermarkImage.Image = bmp;
                    //Bitmap bmp = new Bitmap(watermarkImage.Image);               
                    ConservativeSmoothing filter3 = new ConservativeSmoothing();
                    filter3.ApplyInPlace(greenbmp);
                    extractedImageBlue.Image = bluebmp;
                    #endregion

                    if (watermarkImage.Image != null)
                    {
                        #region Red BER Calculation
                        /// Test
                        int counter = 0;
                        for (int i = 0; i < RedExtractedWatermark.Length; i++)
                        {
                            if (RedExtractedWatermark[i] == Real_Watermark[i])
                            {
                                counter++;
                            }
                        }
                        double akurasi = ((double)counter / (double)RedExtractedWatermark.Length) * 100;
                        double BER = 100 - akurasi;
                        bertxt.Text += "> " + Math.Round(BER, 2) + " %" + "\n";
                        //double BER = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImageGreen.Image));
                        RedextractedBERtxt.Text = Math.Round(BER, 2).ToString();
                        //MessageBox.Show("Akurasi: " + BER, "Succeed!", MessageBoxButtons.OK);
                        #endregion

                        #region Green BER Calculation
                        /// Test
                        int counter2 = 0;
                        for (int i = 0; i < GreenExtractedWatermark.Length; i++)
                        {
                            if (GreenExtractedWatermark[i] == Real_Watermark[i])
                            {
                                counter2++;
                            }
                        }
                        double akurasi2 = ((double)counter2 / (double)GreenExtractedWatermark.Length) * 100;
                        double BER2 = 100 - akurasi2;
                        bertxt.Text += "> " + Math.Round(BER2, 2) + " %" + "\n";
                        //double BER = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImageGreen.Image));
                        GreenextractedBERtxt.Text = Math.Round(BER2, 2).ToString();
                        //MessageBox.Show("Akurasi: " + BER, "Succeed!", MessageBoxButtons.OK);
                        #endregion

                        #region Blue BER Calculation
                        /// Test
                        int counter3 = 0;
                        for (int i = 0; i < BlueExtractedWatermark.Length; i++)
                        {
                            if (BlueExtractedWatermark[i] == Real_Watermark[i])
                            {
                                counter3++;
                            }
                        }
                        double akurasi3 = ((double)counter3 / (double)BlueExtractedWatermark.Length) * 100;
                        double BER3 = 100 - akurasi3;
                        bertxt.Text += "> " + Math.Round(BER3, 2) + " %" + "\n";
                        //double BER = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImageGreen.Image));
                        BlueextractedBERtxt.Text = Math.Round(BER3, 2).ToString();
                        //MessageBox.Show("Akurasi: " + BER, "Succeed!", MessageBoxButtons.OK);
                        #endregion

                    }

                    GUIEnd("Watermark Extracted!", 0, 0, 0);
                }
                else
                {
                    MessageBox.Show("Train and Detect Watermark first!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
                }
                #endregion

                GUIEnd("Watermark Extracted!", 0, 0, 0);
            }
            
        }

        /// <summary>
        /// Embed the Watermark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e/*, double[,] coeffs, List<int> scrambled_Watermark*/) //Embed Watermark
        {
            if (hostImage.Image == null)
            {
                MessageBox.Show("Load Host Image Image First!", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }
            else if (watermarkImage.Image == null)
            {
                MessageBox.Show("Load Watermark Image First!", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }
            else
            {
                if(HVSValue.Text== "Xie Model")
                {
                    #region Xie Model HVS
                    GUIStart("Processing.....!");
                    List<List<int>> Segmented = Scramble.Segment(Scrambled_Watermark);
                    //double[,] MappedWatermark = Scramble.Mapping(Segmented); // Use mapping into 15 bit each 5 segment of scrambled watermark
                    double[,] MappedWatermark = Scramble.Mapping2(Segmented);  // Don't Use mapping 
                    Mapped_Watermark = MappedWatermark;

                    #region Red Adaptive HVS
                    double[,] RedHVSValues = new double[Red_Coeffs.GetLength(0), Red_Coeffs.GetLength(1)];
                    double[,] RedAdaptiveHVS = new double[Red_Coeffs.GetLength(0), Red_Coeffs.GetLength(1)];
                    double[,] Redpixels = ImageProcessing.ConvertToMatrix2(new Bitmap(transformedImage.Image)).Item1;
                    Bitmap RedEdgeImage = ImageProcessing.LaplaceEdge(new Bitmap(transformedImage.Image));
                    double[,] Redpixels2 = ImageProcessing.ConvertToMatrix2(RedEdgeImage).Item1;
                    int RedNumOfTree = Scrambled_Watermark.Count / 5;
                    RedAdaptiveHVS = Embed.AdaptiveHVS(Red_Coeffs, Redpixels, Redpixels2, RedNumOfTree);
                    #endregion

                    #region Green Adaptive HVS
                    double[,] GreenHVSValues = new double[Green_Coeffs.GetLength(0), Green_Coeffs.GetLength(1)];
                    double[,] GreenAdaptiveHVS = new double[Green_Coeffs.GetLength(0), Green_Coeffs.GetLength(1)];
                    double[,] Greenpixels = ImageProcessing.ConvertToMatrix2(new Bitmap(transformedImage.Image)).Item2;
                    Bitmap GreenEdgeImage = ImageProcessing.LaplaceEdge(new Bitmap(transformedImage.Image));
                    double[,] Greenpixels2 = ImageProcessing.ConvertToMatrix2(GreenEdgeImage).Item2;
                    int GreenNumOfTree = Scrambled_Watermark.Count / 5;
                    GreenAdaptiveHVS = Embed.AdaptiveHVS(Green_Coeffs, Greenpixels, Greenpixels2, GreenNumOfTree);
                    #endregion

                    #region Blue Adaptive HVS
                    double[,] BlueHVSValues = new double[Blue_Coeffs.GetLength(0), Blue_Coeffs.GetLength(1)];
                    double[,] BlueAdaptiveHVS = new double[Blue_Coeffs.GetLength(0), Blue_Coeffs.GetLength(1)];
                    double[,] Bluepixels = ImageProcessing.ConvertToMatrix2(new Bitmap(transformedImage.Image)).Item3;
                    Bitmap BlueEdgeImage = ImageProcessing.LaplaceEdge(new Bitmap(transformedImage.Image));
                    double[,] Bluepixels2 = ImageProcessing.ConvertToMatrix2(BlueEdgeImage).Item3;
                    int BlueNumOfTree = Scrambled_Watermark.Count / 5;
                    BlueAdaptiveHVS = Embed.AdaptiveHVS(Blue_Coeffs, Bluepixels, Bluepixels2, BlueNumOfTree);
                    #endregion

                    string subband = subbandValue.Text;
                    double embed_constant = Convert.ToDouble(embedConstantValue.Text);
                    //double[,] EmbeddedWatermark = Embed.Embedding(Wavelet_Coefficients,MappedWatermark, AdaptiveHVS, subband ,embed_constant);
                    double[,] RedEmbeddedWatermark = Embed.Embedding(Red_Coeffs, MappedWatermark, RedAdaptiveHVS, subband, embed_constant);
                    double[,] GreenEmbeddedWatermark = Embed.Embedding(Green_Coeffs, MappedWatermark, GreenAdaptiveHVS, subband, embed_constant);
                    double[,] BlueEmbeddedWatermark = Embed.Embedding(Blue_Coeffs, MappedWatermark, BlueAdaptiveHVS, subband, embed_constant);
                    RedEmbedded_Wavelet_Coefficients = RedEmbeddedWatermark;
                    GreenEmbedded_Wavelet_Coefficients = GreenEmbeddedWatermark;
                    BlueEmbedded_Wavelet_Coefficients = BlueEmbeddedWatermark;

                    GUIEnd("Embedding Succeed!", 0, 0, 0);
                    #endregion
                }
                else
                {
                    #region Edge Based Model
                    GUIStart("Processing.....!");
                    List<List<int>> Segmented = Scramble.Segment(Scrambled_Watermark);
                    //double[,] MappedWatermark = Scramble.Mapping(Segmented); // Use mapping into 15 bit each 5 segment of scrambled watermark
                    double[,] MappedWatermark = Scramble.Mapping2(Segmented);  // Don't Use mapping 
                    Mapped_Watermark = MappedWatermark;

                    #region Red Adaptive HVS
                    double[,] RedEdgeHVS = Embed.EdgeBasedHVS(new Bitmap(transformedImage.Image)).Item1;
                    #endregion

                    #region Green Adaptive HVS
                    double[,] GreenHVS = Embed.EdgeBasedHVS(new Bitmap(transformedImage.Image)).Item2;
                    #endregion

                    #region Blue Adaptive HVS
                    double[,] BlueHVS = Embed.EdgeBasedHVS(new Bitmap(transformedImage.Image)).Item3;
                    #endregion

                    string subband = subbandValue.Text;
                    double embed_constant = Convert.ToDouble(embedConstantValue.Text);
                    //double[,] EmbeddedWatermark = Embed.Embedding(Wavelet_Coefficients,MappedWatermark, AdaptiveHVS, subband ,embed_constant);
                    double[,] RedEmbeddedWatermark = Embed.Embedding(Red_Coeffs, MappedWatermark, RedEdgeHVS, subband, embed_constant);
                    double[,] GreenEmbeddedWatermark = Embed.Embedding(Green_Coeffs, MappedWatermark, GreenHVS, subband, embed_constant);
                    double[,] BlueEmbeddedWatermark = Embed.Embedding(Blue_Coeffs, MappedWatermark, BlueHVS, subband, embed_constant);
                    RedEmbedded_Wavelet_Coefficients = RedEmbeddedWatermark;
                    GreenEmbedded_Wavelet_Coefficients = GreenEmbeddedWatermark;
                    BlueEmbedded_Wavelet_Coefficients = BlueEmbeddedWatermark;

                    GUIEnd("Embedding Succeed!", 0, 0, 0);
                    #endregion
                }
                
            }
        }


        /// <summary>
        /// Inverse DWT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e/*, double[,] Embedded_Coefficients*/) //Inverse Transform
        {
            if(dwtTypeValue.Text == "Haar")
            {
                if (transformedImage.Image == null)
                {
                    MessageBox.Show("There is no Transformed image yet", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
                }
                else
                {
                    GUIStart("Processing.....!");
                    //int level = Convert.ToInt32(HVSValue.Text);
                    //MessageBox.Show("Embedded Wavelet Coefficients: "+Embedded_Wavelet_Coefficients[0,0],"blabal",MessageBoxButtons.OK);
                    //double[,] InverseDWT = Haar.WaveletCoeff(Embedded_Wavelet_Coefficients, false, level);
                    double[,] RedInverseDWT = Haar.WaveletCoeff(RedEmbedded_Wavelet_Coefficients, false, 2);
                    double[,] GreenInverseDWT = Haar.WaveletCoeff(GreenEmbedded_Wavelet_Coefficients, false, 2);
                    double[,] BlueInverseDWT = Haar.WaveletCoeff(BlueEmbedded_Wavelet_Coefficients, false, 2);


                    /// Round all elements in InverseDWT
                    //double[,] RoundedInversedDWT = Statistic.RoundAll(InverseDWT);

                    //transformedImage.Image = ImageProcessing.ConvertToBitmap(InverseDWT);
                    transformedImage.Image = ImageProcessing.ConvertToBitmap2(RedInverseDWT, GreenInverseDWT, BlueInverseDWT);
                    double mse = Statistic.MSE(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
                    double psnr = Statistic.PSNR(new Bitmap(transformedImage.Image), mse);
                    double ber = Statistic.BER(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
                    GUIEnd("IDWT Succeed!", mse, psnr, ber);

                    /// Test
                    //double[,] WC = DWT.WaveletCoeff(InverseDWT, true, 2);
                    /// Test
                    //int h = 1;
                    //TextWriter tw1 = new StreamWriter("InversedDWT.txt");
                    //tw1.WriteLine("Total Watermark: " + InverseDWT.GetLength(0));
                    //for (int i = 0; i < InverseDWT.GetLength(0); i++)
                    //{
                    //    for (int j = 0; j < InverseDWT.GetLength(1); j++)
                    //    {
                    //        tw1.Write("[" + h + "]" + InverseDWT[i, j] + " # ");
                    //        h++;
                    //    }
                    //    tw1.WriteLine();
                    //}
                    ////foreach (double i in ExtractedWatermark)
                    ////{
                    ////    tw1.WriteLine(i);
                    ////}
                    //tw1.Close();

                    ///Activate Attack Button
                    //histeqBtn.Enabled = true;
                    //histeqBtn.BackColor = Color.DeepSkyBlue;
                    //meanFilterBtn.Enabled = true;
                    //meanFilterBtn.BackColor = Color.DeepSkyBlue;
                    //medianFilterBtn.Enabled = true;
                    //medianFilterBtn.BackColor = Color.DeepSkyBlue;
                    //modusFilterBtn.Enabled = true;
                    //modusFilterBtn.BackColor = Color.DeepSkyBlue;
                    //jpegencoderBtn.Enabled = true;
                    //jpegencoderBtn.BackColor = Color.DeepSkyBlue;

                    resultLbl.Text = "Haar Watermarked Host Image";
                    WatermarkedImage = new Bitmap(transformedImage.Image);
                    Transformed_Image = new Bitmap(transformedImage.Image);

                    double psnrvalue = Math.Round(psnr, 2);
                    //double bervalue = Math.Round(psnr, 2);
                    double msevalue = Math.Round(psnr, 2);
                    psnrtxt.Text += ">" + file_name + "\n" + "PSNR:" + psnrvalue + "\n" + "Key is saved!" + "\n" + "-----------" + "\n";

                    dwtTypeValue2.Text = dwtTypeValue.Text;
                    embedConstantValue2.Text = embedConstantValue.Text;
                    subbandValue2.Text = subbandValue.Text;


                    drawing = new Bitmap(transformedImage.Image);
                    //test
                    //MessageBox.Show("Red: " + IMatrixR[0, 0].ToString() + ", Green: " + IMatrixG[0, 0].ToString() + ", Blue: " + IMatrixB[0, 0].ToString(), "Values of RGB");
                }
            }
            else if(dwtTypeValue.Text == "Db2")
            {
                if (transformedImage.Image == null)
                {
                    MessageBox.Show("There is no Transformed image yet", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
                }
                else
                {
                    GUIStart("Processing.....!");
                    //int level = Convert.ToInt32(HVSValue.Text);
                    //MessageBox.Show("Embedded Wavelet Coefficients: "+Embedded_Wavelet_Coefficients[0,0],"blabal",MessageBoxButtons.OK);
                    //double[,] InverseDWT = Haar.WaveletCoeff(Embedded_Wavelet_Coefficients, false, level);
                    double[,] RedInverseDWT = Daubechies2.WaveletCoeff(RedEmbedded_Wavelet_Coefficients, false, 2);
                    double[,] GreenInverseDWT = Daubechies2.WaveletCoeff(GreenEmbedded_Wavelet_Coefficients, false, 2);
                    double[,] BlueInverseDWT = Daubechies2.WaveletCoeff(BlueEmbedded_Wavelet_Coefficients, false, 2);


                    /// Round all elements in InverseDWT
                    //double[,] RoundedInversedDWT = Statistic.RoundAll(InverseDWT);

                    //transformedImage.Image = ImageProcessing.ConvertToBitmap(InverseDWT);
                    transformedImage.Image = ImageProcessing.ConvertToBitmap2(RedInverseDWT, GreenInverseDWT, BlueInverseDWT);
                    double mse = Statistic.MSE(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
                    double psnr = Statistic.PSNR(new Bitmap(transformedImage.Image), mse);
                    double ber = Statistic.BER(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
                    GUIEnd("IDWT Succeed!", mse, psnr, ber);

                    /// Test
                    //double[,] WC = DWT.WaveletCoeff(InverseDWT, true, 2);
                    /// Test
                    //int h = 1;
                    //TextWriter tw1 = new StreamWriter("InversedDWT.txt");
                    //tw1.WriteLine("Total Watermark: " + InverseDWT.GetLength(0));
                    //for (int i = 0; i < InverseDWT.GetLength(0); i++)
                    //{
                    //    for (int j = 0; j < InverseDWT.GetLength(1); j++)
                    //    {
                    //        tw1.Write("[" + h + "]" + InverseDWT[i, j] + " # ");
                    //        h++;
                    //    }
                    //    tw1.WriteLine();
                    //}
                    ////foreach (double i in ExtractedWatermark)
                    ////{
                    ////    tw1.WriteLine(i);
                    ////}
                    //tw1.Close();

                    ///Activate Attack Button
                    //histeqBtn.Enabled = true;
                    //histeqBtn.BackColor = Color.DeepSkyBlue;
                    //meanFilterBtn.Enabled = true;
                    //meanFilterBtn.BackColor = Color.DeepSkyBlue;
                    //medianFilterBtn.Enabled = true;
                    //medianFilterBtn.BackColor = Color.DeepSkyBlue;
                    //modusFilterBtn.Enabled = true;
                    //modusFilterBtn.BackColor = Color.DeepSkyBlue;
                    //jpegencoderBtn.Enabled = true;
                    //jpegencoderBtn.BackColor = Color.DeepSkyBlue;

                    resultLbl.Text = " Daubechies 2 Watermarked Host Image";
                    WatermarkedImage = new Bitmap(transformedImage.Image);
                    Transformed_Image = new Bitmap(transformedImage.Image);

                    double psnrvalue = Math.Round(psnr, 2);
                    //double bervalue = Math.Round(psnr, 2);
                    double msevalue = Math.Round(psnr, 2);
                    psnrtxt.Text += ">" + file_name + "\n" + "PSNR:" + psnrvalue + "\n" + "Key is saved!" + "\n" + "-----------" + "\n";

                    dwtTypeValue2.Text = dwtTypeValue.Text;
                    embedConstantValue2.Text = embedConstantValue.Text;
                    subbandValue2.Text = subbandValue.Text;


                    drawing = new Bitmap(transformedImage.Image);
                    //test
                    //MessageBox.Show("Red: " + IMatrixR[0, 0].ToString() + ", Green: " + IMatrixG[0, 0].ToString() + ", Blue: " + IMatrixB[0, 0].ToString(), "Values of RGB");
                }
            }
            else
            {
                GUIStart("Processing.....!");
                //int level = Convert.ToInt32(HVSValue.Text);
                //MessageBox.Show("Embedded Wavelet Coefficients: "+Embedded_Wavelet_Coefficients[0,0],"blabal",MessageBoxButtons.OK);
                //double[,] InverseDWT = Haar.WaveletCoeff(Embedded_Wavelet_Coefficients, false, level);
                double[,] RedInverseDWT = Daubechies3.WaveletCoeff(RedEmbedded_Wavelet_Coefficients, false, 2);
                double[,] GreenInverseDWT = Daubechies3.WaveletCoeff(GreenEmbedded_Wavelet_Coefficients, false, 2);
                double[,] BlueInverseDWT = Daubechies3.WaveletCoeff(BlueEmbedded_Wavelet_Coefficients, false, 2);


                /// Round all elements in InverseDWT
                //double[,] RoundedInversedDWT = Statistic.RoundAll(InverseDWT);

                //transformedImage.Image = ImageProcessing.ConvertToBitmap(InverseDWT);
                transformedImage.Image = ImageProcessing.ConvertToBitmap2(RedInverseDWT, GreenInverseDWT, BlueInverseDWT);
                double mse = Statistic.MSE(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
                double psnr = Statistic.PSNR(new Bitmap(transformedImage.Image), mse);
                double ber = Statistic.BER(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
                GUIEnd("IDWT Succeed!", mse, psnr, ber);

                /// Test
                //double[,] WC = DWT.WaveletCoeff(InverseDWT, true, 2);
                /// Test
                //int h = 1;
                //TextWriter tw1 = new StreamWriter("InversedDWT.txt");
                //tw1.WriteLine("Total Watermark: " + InverseDWT.GetLength(0));
                //for (int i = 0; i < InverseDWT.GetLength(0); i++)
                //{
                //    for (int j = 0; j < InverseDWT.GetLength(1); j++)
                //    {
                //        tw1.Write("[" + h + "]" + InverseDWT[i, j] + " # ");
                //        h++;
                //    }
                //    tw1.WriteLine();
                //}
                ////foreach (double i in ExtractedWatermark)
                ////{
                ////    tw1.WriteLine(i);
                ////}
                //tw1.Close();

                ///Activate Attack Button
                //histeqBtn.Enabled = true;
                //histeqBtn.BackColor = Color.DeepSkyBlue;
                //meanFilterBtn.Enabled = true;
                //meanFilterBtn.BackColor = Color.DeepSkyBlue;
                //medianFilterBtn.Enabled = true;
                //medianFilterBtn.BackColor = Color.DeepSkyBlue;
                //modusFilterBtn.Enabled = true;
                //modusFilterBtn.BackColor = Color.DeepSkyBlue;
                //jpegencoderBtn.Enabled = true;
                //jpegencoderBtn.BackColor = Color.DeepSkyBlue;

                resultLbl.Text = " Daubechies 2 Watermarked Host Image";
                WatermarkedImage = new Bitmap(transformedImage.Image);
                Transformed_Image = new Bitmap(transformedImage.Image);

                double psnrvalue = Math.Round(psnr, 2);
                //double bervalue = Math.Round(psnr, 2);
                double msevalue = Math.Round(psnr, 2);
                psnrtxt.Text += ">" + file_name + "\n" + "PSNR:" + psnrvalue + "\n" + "Key is saved!" + "\n" + "-----------" + "\n";

                dwtTypeValue2.Text = dwtTypeValue.Text;
                embedConstantValue2.Text = embedConstantValue.Text;
                subbandValue2.Text = subbandValue.Text;


                drawing = new Bitmap(transformedImage.Image);
                //test
                //MessageBox.Show("Red: " + IMatrixR[0, 0].ToString() + ", Green: " + IMatrixG[0, 0].ToString() + ", Blue: " + IMatrixB[0, 0].ToString(), "Values of RGB");
            }


        }

        

        #region Attack Image
        private void button12_Click(object sender, EventArgs e) //Histogram Equilization Attack
        {
            if (transformedImage.Image != null)
            {
                transformedImage.Image = ImageAttack.HistEq(WatermarkedImage);
                resultLbl.Text = "Histogram Equilization of Watermarked Image";
            }
            else
            {
                MessageBox.Show("Watermark the Host Image first!", "Incomplete Procedure Detected");
            }
        }

        private void meanFilterBtn_Click(object sender, EventArgs e) //Mean Filter Attack
        {
            if (transformedImage.Image != null)
            {
                if(comboBox1.Text == "3x3")
                {
                    transformedImage.Image = ImageAttack.MeanFilter(WatermarkedImage, 1);
                    resultLbl.Text = "Mean Filter of Watermarked Image";
                }
                else if(comboBox1.Text == "5x5")
                {
                    transformedImage.Image = ImageAttack.MeanFilter(WatermarkedImage, 2);
                    resultLbl.Text = "Mean Filter of Watermarked Image";
                }
                else
                {
                    transformedImage.Image = ImageAttack.MeanFilter(WatermarkedImage, 3);
                    resultLbl.Text = "Mean Filter of Watermarked Image";
                } 
            }
            else
            {
                MessageBox.Show("Watermark the Host Image first!", "Incomplete Procedure Detected");
            }
        }

        private void medianFilterBtn_Click(object sender, EventArgs e) //Median Filter Attack
        {
            if (transformedImage.Image != null)
            {
                if(comboBox2.Text == "3x3")
                {
                    transformedImage.Image = ImageAttack.MedianFilter(WatermarkedImage, 3);
                    resultLbl.Text = "Median Filter of Watermarked Image";
                }
                else if(comboBox2.Text == "5x5")
                {
                    transformedImage.Image = ImageAttack.MedianFilter(WatermarkedImage, 5);
                    resultLbl.Text = "Median Filter of Watermarked Image";
                }
                else
                {
                    transformedImage.Image = ImageAttack.MedianFilter(WatermarkedImage, 7);
                    resultLbl.Text = "Median Filter of Watermarked Image";
                }
                
            }
            else
            {
                MessageBox.Show("Watermark the Host Image first!", "Incomplete Procedure Detected");
            }
        }

        private void modusFilterBtn_Click(object sender, EventArgs e) //Modus Filter Attack
        {
            if (transformedImage.Image != null)
            {
                if(comboBox3.Text == "3x3")
                {
                    transformedImage.Image = ImageAttack.ModusFilter(WatermarkedImage, 1);
                    resultLbl.Text = "Modus Filter of Watermarked Image";
                }
                else if(comboBox3.Text == "5x5")
                {
                    transformedImage.Image = ImageAttack.ModusFilter(WatermarkedImage, 2);
                    resultLbl.Text = "Modus Filter of Watermarked Image";
                }
                else
                {
                    transformedImage.Image = ImageAttack.ModusFilter(WatermarkedImage, 3);
                    resultLbl.Text = "Modus Filter of Watermarked Image";
                }
                
            }
            else
            {
                MessageBox.Show("Watermark the Host Image first!", "Incomplete Procedure Detected");
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if(transformedImage.Image != null)
            {
                int value = Convert.ToInt32(jpegCompressionValue.Text);
                transformedImage.Image = ImageAttack.JpegCompress(WatermarkedImage, value, "CompressedImage20.jpg");
                MessageBox.Show("Image Successfully Compressed!", "Success", MessageBoxButtons.OK);
                resultLbl.Text = "Image Compressed";
            }
            else
            {
                MessageBox.Show("Watermark the host image first!", "Incomplete Procedure Detected");
            }
        }

        #endregion

             

        private void button13_Click(object sender, EventArgs e) //Train HMM and detect Watermark
        {           
            /// Detection            
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a key accordingly";
            ofd.InitialDirectory = @"F:\College\Semester 8\TA2\TugasAkhir1\TugasAkhir1\Key";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamReader objstream = new StreamReader(ofd.FileName);
                string[] lines = objstream.ReadToEnd().Split(new char[] { '\n' });
                int hostheight = Convert.ToInt32(lines[1]);
                int hostwidth = Convert.ToInt32(lines[2]);
                int NumOfTrees = Convert.ToInt32(lines[7]);
                KeyFileName = ofd.FileName;

                /// Get PNSeq
                List<int> PNSeq = new List<int>();
                for (int i = 9; i < lines.Length - 1; i++)
                {
                    PNSeq.Add(Convert.ToInt32(lines[i]));
                }

                GUIStart("Training HMM Model...");
                //transformedImage.Image = DWT.TransformDWT(true, false, 2, OriginalImage);

                ///For Wavelet Coefficients Extraction
                Bitmap b = new Bitmap(transformedImage.Image);
                IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
                IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
                IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;
                
                double[,] ArrayImage = IMatrixG; //Embedding in Green 
                Watermarked_Wavelet_Coefficients = Haar.WaveletCoeff(ArrayImage, true, 2);                
                int NumOfScale2 = ((hostheight*hostwidth)/16)*3;


                Image decomposed = Haar.TransformDWT(true, false, 2, new Bitmap(transformedImage.Image)).Item4;
                //ExtractedWatermark = Extract.BaumWelchDetectionInLH_2(Watermarked_Wavelet_Coefficients, decomposed, NumOfScale2, NumOfTrees, PNSeq /*, rootpmf, transition, variances*/);
                //detectedWatermark = Extract.BaumWelchDetection(Watermarked_Wavelet_Coefficients, transformedImage.Image, NumOfScale2, NumOfTrees, PNSeq /*, rootpmf, transition, variances*/);

                /// Test
                //int f = 1;
                //double[][] Nested = HMM.ConvertToNestedArray(Watermarked_Wavelet_Coefficients);
                //TextWriter tw1 = new StreamWriter("Watermarked_Wavelet_Coefficients.txt");
                //tw1.WriteLine("Total Watermark: " + Watermarked_Wavelet_Coefficients.GetLength(0));
                //for (int i = 0; i < Watermarked_Wavelet_Coefficients.GetLength(0); i++)
                //{
                //    for (int j = 0; j < Watermarked_Wavelet_Coefficients.GetLength(1); j++)
                //    {
                //        tw1.WriteLine("["+f+"]"+Watermarked_Wavelet_Coefficients[i,j]);
                //    }
                //}
                ////foreach (double i in ExtractedWatermark)
                ////{
                ////    tw1.WriteLine(i);
                ////}
                //tw1.Close();

                GUIEnd("HMM Model Trained and detected!", 0, 0, 0);
                //MessageBox.Show("Training and Detecting HMM Model Succeed!", "Succeed", MessageBoxButtons.OK);

            }
            else
            {
                MessageBox.Show("Select Key accordingly first!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
            }
            


        }

        

        private void button17_Click(object sender, EventArgs e)
        {
            if (KeyFileName != null)
            {
                GUIStart("Extracting Watermark...");

                List<int> PNSeq = new List<int>();

                StreamReader objstream = new StreamReader(KeyFileName);
                string[] lines = objstream.ReadToEnd().Split(new char[] { '\n' });
                int height = Convert.ToInt32(lines[4]);
                int width = Convert.ToInt32(lines[5]);
                int NumOfTrees = Convert.ToInt32(lines[7]);
                for (int i = 9; i < lines.Length - 1; i++)
                {
                    PNSeq.Add(Convert.ToInt32(lines[i]));
                }

                #region Extracting Image
                ///Not using 15 bit mapping
                Bitmap bmp = ImageProcessing.ConvertListToWatermark2(ExtractedWatermark, height, width);
                //watermarkImage.Image = bmp;
                //Bitmap bmp = new Bitmap(watermarkImage.Image);               
                ConservativeSmoothing filter = new ConservativeSmoothing();
                filter.ApplyInPlace(bmp);
                extractedImageRed.Image = bmp;
                #endregion

                /// Test
                int counter = 0;
                for (int i = 0; i < ExtractedWatermark.Length; i++)
                {
                    if(ExtractedWatermark[i] == Real_Watermark[i])
                    {
                        counter++;
                    }
                }
                double akurasi = ((double)counter/(double)ExtractedWatermark.Length)*100;
                double BER = 100 - akurasi;
                bertxt.Text += Math.Round(BER,2)+"\n";
                //MessageBox.Show("Akurasi: " + BER, "Succeed!", MessageBoxButtons.OK);

                GUIEnd("Watermark Extracted!", 0, 0, 0);
            }
            else
            {
                MessageBox.Show("Train and Detect Watermark first!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
            }                
        }

        

        

        private void noiseBtn_Click(object sender, EventArgs e)
        {
            int value =Convert.ToInt32(gaussianNoiseValue.Text);
            transformedImage.Image = ImageAttack.GaussianNoise(WatermarkedImage, value);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(saltPepperValue.Text);
            transformedImage.Image = ImageAttack.SaltAndPepper(WatermarkedImage, value);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(medianCutValue.Text);
            transformedImage.Image = ImageAttack.MedianCut(WatermarkedImage, value);
        }

        private void button23_Click(object sender, EventArgs e)
        {            
            transformedImage.Image = ImageAttack.Sharpen(WatermarkedImage);
        }

        

        private void button25_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(burkesValue.Text);
            transformedImage.Image = ImageAttack.BurkesColorDithering(WatermarkedImage, value);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(floydSteinbergValue.Text);
            transformedImage.Image = ImageAttack.FloydSteinbergColorDithering(WatermarkedImage, value);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(JJNValue.Text);
            transformedImage.Image = ImageAttack.JarvisJudiceNinkeColorDithering(WatermarkedImage, value);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            transformedImage.Image = ImageAttack.SierraColorDithering(WatermarkedImage);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(stuckiValue.Text);
            transformedImage.Image = ImageAttack.StuckiColorDithering(WatermarkedImage, value);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(orderedValue.Text);
            transformedImage.Image = ImageAttack.OrderedColorDithering(WatermarkedImage, value);
        }

        #region Scratch On Watermarked Image
        private void transformedImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw)
            {
                int penWidth = Convert.ToInt32(penWidthValue.Value);
                if(blackCheck.Checked == true) ///black pen
                {
                    Graphics panel = Graphics.FromImage(drawing);

                    Pen pen = new Pen(Color.Black, penWidth);

                    pen.EndCap = LineCap.Round;
                    pen.StartCap = LineCap.Round;

                    panel.DrawLine(pen, pX, pY, e.X, e.Y);

                    transformedImage.CreateGraphics().DrawImageUnscaled(drawing, new Point(0, 0));
                }
                else if(redCheck.Checked == true) /// red pen
                {
                    Graphics panel = Graphics.FromImage(drawing);

                    Pen pen = new Pen(Color.Red, penWidth);

                    pen.EndCap = LineCap.Round;
                    pen.StartCap = LineCap.Round;

                    panel.DrawLine(pen, pX, pY, e.X, e.Y);

                    transformedImage.CreateGraphics().DrawImageUnscaled(drawing, new Point(0, 0));
                }
                else if(yellowCheck.Checked == true) /// yellow
                {
                    Graphics panel = Graphics.FromImage(drawing);

                    Pen pen = new Pen(Color.Yellow, penWidth);

                    pen.EndCap = LineCap.Round;
                    pen.StartCap = LineCap.Round;

                    panel.DrawLine(pen, pX, pY, e.X, e.Y);

                    transformedImage.CreateGraphics().DrawImageUnscaled(drawing, new Point(0, 0));
                }
                else /// green
                {
                    Graphics panel = Graphics.FromImage(drawing);

                    Pen pen = new Pen(Color.Green, penWidth);

                    pen.EndCap = LineCap.Round;
                    pen.StartCap = LineCap.Round;

                    panel.DrawLine(pen, pX, pY, e.X, e.Y);

                    transformedImage.CreateGraphics().DrawImageUnscaled(drawing, new Point(0, 0));
                }
                
            }

            pX = e.X;
            pY = e.Y;
        }

        private void transformedImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (scratchCheck.Checked == true)
            {
                draw = true;
                pX = e.X;
                pY = e.Y;
            }
        }

        private void transformedImage_MouseUp(object sender, MouseEventArgs e)
        {
            draw = false;
        }

        private void transformedImage_Paint(object sender, PaintEventArgs e)
        {
            if(scratchCheck.Checked == true)
            {
                e.Graphics.DrawImageUnscaled(drawing, new Point(0, 0));
            }
        }

        private void redCheck_CheckedChanged(object sender, EventArgs e)
        {
            blackCheck.Checked = false;
            yellowCheck.Checked = false;
            greenCheck.Checked = false;
        }

        private void blackCheck_CheckedChanged(object sender, EventArgs e)
        {
            redCheck.Checked = false;
            yellowCheck.Checked = false;
            greenCheck.Checked = false;
        }

        private void yellowCheck_CheckedChanged(object sender, EventArgs e)
        {
            blackCheck.Checked = false;
            redCheck.Checked = false;
            greenCheck.Checked = false;
        }

        private void greenCheck_CheckedChanged(object sender, EventArgs e)
        {
            blackCheck.Checked = false;
            redCheck.Checked = false;
            yellowCheck.Checked = false;
        }

        private void scratchCheck_CheckedChanged(object sender, EventArgs e)
        {
            //drawing = new Bitmap(transformedImage.Image, Transformed_Image.Width, Transformed_Image.Height);//new Bitmap(transformedImage.Width, transformedImage.Height, transformedImage.CreateGraphics());
            //Graphics.FromImage(drawing).Clear(Color.Transparent);
            //transformedImage.Image = drawing;
            //drawing = new Bitmap(transformedImage.Image);           
        }

        private void button31_Click(object sender, EventArgs e)
        {
            transformedImage.Image = ImageAttack.Blur(WatermarkedImage);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            transformedImage.Image = ImageAttack.GaussianBlur(WatermarkedImage, 10);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            transformedImage.Image = ImageAttack.Median(WatermarkedImage);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            transformedImage.Image = ImageAttack.Mean(WatermarkedImage);
        }

        private void button35_Click(object sender, EventArgs e)
        {
            transformedImage.Image = ImageAttack.Jitter(WatermarkedImage,4);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            transformedImage.Image = ImageAttack.ResizeBilinear(WatermarkedImage, 256, 256);
        }

        private void button37_Click(object sender, EventArgs e)
        {
            transformedImage.Image = ImageAttack.RotateBilinear(WatermarkedImage, 30);
        }

        private void button38_Click(object sender, EventArgs e)
        {
            transformedImage.Image = ImageAttack.Crop(WatermarkedImage, 100, 100, 150, 150);
        }

        private void button39_Click(object sender, EventArgs e)
        {
            AForge.Imaging.ImageStatistics stat = new AForge.Imaging.ImageStatistics(WatermarkedImage);
             //red = stat.Red;
        }       
        

        private void button24_Click_1(object sender, EventArgs e)
        {           
            if(comboBox4.Text == "3x3")
            {
                transformedImage.Image = ImageAttack.GaussianSharpen(WatermarkedImage, 4, 3);
            }
            else if(comboBox4.Text == "5x5")
            {
                transformedImage.Image = ImageAttack.GaussianSharpen(WatermarkedImage, 4, 5);
            }
            else if(comboBox4.Text == "7x7")
            {
                transformedImage.Image = ImageAttack.GaussianSharpen(WatermarkedImage, 4, 7);
            }
            else if (comboBox4.Text == "10x10")
            {
                transformedImage.Image = ImageAttack.GaussianSharpen(WatermarkedImage, 4, 10);
            }
            else if (comboBox4.Text == "15x15")
            {
                transformedImage.Image = ImageAttack.GaussianSharpen(WatermarkedImage, 4, 15);
            }
            else
            {
                transformedImage.Image = ImageAttack.GaussianSharpen(WatermarkedImage, 4, 30);
            }
        }

        private void button23_Click_1(object sender, EventArgs e)
        {
            transformedImage.Image = ImageAttack.Sharpen(WatermarkedImage);
        }

        private void button42_Click(object sender, EventArgs e)
        {
            hostImage.Image = null;
            watermarkImage.Image = null;
            transformedImage.Image = null;
            extractedImageRed.Image = null;
            Wavelet_Coefficients = null;
            Scrambled_Watermark = null;
            Embedded_Wavelet_Coefficients = null;

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (watermarkImage.Image != null)
            {
                #region Red BER
                Bitmap b = ImageProcessing.ConvertToBinary(new Bitmap(watermarkImage.Image));
                List<int> VectorImage = Scramble.ConvertToVectorMatrix(b); //Include integer values between 255 or 0
                List<int> BinaryVectorImage = Scramble.ConvertToBinaryVectorMatrix(VectorImage); //Include integer values between 1 or 0
                List<int> RealWatermark2 = BinaryVectorImage;
                int counter = 0;
                for (int i = 0; i < RedExtractedWatermark.Length; i++)
                {
                    if (RedExtractedWatermark[i] == RealWatermark2[i])
                    {
                        counter++;
                    }
                }
                double akurasi = ((double)counter / (double)RedExtractedWatermark.Length) * 100;
                double BER = 100 - akurasi;
                double ber = Math.Round(BER, 2);
                //double ber = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImage.Image));
                RedextractedBERtxt.Text = ber.ToString();
                //bertxt.Text += "> " + ber.ToString() + " %" + "\n";
                #endregion

                #region Green BER
                Bitmap b2 = ImageProcessing.ConvertToBinary(new Bitmap(watermarkImage.Image));
                List<int> VectorImage2 = Scramble.ConvertToVectorMatrix(b2); //Include integer values between 255 or 0
                List<int> BinaryVectorImage2 = Scramble.ConvertToBinaryVectorMatrix(VectorImage2); //Include integer values between 1 or 0
                List<int> RealWatermark22 = BinaryVectorImage2;
                int counter2 = 0;
                for (int i = 0; i < GreenExtractedWatermark.Length; i++)
                {
                    if (GreenExtractedWatermark[i] == RealWatermark22[i])
                    {
                        counter2++;
                    }
                }
                double akurasi2 = ((double)counter2 / (double)GreenExtractedWatermark.Length) * 100;
                double BER2 = 100 - akurasi2;
                double ber2 = Math.Round(BER2, 2);
                //double ber = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImage.Image));
                GreenextractedBERtxt.Text = ber2.ToString();
                //bertxt.Text += "> " + ber.ToString() + " %" + "\n";
                #endregion

                #region Blue BER
                Bitmap b3 = ImageProcessing.ConvertToBinary(new Bitmap(watermarkImage.Image));
                List<int> VectorImage3 = Scramble.ConvertToVectorMatrix(b3); //Include integer values between 255 or 0
                List<int> BinaryVectorImage3 = Scramble.ConvertToBinaryVectorMatrix(VectorImage3); //Include integer values between 1 or 0
                List<int> RealWatermark23 = BinaryVectorImage3;
                int counter3 = 0;
                for (int i = 0; i < BlueExtractedWatermark.Length; i++)
                {
                    if (BlueExtractedWatermark[i] == RealWatermark23[i])
                    {
                        counter3++;
                    }
                }
                double akurasi3 = ((double)counter3 / (double)BlueExtractedWatermark.Length) * 100;
                double BER3 = 100 - akurasi3;
                double ber3 = Math.Round(BER3, 3);
                //double ber = Statistic.BER(new Bitmap(watermarkImage.Image), new Bitmap(extractedImage.Image));
                BlueextractedBERtxt.Text = ber3.ToString();
                //bertxt.Text += "> " + ber.ToString() + " %" + "\n";
                #endregion

                List<double> berlist = new List<double>();
                berlist.Add(ber);
                berlist.Add(ber2);
                berlist.Add(ber3);
                double minBer = berlist.Min();
                bertxt.Text += "> " + minBer.ToString() + " %" + "\n";
                bertxt.Text += "----------" + "\n";


            }
            else
            {
                MessageBox.Show("Input Original Watermark Image to Calculate BER");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //Bitmap bmp = new Bitmap(hostImage.Image);
            //double[,] pixels = ImageProcessing.ConvertToMatrix2(bmp).Item2;
            //double[,] redCoeffs = Haar.TransformDWT(true, false, 2, bmp).Item1;
            //double[,] coeffs = db2.WaveletCoeff(pixels, true, 2);
            ////transformedImage.Image = db2.TransformDWT(true, false, 3, bmp).Item4;
            //TextWriter tw1 = new StreamWriter("WaveletCoeffsdb2.txt");
            ////tw1.WriteLine("Total Real Watermark: " );
            //int c = 1;
            //for(int i = 0; i < redCoeffs.GetLength(0); i++)
            //{
            //    for(int j = 0; j < redCoeffs.GetLength(1); j++)
            //    {
            //        tw1.Write("["+c+"]"+coeffs[i,j]);
            //        c++;
            //    }
            //    tw1.WriteLine();
            //}

            //tw1.Close();
            //double[,] ConvertEdgeToMatrix = Embed.ConvertEdgeToMatrix(new Bitmap(transformedImage.Image));
            //transformedImage.Image = ImageProcessing.LaplaceEdge(new Bitmap(transformedImage.Image));
            //double[,] GreenMatrix = ImageProcessing.ConvertToMatrix2(new Bitmap(hostImage.Image)).Item2;
            //double[,] db2Coeffs = db2.WaveletCoeff(GreenMatrix, true, 2);
            //double[,] k = Daubechies2.InverseDb2Kernel(512);
            //TextWriter tw1 = new StreamWriter("Daubechies2_Kernel2.txt");
            ////tw1.WriteLine("Total Real Watermark: " );
            //int c = 1;
            //for (int i = 0; i < k.GetLength(0); i++)
            //{
            //    for (int j = 0; j < k.GetLength(1); j++)
            //    {
            //        tw1.Write("[" + c + "]" + k[i, j]);
            //        c++;
            //    }
            //    tw1.WriteLine();
            //}
            //tw1.Close();

            double[,] pixels = ImageProcessing.ConvertToMatrix2(new Bitmap(hostImage.Image)).Item1;
            double[,] coeffs = Daubechies3.WaveletCoeff(pixels, true, 2);

            transformedImage.Image = Daubechies3.TransformDWT(true, false, 2, new Bitmap(hostImage.Image)).Item4;
            double[,] inversePixels = Daubechies3.WaveletCoeff(coeffs, false, 2);
            transformedImage.Image = ImageProcessing.ConvertToBitmap2(inversePixels, inversePixels, inversePixels);


            //TextWriter tw1 = new StreamWriter("Daubechies2_Coeffs.txt");
            ////tw1.WriteLine("Total Real Watermark: " );
            //int c = 1;
            //for (int i = 0; i < coeffs.GetLength(0); i++)
            //{
            //    for (int j = 0; j < coeffs.GetLength(1); j++)
            //    {
            //        tw1.Write("[" + c + "]" + coeffs[i, j]);
            //        c++;
            //    }
            //    tw1.WriteLine();
            //}
            //tw1.Close();
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            double mse = Statistic.MSE(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
            double psnr = Statistic.PSNR(new Bitmap(transformedImage.Image), mse);
            MSEValue.Text = Math.Round(mse, 2).ToString();
            PSNRValue.Text = Math.Round(psnr, 2).ToString();
        }

        private void button13_Click_1(object sender, EventArgs e)
        {

        }

        private void button19_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(brightnessValue.Text);
            transformedImage.Image = ImageAttack.SetBrightness(WatermarkedImage, value);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(contrastValue.Text);
            transformedImage.Image = ImageAttack.SetContrast(WatermarkedImage, value);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            int redvalue = Convert.ToInt32(redgammaValue.Text);
            int greenvalue = Convert.ToInt32(greengammavalue.Text);
            int bluevalue = Convert.ToInt32(bluegammavalue.Text);
            transformedImage.Image = ImageAttack.SetGamma(WatermarkedImage,redvalue,greenvalue,bluevalue);
        }



        #endregion

        ///END
    }
}
