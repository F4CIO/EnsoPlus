using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using CraftSynth.BuildingBlocks.Common;
using CBB_IOFileSystem = CraftSynth.BuildingBlocks.IO.FileSystem;
using CBB_CommonMisc = CraftSynth.BuildingBlocks.Common.Misc;
using CBB_UIConsole = CraftSynth.BuildingBlocks.UI.Console;
using CBB_UIWindowsFormsMisc = CraftSynth.BuildingBlocks.UI.WindowsForms.Misc;

namespace CraftSynth.BuildingBlocks.Logging
{
    public class Misc
    {
	    public static void AddTimestampedLineToApplicationWideLog(string line, bool inNewLine = true, string fileName = null, bool prependWithTimestamp = true, bool forceMutexUsage = false)
		{
			if (fileName == null)
			{
				switch (CBB_CommonMisc.GetApplicationType())
				{
					case CBB_CommonMisc.ApplicationType.Console:
						fileName = Path.GetFileNameWithoutExtension(CBB_UIConsole.ApplicationPhysicalExeFilePath) + ".log";
						break;
					case CBB_CommonMisc.ApplicationType.Forms:
						fileName = Path.GetFileNameWithoutExtension(CBB_UIWindowsFormsMisc.ApplicationPhysicalExeFilePath) + ".log";
						break;
					case CBB_CommonMisc.ApplicationType.AspNet:
						fileName = "Exceptions.log";
						break;
					default:
						throw new Exception("AddTimestampedExceptionInfoToApplicationWideLog: Application type not supported. Application type:" + CBB_CommonMisc.GetApplicationType());
				}
			}
			string filePath = Path.Combine(CBB_CommonMisc.ApplicationRootFolderPath, fileName);
		    if (prependWithTimestamp && inNewLine)
		    {
			    line = line.PrependWithTimestamp();
		    }
		    CBB_IOFileSystem.AppendFile(filePath,(inNewLine?"\r\n":string.Empty) + line,  forceMutexUsage?CBB_IOFileSystem.ConcurrencyProtectionMechanism.Mutex:CBB_IOFileSystem.ConcurrencyProtectionMechanism.Lock);
		}

		public static void AddTimestampedExceptionInfoToApplicationWideLog(Exception exception, string fileName = null, bool justDeepestException = true, bool prependWithTimestamp = true, bool forceMutexUsage = false)
		{
			var concurrencyProtectionMechanism = forceMutexUsage?CBB_IOFileSystem.ConcurrencyProtectionMechanism.Mutex:CBB_IOFileSystem.ConcurrencyProtectionMechanism.Lock;
			if (fileName == null)
			{
				switch (CBB_CommonMisc.GetApplicationType())
				{
					case CBB_CommonMisc.ApplicationType.Console:
						fileName = Path.GetFileNameWithoutExtension(CBB_UIConsole.ApplicationPhysicalExeFilePath) + ".log";
						break;
					case CBB_CommonMisc.ApplicationType.Forms:
						fileName  = Path.GetFileNameWithoutExtension(CBB_UIWindowsFormsMisc.ApplicationPhysicalExeFilePath)+".log";
						break;
					case CBB_CommonMisc.ApplicationType.AspNet:
						fileName = "Exceptions.log";
						break;
					default:
						throw new Exception("AddTimestampedExceptionInfoToApplicationWideLog: Application type not supported. Application type:"+CBB_CommonMisc.GetApplicationType());
				}
			}
			string filePath = Path.Combine(CBB_CommonMisc.ApplicationRootFolderPath, fileName);

			List<Exception> exceptions = null;
			if (justDeepestException)
			{
				exceptions = new List<Exception>();
				exceptions.Add(CBB_CommonMisc.GetDeepestException(exception));
			}
			else
			{
				exceptions = CBB_CommonMisc.GetInnerExceptions(exception);
			}

			string firstLine = "===================================================================";
			if (prependWithTimestamp)
		    {
			    firstLine = firstLine.PrependWithTimestamp();
		    }
			CBB_IOFileSystem.AppendFile(filePath, "\r\n"+firstLine + "\r\n", concurrencyProtectionMechanism);
			int i = 0;
			foreach (var ie in exceptions)
			{
				i++;
				CBB_IOFileSystem.AppendFile(filePath, ie.Message + "\r\n",concurrencyProtectionMechanism);
				CBB_IOFileSystem.AppendFile(filePath, ie.StackTrace + "\r\n", concurrencyProtectionMechanism);
				if (i < exceptions.Count)
				{
					CBB_IOFileSystem.AppendFile(filePath, "-------------------------------------------------" + "\r\n", concurrencyProtectionMechanism);
				}
			}
			CBB_IOFileSystem.AppendFile(filePath, "===============================================================================================" + "\r\n", concurrencyProtectionMechanism);
			CBB_IOFileSystem.AppendFile(filePath, "\r\n", concurrencyProtectionMechanism);
		}

	    public static string GetExceptionDescription(Exception exception, bool justDeepestException, bool prependWithTimestamp)
	    {
		    string r = string.Empty;

		    List<Exception> exceptions = null;
		    if (justDeepestException)
		    {
			    exceptions = new List<Exception>();
			    exceptions.Add(CBB_CommonMisc.GetDeepestException(exception));
		    }
		    else
		    {
			    exceptions = CBB_CommonMisc.GetInnerExceptions(exception);
		    }

		    string firstLine = "===================================================================";
		    if (prependWithTimestamp)
		    {
			    firstLine = firstLine.PrependWithTimestamp();
		    }
		    r = r + "\r\n" + firstLine + "\r\n";
		    int i = 0;
		    foreach (var ie in exceptions)
		    {
			    i++;
			    r = r +ie.Message + "\r\n";
			    r = r +ie.StackTrace + "\r\n";
			    if (i < exceptions.Count)
			    {
				    r = r + "-------------------------------------------------" + "\r\n";
			    }
		    }
		    r = r + "===============================================================================================" + "\r\n";
		    r = r + "\r\n";

		    return r;
	    } 
    }
}
