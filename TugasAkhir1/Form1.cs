using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Imaging;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;


namespace TugasAkhir1
{
    public partial class Form1 : Form
    {
        public Bitmap OriginalImage { get; set; }
        public Bitmap DWTImage { get; set; }

        //Global Variables
        public double[,] Wavelet_Coefficients;
        public double[,] Watermarked_Wavelet_Coefficients;
        public List<int> Scrambled_Watermark = new List<int>();
        public double[,] Embedded_Wavelet_Coefficients; 
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
        public Form1()
        {
            InitializeComponent();
            this.hostImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.transformedImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.watermarkImage.SizeMode = PictureBoxSizeMode.Zoom;

            //GUI Initialize
            HostImageLocationTxt.Text = "Browse Image to be inserted watermark";
            HostImageLocationTxt.ForeColor = Color.LightGray;
            WatermarkImageLocationTxt.Text = "Browse Watermark Image";
            WatermarkImageLocationTxt.ForeColor = Color.LightGray;
            WatermarkedImageTxt.Text = "Browse Watermarked Image to be Attacked";
            WatermarkedImageTxt.ForeColor = Color.LightGray;
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
            PSNRlbl.BackColor = Color.Gray;
            MSElbl.BackColor = Color.Gray;
            BERlbl.BackColor = Color.Gray;
            BERValue.BackColor = Color.Gray;
            PSNRValue.BackColor = Color.Gray;
            TimeExecTxt.Text = "0";
            BERValue.Text = "0";
            PSNRValue.Text = "0";
            StatusTxt.Refresh();
            label4.Refresh();
            label5.Refresh();
            PSNRlbl.Refresh();
            BERlbl.Refresh();
            PSNRValue.Refresh();
            BERValue.Refresh();
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
            BERValue.Text = String.Format("{0:0.00}", ber);//ber.ToString();
            MSEValue.Text = String.Format("{0:0.00}", mse);//mse.ToString();
            MSElbl.BackColor = Color.LightSkyBlue;
            PSNRlbl.BackColor = Color.LightSkyBlue;
            BERlbl.BackColor = Color.LightSkyBlue;
            BERValue.BackColor = Color.LightSkyBlue;
            PSNRValue.BackColor = Color.LightSkyBlue;
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
                Bitmap bmp = Create24bpp(transformedImage.Image); ////Resave image using 24 bit format.

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
                WatermarkedImageTxt.Text = ofd.FileName;
                transformedImage.Image = new Bitmap(ofd.FileName);
                WatermarkedImage = new Bitmap(transformedImage.Image);

                //Enable button
                histeqBtn.Enabled = true;
                meanFilterBtn.Enabled = true;
                medianFilterBtn.Enabled = true;
                modusFilterBtn.Enabled = true;
                jpegencoderBtn.Enabled = true;

                histeqBtn.BackColor = Color.DeepSkyBlue;
                meanFilterBtn.BackColor = Color.DeepSkyBlue;
                medianFilterBtn.BackColor = Color.DeepSkyBlue;
                modusFilterBtn.BackColor = Color.DeepSkyBlue;
                jpegencoderBtn.BackColor = Color.DeepSkyBlue;

            }
        }


