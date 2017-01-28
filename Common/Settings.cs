using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using CraftSynth.BuildingBlocks.IO;

namespace Common
{
    public class Settings
    {
        #region UserData

	    public string version = "2.0";
		public string ActionsLogFilePath =    @"{EnsoPlusDataFolder}\Logs\Enso+.log";
		public string ErrorsLogFilePath =     @"{EnsoPlusDataFolder}\Logs\Enso+.log";
		public string ExceptionsLogFilePath = @"{EnsoPlusDataFolder}\Logs\Enso+.log";
	    public bool DeleteActionsLogOnStart = false;
	    public bool DeleteErrorsLogOnStart = false;
	    public bool DeleteExceptionsLogOnStart = false;

        //Put settings fields here with their default values
        public bool firstRun = true;
        public string startOfSyntaxParameter = "[";
        public string endOfSyntaxParameter = "]";
        public string startOfPostfixParameter = "+";
        public string endOfPostfixParameter = "+";
        public string selectionInPostfix1 = "s";
        public string selectionInPostfix2 = "selection";
        public string lastMessageInPostfix = "last message";
        public string lastParameterInPostfix = "last";

		public string EnsoLearnAsOpenCommandsFolder = @"D:\Enso+\Shortcuts\";
        public string EnsoLearnAsOpenCommandsRetiredFilesFolder = @"D:\Enso+ Shortcuts\Retired";
        public string EnsoPlusDataFolder = @"D:\My Dropbox\Settings\Enso+ Data";

        public bool CallerHangUpAfterDialing = true;
        public int CallerHangUpAfterSeconds = 7;
        public string CallerE61ContactsFilePath = @"{EnsoPlusDataFolder}\Caller Data\Nokia E61 Contacts.txt";
        public string CallerNetworkConnectionName = "ENSO Caller";
        public string NetworkCredentialUsername = "F4CIO";
        public string NetworkCredentialPassword = "wni";

        public string MacrosDataFolder = @"{EnsoPlusDataFolder}\Macros";
        public string startOfMacroParameter = "{";
        public string endOfMacroParameter = "}";

        public string MemorizerDataFolder = @"{EnsoPlusDataFolder}\Memorized Data";

        public string TOpenerExePath = @"C:\Program Files\Total Commander\TOTALCMD.EXE";
        public string TOpenerParametersFormatForOpenInLeftPane = @" /O /P=L /L=""{0}""";
        public string TOpenerParametersFormatForOpenInRightPane = @" /O /P=R /R=""{0}""";

	    //public string RdcExePath = @"mstsc";
	    //public string RdcParamatersFormat = @"/v:{0}";
	    public string RdcDefaultUsername = "Administrator";
	    public string RdcDefaultPassword = "";

        public string FOpenerExePath = @"c:\Program Files\Mozilla Firefox\firefox.exe";
        public string FOpenerParametersFormatForOpenInNewWindow = @" -new-window ""{0}""";
        public string FOpenerParametersFormatForOpenInNewTab = @" -new-tab ""{0}""";

        public string COpenerExePath = @"C:\Users\F4CIO\AppData\Local\Google\Chrome\Application\chrome.exe";
        public string COpenerParametersFormatForOpenInNewWindow = @" -new-window ""{0}""";
        public string COpenerParametersFormatForOpenInNewTab = @" ""{0}""";
        public string COpenerParametersFormatForOpenInNewIncognitoWindow = @" -new-window -incognito ""{0}""";
        public string COpenerParametersFormatForOpenInNewIncognitoTab = @" -incognito ""{0}""";

        public string IOpenerExePath = @"c:\Program Files\Internet Explorer\iexplore.exe";
        public string IOpenerParametersFormatForOpenInNewWindow = @" ""{0}""";
        public string IOpenerParametersFormatForOpenInNewTab = @" ""{0}""";

        public string FileManagerSaveToDefaultFolder = @"C:\";

		public string WebSearchExtensionFolder = @"{EnsoPlusDataFolder}\OpenSearchDescriptions";

        public string ImagesFolder = @"{EnsoPlusDataFolder}\Images";

