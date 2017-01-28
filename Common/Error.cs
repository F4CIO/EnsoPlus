using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Common
{
	public class Errors :List<Error>
	{
		public bool IsEmpty
		{
			get { return this.Count == 0; }
		}

		public void Add(string key, string friendlyMessage, Exception innerException = null)
		{
			this.Add(new Error(key,friendlyMessage, innerException));
		}

		public Errors OnlyOnesToShowOnUI
		{
			get
			{
				var r = new Errors();
				r.AddRange(this.Where(e => !string.IsNullOrEmpty(e.FriendlyMessage) && e.SkipLoggingIntoDbAndMail == true));
				return r;
			}
		}
	}

	//[Serializable] - throws NullReferenceException
	public class Error : Exception
	{
		/// <summary>
		/// Usualy used to keep name of input field where validation did't pass. 
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Tf exception is not handled/catched explicitly in code this message will be displayed to the user.
		/// If this property is null then generic/default error message is displayed.
		/// </summary>
		public string FriendlyMessage { get; set; }

		/// <summary>
		/// Attach additional data here like trace log and values of local variables where exception occured. 
		/// To build trace string use CustomTraceLog class.
		/// </summary>
		public string LongMessage { get; set; }

		/// <summary>
		/// If used with friendly message user will be notified about error but nothing will be logged in our system.
		/// </summary>
		public bool SkipLoggingIntoDbAndMail { get; set; }

		public Error(string message)
			: base(message)
		{

		}

		/// <summary>
		/// Use this constructor to create error that will be showed to user on UI and that should not be logged into system.
		/// </summary>
		/// <param name="key">Usualy name of input field where validation did't pass.</param>
		/// <param name="friendlyMessage">Message that should be displayed on screen to user.</param>
		/// <param name="innerException">Can be used to show technical details of an error if user insist.</param>
		public Error(string key, string friendlyMessage, Exception innerException = null): base(friendlyMessage, innerException)
		{
			this.FriendlyMessage = friendlyMessage;
			this.LongMessage = null;
			this.SkipLoggingIntoDbAndMail = true;
			this.Key = key;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		/// <param name="longMessage">
		/// Attach additional data here like trace log and values of local variables where exception occured. 
		/// To build trace string use CustomTraceLog class.
		/// </param>
		/// <param name="friendlyMessage">
		/// Tf exception is not handled/catched explicitly in code this message will be displayed to the user.
		/// If this property is null then generic/default error message is displayed.
		/// </param>
		/// <param name="skipLoggingIntoDbAndMail">
		/// If used with friendly message user will be notified about error but nothing will be logged in our system.
		/// </param>
		/// <param name="key">
		/// Usualy used to keep name of input field where validation did't pass.
		/// </param> 
		public Error(string message, Exception innerException = null, string longMessage = null, string friendlyMessage = null, bool skipLoggingIntoDbAndMail = false, string key = null)
			: base(message, innerException)
		{
			this.FriendlyMessage = friendlyMessage;
			this.LongMessage = longMessage;
			this.SkipLoggingIntoDbAndMail = skipLoggingIntoDbAndMail;
			this.Key = key;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			if (info != null)
			{
				info.AddValue("LongMessage", this.LongMessage ?? String.Empty);
				info.AddValue("FriendlyMessage", this.FriendlyMessage ?? String.Empty);
			}
		}
	}
}
