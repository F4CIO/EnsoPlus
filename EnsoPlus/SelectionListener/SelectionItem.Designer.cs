namespace EnsoPlus.SelectionListener
{
    partial class SelectionItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbContent = new System.Windows.Forms.TextBox();
            this.cbLock = new System.Windows.Forms.CheckBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbContent
            // 
            this.tbContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbContent.Cursor = System.Windows.Forms.Cursors.Default;
            this.tbContent.Location = new System.Drawing.Point(16, 2);
            this.tbContent.Name = "tbContent";
            this.tbContent.Size = new System.Drawing.Size(100, 20);
            this.tbContent.TabIndex = 1;
            this.tbContent.WordWrap = false;
            // 
            // cbLock
            // 
            this.cbLock.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbLock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cbLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbLock.Location = new System.Drawing.Point(0, 2);
            this.cbLock.Margin = new System.Windows.Forms.Padding(0);
            this.cbLock.Name = "cbLock";
            this.cbLock.Size = new System.Drawing.Size(15, 20);
            this.cbLock.TabIndex = 0;
            this.cbLock.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.BackgroundImage = global::EnsoPlus.Properties.Resources.Remove;
            this.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(146, 2);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(15, 20);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.BackgroundImage = global::EnsoPlus.Properties.Resources.Copy;
            this.btnCopy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopy.Location = new System.Drawing.Point(131, 2);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(15, 20);
            this.btnCopy.TabIndex = 3;
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnView
            // 
            this.btnView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnView.BackgroundImage = global::EnsoPlus.Properties.Resources.MagnifyingGlass;
            this.btnView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnView.Location = new System.Drawing.Point(117, 2);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(15, 20);
            this.btnView.TabIndex = 2;
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // SelectionItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.tbContent);
            this.Controls.Add(this.cbLock);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SelectionItem";
            this.Size = new System.Drawing.Size(162, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbContent;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnCopy;
        internal System.Windows.Forms.CheckBox cbLock;
        private System.Windows.Forms.Button btnRemove;
    }
}
