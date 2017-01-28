namespace Extension.ControlSimpleMenu
{
    partial class FormContent
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
            this.pnlFrame = new System.Windows.Forms.Panel();
            this.pnlCurrent = new System.Windows.Forms.Panel();
            this.pnlItems = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblItem1 = new System.Windows.Forms.Label();
            this.lblArrowBottom = new System.Windows.Forms.Label();
            this.lblArrowTop = new System.Windows.Forms.Label();
            this.splHeaderItems = new System.Windows.Forms.Splitter();
            this.lblHeader = new System.Windows.Forms.Label();
            this.tmrSlider = new System.Windows.Forms.Timer(this.components);
            this.pnlFrame.SuspendLayout();
            this.pnlCurrent.SuspendLayout();
            this.pnlItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlFrame
            // 
            this.pnlFrame.BackColor = System.Drawing.Color.Orange;
            this.pnlFrame.Controls.Add(this.pnlCurrent);
            this.pnlFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFrame.Location = new System.Drawing.Point(5, 5);
            this.pnlFrame.Name = "pnlFrame";
            this.pnlFrame.Padding = new System.Windows.Forms.Padding(3);
            this.pnlFrame.Size = new System.Drawing.Size(622, 363);
            this.pnlFrame.TabIndex = 0;
            // 
            // pnlCurrent
            // 
            this.pnlCurrent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.pnlCurrent.Controls.Add(this.pnlItems);
            this.pnlCurrent.Controls.Add(this.splHeaderItems);
            this.pnlCurrent.Controls.Add(this.lblHeader);
            this.pnlCurrent.Location = new System.Drawing.Point(3, 3);
            this.pnlCurrent.Name = "pnlCurrent";
            this.pnlCurrent.Size = new System.Drawing.Size(616, 357);
            this.pnlCurrent.TabIndex = 0;
            // 
            // pnlItems
            // 
            this.pnlItems.Controls.Add(this.label3);
            this.pnlItems.Controls.Add(this.label2);
            this.pnlItems.Controls.Add(this.label1);
            this.pnlItems.Controls.Add(this.lblItem1);
            this.pnlItems.Controls.Add(this.lblArrowBottom);
            this.pnlItems.Controls.Add(this.lblArrowTop);
            this.pnlItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlItems.Location = new System.Drawing.Point(0, 53);
            this.pnlItems.Name = "pnlItems";
            this.pnlItems.Padding = new System.Windows.Forms.Padding(10);
            this.pnlItems.Size = new System.Drawing.Size(616, 304);
            this.pnlItems.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Arial", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label3.Location = new System.Drawing.Point(10, 197);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(596, 49);
            this.label3.TabIndex = 8;
            this.label3.Text = "Accessories";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.Visible = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Arial", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(10, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(596, 49);
            this.label2.TabIndex = 7;
            this.label2.Text = "Accessories";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Visible = false;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Arial", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label1.Location = new System.Drawing.Point(10, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(596, 49);
            this.label1.TabIndex = 6;
            this.label1.Text = "Fun";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Visible = false;
            // 
            // lblItem1
            // 
            this.lblItem1.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblItem1.Font = new System.Drawing.Font("Arial", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItem1.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.lblItem1.Location = new System.Drawing.Point(10, 50);
            this.lblItem1.Name = "lblItem1";
            this.lblItem1.Size = new System.Drawing.Size(596, 49);
            this.lblItem1.TabIndex = 5;
            this.lblItem1.Text = "     Accessories ►";
            this.lblItem1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblItem1.Visible = false;
            // 
            // lblArrowBottom
            // 
            this.lblArrowBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblArrowBottom.Font = new System.Drawing.Font("Times New Roman", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArrowBottom.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.lblArrowBottom.Location = new System.Drawing.Point(10, 254);
            this.lblArrowBottom.Name = "lblArrowBottom";
            this.lblArrowBottom.Size = new System.Drawing.Size(596, 40);
            this.lblArrowBottom.TabIndex = 4;
            this.lblArrowBottom.Text = "▼";
            this.lblArrowBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblArrowTop
            // 
            this.lblArrowTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblArrowTop.Font = new System.Drawing.Font("Times New Roman", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArrowTop.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.lblArrowTop.Location = new System.Drawing.Point(10, 10);
            this.lblArrowTop.Name = "lblArrowTop";
            this.lblArrowTop.Size = new System.Drawing.Size(596, 40);
            this.lblArrowTop.TabIndex = 3;
            this.lblArrowTop.Text = "▲";
            this.lblArrowTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splHeaderItems
            // 
            this.splHeaderItems.BackColor = System.Drawing.Color.Orange;
            this.splHeaderItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splHeaderItems.Dock = System.Windows.Forms.DockStyle.Top;
            this.splHeaderItems.Location = new System.Drawing.Point(0, 49);
            this.splHeaderItems.Name = "splHeaderItems";
            this.splHeaderItems.Size = new System.Drawing.Size(616, 4);
            this.splHeaderItems.TabIndex = 1;
            this.splHeaderItems.TabStop = false;
            // 
            // lblHeader
            // 
            this.lblHeader.BackColor = System.Drawing.Color.Black;
            this.lblHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(0, 0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(616, 49);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Header";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHeader.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.lblHeader_PreviewKeyDown);
            this.lblHeader.DoubleClick += new System.EventHandler(this.lblHeader_DoubleClick);
            this.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseMove);
            this.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseDown);
            this.lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseUp);
            // 
            // FormContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(632, 373);
            this.Controls.Add(this.pnlFrame);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "FormContent";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.Load += new System.EventHandler(this.FormContent_Load);
            this.SizeChanged += new System.EventHandler(this.FormContent_SizeChanged);
            this.DoubleClick += new System.EventHandler(this.FormContent_DoubleClick);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormContent_FormClosed);
            this.LocationChanged += new System.EventHandler(this.FormContent_LocationChanged);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.FormContent_PreviewKeyDown);
            this.pnlFrame.ResumeLayout(false);
            this.pnlCurrent.ResumeLayout(false);
            this.pnlItems.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFrame;
        private System.Windows.Forms.Panel pnlCurrent;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Splitter splHeaderItems;
        private System.Windows.Forms.Panel pnlItems;
        private System.Windows.Forms.Label lblItem1;
        private System.Windows.Forms.Label lblArrowBottom;
        private System.Windows.Forms.Label lblArrowTop;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer tmrSlider;


    }
}