        /// <summary>
        /// Forward DWT Transform and Extract the Wavelet Coefficients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) //Forward Transform
        {
            if (hostImage.Image != null)
            {
                GUIStart("Processing...!");
                ///For Visualization
                OriginalImage = new Bitmap(hostImage.Image);
                transformedImage.Image = DWT.TransformDWT(true, false, 2, OriginalImage);

                ///For Wavelet Coefficients Extraction
                Bitmap b = new Bitmap(hostImage.Image);
                IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
                IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
                IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;
                //double[,] IMatrix = ImageProcessing.ConvertToMatrix(b);
                //Test
                //MessageBox.Show("Red: " + IMatrixR[0, 0].ToString() + ", Green: " + IMatrixG[0, 0].ToString() + ", Blue: " + IMatrixB[0, 0].ToString(), "Values of RGB");

                double[,] ArrayImage = IMatrixG; //Embedding in Green 
                Wavelet_Coefficients = DWT.WaveletCoeff(ArrayImage, true, 2);
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
                GUIStart("Processing......!");
                //double[,] coefficients = coeffs;
                Bitmap bmp = new Bitmap(watermarkImage.Image);
                Scramble m = new Scramble();
                Bitmap b = ImageProcessing.ConvertToBinary(bmp);
                List<int> VectorImage = Scramble.ConvertToVectorMatrix(b); //Include integer values between 255 or 0
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
                string name = hostName +"_"+ watermarkName +"_Key.txt";
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

        public static Bitmap Create8bpp(Image image)
        {
            Bitmap b = new Bitmap(image);
            Bitmap bmp = ImageProcessing.CopyToBpp(b, 8);
            return bmp;          
        }
        #region Save Extracted Watermark
        private void button6_Click(object sender, EventArgs e) //Save Watermark
        {
            if (watermarkImage.Image != null)
            {
                Bitmap bmp = new Bitmap(watermarkImage.Image);
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



        private void button9_Click(object sender, EventArgs e)
        {
            //int c = 0;
            //double[,] listOfCoeffs = DWT.ListOfCoeffs(Wavelet_Coefficients);
            ////double[] scale2 = DWT.Scale2To1DCoeff(Wavelet_Coefficients);
            ////double[,] scale1 = DWT.Scale1To1DCoeff(Wavelet_Coefficients);
            //TextWriter tw1 = new StreamWriter("List Of Wavelet trees.txt");
            //tw1.WriteLine("Total Real Watermark: " + listOfCoeffs.GetLength(0));
            //for (int i = 0; i < listOfCoeffs.GetLength(0); i++)
            //{
            //    for (int j = 0; j < listOfCoeffs.GetLength(1); j++)
            //    {
            //        tw1.Write("["+j+"]"+ listOfCoeffs[i, j]);
            //    }
            //    tw1.WriteLine();
            //}
            ////foreach (double i in scale2)
            ////    tw1.WriteLine(i);
            //tw1.Close();

            //double[][] nestedArray = HMM.ConvertToNestedArray(Wavelet_Coefficients);
            //TextWriter tw2 = new StreamWriter("List Of Nested Wavelet trees.txt");
            //tw2.WriteLine("Total Real Watermark: " + listOfCoeffs.GetLength(0));
            //for (int i = 0; i < listOfCoeffs.GetLength(0); i++)
            //{
            //    for (int j = 0; j < listOfCoeffs.GetLength(1); j++)
            //    {
            //        tw2.Write("[" + j + "]" + nestedArray[i][j]);
            //    }
            //    tw2.WriteLine();
            //}
            //tw2.Close();

            //double[][] TreeOfWatermark = Extract.TreeOfWatermark2(nestedArray, 8100);
            //TextWriter tw3 = new StreamWriter("List Of Real Watermark Wavelet trees.txt");
            //tw3.WriteLine("Total Real Watermark: " + TreeOfWatermark.GetLength(0));
            //for (int i = 0; i < TreeOfWatermark.GetLength(0); i++)
            //{
            //    for (int j = 0; j < TreeOfWatermark[i].Length; j++)
            //    {
            //        tw3.Write("[" + j + "]" + TreeOfWatermark[i][j]);
            //    }
            //    tw3.WriteLine();
            //}
            //tw3.Close();

            Bitmap EdgyImage = ImageProcessing.LaplaceEdge(new Bitmap(transformedImage.Image));
            double[,] hvs = ImageProcessing.HVS(EdgyImage);
            TextWriter tw3 = new StreamWriter("HVS VALUES2.txt");
            //tw3.WriteLine("Total Real Watermark: " + hvs.GetLength(0));
            for (int i = 0; i < hvs.GetLength(0); i++)
            {
                for (int j = 0; j < hvs.GetLength(1); j++)
                {
                    tw3.Write(hvs[i,j]+"#");
                }
                tw3.WriteLine();
            }
            tw3.Close();

        }





        private void button8_Click(object sender, EventArgs e) /// Do Embedding Process at once
        {
            //var mse = Statistic.MSE(new Bitmap(hostImage.Image), new Bitmap(watermarkImage.Image));
            //var psnr = Statistic.PSNR(new Bitmap(watermarkImage.Image), mse);
            //var ber = Statistic.BER(new Bitmap(hostImage.Image), new Bitmap(watermarkImage.Image));
            //MessageBox.Show("MSE: " + mse + "\n" + "PSNR: " + psnr + "\n" + "BER: " + ber, "succeed", MessageBoxButtons.OK);

            GUIStart("Processing.....!");

            #region Decompose Image into 2 level Haar DWT
            Bitmap TransormedImage = DWT.TransformDWT(true, false, 2, new Bitmap(hostImage.Image));
            Bitmap b = new Bitmap(hostImage.Image);
            IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
            IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
            IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;
            //double[,] IMatrix = ImageProcessing.ConvertToMatrix(b);
            //Test
            //MessageBox.Show("Red: " + IMatrixR[0, 0].ToString() + ", Green: " + IMatrixG[0, 0].ToString() + ", Blue: " + IMatrixB[0, 0].ToString(), "Values of RGB");

            double[,] ArrayImage = IMatrixG; //Embedding in Green 
            double[,] coefficients = DWT.WaveletCoeff(ArrayImage, true, 2);
            #endregion

            #region Watermark Scrambling
            Bitmap bmp = new Bitmap(watermarkImage.Image);
            Scramble m = new Scramble();
            Bitmap b2 = ImageProcessing.ConvertToBinary(bmp);
            List<int> VectorImage = Scramble.ConvertToVectorMatrix(b2); //Include integer values between 255 or 0
            List<int> BinaryVectorImage = Scramble.ConvertToBinaryVectorMatrix(VectorImage); //Include integer values between 1 or 0
                                                                                             //Real_Watermark = BinaryVectorImage;

            #region Save key into TXT
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
            }
            #endregion

            //List<int> oneminusone = Scramble.ConvertTo1minus1(scrambled_Watermark);
            List<int> dsssWatermark = scrambled_Watermark;
            #endregion

            #region Embed Watermark into decomposed host Image
            List<List<int>> Segmented = Scramble.Segment(dsssWatermark);
            //double[,] MappedWatermark = Scramble.Mapping(Segmented);
            double[,] MappedWatermark = Scramble.Mapping2(Segmented);
            Mapped_Watermark = MappedWatermark;

            double[,] HVSValues = new double[coefficients.GetLength(0), coefficients.GetLength(1)];
            if (TransormedImage == null)
            {
                MessageBox.Show("Do Forward Transform First!", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }
            else
            {
                Bitmap EdgyImage = ImageProcessing.LaplaceEdge(new Bitmap(TransormedImage));
                HVSValues = ImageProcessing.HVS(EdgyImage);
            }
            double[,] EmbeddedWatermark = Embed.Embedding(coefficients, MappedWatermark, HVSValues);
            double[,] embedded_coefficients = EmbeddedWatermark;
            #endregion

            #region Inverse DWT
            double[,] InverseDWT = DWT.WaveletCoeff(embedded_coefficients, false, 2);

            /// Round all elements in InverseDWT
            //double[,] RoundedInversedDWT = Statistic.RoundAll(InverseDWT);

            //transformedImage.Image = ImageProcessing.ConvertToBitmap(InverseDWT);
            transformedImage.Image = ImageProcessing.ConvertToBitmap2(IMatrixR, InverseDWT, IMatrixB);
            double mse = Statistic.MSE(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
            double psnr = Statistic.PSNR(new Bitmap(transformedImage.Image), mse);
            double ber = Statistic.BER(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
            #endregion

            GUIEnd("IDWT Succeed!", mse, psnr, ber);

            #region Update UI
            ///Activate Attack Button
            histeqBtn.Enabled = true;
            histeqBtn.BackColor = Color.DeepSkyBlue;
            meanFilterBtn.Enabled = true;
            meanFilterBtn.BackColor = Color.DeepSkyBlue;
            medianFilterBtn.Enabled = true;
            medianFilterBtn.BackColor = Color.DeepSkyBlue;
            modusFilterBtn.Enabled = true;
            modusFilterBtn.BackColor = Color.DeepSkyBlue;
            jpegencoderBtn.Enabled = true;
            jpegencoderBtn.BackColor = Color.DeepSkyBlue;

            resultLbl.Text = "Watermarked Host Image";
            WatermarkedImage = new Bitmap(transformedImage.Image);
            #endregion
        }

        private void button18_Click(object sender, EventArgs e) /// Extract Watermark At Once
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

                
                //transformedImage.Image = DWT.TransformDWT(true, false, 2, OriginalImage);

                ///For Wavelet Coefficients Extraction
                Bitmap b = new Bitmap(transformedImage.Image);
                IMatrixR = ImageProcessing.ConvertToMatrix2(b).Item1;
                IMatrixG = ImageProcessing.ConvertToMatrix2(b).Item2;
                IMatrixB = ImageProcessing.ConvertToMatrix2(b).Item3;

                double[,] ArrayImage = IMatrixG; //Embedding in Green 
                Watermarked_Wavelet_Coefficients = DWT.WaveletCoeff(ArrayImage, true, 2);
                int NumOfScale2 = ((hostheight * hostwidth) / 16) * 3;
                ExtractedWatermark = Extract.BaumWelchDetectionInLH_2(Watermarked_Wavelet_Coefficients, transformedImage.Image, NumOfScale2, NumOfTrees, PNSeq /*, rootpmf, transition, variances*/);
            }else
            {
                MessageBox.Show("Select Key First!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
            }
            #endregion

            #region Extraction
            List<int> PNSeq2 = new List<int>();

            StreamReader objstream2 = new StreamReader(KeyFileName);
            string[] lines2 = objstream2.ReadToEnd().Split(new char[] { '\n' });
            int height = Convert.ToInt32(lines2[4]);
            int width = Convert.ToInt32(lines2[5]);
            int NumOfTrees2 = Convert.ToInt32(lines2[7]);
            for (int i = 9; i < lines2.Length - 1; i++)
            {
                PNSeq2.Add(Convert.ToInt32(lines2[i]));
            }

            Bitmap bmp = ImageProcessing.ConvertListToWatermark2(ExtractedWatermark, height, width);
            watermarkImage.Image = bmp;
            #endregion

            GUIEnd("Watermark Extracted!", 0, 0, 0);
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
                GUIStart("Processing.....!");
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
                AdaptiveHVS = Embed.AdaptiveHVS(Wavelet_Coefficients);

                double[,] EmbeddedWatermark = Embed.Embedding(Wavelet_Coefficients,MappedWatermark, AdaptiveHVS);
                Embedded_Wavelet_Coefficients = EmbeddedWatermark;

                //EMBEDDED_WATERMARK_For_Extraction = Embedded_Wavelet_Coefficients;
                /// Test
                //int h = 1;
                //TextWriter tw1 = new StreamWriter("WaveletCoefficientsAfterEmbedding.txt");
                //tw1.WriteLine("Total Watermark: " + EmbeddedWatermark.GetLength(0));
                //for (int i = 0; i < EmbeddedWatermark.GetLength(0); i++)
                //{
                //    for (int j = 0; j < EmbeddedWatermark.GetLength(1); j++)
                //    {
                //        tw1.Write("[" + h + "]" + EmbeddedWatermark[i,j] + " # ");
                //        h++;
                //    }
                //    tw1.WriteLine();
                //}
                ////foreach (double i in ExtractedWatermark)
                ////{
                ////    tw1.WriteLine(i);
                ////}
                //tw1.Close();

                GUIEnd("Embedding Succeed!", 0, 0, 0);
                //MessageBox.Show("Embedding Succeed!", "Embedding Process : "+Segmented.Count, MessageBoxButtons.OK);
            }
        }


        /// <summary>
        /// Inverse DWT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e/*, double[,] Embedded_Coefficients*/) //Inverse Transform
        {
            if (transformedImage.Image == null)
            {
                MessageBox.Show("There is no Transformed image yet", "Incomplete Procedure Detected!", MessageBoxButtons.OK);
            }
            else
            {
                GUIStart("Processing.....!");
                //MessageBox.Show("Embedded Wavelet Coefficients: "+Embedded_Wavelet_Coefficients[0,0],"blabal",MessageBoxButtons.OK);
                double[,] InverseDWT = DWT.WaveletCoeff(Embedded_Wavelet_Coefficients, false, 2);

                /// Round all elements in InverseDWT
                //double[,] RoundedInversedDWT = Statistic.RoundAll(InverseDWT);

                //transformedImage.Image = ImageProcessing.ConvertToBitmap(InverseDWT);
                transformedImage.Image = ImageProcessing.ConvertToBitmap2(IMatrixR, InverseDWT, IMatrixB);
                double mse = Statistic.MSE(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
                double psnr = Statistic.PSNR(new Bitmap(transformedImage.Image), mse);
                double ber = Statistic.BER(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
                GUIEnd("IDWT Succeed!",mse,psnr,ber);

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
                histeqBtn.Enabled = true;
                histeqBtn.BackColor = Color.DeepSkyBlue;
                meanFilterBtn.Enabled = true;
                meanFilterBtn.BackColor = Color.DeepSkyBlue;
                medianFilterBtn.Enabled = true;
                medianFilterBtn.BackColor = Color.DeepSkyBlue;
                modusFilterBtn.Enabled = true;
                modusFilterBtn.BackColor = Color.DeepSkyBlue;
                jpegencoderBtn.Enabled = true;
                jpegencoderBtn.BackColor = Color.DeepSkyBlue;

                resultLbl.Text = "Watermarked Host Image";
                WatermarkedImage = new Bitmap(transformedImage.Image);
                //test
                //MessageBox.Show("Red: " + IMatrixR[0, 0].ToString() + ", Green: " + IMatrixG[0, 0].ToString() + ", Blue: " + IMatrixB[0, 0].ToString(), "Values of RGB");
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            //Scale 2
            List<double> LL2 = new List<double>();
            List<double> LH2 = new List<double>();
            List<double> HH2 = new List<double>();
            List<double> HL2 = new List<double>();

            //Scale 1
            List<double> LH1 = new List<double>();
            List<double> HH1 = new List<double>();
            List<double> HL1 = new List<double>();

            //LL2
            for (int i = 0; i < Wavelet_Coefficients.GetLength(0) / 4; i++)
            {
                for (int j = 0; j < Wavelet_Coefficients.GetLength(1) / 4; j++)
                {
                    LL2.Add(Wavelet_Coefficients[i, j]);
                }
            }

            //LH2
            for (int i = 0; i < Wavelet_Coefficients.GetLength(0) / 4; i++)
            {
                for (int j = Wavelet_Coefficients.GetLength(1) / 4; j < Wavelet_Coefficients.GetLength(1) / 2; j++)
                {
                    LH2.Add(Wavelet_Coefficients[i, j]);
                }
            }

            //HH2
            for (int i = Wavelet_Coefficients.GetLength(0) / 4; i < Wavelet_Coefficients.GetLength(0) / 2; i++)
            {
                for (int j = Wavelet_Coefficients.GetLength(1) / 4; j < Wavelet_Coefficients.GetLength(1) / 2; j++)
                {
                    HH2.Add(Wavelet_Coefficients[i, j]);
                }
            }

            //HL2
            for (int i = Wavelet_Coefficients.GetLength(0) / 4; i < Wavelet_Coefficients.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < Wavelet_Coefficients.GetLength(1) / 4; j++)
                {
                    HL2.Add(Wavelet_Coefficients[i, j]);
                }
            }


            //LH1
            for (int i = 0 ; i < Wavelet_Coefficients.GetLength(0) / 2; i++)
            {
                for (int j = Wavelet_Coefficients.GetLength(1) / 2; j < Wavelet_Coefficients.GetLength(1); j++)
                {
                    LH1.Add(Wavelet_Coefficients[i, j]);
                }
            }

            //HH1
            for (int i = Wavelet_Coefficients.GetLength(0) / 2; i < Wavelet_Coefficients.GetLength(0); i++)
            {
                for (int j = Wavelet_Coefficients.GetLength(1) / 2; j < Wavelet_Coefficients.GetLength(1); j++)
                {
                    HH1.Add(Wavelet_Coefficients[i, j]);
                }
            }

            //HL1
            for (int i = Wavelet_Coefficients.GetLength(0) / 2; i < Wavelet_Coefficients.GetLength(0); i++)
            {
                for (int j = 0; j < Wavelet_Coefficients.GetLength(1)/2; j++)
                {
                    HL1.Add(Wavelet_Coefficients[i, j]);
                }
            }

            //LL2.Sort();
            //LH2.Sort();
            //HH2.Sort();
            //HL2.Sort();

            //LH1.Sort();
            //HH1.Sort();
            //HL1.Sort();
            //TextWriter tw1 = new StreamWriter("SortedLenaWCLL2.txt");
            //tw1.WriteLine("Total of Wavelet Coefficients: " + LL2.Count);
            //foreach (double i in LL2)
            //    tw1.WriteLine(i);
            //tw1.Close();

            //TextWriter tw2 = new StreamWriter("SortedLenaWCLH2.txt");
            //tw2.WriteLine("Total of Wavelet Coefficients: " + LH2.Count);
            //foreach (double i in LH2)
            //    tw2.WriteLine(i);
            //tw2.Close();

            //TextWriter tw3 = new StreamWriter("SortedLenaWCHH2.txt");
            //tw3.WriteLine("Total of Wavelet Coefficients: " + HH2.Count);
            //foreach (double i in HH2)
            //    tw3.WriteLine(i);
            //tw3.Close();

            //TextWriter tw4 = new StreamWriter("SortedLenaWCHL2.txt");
            //tw4.WriteLine("Total of Wavelet Coefficients: " + HL2.Count);
            //foreach (double i in HL2)
            //    tw4.WriteLine(i);
            //tw4.Close();

            //TextWriter tw5 = new StreamWriter("SortedLenaWCLH1.txt");
            //tw5.WriteLine("Total of Wavelet Coefficients: " + LH1.Count);
            //foreach (double i in LH1)
            //    tw5.WriteLine(i);
            //tw5.Close();

            //TextWriter tw6 = new StreamWriter("SortedLenaWCHH1.txt");
            //tw6.WriteLine("Total of Wavelet Coefficients: " + HH1.Count);
            //foreach (double i in HH1)
            //    tw6.WriteLine(i);
            //tw6.Close();

            //TextWriter tw7 = new StreamWriter("LenaWCHL1_2.txt");
            //tw7.WriteLine("Total of Wavelet Coefficients: " + HL1.Count);
            //foreach (double i in HL1)
            //    tw7.WriteLine(i);
            //tw7.Close();
            double meanLL2 = Statistic.Mode2(LL2);
            double meanLH2 = Statistic.Mode2(LH2);
            double meanHH2 = Statistic.Mode2(HH2);
            double meanHL2 = Statistic.Mode2(HL2);

            double meanLH1 = Statistic.Mode2(LH1);
            double meanHH1 = Statistic.Mode2(HH1);
            double meanHL1 = Statistic.Mode2(HL1);

            double mode = HMM.Threshold2(Wavelet_Coefficients);
            MessageBox.Show("LL2: " + meanLL2 
                + "\n LH2: " + meanLH2 
                + "\n HH2: " + meanHH2 
                + "\n HL2: " + meanHL2 
                + "\n LH1: " + meanLH1 
                + "\n HH1: " + meanHH1 
                + "\n HL1: " + meanHL1+"  "+mode,"Succeed", MessageBoxButtons.OK);
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
                transformedImage.Image = ImageAttack.MeanFilter(WatermarkedImage, 1);
                resultLbl.Text = "Mean Filter of Watermarked Image";
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
                transformedImage.Image = ImageAttack.MedianFilter(WatermarkedImage, 3);
                resultLbl.Text = "Median Filter of Watermarked Image";
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
                transformedImage.Image = ImageAttack.ModusFilter(WatermarkedImage, 1);
                resultLbl.Text = "Modus Filter of Watermarked Image";
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
                transformedImage.Image = ImageAttack.JpegCompress(WatermarkedImage, 20, "CompressedImage20.jpg");
                MessageBox.Show("Image Successfully Compressed!", "Success", MessageBoxButtons.OK);
                resultLbl.Text = "Compressed Image";
            }
            else
            {
                MessageBox.Show("Watermark the host image first!", "Incomplete Procedure Detected");
            }
        }

        #endregion

        private void button14_Click_1(object sender, EventArgs e)
        {
            //double[,] hiddenstates = HMM.GetHiddenStateValue(Wavelet_Coefficients);
            //double threshold = HMM.Threshold2(Wavelet_Coefficients);
            //double probm1 = HMM.StateProbability2(hiddenstates, 1, 1);
            //double probm2 = HMM.StateProbability2(hiddenstates, 1, 2);
            //double prob = probm1 + probm2; //Summation of the probability should be 1
            ////MessageBox.Show("Prob m = 1(High): " + probm1 + "\n Prob m = 2(Low): " + probm2 + "\n Sum: " + prob + "\n Threshold: " + threshold, "Succeed", MessageBoxButtons.OK);
            //HMM.TransitionProbability(Wavelet_Coefficients, 1, 1);

            ////MessageBox.Show("256: " + Wavelet_Coefficients[0, 256] + "\n 257: " + Wavelet_Coefficients[0, 257] + "\n 768:" + Wavelet_Coefficients[1, 256] + "\n 769:" + Wavelet_Coefficients[1, 257]);
            ////double mean = HMM.Threshold2(Wavelet_Coefficients);
            ////MessageBox.Show("Mean: " + mean, "Succeed!", MessageBoxButtons.OK);

            //double[,] pixels = ImageProcessing.ConvertToMatrix2(new Bitmap(hostImage.Image)).Item2;
            //double[,] coeffs = DWT.WaveletCoeff(pixels,true,2);

            Bitmap laplaceEdge = ImageProcessing.LaplaceEdge(new Bitmap(hostImage.Image));
            transformedImage.Image = laplaceEdge;


        }

        private void button15_Click(object sender, EventArgs e)
        {
            double[,] hiddenstates = HMM.GetHiddenStateValue(Wavelet_Coefficients);
            int count1 = 0;
            int count2 = 0;
            for(int i = 0; i < hiddenstates.GetLength(0); i++)
            {
                for(int j = 0; j < hiddenstates.GetLength(1); j++)
                {
                    if(hiddenstates[i,j] == 1)
                    {
                        count1++;
                    }else
                    {
                        count2++;
                    }
                }
            }
            int summation = count1 + count2;
            MessageBox.Show("Count 1: " + count1 + "\n Count2: " + count2+"\n Sum:  "+summation,"Succeed", MessageBoxButtons.OK);
        }

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
                Watermarked_Wavelet_Coefficients = DWT.WaveletCoeff(ArrayImage, true, 2);                
                int NumOfScale2 = ((hostheight*hostwidth)/16)*3;



                ExtractedWatermark = Extract.BaumWelchDetectionInLH_2(Watermarked_Wavelet_Coefficients, transformedImage.Image, NumOfScale2, NumOfTrees, PNSeq /*, rootpmf, transition, variances*/);
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

        private void button16_Click(object sender, EventArgs e)
        {
            double mse = Statistic.MSE(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
            double psnr = Statistic.PSNR(new Bitmap(transformedImage.Image), mse);
            double ber = Statistic.BER(new Bitmap(hostImage.Image), new Bitmap(transformedImage.Image));
            MSEValue.Text = mse.ToString();
            PSNRValue.Text = psnr.ToString();
            BERValue.Text = ber.ToString();

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

                /// Using 15 bit mapping
                //CombinedTree = Extract.CombineTrees(detectedWatermark);
                //InversedMappingTriangle = Extract.InverseMapping(CombinedTree).Item1;
                //InversedMappingCircle = Extract.InverseMapping(CombinedTree).Item2;
                //InversedMappingSquare = Extract.InverseMapping(CombinedTree).Item3;
                //List<double> Merged = Scramble.MergeSegmentedWatermark(InversedMappingTriangle);
                //List<int> InversedDSSS = Scramble.InverseDSSS(Merged, PNSeq);
                //Bitmap bmp = ImageProcessing.ConvertListToWatermark(InversedDSSS, height, width);

                ///Not using 15 bit mapping
                Bitmap bmp = ImageProcessing.ConvertListToWatermark2(ExtractedWatermark, height, width);
                watermarkImage.Image = bmp;

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
                MessageBox.Show("Akurasi: " + akurasi, "Succeed!", MessageBoxButtons.OK);

                GUIEnd("Watermark Extracted!", 0, 0, 0);
            }
            else
            {
                MessageBox.Show("Train and Detect Watermark first!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
            }                
        }

        private void button19_Click(object sender, EventArgs e)
        {
            double variance = Statistic.VarianceOfImage(new Bitmap(hostImage.Image));
            //double variance = Statistic.MaxPixel(new Bitmap(hostImage.Image));
            MessageBox.Show("Variance: " + variance, "Variance Valculation", MessageBoxButtons.OK);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            int[,] green = Statistic.ExtractGreen(new Bitmap(hostImage.Image));
            TextWriter tw1 = new StreamWriter("DecomposedNewsGreenPixel.txt");
            //tw1.WriteLine("Total Watermark: " + green.GetLength(0));
            for (int i = 0; i < green.GetLength(0); i++)
            {
                for (int j = 0; j < green.GetLength(1); j++)
                {
                    tw1.WriteLine(green[i, j]+"#");
                }
            }
            tw1.Close();
        }

        ///END
    }
}
