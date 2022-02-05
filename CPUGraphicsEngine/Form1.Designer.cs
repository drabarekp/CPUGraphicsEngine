namespace CPUGraphicsEngine
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainPicture = new System.Windows.Forms.PictureBox();
            this.basicButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.shadingGroup = new System.Windows.Forms.GroupBox();
            this.PhongRadio = new System.Windows.Forms.RadioButton();
            this.gouraudRadio = new System.Windows.Forms.RadioButton();
            this.flatRadio = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.mainPicture)).BeginInit();
            this.shadingGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPicture
            // 
            this.mainPicture.Location = new System.Drawing.Point(0, 0);
            this.mainPicture.Name = "mainPicture";
            this.mainPicture.Size = new System.Drawing.Size(850, 850);
            this.mainPicture.TabIndex = 0;
            this.mainPicture.TabStop = false;
            this.mainPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.mainPicture_Paint);
            // 
            // basicButton
            // 
            this.basicButton.Location = new System.Drawing.Point(865, 130);
            this.basicButton.Name = "basicButton";
            this.basicButton.Size = new System.Drawing.Size(94, 29);
            this.basicButton.TabIndex = 1;
            this.basicButton.Text = "do stuff";
            this.basicButton.UseVisualStyleBackColor = true;
            this.basicButton.Click += new System.EventHandler(this.basicButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(865, 214);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(94, 29);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "start task";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(865, 285);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(94, 29);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "cancel task";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // shadingGroup
            // 
            this.shadingGroup.Controls.Add(this.PhongRadio);
            this.shadingGroup.Controls.Add(this.gouraudRadio);
            this.shadingGroup.Controls.Add(this.flatRadio);
            this.shadingGroup.Location = new System.Drawing.Point(856, 414);
            this.shadingGroup.Name = "shadingGroup";
            this.shadingGroup.Size = new System.Drawing.Size(250, 125);
            this.shadingGroup.TabIndex = 4;
            this.shadingGroup.TabStop = false;
            this.shadingGroup.Text = "Shading Modes";
            // 
            // PhongRadio
            // 
            this.PhongRadio.AutoSize = true;
            this.PhongRadio.Location = new System.Drawing.Point(9, 86);
            this.PhongRadio.Name = "PhongRadio";
            this.PhongRadio.Size = new System.Drawing.Size(130, 24);
            this.PhongRadio.TabIndex = 2;
            this.PhongRadio.Text = "Phong Shading";
            this.PhongRadio.UseVisualStyleBackColor = true;
            this.PhongRadio.CheckedChanged += new System.EventHandler(this.PhongRadio_CheckedChanged);
            // 
            // gouraudRadio
            // 
            this.gouraudRadio.AutoSize = true;
            this.gouraudRadio.Location = new System.Drawing.Point(9, 56);
            this.gouraudRadio.Name = "gouraudRadio";
            this.gouraudRadio.Size = new System.Drawing.Size(145, 24);
            this.gouraudRadio.TabIndex = 1;
            this.gouraudRadio.Text = "Gouraud Shading";
            this.gouraudRadio.UseVisualStyleBackColor = true;
            this.gouraudRadio.CheckedChanged += new System.EventHandler(this.gouraudRadio_CheckedChanged);
            // 
            // flatRadio
            // 
            this.flatRadio.AutoSize = true;
            this.flatRadio.Location = new System.Drawing.Point(9, 26);
            this.flatRadio.Name = "flatRadio";
            this.flatRadio.Size = new System.Drawing.Size(112, 24);
            this.flatRadio.TabIndex = 0;
            this.flatRadio.TabStop = true;
            this.flatRadio.Text = "Flat Shading";
            this.flatRadio.UseVisualStyleBackColor = true;
            this.flatRadio.CheckedChanged += new System.EventHandler(this.flatRadio_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1114, 953);
            this.Controls.Add(this.shadingGroup);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.basicButton);
            this.Controls.Add(this.mainPicture);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.mainPicture)).EndInit();
            this.shadingGroup.ResumeLayout(false);
            this.shadingGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox mainPicture;
        private System.Windows.Forms.Button basicButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button cancelButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox shadingGroup;
        private System.Windows.Forms.RadioButton PhongRadio;
        private System.Windows.Forms.RadioButton gouraudRadio;
        private System.Windows.Forms.RadioButton flatRadio;
    }
}