        public string BackupManagerExePath = @"{EnsoPlusWorkingFolder}\Tools\7-zip\7z.exe";
        public string BackupManagerParameters = @"a -r ""{DestinationFolderPath}\{CurrentDateTime} {Label}\{SourceFolderName}"" ""{SourceFolderPath}""";
        public string BackupProfilesFolder = @"{EnsoPlusDataFolder}\Backup Profiles";

	    /// <summary>
	    /// If settings file not exist this constructors set default values (after inline constrictors from above)
	    /// </summary>

	    #region Settings fields first-time-constructors
	    private void InitLogs()
	    {
		    bool performDelay = false;
		    if (!Directory.Exists(Path.GetDirectoryName(this.ActionsLogFilePath)))
		    {
			    Directory.CreateDirectory(Path.GetDirectoryName(this.ActionsLogFilePath));
			    performDelay = true;
		    }
		    if (!File.Exists(this.ActionsLogFilePath))
		    {
				using (StreamWriter sw = new StreamWriter(this.ActionsLogFilePath, true)) { }
			    performDelay = true;
		    }

			if (!Directory.Exists(Path.GetDirectoryName(this.ErrorsLogFilePath)))
		    {
				Directory.CreateDirectory(Path.GetDirectoryName(this.ErrorsLogFilePath));
			    performDelay = true;
		    }
			if (!File.Exists(this.ErrorsLogFilePath))
		    {
				using (StreamWriter sw = new StreamWriter(this.ErrorsLogFilePath, true)) { }
			    performDelay = true;
		    }
			
			if (!Directory.Exists(Path.GetDirectoryName(this.ExceptionsLogFilePath)))
		    {
				Directory.CreateDirectory(Path.GetDirectoryName(this.ExceptionsLogFilePath));
			    performDelay = true;
		    }
			if (!File.Exists(this.ExceptionsLogFilePath))
		    {
				using (StreamWriter sw = new StreamWriter(this.ExceptionsLogFilePath, true)) { }
			    performDelay = true;
		    }

		    if (performDelay)
		    {
				//for (int i = 25 - 1; i >= 0; i--)
				//{
				//	Application.DoEvents();
				//	Thread.Sleep(100);
				//}
		    }
	    }

