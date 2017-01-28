using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EnsoPlus
{
    public partial class FormMain : Form
	{
		#region Private Members
		#endregion

		#region Properties
		#endregion

		#region Public Methods
		#endregion

		#region Constructors And Initialization	
		public FormMain()
        {
            InitializeComponent();
        }

		private void FormMain_Load(object sender, EventArgs e)
		{
		
		}
		#endregion

		#region Deinitialization And Destructors
		#endregion

		#region Event Handlers	
	

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile("http://www.f4cio.com/ensoplus.aspx");
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile("http://www.f4cio.com/ensoplus.aspx");
		}

		private void restartToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandsProviders.Misc.Misc.Restart();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandsProviders.Misc.Misc.Exit();
		}	
		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.TrayIcon.Visible = false;
		}
		#endregion

		#region Private Methods
		#endregion

		#region Helpers
		#endregion
		
		
	

	
    }
}
