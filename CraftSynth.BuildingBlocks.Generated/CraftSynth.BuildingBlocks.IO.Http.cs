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
		public static string RequestUri(string uri, string username=null, string password=null, string domain=null, /*string accessToken = null, */ List<string> headers = null, bool isPostMethod = false,string postData = null, int requestUriContentType = 0, object customTraceLog = null, bool blockAllExceptions = false, string errorCaseResult=null, bool expectCookies = false, List<Cookie> cookies = null, bool detailedLog = true, System.Text.Encoding encoderForParsingResult = null, string fileNameWhereToDumpResponse = null, bool? allowAutoRedirect = null)
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
				WebRequest request = (WebRequest)WebRequest.Create(uri);
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
							if (request is HttpWebRequest)
							{
								(request as HttpWebRequest).KeepAlive = true;
							}
							else
							{
								throw new Exception("Can not set http headers when using WebRequest.");
							}
						}
						else if(header.Split(':')[0].ToLower()=="referer")
						{
							
							if (request is HttpWebRequest)
							{
								(request as HttpWebRequest).Referer = header.Substring(header.IndexOf(':') + 1);
							}
							else
							{
								throw new Exception("Can not set http headers when using WebRequest.");
							}
						}
						else if(header.Split(':')[0].ToLower()=="user-agent")
						{
							
							if (request is HttpWebRequest)
							{
								(request as HttpWebRequest).UserAgent = header.Substring(header.IndexOf(':') + 1);
							}
							else
							{
								throw new Exception("Can not set http headers when using WebRequest.");
							}
						}
						else if(header.Split(':')[0].ToLower()=="accept")
						{
							if (request is HttpWebRequest)
							{
								(request as HttpWebRequest).Accept = header.Substring(header.IndexOf(':') + 1);
							}
							else
							{
								throw new Exception("Can not set http headers when using WebRequest.");
							}
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
					case (int)RequestUriContentType.ImageJpeg:
					case (int)RequestUriContentType.ImagePng:
					case (int)RequestUriContentType.ImageGif:
						switch (requestUriContentType)
						{
							case (int)RequestUriContentType.ImageJpeg:request.ContentType = "image/jpeg";break;
							case (int)RequestUriContentType.ImagePng:request.ContentType = "image/png";break;
							case (int)RequestUriContentType.ImageGif:request.ContentType = "image/gif";break;
						}

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
					if (request is HttpWebRequest)
					{
						(request as HttpWebRequest).CookieContainer = new CookieContainer();
						foreach (Cookie cookie in cookies)
						{
							(request as HttpWebRequest).CookieContainer.Add(cookie);
						} 
					}
					else
					{
						throw new Exception("Can not set http headers when using WebRequest.");
					}
					
				}

				if (request is HttpWebRequest && allowAutoRedirect!=null)
				{
					(request as HttpWebRequest).AllowAutoRedirect = allowAutoRedirect.Value;
				}
				
				// execute the request
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				//if (
				//	requestUriContentType == (int) RequestUriContentType.ImageJpeg ||
				//	requestUriContentType == (int) RequestUriContentType.ImagePng ||
				//	requestUriContentType == (int) RequestUriContentType.ImageGif
				//	)
				//{
				//	// Check that the remote file was found. The ContentType
				//	// check is performed since a request for a non-existent
				//	// image file might be redirected to a 404-page, which would
				//	// yield the StatusCode "OK", even though the image was not
				//	// found.
				//	if ((response.StatusCode == HttpStatusCode.OK ||
				//		response.StatusCode == HttpStatusCode.Moved ||
				//		response.StatusCode == HttpStatusCode.Redirect) &&
				//		response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
				//	{

				//		// if the remote file was found, download it
				//		//code moved to dump response region below...
						
				//		return true;
				//	}
				//	else
				//		return false;
				//}

				if (fileNameWhereToDumpResponse != null)
				{
					using (Stream inputStream = response.GetResponseStream())
					using (Stream outputStream = File.OpenWrite(fileNameWhereToDumpResponse))
					{
						byte[] buffer = new byte[4096];
						int bytesRead;
						do
						{
							bytesRead = inputStream.Read(buffer, 0, buffer.Length);
							outputStream.Write(buffer, 0, bytesRead);
						} while (bytesRead != 0);

						inputStream.Seek(0, SeekOrigin.Begin);
					}
				}

				if (encoderForParsingResult == null)
				{
					r = DecodeWebResponse(response, log);
				}
				else
				{
					// we will read data via the response stream
					Stream resStream = response.GetResponseStream();

					string tempString = null;
					int count = 0;

					encoderForParsingResult = encoderForParsingResult ?? Encoding.ASCII;

					do
					{
						// fill the buffer with data
						count = resStream.Read(buf, 0, buf.Length);

						// make sure we read some data
						if (count != 0)
						{
							// translate from bytes to ASCII text
							tempString = encoderForParsingResult.GetString(buf, 0, count);

							// continue building the string
							sb.Append(tempString);
						}
					} while (count > 0); // any more data to read?

					r = sb.ToString();
				}

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

		/// <summary>
		/// Decodes the web response encoding.
		/// Source: https://blogs.msdn.microsoft.com/feroze_daud/2004/03/30/downloading-content-from-the-web-using-different-encodings/
		/// </summary>
		/// <param name="w">The w.</param>
		/// <param name="log">The log.</param>
		/// <returns></returns>
		private static String DecodeWebResponse(WebResponse w, CBB_LoggingCustomTraceLog log)
		{

			//
			// first see if content length header has charset = calue
			//
			String charset = null;
			String ctype = w.Headers["content - type"];
			if (ctype != null)
			{
				int ind = CBB_CommonExtenderClass.IndexAfterWords(ctype, true, "charset", CBB_CommonExtenderClass.OPTIONAL_SPACES, "=");
				if (ind != -1)
				{
					charset = ctype.Substring(ind + 8);
					CBB_LoggingCustomTraceLogExtensions.AddLine(log, "CT: charset =" + charset);
				}
			}

			// save data to a memorystream
			MemoryStream rawdata = new MemoryStream();
			byte[] buffer = new byte[1024];
			Stream rs = w.GetResponseStream();
			int read = rs.Read(buffer, 0, buffer.Length);
			while (read > 0)
			{
				rawdata.Write(buffer, 0, read);
				read = rs.Read(buffer, 0, buffer.Length);
			}

			rs.Close();

			//
			// if ContentType is null, or did not contain charset, we search in body
			//
			if (charset == null)
			{
				MemoryStream ms = rawdata;
				ms.Seek(0, SeekOrigin.Begin);

				StreamReader srr = new StreamReader(ms, Encoding.ASCII);
				String meta = srr.ReadToEnd();

				if (meta != null)
				{
					int start_ind = CBB_CommonExtenderClass.IndexAfterWords(meta, true, "charset", CBB_CommonExtenderClass.OPTIONAL_SPACES, "=", CBB_CommonExtenderClass.OPTIONAL_SPACES, "\"");
					int end_ind = -1;
					if (start_ind != -1)
					{
						end_ind = meta.IndexOf("\"", start_ind);
						if (end_ind != -1)
						{
							charset = meta.Substring(start_ind, end_ind - start_ind);

							CBB_LoggingCustomTraceLogExtensions.AddLine(log, "META: charset=" + charset);
						}
					}
				}
			}

			Encoding e = null;
			if (charset == null)
			{
				e = Encoding.ASCII; //default encoding
			}
			else
			{
				try
				{
					e = Encoding.GetEncoding(charset);
				}
				catch (Exception ee)
				{
					CBB_LoggingCustomTraceLogExtensions.AddLine(log, "Exception: GetEncoding: " + charset);
					CBB_LoggingCustomTraceLogExtensions.AddLine(log, ee.ToString());
					e = Encoding.ASCII;
				}
			}

			rawdata.Seek(0, SeekOrigin.Begin);

			StreamReader sr = new StreamReader(rawdata, e);

			String s = sr.ReadToEnd();

			return s;
		}


		public static byte[] DownloadFile(string uri, object customTraceLog = null, bool detailedLog = true, bool blockAllExceptions = false, byte[] errorCaseResult = null)
		{
			byte[] r = null;

			CBB_LoggingCustomTraceLog log = CBB_LoggingCustomTraceLog.Unbox(customTraceLog);
			try
			{
				CBB_LoggingCustomTraceLogExtensions.AddLineAndIncreaseIdent(log, "Requesting '" + uri + "'...");

				//source: http://stackoverflow.com/questions/3615800/download-image-from-the-site-in-net-c
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				// Check that the remote file was found. The ContentType
				// check is performed since a request for a non-existent
				// image file might be redirected to a 404-page, which would
				// yield the StatusCode "OK", even though the image was not
				// found.
				if ((response.StatusCode == HttpStatusCode.OK ||
					 response.StatusCode == HttpStatusCode.Moved ||
					 response.StatusCode == HttpStatusCode.Redirect)
					//&& response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)
					)
				{
					// if the remote file was found, download it
					using (Stream inputStream = response.GetResponseStream())
					{
						using (MemoryStream outputStream = new MemoryStream())
						{
							byte[] buffer = new byte[4096];
							int bytesRead;
							do
							{
								bytesRead = inputStream.Read(buffer, 0, buffer.Length);
								outputStream.Write(buffer, 0, bytesRead);
							} while (bytesRead != 0);

							if (detailedLog)
							{
								CBB_LoggingCustomTraceLogExtensions.AddLine(log, "Serializing to byte array...");
							}
							r = outputStream.ToArray();
						}
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
			finally
			{
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
		TextJson = 3,
		ImageJpeg = 4,
		ImagePng = 5,
		ImageGif = 6
	}
}
