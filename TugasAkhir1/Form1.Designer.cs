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
            this.totalScrambledTxt = new System.Windows.Forms.Label();
            this.totalWatermarkTxt = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.TimeExecTxt = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.StatusTxt = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hostImage)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transformedImage)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.watermarkImage)).BeginInit();
            this.StatusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Monaco", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.HostImageLocationTxt.Location = new System.Drawing.Point(85, 5);
            this.HostImageLocationTxt.Multiline = true;
            this.HostImageLocationTxt.Name = "HostImageLocationTxt";
            this.HostImageLocationTxt.Size = new System.Drawing.Size(284, 20);
            this.HostImageLocationTxt.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.button2.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Monaco", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.transformedImage);
            this.panel2.Location = new System.Drawing.Point(383, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(653, 656);
            this.panel2.TabIndex = 3;
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
            this.transformedImage.Size = new System.Drawing.Size(645, 628);
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
            this.button3.Font = new System.Drawing.Font("Monaco", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.WatermarkImageLocationTxt.Location = new System.Drawing.Point(171, 372);
            this.WatermarkImageLocationTxt.Multiline = true;
            this.WatermarkImageLocationTxt.Name = "WatermarkImageLocationTxt";
            this.WatermarkImageLocationTxt.Size = new System.Drawing.Size(206, 20);
            this.WatermarkImageLocationTxt.TabIndex = 4;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Monaco", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.button5.Font = new System.Drawing.Font("Monaco", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.button6.Size = new System.Drawing.Size(75, 23);
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
            this.StatusPanel.Controls.Add(this.totalScrambledTxt);
            this.StatusPanel.Controls.Add(this.totalWatermarkTxt);
            this.StatusPanel.Controls.Add(this.label8);
            this.StatusPanel.Controls.Add(this.label7);
            this.StatusPanel.Controls.Add(this.TimeExecTxt);
            this.StatusPanel.Controls.Add(this.label6);
            this.StatusPanel.Controls.Add(this.label5);
            this.StatusPanel.Controls.Add(this.StatusTxt);
            this.StatusPanel.Controls.Add(this.label4);
            this.StatusPanel.Location = new System.Drawing.Point(3, 661);
            this.StatusPanel.Name = "StatusPanel";
            this.StatusPanel.Size = new System.Drawing.Size(1033, 31);
            this.StatusPanel.TabIndex = 9;
            // 
            // totalScrambledTxt
            // 
            this.totalScrambledTxt.AutoSize = true;
            this.totalScrambledTxt.Location = new System.Drawing.Point(918, 10);
            this.totalScrambledTxt.Name = "totalScrambledTxt";
            this.totalScrambledTxt.Size = new System.Drawing.Size(13, 13);
            this.totalScrambledTxt.TabIndex = 8;
            this.totalScrambledTxt.Text = "0";
            // 
            // totalWatermarkTxt
            // 
            this.totalWatermarkTxt.AutoSize = true;
            this.totalWatermarkTxt.Location = new System.Drawing.Point(715, 10);
            this.totalWatermarkTxt.Name = "totalWatermarkTxt";
            this.totalWatermarkTxt.Size = new System.Drawing.Size(13, 13);
            this.totalWatermarkTxt.TabIndex = 7;
            this.totalWatermarkTxt.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(834, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Total Scrambled: ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(626, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Total Watermark: ";
            // 
            // TimeExecTxt
            // 
            this.TimeExecTxt.AutoSize = true;
            this.TimeExecTxt.Location = new System.Drawing.Point(187, 10);
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
            this.label5.Location = new System.Drawing.Point(107, 10);
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
            this.button7.Font = new System.Drawing.Font("Monaco", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(7, 467);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(80, 46);
            this.button7.TabIndex = 10;
            this.button7.TabStop = false;
            this.button7.Text = "Embed Watermark";
            this.button7.UseVisualStyleBackColor = false;
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button8.FlatAppearance.BorderSize = 0;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Font = new System.Drawing.Font("Monaco", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.Location = new System.Drawing.Point(7, 565);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(80, 46);
            this.button8.TabIndex = 11;
            this.button8.TabStop = false;
            this.button8.Text = "Extract Watermark";
            this.button8.UseVisualStyleBackColor = false;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(7, 617);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 23);
            this.button9.TabIndex = 12;
            this.button9.Text = "button9";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1038, 693);
            this.Controls.Add(this.button9);
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
        private System.Windows.Forms.Label totalScrambledTxt;
        private System.Windows.Forms.Label totalWatermarkTxt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
    }
}

