using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CraftSynth.BuildingBlocks.Common;

namespace EnsoPlus.CommandsProviders.SsmsOpener
{
	
	public class DbConnectionInfo
	{
		public string rawString;
		public string serverName;
		public string serverPort;
		public string instanceName;
		public string databaseName;
		public bool useIntegratedSecurity;
		public string username;
		public string password;
		 
		public bool isValid
		{
			get
			{
				return this.serverName.IsNOTNullOrWhiteSpace() && 
					(this.serverPort==null || CraftSynth.BuildingBlocks.Validation.IsWholeNumber(this.serverPort));
			}
		}

		public override string ToString()
		{
			return this.ToString(null);
		}

		public string ToString(string replacePasswordWith=null)
		{
			return
				string.Format("serverName={0};serverPort={1};instanceName={2};databaseName={3};useIntegratedSecurity={4};username={5};password={6}",
				this.serverName.ToNonNullString("null"),	
				this.serverPort.ToNonNullString("null"),	
				this.instanceName.ToNonNullString("null"),	
				this.databaseName.ToNonNullString("null"),	
				this.useIntegratedSecurity.ToString(),	
				this.username.ToNonNullString("null"),	
				this.password==null?"null":(replacePasswordWith??this.password)
				);
		}

		public DbConnectionInfo(string connectionInfo)
		{
			this.rawString = connectionInfo;
			TryParsingAsDotNetConnectionString(connectionInfo);
			if (!this.isValid)
			{
				TryParsingAsSpaceSeparatedVaules(connectionInfo);
			}
		}

		private void TryParsingAsSpaceSeparatedVaules(string connectionInfo)
		{
			try
			{
				List<string> lines = connectionInfo.ToNonNullString().RemoveRepeatedSpaces().ToLines(false, false, null);
				if (lines.Count == 1)
				{
					string[] parts = lines[0].Split(' ');
					// server:port\instance databaseName username password
					if (parts.Length < 1 || parts.Length > 4)
					{
						throw new Exception("Invalid word count for syntax: server:port\\instance databaseName userdomain\\username password");
					}
					this.serverName = parts[0].Trim();
					if (this.serverName.Contains(':'))
					{
						this.serverPort = this.serverName.Split(':')[1].Trim();
						this.serverName = this.serverName.Split(':')[0].Trim();

						if (this.serverPort.Contains('\\'))
						{
							this.instanceName = this.serverPort.Split('\\')[1].Trim();
							this.serverPort = this.serverPort.Split('\\')[0].Trim();
						}
					}
					else if (this.serverName.Contains('\\'))
					{
						this.instanceName = this.serverName.Split('\\')[1].Trim();
						this.serverName = this.serverName.Split('\\')[0].Trim();
					}

					this.databaseName = parts[1];
					this.username = parts[2];
					this.password = parts[3];
				}
			}
			catch (Exception)
			{
				
			}
		}

		private void TryParsingAsDotNetConnectionString(string connectionInfo)
		{
			connectionInfo = connectionInfo.ToNonNullString().RemoveRepeatedSpaces().Trim().Replace("\\r","").Replace("\n","");
			List<string> parts = connectionInfo.Split(';').ToList();
			for (int i = parts.Count-1; i >= 0; i--)
			{
				parts[i] = parts[i].Trim();
			}
			try
			{//Data Source=64.90.169.231;user id=ica_intranet; pwd=xxx;database=ICA_INTRANET;Timeout=100000

				this.serverName = parts.First(p => p.ToLower().StartsWith("data source")).Split('=')[1].Trim();
				if (this.serverName.Contains(':'))
				{
					this.serverPort = this.serverName.Split(':')[1].Trim();
					this.serverName = this.serverName.Split(':')[0].Trim();

					if (this.serverPort.Contains('\\'))
					{
						this.instanceName = this.serverPort.Split('\\')[1].Trim();
						this.serverPort = this.serverPort.Split('\\')[0].Trim();
					}
				}
				else if (this.serverName.Contains('\\'))
				{
					this.instanceName = this.serverName.Split('\\')[1].Trim();
					this.serverName = this.serverName.Split('\\')[0].Trim();
				}
			}
			catch (Exception e)
			{
				this.serverName = null;
				this.serverPort = null;
				this.instanceName = null;
			}

			try
			{
				this.databaseName = parts.First(p => p.ToLower().StartsWith("initial catalog")).Split('=')[1].Trim();
			}
			catch (Exception e)
			{
				try
				{
					this.databaseName = parts.First(p => p.ToLower().StartsWith("database")).Split('=')[1].Trim();
				}
				catch (Exception e1)
				{
					this.databaseName = null;
				}
			}

			try
			{
				this.useIntegratedSecurity =
					bool.Parse(
						parts.First(p => p.ToLower().Contains("trusted") && p.ToLower().Contains("connection")).Split('=')[1].Trim());
			}
			catch (Exception e)
			{
				try
				{
					this.useIntegratedSecurity =
						bool.Parse(
							parts.First(p => p.ToLower().Contains("integrated") && p.ToLower().Contains("security")).Split('=')[1].Trim());
				}
				catch (Exception e1)
				{
					this.useIntegratedSecurity = false;
				}
			}

			try
			{
				this.username = parts.First(p => p.ToLower().StartsWith("user id")).Split('=')[1].Trim();
			}
			catch (Exception e)
			{
				this.username = null;
			}

			try
			{
				this.password = parts.First(p => p.ToLower().StartsWith("pwd")).Split('=')[1].Trim();
			}
			catch (Exception e)
			{
				try
				{
					this.password = parts.First(p => p.ToLower().StartsWith("password")).Split('=')[1].Trim();
				}
				catch
				{
					this.password = null;
				}
				
			}
		}
	}
}
