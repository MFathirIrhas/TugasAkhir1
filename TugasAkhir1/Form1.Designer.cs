namespace TugasAkhir1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.hostImage = new System.Windows.Forms.PictureBox();
            this.HostImageLocationTxt = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button11 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.transformedImage = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.watermarkImage = new System.Windows.Forms.PictureBox();
            this.WatermarkImageLocationTxt = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.StatusPanel = new System.Windows.Forms.Panel();
            this.MSEValue = new System.Windows.Forms.Label();
            this.MSElbl = new System.Windows.Forms.Label();
            this.BERValue = new System.Windows.Forms.Label();
            this.PSNRValue = new System.Windows.Forms.Label();
            this.BERlbl = new System.Windows.Forms.Label();
            this.PSNRlbl = new System.Windows.Forms.Label();
            this.TimeExecTxt = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.StatusTxt = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.histeqBtn = new System.Windows.Forms.Button();
            this.meanFilterBtn = new System.Windows.Forms.Button();
            this.medianFilterBtn = new System.Windows.Forms.Button();
            this.modusFilterBtn = new System.Windows.Forms.Button();
            this.WatermarkedImageTxt = new System.Windows.Forms.TextBox();
            this.button12 = new System.Windows.Forms.Button();
            this.resultLbl = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hostImage)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transformedImage)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.watermarkImage)).BeginInit();
            this.StatusPanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.TabStop = false;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.hostImage);
            this.panel1.Controls.Add(this.HostImageLocationTxt);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(374, 360);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Host Image :";
            // 
            // hostImage
            // 
            this.hostImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.hostImage.InitialImage = ((System.Drawing.Image)(resources.GetObject("hostImage.InitialImage")));
            this.hostImage.Location = new System.Drawing.Point(3, 51);
            this.hostImage.Name = "hostImage";
            this.hostImage.Size = new System.Drawing.Size(366, 304);
            this.hostImage.TabIndex = 2;
            this.hostImage.TabStop = false;
            // 
            // HostImageLocationTxt
            // 
            this.HostImageLocationTxt.BackColor = System.Drawing.SystemColors.Window;
            this.HostImageLocationTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HostImageLocationTxt.Enabled = false;
            this.HostImageLocationTxt.ForeColor = System.Drawing.SystemColors.InfoText;
            this.HostImageLocationTxt.Location = new System.Drawing.Point(76, 3);
            this.HostImageLocationTxt.Multiline = true;
            this.HostImageLocationTxt.Name = "HostImageLocationTxt";
            this.HostImageLocationTxt.Size = new System.Drawing.Size(293, 23);
            this.HostImageLocationTxt.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.button2.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(7, 369);
            this.button2.Name = "button2";
            this.button2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button2.Size = new System.Drawing.Size(80, 40);
            this.button2.TabIndex = 2;
            this.button2.TabStop = false;
            this.button2.Text = "FDWT";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.resultLbl);
            this.panel2.Controls.Add(this.WatermarkedImageTxt);
            this.panel2.Controls.Add(this.button12);
            this.panel2.Controls.Add(this.button10);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.transformedImage);
            this.panel2.Location = new System.Drawing.Point(383, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(653, 656);
            this.panel2.TabIndex = 3;
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(595, 3);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(75, 23);
            this.button11.TabIndex = 14;
            this.button11.Text = "Grayscale";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(483, 3);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 23);
            this.button9.TabIndex = 12;
            this.button9.Text = "button9";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button10.FlatAppearance.BorderSize = 0;
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.Location = new System.Drawing.Point(573, 628);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 23);
            this.button10.TabIndex = 13;
            this.button10.TabStop = false;
            this.button10.Text = "Save";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Result :";
            // 
            // transformedImage
            // 
            this.transformedImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.transformedImage.InitialImage = ((System.Drawing.Image)(resources.GetObject("transformedImage.InitialImage")));
            this.transformedImage.Location = new System.Drawing.Point(3, 23);
            this.transformedImage.Name = "transformedImage";
            this.transformedImage.Size = new System.Drawing.Size(645, 603);
            this.transformedImage.TabIndex = 0;
            this.transformedImage.TabStop = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.button3.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(7, 519);
            this.button3.Name = "button3";
            this.button3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button3.Size = new System.Drawing.Size(80, 40);
            this.button3.TabIndex = 4;
            this.button3.TabStop = false;
            this.button3.Text = "IDWT";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.watermarkImage);
            this.panel3.Location = new System.Drawing.Point(93, 415);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(284, 244);
            this.panel3.TabIndex = 5;
            // 
            // watermarkImage
            // 
            this.watermarkImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.watermarkImage.InitialImage = ((System.Drawing.Image)(resources.GetObject("watermarkImage.InitialImage")));
            this.watermarkImage.Location = new System.Drawing.Point(3, 3);
            this.watermarkImage.Name = "watermarkImage";
            this.watermarkImage.Size = new System.Drawing.Size(276, 236);
            this.watermarkImage.TabIndex = 0;
            this.watermarkImage.TabStop = false;
            // 
            // WatermarkImageLocationTxt
            // 
            this.WatermarkImageLocationTxt.BackColor = System.Drawing.SystemColors.Window;
            this.WatermarkImageLocationTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.WatermarkImageLocationTxt.Enabled = false;
            this.WatermarkImageLocationTxt.ForeColor = System.Drawing.SystemColors.InfoText;
            this.WatermarkImageLocationTxt.Location = new System.Drawing.Point(167, 369);
            this.WatermarkImageLocationTxt.Multiline = true;
            this.WatermarkImageLocationTxt.Name = "WatermarkImageLocationTxt";
            this.WatermarkImageLocationTxt.Size = new System.Drawing.Size(210, 23);
            this.WatermarkImageLocationTxt.TabIndex = 4;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(93, 369);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 4;
            this.button4.TabStop = false;
            this.button4.Text = "Browse";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(7, 415);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(80, 46);
            this.button5.TabIndex = 6;
            this.button5.TabStop = false;
            this.button5.Text = "Scramble Watermark";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button6.FlatAppearance.BorderSize = 0;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Location = new System.Drawing.Point(7, 636);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(80, 23);
            this.button6.TabIndex = 7;
            this.button6.TabStop = false;
            this.button6.Text = "TEST";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 400);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Watermark Image:";
            // 
            // StatusPanel
            // 
            this.StatusPanel.BackColor = System.Drawing.Color.LightSkyBlue;
            this.StatusPanel.Controls.Add(this.MSEValue);
            this.StatusPanel.Controls.Add(this.button11);
            this.StatusPanel.Controls.Add(this.MSElbl);
            this.StatusPanel.Controls.Add(this.BERValue);
            this.StatusPanel.Controls.Add(this.button9);
            this.StatusPanel.Controls.Add(this.PSNRValue);
            this.StatusPanel.Controls.Add(this.BERlbl);
            this.StatusPanel.Controls.Add(this.PSNRlbl);
            this.StatusPanel.Controls.Add(this.TimeExecTxt);
            this.StatusPanel.Controls.Add(this.label6);
            this.StatusPanel.Controls.Add(this.label5);
            this.StatusPanel.Controls.Add(this.StatusTxt);
            this.StatusPanel.Controls.Add(this.label4);
            this.StatusPanel.Location = new System.Drawing.Point(3, 661);
            this.StatusPanel.Name = "StatusPanel";
            this.StatusPanel.Size = new System.Drawing.Size(1132, 31);
            this.StatusPanel.TabIndex = 9;
            // 
            // MSEValue
            // 
            this.MSEValue.AutoSize = true;
            this.MSEValue.Location = new System.Drawing.Point(922, 10);
            this.MSEValue.Name = "MSEValue";
            this.MSEValue.Size = new System.Drawing.Size(13, 13);
            this.MSEValue.TabIndex = 10;
            this.MSEValue.Text = "0";
            // 
            // MSElbl
            // 
            this.MSElbl.AutoSize = true;
            this.MSElbl.Location = new System.Drawing.Point(890, 10);
            this.MSElbl.Name = "MSElbl";
            this.MSElbl.Size = new System.Drawing.Size(36, 13);
            this.MSElbl.TabIndex = 9;
            this.MSElbl.Text = "MSE :";
            // 
            // BERValue
            // 
            this.BERValue.AutoSize = true;
            this.BERValue.Location = new System.Drawing.Point(1086, 10);
            this.BERValue.Name = "BERValue";
            this.BERValue.Size = new System.Drawing.Size(13, 13);
            this.BERValue.TabIndex = 8;
            this.BERValue.Text = "0";
            // 
            // PSNRValue
            // 
            this.PSNRValue.AutoSize = true;
            this.PSNRValue.Location = new System.Drawing.Point(1011, 10);
            this.PSNRValue.Name = "PSNRValue";
            this.PSNRValue.Size = new System.Drawing.Size(13, 13);
            this.PSNRValue.TabIndex = 7;
            this.PSNRValue.Text = "0";
            // 
            // BERlbl
            // 
            this.BERlbl.AutoSize = true;
            this.BERlbl.Location = new System.Drawing.Point(1055, 10);
            this.BERlbl.Name = "BERlbl";
            this.BERlbl.Size = new System.Drawing.Size(38, 13);
            this.BERlbl.TabIndex = 6;
            this.BERlbl.Text = "BER : ";
            // 
            // PSNRlbl
            // 
            this.PSNRlbl.AutoSize = true;
            this.PSNRlbl.Location = new System.Drawing.Point(969, 10);
            this.PSNRlbl.Name = "PSNRlbl";
            this.PSNRlbl.Size = new System.Drawing.Size(46, 13);
            this.PSNRlbl.TabIndex = 5;
            this.PSNRlbl.Text = "PSNR : ";
            // 
            // TimeExecTxt
            // 
            this.TimeExecTxt.AutoSize = true;
            this.TimeExecTxt.Location = new System.Drawing.Point(264, 10);
            this.TimeExecTxt.Name = "TimeExecTxt";
            this.TimeExecTxt.Size = new System.Drawing.Size(13, 13);
            this.TimeExecTxt.TabIndex = 4;
            this.TimeExecTxt.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(187, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 13);
            this.label6.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(172, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Execution Time: ";
            // 
            // StatusTxt
            // 
            this.StatusTxt.AutoSize = true;
            this.StatusTxt.ForeColor = System.Drawing.Color.DarkGreen;
            this.StatusTxt.Location = new System.Drawing.Point(49, 10);
            this.StatusTxt.Name = "StatusTxt";
            this.StatusTxt.Size = new System.Drawing.Size(38, 13);
            this.StatusTxt.TabIndex = 1;
            this.StatusTxt.Text = "Ready";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Status: ";
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button7.FlatAppearance.BorderSize = 0;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(7, 467);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(80, 46);
            this.button7.TabIndex = 10;
            this.button7.TabStop = false;
            this.button7.Text = "Embed Watermark";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button8.FlatAppearance.BorderSize = 0;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.Location = new System.Drawing.Point(7, 584);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(80, 46);
            this.button8.TabIndex = 11;
            this.button8.TabStop = false;
            this.button8.Text = "Extract Watermark";
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label7.Location = new System.Drawing.Point(4, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Attack:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.modusFilterBtn);
            this.panel4.Controls.Add(this.medianFilterBtn);
            this.panel4.Controls.Add(this.meanFilterBtn);
            this.panel4.Controls.Add(this.histeqBtn);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Location = new System.Drawing.Point(1038, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(97, 656);
            this.panel4.TabIndex = 13;
            // 
            // histeqBtn
            // 
            this.histeqBtn.BackColor = System.Drawing.Color.LightSlateGray;
            this.histeqBtn.Enabled = false;
            this.histeqBtn.FlatAppearance.BorderSize = 0;
            this.histeqBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.histeqBtn.Location = new System.Drawing.Point(3, 26);
            this.histeqBtn.Name = "histeqBtn";
            this.histeqBtn.Size = new System.Drawing.Size(91, 23);
            this.histeqBtn.TabIndex = 13;
            this.histeqBtn.Text = "HistEq";
            this.histeqBtn.UseVisualStyleBackColor = false;
            this.histeqBtn.Click += new System.EventHandler(this.button12_Click);
            // 
            // meanFilterBtn
            // 
            this.meanFilterBtn.BackColor = System.Drawing.Color.SlateGray;
            this.meanFilterBtn.Enabled = false;
            this.meanFilterBtn.FlatAppearance.BorderSize = 0;
            this.meanFilterBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.meanFilterBtn.Location = new System.Drawing.Point(3, 55);
            this.meanFilterBtn.Name = "meanFilterBtn";
            this.meanFilterBtn.Size = new System.Drawing.Size(91, 23);
            this.meanFilterBtn.TabIndex = 14;
            this.meanFilterBtn.Text = "Mean Filter";
            this.meanFilterBtn.UseVisualStyleBackColor = false;
            this.meanFilterBtn.Click += new System.EventHandler(this.meanFilterBtn_Click);
            // 
            // medianFilterBtn
            // 
            this.medianFilterBtn.BackColor = System.Drawing.Color.SlateGray;
            this.medianFilterBtn.Enabled = false;
            this.medianFilterBtn.FlatAppearance.BorderSize = 0;
            this.medianFilterBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.medianFilterBtn.Location = new System.Drawing.Point(3, 84);
            this.medianFilterBtn.Name = "medianFilterBtn";
            this.medianFilterBtn.Size = new System.Drawing.Size(91, 23);
            this.medianFilterBtn.TabIndex = 15;
            this.medianFilterBtn.Text = "Median Filter";
            this.medianFilterBtn.UseVisualStyleBackColor = false;
            this.medianFilterBtn.Click += new System.EventHandler(this.medianFilterBtn_Click);
            // 
            // modusFilterBtn
            // 
            this.modusFilterBtn.BackColor = System.Drawing.Color.SlateGray;
            this.modusFilterBtn.Enabled = false;
            this.modusFilterBtn.FlatAppearance.BorderSize = 0;
            this.modusFilterBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.modusFilterBtn.Location = new System.Drawing.Point(3, 113);
            this.modusFilterBtn.Name = "modusFilterBtn";
            this.modusFilterBtn.Size = new System.Drawing.Size(91, 23);
            this.modusFilterBtn.TabIndex = 16;
            this.modusFilterBtn.Text = "Modus Filter";
            this.modusFilterBtn.UseVisualStyleBackColor = false;
            this.modusFilterBtn.Click += new System.EventHandler(this.modusFilterBtn_Click);
            // 
            // WatermarkedImageTxt
            // 
            this.WatermarkedImageTxt.BackColor = System.Drawing.SystemColors.Window;
            this.WatermarkedImageTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.WatermarkedImageTxt.Enabled = false;
            this.WatermarkedImageTxt.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.WatermarkedImageTxt.Location = new System.Drawing.Point(77, 630);
            this.WatermarkedImageTxt.Multiline = true;
            this.WatermarkedImageTxt.Name = "WatermarkedImageTxt";
            this.WatermarkedImageTxt.Size = new System.Drawing.Size(291, 20);
            this.WatermarkedImageTxt.TabIndex = 5;
            // 
            // button12
            // 
            this.button12.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button12.FlatAppearance.BorderSize = 0;
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button12.Location = new System.Drawing.Point(3, 630);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(75, 20);
            this.button12.TabIndex = 4;
            this.button12.TabStop = false;
            this.button12.Text = "Browse ";
            this.button12.UseVisualStyleBackColor = false;
            this.button12.Click += new System.EventHandler(this.button12_Click_1);
            // 
            // resultLbl
            // 
            this.resultLbl.AutoSize = true;
            this.resultLbl.Location = new System.Drawing.Point(53, 5);
            this.resultLbl.Name = "resultLbl";
            this.resultLbl.Size = new System.Drawing.Size(16, 13);
            this.resultLbl.TabIndex = 14;
            this.resultLbl.Text = "---";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1139, 693);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.StatusPanel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.WatermarkImageLocationTxt);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tugas Akhir 1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hostImage)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transformedImage)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.watermarkImage)).EndInit();
            this.StatusPanel.ResumeLayout(false);
            this.StatusPanel.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox hostImage;
        private System.Windows.Forms.TextBox HostImageLocationTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox transformedImage;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox watermarkImage;
        private System.Windows.Forms.TextBox WatermarkImageLocationTxt;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel StatusPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label StatusTxt;
        private System.Windows.Forms.Label TimeExecTxt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label BERValue;
        private System.Windows.Forms.Label PSNRValue;
        private System.Windows.Forms.Label BERlbl;
        private System.Windows.Forms.Label PSNRlbl;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Label MSEValue;
        private System.Windows.Forms.Label MSElbl;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button histeqBtn;
        private System.Windows.Forms.Button modusFilterBtn;
        private System.Windows.Forms.Button medianFilterBtn;
        private System.Windows.Forms.Button meanFilterBtn;
        private System.Windows.Forms.TextBox WatermarkedImageTxt;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Label resultLbl;
    }
}