        private void InitEnsoLearnAsOpenCommandsFolder()
        {
            try
            {
                this.EnsoLearnAsOpenCommandsFolder = Path.Combine(this.EnsoPlusDataFolder,@"Shortcuts\");
	            if (!Directory.Exists(this.EnsoLearnAsOpenCommandsFolder))
	            {
		            Directory.CreateDirectory(this.EnsoLearnAsOpenCommandsFolder);
	            }
			}
            catch(Exception exception)
            {
				Common.Logging.AddExceptionLog(exception);
            }           
        }

        private void InitEnsoLearnAsOpenCommandsRetiredFilesFolder()
        {
            try
            {
                this.EnsoLearnAsOpenCommandsRetiredFilesFolder = Path.Combine(this.EnsoPlusDataFolder, @"Shortcuts\Retired");
	            if (!Directory.Exists(this.EnsoLearnAsOpenCommandsRetiredFilesFolder))
	            {
		            Directory.CreateDirectory(this.EnsoLearnAsOpenCommandsRetiredFilesFolder);
	            }
			}
            catch (Exception exception)
            {
				Common.Logging.AddExceptionLog(exception);
            }
        }

        private void InitEnsoPlusDataFolder()
        {
			//string keyPath = null;
			//try
			//{
			//	//Try set data folder in dropBox if dropbox is installed-otherwise set it in my documents folder
			//	//keyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Evenflow Software\Dropbox\InstallPath";
			//	//string value = Helper.GetRegistryStringValue(keyPath);
			//	//if(string.IsNullOrEmpty(value))
			//	//{
			//		keyPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders\Personal";
			//		string value = CraftSynth.BuildingBlocks.IO.Registry.GetRegistryStringValue(keyPath);
			//	//}
			//	this.EnsoPlusDataFolder = Path.Combine(value, @"Enso+ Data");
			//}
			//catch (Exception exception)
			//{
			//	Exception warperException = new Exception(string.Format("Can not read from registry key: '{0}'", keyPath), exception);
			//	Common.Logging.AddExceptionLog(warperException);
			//}

			string r = CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath;
			r = Path.Combine(r, "User Data");
			r = Path.Combine(r, Common.Helper.GetCurrentWindowsUserName());
			r = Path.Combine(r, "Enso+ Data");
			this.EnsoPlusDataFolder = r;
        }

		private void InitCOpenerExePath()
		{
			string keyPath = null;
			try
			{
				keyPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\Google Chrome\InstallLocation";
				string value = CraftSynth.BuildingBlocks.IO.Registry.GetRegistryStringValue(keyPath, RegistryRetryScenario.TryDefaultAndThen32AndThen64);
				
				if (value == null)
				{
					keyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Google Chrome\InstallLocation";
					value = CraftSynth.BuildingBlocks.IO.Registry.GetRegistryStringValue(keyPath, RegistryRetryScenario.TryDefaultAndThen32AndThen64);
				}
				
				if (value != null)
				{
					value = Path.Combine(value, "chrome.exe");
				}
				this.COpenerExePath = value;
			}
			catch (Exception exception)
			{
				Exception warperException = new Exception(string.Format("Can not read from registry key: '{0}'", keyPath), exception);
				Common.Logging.AddExceptionLog(warperException);
			}
		}

        private void InitFOpenerExePath()
        {
            string keyPath = null;
            try
            {
                keyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Mozilla\Mozilla Firefox\{current version}\Main\PathToExe";
				keyPath = keyPath.Replace("{current version}", CraftSynth.BuildingBlocks.IO.Registry.GetRegistryStringValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Mozilla\Mozilla Firefox\CurrentVersion", RegistryRetryScenario.TryDefaultAndThen32AndThen64));
				string value = CraftSynth.BuildingBlocks.IO.Registry.GetRegistryStringValue(keyPath, RegistryRetryScenario.TryDefaultAndThen32AndThen64);
                this.FOpenerExePath = value;
            }
            catch(Exception exception)
            {
                Exception warperException = new Exception(string.Format("Can not read from registry key: '{0}'", keyPath), exception);
				Common.Logging.AddExceptionLog(warperException);
            }           
        }

        private void InitTOpenerExePath()
        {
            string keyPath = null;
            try
            {
                keyPath = @"HKEY_CURRENT_USER\Software\Ghisler\Total Commander\InstallDir";
				string value = CraftSynth.BuildingBlocks.IO.Registry.GetRegistryStringValue(keyPath, RegistryRetryScenario.TryDefaultAndThen32AndThen64);
                this.TOpenerExePath = Path.Combine(value,"Totalcmd.exe");
            }
            catch(Exception exception)
            {
                Exception warperException = new Exception(string.Format("Can not read from registry key: '{0}'", keyPath), exception);
				Common.Logging.AddExceptionLog(warperException);
            }           
        }

        private static void ReplaceEnsoPlusDataFolderKeyword(Settings instance)
        {
            foreach (FieldInfo fieldInfo in instance.GetType().GetFields())
            {
                if (fieldInfo.FieldType==typeof(string))
                {
                    string stringValue = fieldInfo.GetValue(instance) as string;
                    if (!string.IsNullOrEmpty(stringValue) && stringValue.Contains("{EnsoPlusDataFolder}"))
                    {
                        try
                        {
                            stringValue = stringValue.Replace("{EnsoPlusDataFolder}", instance.EnsoPlusDataFolder);

                            fieldInfo.SetValue(instance, stringValue);
                        }
                        catch { }
                    }
                }
            }
        }

        private static void ReplaceEnsoPlusWorkingFolderKeyword(Settings instance)
        {
            foreach (FieldInfo fieldInfo in instance.GetType().GetFields())
            {
                if (fieldInfo.FieldType == typeof(string))
                {
                    string stringValue = fieldInfo.GetValue(instance) as string;
                    if (!string.IsNullOrEmpty(stringValue) && stringValue.Contains("{EnsoPlusWorkingFolder}"))
                    {
                        try
                        {
                            stringValue = stringValue.Replace("{EnsoPlusWorkingFolder}", Common.Helper.GetEnsoPlusWorkingFolder());

                            fieldInfo.SetValue(instance, stringValue);
                        }
                        catch { }
                    }
                }
            }
        }
        #endregion 

        public Settings()
        {
			this.InitEnsoPlusDataFolder();
            this.InitEnsoLearnAsOpenCommandsFolder();
            this.InitEnsoLearnAsOpenCommandsRetiredFilesFolder();
           
            this.InitCOpenerExePath();
            this.InitFOpenerExePath();
            this.InitTOpenerExePath();  
        }
        #endregion UserData

        private static Settings _current;
        public static Settings Current
        {
            get
            {
                if (_current == null)
                {
                    Initialize();
                }
                return _current;
            }
        }

        /// <summary>
        /// If settings exists loads it, if not loads default values.
        /// </summary>
        public static void Initialize()
        {
            if (!SettingsExist())
            {
                _current = new Settings();
                Save(_current);
                ReplaceEnsoPlusDataFolderKeyword(_current);
                ReplaceEnsoPlusWorkingFolderKeyword(_current);
            }
            else
            {
                _current = Load();
            }
			_current.InitLogs();
        }

        public static string FilePath
        {
            get
            {
	            string r = CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath;
	            r = Path.Combine(r, "User Data");
	            r = Path.Combine(r, Common.Helper.GetCurrentWindowsUserName());
				r = Path.Combine(r, "Enso+ Data");
	            r = Path.Combine(r, "Settings");
	            r = Path.Combine(r, "Enso+.xml");
                return r;
            }
        }

	    public void Save()
	    {
		    Settings.Save(_current);

			string content = File.ReadAllText(Settings.FilePath);
		    
			content = content.Replace(_current.EnsoPlusDataFolder, "{EnsoPlusDataFolder}")
				             .Replace("<EnsoPlusDataFolder>{EnsoPlusDataFolder}</EnsoPlusDataFolder>", string.Format("<EnsoPlusDataFolder>{0}</EnsoPlusDataFolder>", _current.EnsoPlusDataFolder));
			content = content.Replace(Common.Helper.GetEnsoPlusWorkingFolder(), "{EnsoPlusWorkingFolder}");
			File.WriteAllText(Settings.FilePath, content);
	    }

        /// <summary>
        /// Tells if settings are present in data source.
        /// </summary>
        /// <returns></returns>
        protected static bool SettingsExist()
        {
            return File.Exists(FilePath);
        }

        /// <summary>
        /// Loads settings from data source and returns it.
        /// </summary>
        /// <returns></returns>
        protected static Settings Load()
        {
            Settings settings = null;
            try
            {
				string content = File.ReadAllText(Settings.FilePath);
				content = Settings.UpgradeContentToLatestVersionWhereOldRecognized(content);
				File.WriteAllText(Settings.FilePath, content);

				settings = (Settings)CraftSynth.BuildingBlocks.IO.Xml.Misc.Deserialize(typeof(Settings), FilePath);
                ReplaceEnsoPlusDataFolderKeyword(settings);
                ReplaceEnsoPlusWorkingFolderKeyword(settings);
            }
            catch (Exception exception)
            {
				Common.Logging.AddExceptionLog(exception);
	            throw exception;
            }
            return settings;
        }
		

        /// <summary>
        /// Saves provided settings to data source.
        /// </summary>
        private static void Save(Settings settings)
        {
            try
            {
	            if (!Directory.Exists(Path.GetDirectoryName(Settings.FilePath)))
	            {
		            Directory.CreateDirectory(Path.GetDirectoryName(Settings.FilePath));
	            }
	            if (!File.Exists(Settings.FilePath))
	            {
					using (StreamWriter sw = new StreamWriter(Settings.FilePath, true)) { }
	            }
				CraftSynth.BuildingBlocks.IO.Xml.Misc.Serialize(settings, FilePath);
            }
            catch (Exception exception)
            {
				Common.Logging.AddExceptionLog(exception);
	            throw exception;
            }
        }

		protected static string UpgradeContentToLatestVersionWhereOldRecognized(string content)
		{
			if (!content.Contains("<FOpenerExePath>"))
			{
				content = content.Replace("</FOpenerExePath>",
										  "<FOpenerExePath>C:\\Program Files\\Mozilla Firefox\\firefox.exe</FOpenerExePath>\r\n");
				
			}

			if (!content.Contains("<RdcDefaultUsername>"))
			{
				
				content = content.Replace("</FOpenerExePath>",
										  "</FOpenerExePath>\r\n<RdcDefaultUsername>Administrator</RdcDefaultUsername>\r\n<RdcDefaultPassword></RdcDefaultPassword>\r\n");
				
			}
			return content;
		}
    }
}
