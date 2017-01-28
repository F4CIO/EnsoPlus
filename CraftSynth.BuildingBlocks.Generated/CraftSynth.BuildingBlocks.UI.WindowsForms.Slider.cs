using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CraftSynth.BuildingBlocks.UI.WindowsForms
{
	public class Slider
	{
		public enum SlideType
		{
			None,
			Fade,
			SlideToLeft,
			SlideToRight
		}

		private Control parentControl;
		private SlideType slideType;
		private Point initialLocation;
		private Size initialSize;
		private Control currentControl;
		private Control newControl;
		private int delta;

		private System.Windows.Forms.Timer timer;
		private TimeSpan duration;
		private DateTime startTime;
		private int inThickProcess = 0;
		private bool finished;

		public Slider(Control parentControl, TimeSpan effectDuration, TimeSpan effectUdateInterval)
		{
			this.parentControl = parentControl;
			this.PrepareTimer(effectDuration, effectUdateInterval);
		}

		private void PrepareTimer(TimeSpan duration, TimeSpan updateInterval)
		{
			this.timer = new System.Windows.Forms.Timer();
			this.timer.Interval = (int)updateInterval.TotalMilliseconds;
			this.timer.Enabled = false;
			this.timer.Tick += new EventHandler(timer_Tick);
			this.duration = duration;
			this.startTime = DateTime.Now;
		}

		public void Execute(Control currentControl, Control newControl, SlideType slideType, bool synchronous)
		{
			this.currentControl = currentControl;
			this.slideType = slideType;
			this.initialLocation = currentControl.Location;
			this.initialSize = currentControl.Size;
			this.newControl = newControl;
			this.newControl.Parent = this.parentControl;
			this.newControl.Visible = false;
			this.newControl.Location = this.initialLocation;
			this.newControl.Size = this.initialSize;
			this.startTime = DateTime.Now;
			System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.DefaultTraceListener());//DelimitedListTraceListener("d:\\ee.txt"));
			this.timer.Start();
			this.finished = false;
			if (synchronous)
			{
				while (!finished)
				{
					Application.DoEvents();
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

						switch (this.slideType)
						{//increment
							case SlideType.None:
								break;
							case SlideType.SlideToLeft:
								//width : percent = delta:100
								//delta = width*100/percent
								delta = (int)Math.Round((double)this.currentControl.Width * precentElapsed);


								int r;
								delta = Math.DivRem(delta, 2, out r);
								this.currentControl.Left = Math.DivRem(this.initialLocation.X, 2, out r) - delta;
								this.newControl.Left = Math.DivRem(this.currentControl.Width, 2, out r) - delta;
								this.newControl.BringToFront();
								System.Diagnostics.Debug.WriteLine(string.Format("{0} {1} {2} {3}", this.newControl.Left, this.currentControl.Width, this.parentControl.Location.X, delta));
								System.Diagnostics.Debug.Flush();
								this.newControl.Visible = true;

								this.parentControl.Refresh();
								Application.DoEvents();
								break;
							case SlideType.SlideToRight:
								//width : percent = delta:100
								//delta = width*100/percent
								delta = (int)Math.Round((double)this.currentControl.Width * precentElapsed);

								this.currentControl.Left = this.initialLocation.X + delta;
								this.newControl.Left = this.initialLocation.X + delta - this.currentControl.Width + 1;
								this.newControl.Visible = true;

								this.parentControl.Refresh();
								Application.DoEvents();
								break;
							case SlideType.Fade:
								throw new NotImplementedException();
						}

					}
					else
					{//finish
						switch (this.slideType)
						{
							case SlideType.None:
								break;
							case SlideType.SlideToLeft:
								this.currentControl.Left = this.initialLocation.X - this.currentControl.Width;
								this.newControl.Left = this.initialLocation.X;

								break;
							case SlideType.SlideToRight:
								this.currentControl.Left = this.initialLocation.X + this.currentControl.Width;
								this.newControl.Left = this.initialLocation.X;
								break;
							case SlideType.Fade:
								throw new NotImplementedException();
						}
						this.timer.Dispose();

						this.parentControl.Refresh();
						Application.DoEvents();

						this.currentControl = null;
						this.newControl = null;

						this.finished = true;
					}
				}
				catch (Exception exception)
				{
					// BuildingBlocks.Exceptions.EnterpriseLibrary.ExceptionHandler.Handle(exception, BuildingBlocks.Exceptions.EnterpriseLibrary.ExceptionHandlingPolicies.Log_And_Block_Exception_Policy);
				}
				finally
				{
					Interlocked.Decrement(ref this.inThickProcess);
				}
			}
		}
	}
}
