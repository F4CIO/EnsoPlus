using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ControlCornerLauncher
{

	public partial class FormCornerLauncher : Form
	{
	
		#region Private Members
		private GuiElements _guiElements;
		#endregion

		#region Properties
		#endregion

		#region Public Methods
		#endregion

		#region Constructors And Initialization	
		internal FormCornerLauncher(GuiElements guiElements)
		{
			this._guiElements = guiElements;
			InitializeComponent();
			this.Visible = false;
		}

		public void CustomShow()
		{
			this.Location = Cursor.Position;
			this.StartPosition = FormStartPosition.Manual;
			this.WindowState = FormWindowState.Maximized;
			this.TopMost = true;
			this.Visible = true;
		}

		#endregion

		#region Deinitialization And Destructors

		public void CustomHide()
		{
			this.Visible = false;
		}
		#endregion

		private void FormCornerLauncher_Paint(object sender, PaintEventArgs e)
		{
			this._guiElements.Draw(e.Graphics);
		}

		#region Event Handlers
		#endregion

		#region Private Methods
		#endregion

		#region Helpers
		#endregion

	}
}
