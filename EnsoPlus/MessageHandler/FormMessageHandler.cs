using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EnsoPlus.MessageHandler
{
	public partial class FormMessageHandler : Form
	{
		public FormMessageHandler()
		{
			InitializeComponent();
		}

		private void tbText_MouseMove(object sender, MouseEventArgs e)
		{
			MessageHandler.RestartInputListenerIfStailed();
		}

		private void tbSubtext_MouseMove(object sender, MouseEventArgs e)
		{
			MessageHandler.RestartInputListenerIfStailed();
		}
	}
}
