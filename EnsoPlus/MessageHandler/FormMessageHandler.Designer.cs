namespace EnsoPlus.MessageHandler
{
	partial class FormMessageHandler
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
			this.tbText = new System.Windows.Forms.TextBox();
			this.tbSubtext = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// tbText
			// 
			this.tbText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbText.BackColor = System.Drawing.Color.Pink;
			this.tbText.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbText.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.tbText.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.tbText.ForeColor = System.Drawing.Color.OliveDrab;
			this.tbText.Location = new System.Drawing.Point(12, 12);
			this.tbText.Multiline = true;
			this.tbText.Name = "tbText";
			this.tbText.ReadOnly = true;
			this.tbText.Size = new System.Drawing.Size(1129, 234);
			this.tbText.TabIndex = 0;
			this.tbText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tbText.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tbText_MouseMove);
			// 
			// tbSubtext
			// 
			this.tbSubtext.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSubtext.BackColor = System.Drawing.Color.Pink;
			this.tbSubtext.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbSubtext.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.tbSubtext.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.tbSubtext.ForeColor = System.Drawing.Color.OliveDrab;
			this.tbSubtext.Location = new System.Drawing.Point(12, 252);
			this.tbSubtext.Multiline = true;
			this.tbSubtext.Name = "tbSubtext";
			this.tbSubtext.ReadOnly = true;
			this.tbSubtext.Size = new System.Drawing.Size(1129, 234);
			this.tbSubtext.TabIndex = 1;
			this.tbSubtext.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tbSubtext.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tbSubtext_MouseMove);
			// 
			// FormMessageHandler
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Pink;
			this.ClientSize = new System.Drawing.Size(1153, 506);
			this.ControlBox = false;
			this.Controls.Add(this.tbSubtext);
			this.Controls.Add(this.tbText);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormMessageHandler";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.TransparencyKey = System.Drawing.Color.Pink;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		internal System.Windows.Forms.TextBox tbText;
		internal System.Windows.Forms.TextBox tbSubtext;

	}
}