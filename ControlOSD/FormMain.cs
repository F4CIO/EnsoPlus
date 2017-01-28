using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OSD
{
    internal partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            OSD.formBackground = new FormBackground();
            OSD.formBackground.TopLevel = true;
            OSD.formBackground.Activated += new EventHandler(formBackground_Activated);
            OSD.formBackground.UpdateSizeAndLocation(this);            
        }

        void formBackground_Activated(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void FormMain_Move(object sender, EventArgs e)
        {
            OSD.formBackground.UpdateSizeAndLocation(OSD.formMain);
        }

        public void FormMain_UpdateSettingsOnMove(object sender, EventArgs e)
        {
            Settings.Current.location = this.Location;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            OSD.formBackground.UpdateSizeAndLocation(OSD.formMain);
        }

        public void formMain_UpdateSettingsOnResize(object sender, EventArgs e)
        {
            Settings.Current.size = this.Size;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //OSD.formBackground.Close();
            if (OSD.formBackground != null && OSD.formBackground.IsHandleCreated)
            {
                OSD.formBackground.Invoke((Action)(() =>
                {
                    OSD.formBackground.Hide();
                }));
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //OSD.formMain.pnlContent2.Size = OSD.formMain.pnlContent.Size;
            //OSD.formMain.pnlContent2.Location = OSD.formMain.pnlContent.Location;
            //OSD.formMain.pnlContent.Focus();
        }


        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            //frame
            Pen framePen = new Pen(new SolidBrush(Settings.Current.frameColor), Settings.Current.frameThickness);
            OSD.formMain.CreateGraphics().DrawRectangle(framePen, new Rectangle(0,0,OSD.formMain.Width , OSD.formMain.Height ));


        }

        private void pnlContent_Paint(object sender, PaintEventArgs e)
        {
            Menu.Menu menuToDraw = null;
            if ( ((bool)((Panel)sender).Tag)==true)
            {
                menuToDraw = OSD.menu;
            }
            else 
            {
                menuToDraw = OSD.menu2;
            }

            if (menuToDraw != null)
            {
                Panel hostControl =(Panel)sender;
                Graphics graphics = hostControl.CreateGraphics();
                menuToDraw.Draw(graphics, new Rectangle(hostControl.Location, hostControl.Size));
                System.Diagnostics.Debug.WriteLine("c:"+((Panel)sender).Name+" " + hostControl.Location + " " + hostControl.Size);
                //graphics.DrawRectangle(new Pen(new SolidBrush(Color.Blue)), new Rectangle(hostControl.Location, hostControl.Size));                
            }
        }

        private void FormMain_VisibleChanged(object sender, EventArgs e)
        {
            OSD.formBackground.Visible = this.Visible;
            OSD.formBackground.UpdateSizeAndLocation(this);            
        }
    }
}
