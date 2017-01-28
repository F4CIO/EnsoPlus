using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.IO;

namespace Common
{
    public class Logging
    {
        public static void AddActionLog(string message)
        {
	        bool inNewLine = true;
	        bool prependWithTimestamp = true;
			if (prependWithTimestamp && inNewLine)
			{
				message = message.PrependWithTimestamp();
			}
			CraftSynth.BuildingBlocks.IO.FileSystem.AppendFile(Settings.Current.ActionsLogFilePath, (inNewLine ? "\r\n" : string.Empty) + message, FileSystem.ConcurrencyProtectionMechanism.Lock);
        }
		public static void AddDebugLog(string message)
        {
			if (true) { 
				bool inNewLine = true;
				bool prependWithTimestamp = true;
				if (prependWithTimestamp && inNewLine)
				{
					message = message.PrependWithTimestamp();
				}
				CraftSynth.BuildingBlocks.IO.FileSystem.AppendFile(Settings.Current.ActionsLogFilePath, (inNewLine ? "\r\n" : string.Empty) + message, FileSystem.ConcurrencyProtectionMechanism.Lock);
			}
		}

        public static void AddErrorLog(string message)
        {
			bool inNewLine = true;
			bool prependWithTimestamp = true;
			if (prependWithTimestamp && inNewLine)
			{
				message = message.PrependWithTimestamp();
			}
			CraftSynth.BuildingBlocks.IO.FileSystem.AppendFile(Settings.Current.ErrorsLogFilePath, (inNewLine ? "\r\n" : string.Empty) + message, FileSystem.ConcurrencyProtectionMechanism.Lock);
        }

		public static void AddExceptionLog(Exception exception)
		{
			string filePath = Settings.Current.ExceptionsLogFilePath;
			bool justDeepestException = false;
			bool prependWithTimestamp = true;
			FileSystem.ConcurrencyProtectionMechanism concurrencyProtectionMechanism = FileSystem.ConcurrencyProtectionMechanism.Lock;

			List<Exception> exceptions = null;
			if (justDeepestException)
			{
				exceptions = new List<Exception>();
				exceptions.Add(Misc.GetDeepestException(exception));
			}
			else
			{
				exceptions = Misc.GetInnerExceptions(exception);
			}

			string firstLine = "===================================================================";
			if (prependWithTimestamp)
			{
				firstLine = firstLine.PrependWithTimestamp();
			}
			FileSystem.AppendFile(filePath, "\r\n" + firstLine + "\r\n", concurrencyProtectionMechanism);
			int i = 0;
			foreach (var ie in exceptions)
			{
				i++;
				FileSystem.AppendFile(filePath, ie.Message + "\r\n", concurrencyProtectionMechanism);
				FileSystem.AppendFile(filePath, ie.StackTrace + "\r\n", concurrencyProtectionMechanism);
				if (i < exceptions.Count)
				{
					FileSystem.AppendFile(filePath, "-------------------------------------------------" + "\r\n", concurrencyProtectionMechanism);
				}
			}
			FileSystem.AppendFile(filePath, "===============================================================================================" + "\r\n", concurrencyProtectionMechanism);
			FileSystem.AppendFile(filePath, "\r\n", concurrencyProtectionMechanism);
		}
    }
}
