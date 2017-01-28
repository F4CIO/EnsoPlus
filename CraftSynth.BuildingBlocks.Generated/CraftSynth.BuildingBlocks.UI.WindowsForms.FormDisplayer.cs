using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CraftSynth.BuildingBlocks.UI.WindowsForms
{
	public class FormDisplayer
	{
		public enum ShowHideEffect
		{
			None,
			Fade
		}

		private enum OperationType
		{
			Show,
			Hide
		}

		private Form form;
		private OperationType operation;
		private ShowHideEffect effect;

		private System.Windows.Forms.Timer timer;
		private TimeSpan duration;
		private DateTime startTime;
		private int inThickProcess = 0;

		public FormDisplayer(ShowHideEffect showHideEffect, TimeSpan showHideEffectDuration, TimeSpan showHideEffectUdateInterval)
		{
			this.effect = showHideEffect;
			this.PrepareTimer(showHideEffectDuration, showHideEffectUdateInterval);

		}

		private void PrepareTimer(TimeSpan duration, TimeSpan updateInterval)
		{
			this.timer = new System.Windows.Forms.Timer();
			this.timer.Interval = (int)updateInterval.TotalMilliseconds;
			this.timer.Enabled = false;
			this.timer.Tick += new EventHandler(timer_Tick);
			this.duration = duration;

		}


		public void ShowForm(Form form)
		{

			this.operation = OperationType.Show;
			//this.form.BeginInvoke((Action)(() =>
			//            {
			switch (this.effect)
			{
				case ShowHideEffect.None:

					break;
				case ShowHideEffect.Fade:
					this.form = form;
					this.form.Opacity = 0;
					this.startTime = DateTime.Now;
					this.timer.Start();
					break;
			}
			//}));
		}

		private delegate void HideFormDelegate(Form form);
		public void HideForm(Form form)
		{
			if (form.InvokeRequired)
			{
				form.Invoke(new HideFormDelegate(HideForm), new object[] { form });
			}
			else
			{
				this.operation = OperationType.Hide;
				switch (this.effect)
				{
					case ShowHideEffect.None:

						break;
					case ShowHideEffect.Fade:
						this.form = form;
						this.form.Opacity = 1;
						this.startTime = DateTime.Now;
						this.timer.Start();
						break;
				}
			}
		}


		void timer_Tick(object sender, EventArgs e)
		{
			if (Interlocked.Equals(this.inThickProcess, 0))
			{
				Interlocked.Increment(ref this.inThickProcess);
				try
				{
					if (DateTime.Now.CompareTo(this.startTime.Add(this.duration)) < 0)
					{
						TimeSpan durationPassed = new TimeSpan(DateTime.Now.Ticks - this.startTime.Ticks);
						double precentElapsed = (double)durationPassed.Ticks / (double)duration.Ticks;

						this.form.BeginInvoke((Action)(() =>
						{
							switch (this.operation)
							{
								case OperationType.Show:
									this.form.Opacity = precentElapsed;
									this.form.Refresh();
									Application.DoEvents();
									break;
								case OperationType.Hide:
									this.form.BeginInvoke((Action)(() =>
									{
										this.form.Opacity = 1 - precentElapsed;
										System.Diagnostics.Debug.WriteLine(this.form.Opacity.ToString());
										this.form.Refresh();
										Application.DoEvents();
									}));
									break;
							}
						}));

					}
					else
					{

						switch (this.operation)
						{
							case OperationType.Show:
								this.form.Opacity = 1;
								this.form.Refresh();
								Application.DoEvents();
								break;
							case OperationType.Hide:
								this.form.BeginInvoke((Action)(() =>
								{
									this.form.Opacity = 0;
									this.form.Refresh();
									Application.DoEvents();
								}));
								break;
						}

						this.timer.Dispose();
					}
				}
				finally
				{
					Interlocked.Decrement(ref this.inThickProcess);
				}
			}
		}
	}
}
