using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CraftSynth.BuildingBlocks.UI.WindowsForms
{
	public class FormDriver
	{
		Form handledForm;
		Control controlThatChangesStateOnDoubleClick;

		private bool _editable = false;

		private Point? dragStartLocation = null;
		private Point? dragLastLocation = null;

		private void ChangeState(bool editable)
		{
			this.ChangeState(editable, false);
		}

		private void ChangeState(bool editable, bool force)
		{
			if (force || editable != this._editable)
			{
				this._editable = editable;
				this.handledForm.ControlBox = editable;
				this.handledForm.FormBorderStyle = ((this._editable) ? FormBorderStyle.Sizable : FormBorderStyle.None);
			}
		}

		public FormDriver(Form handledForm, Control controlThatChangesStateOnDoubleClick)
		{
			this.handledForm = handledForm;
			this.controlThatChangesStateOnDoubleClick = controlThatChangesStateOnDoubleClick;
			if (controlThatChangesStateOnDoubleClick != null)
			{
				this.controlThatChangesStateOnDoubleClick.DoubleClick += new EventHandler(handledForm_DoubleClick);
				this.controlThatChangesStateOnDoubleClick.MouseMove += new MouseEventHandler(handledForm_MouseMove);
				this.controlThatChangesStateOnDoubleClick.MouseDown += new MouseEventHandler(handledForm_MouseDown);
				this.controlThatChangesStateOnDoubleClick.MouseUp += new MouseEventHandler(handledForm_MouseUp);
				this.controlThatChangesStateOnDoubleClick.PreviewKeyDown += new PreviewKeyDownEventHandler(handledForm_PreviewKeyDown);

			}
			this.handledForm.DoubleClick += new EventHandler(handledForm_DoubleClick);
			this.handledForm.MouseMove += new MouseEventHandler(handledForm_MouseMove);
			this.handledForm.MouseDown += new MouseEventHandler(handledForm_MouseDown);
			this.handledForm.MouseUp += new MouseEventHandler(handledForm_MouseUp);
			this.handledForm.PreviewKeyDown += new PreviewKeyDownEventHandler(handledForm_PreviewKeyDown);
		}

		#region Events

		void handledForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.dragStartLocation.HasValue)
			{
				this.handledForm.Left += e.X - this.dragLastLocation.Value.X;
				this.handledForm.Top += e.Y - this.dragLastLocation.Value.Y;
			}
		}

		void handledForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (this.dragStartLocation.HasValue && e.KeyCode == Keys.Escape)
			{
				this.handledForm.Location = this.dragStartLocation.Value;
				this.dragStartLocation = null;
			}
			e.IsInputKey = false;

		}

		void handledForm_DoubleClick(object sender, EventArgs e)
		{
			this.ChangeState(!_editable);
		}

		void handledForm_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.dragStartLocation.HasValue)
			{
				this.handledForm.Left += e.X - this.dragLastLocation.Value.X;
				this.handledForm.Top += e.Y - this.dragLastLocation.Value.Y;
				this.dragStartLocation = null;
			}
		}

		void handledForm_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.dragStartLocation = this.handledForm.Location;
				this.dragLastLocation = new Point(e.X, e.Y);
			}
		}

		void handledForm_Move(object sender, EventArgs e)
		{

		}
		#endregion Events
	}
}
