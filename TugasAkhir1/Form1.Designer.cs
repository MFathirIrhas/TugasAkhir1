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
            this.transformedImage = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.watermarkImage = new System.Windows.Forms.PictureBox();
            this.WatermarkImageLocationTxt = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hostImage)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transformedImage)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.watermarkImage)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
            this.hostImage.Image = ((System.Drawing.Image)(resources.GetObject("hostImage.Image")));
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
            this.HostImageLocationTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
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
            this.panel2.Controls.Add(this.transformedImage);
            this.panel2.Location = new System.Drawing.Point(383, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(653, 630);
            this.panel2.TabIndex = 3;
            // 
            // transformedImage
            // 
            this.transformedImage.Image = ((System.Drawing.Image)(resources.GetObject("transformedImage.Image")));
            this.transformedImage.InitialImage = ((System.Drawing.Image)(resources.GetObject("transformedImage.InitialImage")));
            this.transformedImage.Location = new System.Drawing.Point(3, 3);
            this.transformedImage.Name = "transformedImage";
            this.transformedImage.Size = new System.Drawing.Size(645, 622);
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
            this.button3.Location = new System.Drawing.Point(7, 415);
            this.button3.Name = "button3";
            this.button3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button3.Size = new System.Drawing.Size(80, 40);
            this.button3.TabIndex = 4;
            this.button3.TabStop = false;
            this.button3.Text = "IDWT";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.watermarkImage);
            this.panel3.Location = new System.Drawing.Point(93, 389);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(284, 244);
            this.panel3.TabIndex = 5;
            // 
            // watermarkImage
            // 
            this.watermarkImage.Image = ((System.Drawing.Image)(resources.GetObject("watermarkImage.Image")));
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
            this.WatermarkImageLocationTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.WatermarkImageLocationTxt.Enabled = false;
            this.WatermarkImageLocationTxt.Location = new System.Drawing.Point(171, 367);
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
            this.button4.Location = new System.Drawing.Point(93, 364);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 4;
            this.button4.TabStop = false;
            this.button4.Text = "Browse";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1038, 636);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.WatermarkImageLocationTxt);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Tugas Akhir 1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hostImage)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.transformedImage)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.watermarkImage)).EndInit();
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
    }
}

