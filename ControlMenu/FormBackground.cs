using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Extension.ControlSimpleMenu
{
    public partial class FormBackground : Form
    {
        public FormBackground()
        {
            InitializeComponent();
        }

        private void FormBackground_Load(object sender, EventArgs e)
        {
            ControlMenu controlMenu = this.Tag as ControlMenu;
            this.BackColor = controlMenu._backgroundColor;
            this.Opacity = controlMenu._backgroundOpacity;
        }
    }
}
