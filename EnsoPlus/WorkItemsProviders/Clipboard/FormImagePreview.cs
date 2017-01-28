using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Common;
using System.Threading;

namespace EnsoPlus.WorkItemsProviders.Clipboard
{
    public partial class FormImagePreview : Form
    {
		private CraftSynth.BuildingBlocks.UI.WindowsForms.FormDisplayer _formDisplayer;

        public static void Execute(Image image)
        {
            FormImagePreview form = new FormImagePreview();
            form.InitializeComponent();        
            

            if (image.Height > 480 || image.Width > 640)
            {
                if (image.Height > 480 && image.Height>image.Width)
                {
                    form.Height = 485;
                    //480 / X = image.Height / image.Width;
                    form.Width = (int)Math.Round((double)(485 * image.Width / image.Height));
                    form.BackgroundImageLayout = ImageLayout.Zoom;   
                }
                else if (image.Width > 640 && image.Width>image.Height)
                {
                    form.Width = 645;
                    //645/y=image.width/image.height
                    form.Height = (int)Math.Round((double)(645*image.Height/image.Width));
                    form.BackgroundImageLayout = ImageLayout.Zoom;                 
                }
            }
            else
            {                
                form.Width = image.Width+5;
                form.Height = image.Height+5;
                form.BackgroundImageLayout = ImageLayout.Center;
            }


            form.BackgroundImage = image;
            
            form.TopMost = true;
            form.TopLevel = true;
			form._formDisplayer = new CraftSynth.BuildingBlocks.UI.WindowsForms.FormDisplayer(CraftSynth.BuildingBlocks.UI.WindowsForms.FormDisplayer.ShowHideEffect.Fade, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(50));
            form._formDisplayer.ShowForm(form);
            form.Show();
            for (int i = 20; i > 0; i--)
            {
                Thread.Sleep(50);
                Application.DoEvents();
            }
            Thread.Sleep(1000);
            form._formDisplayer.HideForm(form);
            for (int i = 20; i > 0; i--)
            {
                Thread.Sleep(50);
                Application.DoEvents();
            }
            form.Close();               
      
            //Thread thread = new Thread(new ThreadStart(form.WaitAndCloseForm));
            //thread.Start();
            
        }

        private void WaitAndCloseForm()
        {
            for (int i = 20; i > 0; i--)
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }
            Thread.Sleep(2000);
            this._formDisplayer.HideForm(this);
            for (int i = 20; i > 0; i--)
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }
            this.Close();                       
        }

    }
}
