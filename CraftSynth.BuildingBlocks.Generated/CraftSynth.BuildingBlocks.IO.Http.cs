using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using CBB_CommonExtenderClass = CraftSynth.BuildingBlocks.Common.ExtenderClass;
using CBB_LoggingCustomTraceLog = CraftSynth.BuildingBlocks.Logging.CustomTraceLog;
using CBB_LoggingCustomTraceLogExtensions = CraftSynth.BuildingBlocks.Logging.CustomTraceLogExtensions;

namespace CraftSynth.BuildingBlocks.IO
{
    public class Http
    {
		/// <summary>
		/// [contentType] can be for example:image/jpeg, pdf, text/html, application/zip, video/mpeg
		/// For rest refer to:
		/// http://en.wikipedia.org/wiki/MIME_type#List_of_common_media_types
		/// [fileName] desired name that should be given on client.
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="contentType"></param>
		public static void SendBytesToResponseStream(byte[] bytes, string contentType, string fileName)
		{
			//"image/" + Path.GetExtension(localFilePath).Remove(0, 1);
			HttpContext.Current.Response.Buffer = true;
			HttpContext.Current.Response.Charset = "";
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			HttpContext.Current.Response.ContentType = contentType;
			HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName));
			HttpContext.Current.Response.BinaryWrite(bytes);
			HttpContext.Current.Response.Flush();
			HttpContext.Current.Response.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="domain"></param>
		/// <param name="accessToken"></param>
		/// <param name="headers"></param>
		/// <param name="isPostMethod"></param>
		/// <param name="requestUriContentType">0-default; 1-JSON</param>
		/// <param name="customTraceLog"></param>
		/// <param name="blockAllExceptions"></param>
		/// <param name="errorCaseResult"></param>
		/// <returns></returns>
		public static string RequestUri(string uri, string username=null, string password=null, string domain=null, /*string accessToken = null, */ List<string> headers = null, bool isPostMethod = false,string postData = null, int requestUriContentType = 0, object customTraceLog = null, bool blockAllExceptions = false, string errorCaseResult=null, bool expectCookies = false, List<Cookie> cookies = null, bool detailedLog = true)
		{
			string r = null;

			CBB_LoggingCustomTraceLog log = CBB_LoggingCustomTraceLog.Unbox(customTraceLog);
			try
			{
				CBB_LoggingCustomTraceLogExtensions.AddLineAndIncreaseIdent(log, "Requesting '" + uri + "'...");

				// used to build entire input
				StringBuilder sb = new StringBuilder();

				// used on each read operation
				byte[] buf = new byte[8192];

				// prepare the web page we will be asking for
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
				if (CBB_CommonExtenderClass.IsNOTNullOrWhiteSpace(username) && CBB_CommonExtenderClass.IsNullOrWhiteSpace(password))
				{
					throw new Exception("Password must be specified if username is.");
				}
				else if(CBB_CommonExtenderClass.IsNullOrWhiteSpace(username) && CBB_CommonExtenderClass.IsNOTNullOrWhiteSpace(password))
				{
					throw new Exception("Username must be specified when password is.");
				}
				else if(CBB_CommonExtenderClass.IsNOTNullOrWhiteSpace(username) && CBB_CommonExtenderClass.IsNOTNullOrWhiteSpace(password))
				{
					if (CBB_CommonExtenderClass.IsNullOrWhiteSpace(domain))
					{
						request.Credentials = new NetworkCredential(username, password);
					}
					else
					{
						request.Credentials = new NetworkCredential(username, password, domain);
					}

					CBB_LoggingCustomTraceLogExtensions.AssignSensitiveString(log, password, 2);
					if (detailedLog) { 
						CBB_LoggingCustomTraceLogExtensions.AddLine(log, "Requesting with credentials '" + CBB_CommonExtenderClass.AppendIfNotNullOrWhiteSpace(CBB_CommonExtenderClass.ToNonNullString(domain),"\\")+username+" "+CBB_CommonExtenderClass.ToNonNullString(password)+"'...");
					}
				}

				//if (accessToken.IsNOTNullOrWhiteSpace())
				//{
				//	request.Headers.Add("access_token", accessToken);
				//}

				if (headers != null)
				{
					foreach (string header in headers)
					{
						//http://stackoverflow.com/questions/239725/cannot-set-some-http-headers-when-using-system-net-webrequest
						if (header.ToLower() == "connection:keep-alive")
						{
							request.KeepAlive = true;
						}
						else if(header.Split(':')[0].ToLower()=="referer")
						{
							request.Referer = header.Substring(header.IndexOf(':')+1);
						}
						else if(header.Split(':')[0].ToLower()=="user-agent")
						{
							request.UserAgent = header.Substring(header.IndexOf(':') + 1);
						}
						else if(header.Split(':')[0].ToLower()=="accept")
						{
							request.Accept = header.Substring(header.IndexOf(':') + 1);
						}
						else
						{
							if (request.Headers.AllKeys.Contains(header.Split(':')[0]))
							{
								request.Headers.Set(header.Split(':')[0], header.Substring(header.IndexOf(':')+1));
							}
							else
							{
								request.Headers.Add(header);
							}
						}
					}
					if (detailedLog)
					{
						CBB_LoggingCustomTraceLogExtensions.AddLine(log, "Requesting with headers '" + CBB_CommonExtenderClass.ToCSV(headers) + "'...");
					}
				}

				if (isPostMethod)
				{
					request.Method = "POST";
				}

				switch (requestUriContentType)
				{
					case (int)RequestUriContentType.Default:
						break;
					case (int)RequestUriContentType.ApplicationJson:
						//var hk = request.Headers.AllKeys.FirstOrDefault(k => k.ToLower() == "content-type");
						//if(hk!=null)
						//{
						//	request.Headers.Remove(hk);
						//}
						//request.Headers.Add("Content-Type", "application/json");
						request.ContentType = "application/json";

						if (CBB_CommonExtenderClass.IsNOTNullOrWhiteSpace(postData))
						{
							if (detailedLog)
							{
								CBB_LoggingCustomTraceLogExtensions.AddLine(log, "Requesting with post data '" + postData + "'...");
							}
							using (var streamWriter = new StreamWriter(request.GetRequestStream()))
							{
								//initiate the request
								//JavaScriptSerializer serializer = new JavaScriptSerializer();
								//var d = serializer.Deserialize<Dictionary<string, object>>(postData);
								streamWriter.Write(postData);
								streamWriter.Flush();
								streamWriter.Close();
							}
						}
						break;

					case (int)RequestUriContentType.TextJson:
						//var hk = request.Headers.AllKeys.FirstOrDefault(k => k.ToLower() == "content-type");
						//if(hk!=null)
						//{
						//	request.Headers.Remove(hk);
						//}
						//request.Headers.Add("Content-Type", "application/json");
						request.ContentType = "text/json";

						if (CBB_CommonExtenderClass.IsNOTNullOrWhiteSpace(postData))
						{
							if (detailedLog)
							{
								CBB_LoggingCustomTraceLogExtensions.AddLine(log, "Requesting with post data '" + postData + "'...");
							}
							using (var streamWriter = new StreamWriter(request.GetRequestStream()))
							{
								//initiate the request
								//JavaScriptSerializer serializer = new JavaScriptSerializer();
								//var d = serializer.Deserialize<Dictionary<string, object>>(postData);
								streamWriter.Write(postData);
								streamWriter.Flush();
								streamWriter.Close();
							}
						}
						break;
					case (int)RequestUriContentType.ApplicationXWwwFormUlrEncoded:
						request.ContentType = "application/x-www-form-urlencoded";
						ASCIIEncoding encoder = new ASCIIEncoding();
						byte[] data = encoder.GetBytes(postData); // a json object, or xml, whatever...
						request.ContentLength = data.Length;
						//request.Expect = "application/json";
						request.GetRequestStream().Write(data, 0, data.Length);
						break;
				}

				if (cookies != null)
				{
					request.CookieContainer = new CookieContainer();
					foreach (Cookie cookie in cookies)
					{
						request.CookieContainer.Add(cookie);
					}
				}

				request.AllowAutoRedirect = false;
				
				// execute the request
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				// we will read data via the response stream
				Stream resStream = response.GetResponseStream();

				string tempString = null;
				int count = 0;

				do
				{
					// fill the buffer with data
					count = resStream.Read(buf, 0, buf.Length);

					// make sure we read some data
					if (count != 0)
					{
						// translate from bytes to ASCII text
						tempString = Encoding.ASCII.GetString(buf, 0, count);

						// continue building the string
						sb.Append(tempString);
					}
				}
				while (count > 0); // any more data to read?

				r = sb.ToString();

				if (expectCookies && response.Cookies != null && response.Cookies.Count > 0)
				{
					cookies.Clear();
					for (int i = 0; i < response.Cookies.Count; i++)
					{
						cookies.Add(response.Cookies[i]);
					}
				}

				if (detailedLog)
				{
					CBB_LoggingCustomTraceLogExtensions.AddLine(log, "Requesting succeeded. Response length in chars:" + r.Length);
				}
			}
			catch (Exception exception)
			{
				if (detailedLog)
				{
					CBB_LoggingCustomTraceLogExtensions.AddLine(log, "Requesting of '" + uri + "' failed. Error details:" + exception.Message);
				}
				if (!blockAllExceptions)
				{
					throw;
				}
				else
				{
					r = errorCaseResult;
				}
			}
			finally { 
				CBB_LoggingCustomTraceLogExtensions.DecreaseIdent(log);
			}
			return r;
		}

    }

	public enum RequestUriContentType
	{
		Default = 0,
		ApplicationJson = 1,
		ApplicationXWwwFormUlrEncoded = 2,
		TextJson = 3
	}
}
