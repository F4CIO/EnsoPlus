using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftSynth.BuildingBlocks.Common
{
	/// <summary>
	/// Usage:
	/// 
	/// 1. Instantiate eventsDisabler on initialization.
	/// 
	/// 2. Use:
	/// 
	/// using(this._eventsDisabler.DisableInThisBlock())  //events are disabled here
	/// {
	///     //do something...no events will be fired
	/// }                                                 //events are auto-enabled here
	/// </summary>
	public class EventsDisabler
	{
		private int _level = 0;

		public delegate void MethodThatEnablesEvents_Delegate();
		private event MethodThatEnablesEvents_Delegate _methodThatEnablesEvents;
			
		public delegate void MethodThatDisablesEvents_Delegate();
		private event MethodThatDisablesEvents_Delegate _methodThatDisablesEvents;

		private readonly object _lock = new object();

		public EventsDisabler(MethodThatEnablesEvents_Delegate methodThatEnablesEvents, MethodThatDisablesEvents_Delegate methodThatDisablesEvents, bool performEnableEventsNow = false)
		{
			this._methodThatEnablesEvents = methodThatEnablesEvents;
			this._methodThatDisablesEvents = methodThatDisablesEvents;

			if (performEnableEventsNow)
			{
				this.EnableEvents();
			}
		}

		public EventsKeeperSession DisableInThisBlock()
		{
			return new EventsKeeperSession(this);
		}

		public void DisableEvents()
		{
			lock (this._lock)
			{
				if (this._level == 1)
				{
					this._methodThatDisablesEvents.Invoke();
				}
				this._level--;
			}
		}

		public void EnableEvents()
		{
			lock (this._lock)
			{
				if (this._level == 0)
				{
					this._methodThatEnablesEvents.Invoke();
				}
				this._level++;
			}
		}
	}

	public class EventsKeeperSession:IDisposable
	{
		private EventsDisabler _parent;

		public EventsKeeperSession(EventsDisabler parent)
		{
			this._parent = parent;

			this._parent.DisableEvents();                          //DISABLE
		}

		// Public implementation of Dispose pattern callable by consumers. 
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);       
		}

		// Flag: Has Dispose already been called? 
		bool _disposed = false;

		// Protected implementation of Dispose pattern. 
		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				this._parent.EnableEvents();                       //ENABLE

				if (disposing)
				{
					// Free any managed objects here. 
					//
				}

				// Free any unmanaged objects here. 
				//
				

				this._disposed = true;
			}
		}

		~EventsKeeperSession()
		{
			 this.Dispose(false);
		}
	}
}
