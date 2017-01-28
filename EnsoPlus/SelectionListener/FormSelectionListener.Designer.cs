using System;
namespace EnsoPlus.SelectionListener
{
    partial class FormSelectionListener
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
            if (FormSelectionListener._current != null)
            {
                FormSelectionListener._current.Invoke((Action)(() =>
                {
                    base.Dispose(disposing);
                }));
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flpItems = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnClearLockedSelectionItems = new System.Windows.Forms.Button();
            this.btnClearSelectionItems = new System.Windows.Forms.Button();
            this.cbListen = new System.Windows.Forms.CheckBox();
            this.scItems = new System.Windows.Forms.SplitContainer();
            this.flpLockedItems = new System.Windows.Forms.FlowLayoutPanel();
            this.flpPickedSelectionItem = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlHeader.SuspendLayout();
            this.scItems.Panel1.SuspendLayout();
            this.scItems.Panel2.SuspendLayout();
            this.scItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpItems
            // 
            this.flpItems.AutoScroll = true;
            this.flpItems.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.flpItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpItems.Location = new System.Drawing.Point(0, 0);
            this.flpItems.Margin = new System.Windows.Forms.Padding(0);
            this.flpItems.Name = "flpItems";
            this.flpItems.Size = new System.Drawing.Size(244, 83);
            this.flpItems.TabIndex = 1;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.btnClearLockedSelectionItems);
            this.pnlHeader.Controls.Add(this.btnClearSelectionItems);
            this.pnlHeader.Controls.Add(this.cbListen);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(248, 23);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClearLockedSelectionItems
            // 
            this.btnClearLockedSelectionItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearLockedSelectionItems.BackgroundImage = global::EnsoPlus.Properties.Resources.ClearAll;
            this.btnClearLockedSelectionItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClearLockedSelectionItems.Location = new System.Drawing.Point(191, 3);
            this.btnClearLockedSelectionItems.Name = "btnClearLockedSelectionItems";
            this.btnClearLockedSelectionItems.Size = new System.Drawing.Size(24, 17);
            this.btnClearLockedSelectionItems.TabIndex = 2;
            this.btnClearLockedSelectionItems.UseVisualStyleBackColor = true;
            // 
            // btnClearSelectionItems
            // 
            this.btnClearSelectionItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearSelectionItems.BackgroundImage = global::EnsoPlus.Properties.Resources.ClearAll;
            this.btnClearSelectionItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClearSelectionItems.Location = new System.Drawing.Point(221, 3);
            this.btnClearSelectionItems.Name = "btnClearSelectionItems";
            this.btnClearSelectionItems.Size = new System.Drawing.Size(24, 17);
            this.btnClearSelectionItems.TabIndex = 1;
            this.btnClearSelectionItems.UseVisualStyleBackColor = true;
            // 
            // cbListen
            // 
            this.cbListen.AutoSize = true;
            this.cbListen.Location = new System.Drawing.Point(0, 3);
            this.cbListen.Name = "cbListen";
            this.cbListen.Size = new System.Drawing.Size(54, 17);
            this.cbListen.TabIndex = 0;
            this.cbListen.Text = "Listen";
            this.cbListen.UseVisualStyleBackColor = true;
            // 
            // scItems
            // 
            this.scItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scItems.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.scItems.Location = new System.Drawing.Point(0, 23);
            this.scItems.Margin = new System.Windows.Forms.Padding(0);
            this.scItems.Name = "scItems";
            this.scItems.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scItems.Panel1
            // 
            this.scItems.Panel1.Controls.Add(this.flpLockedItems);
            // 
            // scItems.Panel2
            // 
            this.scItems.Panel2.Controls.Add(this.flpItems);
            this.scItems.Size = new System.Drawing.Size(248, 177);
            this.scItems.SplitterDistance = 88;
            this.scItems.SplitterWidth = 2;
            this.scItems.TabIndex = 2;
            // 
            // flpLockedItems
            // 
            this.flpLockedItems.AutoScroll = true;
            this.flpLockedItems.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpLockedItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.flpLockedItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpLockedItems.Location = new System.Drawing.Point(0, 0);
            this.flpLockedItems.Margin = new System.Windows.Forms.Padding(0);
            this.flpLockedItems.Name = "flpLockedItems";
            this.flpLockedItems.Size = new System.Drawing.Size(244, 84);
            this.flpLockedItems.TabIndex = 2;
            // 
            // flpPickedSelectionItem
            // 
            this.flpPickedSelectionItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flpPickedSelectionItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.flpPickedSelectionItem.Location = new System.Drawing.Point(2, 208);
            this.flpPickedSelectionItem.Name = "flpPickedSelectionItem";
            this.flpPickedSelectionItem.Size = new System.Drawing.Size(244, 24);
            this.flpPickedSelectionItem.TabIndex = 3;
            // 
            // FormSelectionListener
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(248, 235);
            this.Controls.Add(this.flpPickedSelectionItem);
            this.Controls.Add(this.scItems);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectionListener";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ENSO+ Selection Listener";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.scItems.Panel1.ResumeLayout(false);
            this.scItems.Panel2.ResumeLayout(false);
            this.scItems.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.SplitContainer scItems;
        private System.Windows.Forms.FlowLayoutPanel flpLockedItems;
        private System.Windows.Forms.FlowLayoutPanel flpItems;
        private System.Windows.Forms.CheckBox cbListen;
        private System.Windows.Forms.Button btnClearSelectionItems;
        private System.Windows.Forms.Button btnClearLockedSelectionItems;
        private System.Windows.Forms.FlowLayoutPanel flpPickedSelectionItem;

    }
}