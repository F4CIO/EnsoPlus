namespace Extension
{
    partial class FormParameterBox
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
			this.components = new System.ComponentModel.Container();
			this.lblCaption = new System.Windows.Forms.Label();
			this.parameterBoxRectangle = new System.Windows.Forms.Panel();
			this.tbParameter = new System.Windows.Forms.TextBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.parameterBoxRectangle.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblCaption
			// 
			this.lblCaption.AutoSize = true;
			this.lblCaption.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.lblCaption.Location = new System.Drawing.Point(85, 36);
			this.lblCaption.Name = "lblCaption";
			this.lblCaption.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lblCaption.Size = new System.Drawing.Size(70, 33);
			this.lblCaption.TabIndex = 3;
			this.lblCaption.Text = "open";
			this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lblCaption.Resize += new System.EventHandler(this.lblCaption_Resize);
			// 
			// parameterBoxRectangle
			// 
			this.parameterBoxRectangle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.parameterBoxRectangle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.parameterBoxRectangle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.parameterBoxRectangle.Controls.Add(this.tbParameter);
			this.parameterBoxRectangle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.parameterBoxRectangle.Location = new System.Drawing.Point(165, 23);
			this.parameterBoxRectangle.Margin = new System.Windows.Forms.Padding(0);
			this.parameterBoxRectangle.Name = "parameterBoxRectangle";
			this.parameterBoxRectangle.Padding = new System.Windows.Forms.Padding(3);
			this.parameterBoxRectangle.Size = new System.Drawing.Size(446, 56);
			this.parameterBoxRectangle.TabIndex = 4;
			this.parameterBoxRectangle.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			// 
			// tbParameter
			// 
			this.tbParameter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbParameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.tbParameter.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbParameter.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbParameter.ForeColor = System.Drawing.Color.White;
			this.tbParameter.Location = new System.Drawing.Point(8, 12);
			this.tbParameter.Name = "tbParameter";
			this.tbParameter.Size = new System.Drawing.Size(430, 33);
			this.tbParameter.TabIndex = 1;
			this.tbParameter.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			this.tbParameter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbParameter_KeyDown);
			// 
			// timer1
			// 
			this.timer1.Interval = 500;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// FormParameterBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(640, 98);
			this.ControlBox = false;
			this.Controls.Add(this.parameterBoxRectangle);
			this.Controls.Add(this.lblCaption);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormParameterBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ParameterBox";
			this.TransparencyKey = System.Drawing.Color.Black;
			this.VisibleChanged += new System.EventHandler(this.FormParameterBox_VisibilityChanged);
			this.parameterBoxRectangle.ResumeLayout(false);
			this.parameterBoxRectangle.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox tbParameter;
        public System.Windows.Forms.Panel parameterBoxRectangle;
        public System.Windows.Forms.Label lblCaption;
		private System.Windows.Forms.Timer timer1;
    }
}