using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CraftSynth.BuildingBlocks
{
	public static class Validation
	{
		public const string Pattern_EMail = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

		public const string Pattern_Url = @"^(?<proto>\w+)://[^/]+?(?<port>:\d+)?/";

		//        foo://example.com:8042/over/there?name=ferret#nose
		//        \_/   \______________/\_________/\__________/ \__/
		//         |           |             |           |        |
		//      scheme     authority       path        query   fragment
		//        s0          a0            p0          q0        f0
		public const string Pattern_Uri = @"^(?<s1>(?<s0>[^:/\?#]+):)?(?<a1>"
												+ @"//(?<a0>[^/\?#]*))?(?<p0>[^\?#]*)"
												+ @"(?<q1>\?(?<q0>[^#]*))?"
												+ @"(?<f1>#(?<f0>.*))?";

		// Function to test for Positive Integers.
		public static bool IsNaturalNumber(this String strNumber)
		{
			Regex objNotNaturalPattern = new Regex("[^0-9]");
			Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
			return !objNotNaturalPattern.IsMatch(strNumber) &&
			objNaturalPattern.IsMatch(strNumber);
		}
		// Function to test for Positive Integers with zero inclusive
		public static bool IsWholeNumber(this String strNumber)
		{
			Regex objNotWholePattern = new Regex("[^0-9]");
			return !objNotWholePattern.IsMatch(strNumber);
		}
		// Function to Test for Integers both Positive & Negative
		public static bool IsInteger(this String strNumber)
		{
			Regex objNotIntPattern = new Regex("[^0-9-]");
			Regex objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$");
			return !objNotIntPattern.IsMatch(strNumber) && objIntPattern.IsMatch(strNumber);
		}
		// Function to Test for Positive Number both Integer & Real
		public static bool IsPositiveNumber(this String strNumber)
		{
			Regex objNotPositivePattern = new Regex("[^0-9.]");
			Regex objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
			Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
			return !objNotPositivePattern.IsMatch(strNumber) &&
			objPositivePattern.IsMatch(strNumber) &&
			!objTwoDotPattern.IsMatch(strNumber);
		}
		// Function to test whether the string is valid number or not
		public static bool IsNumber(this String strNumber)
		{
			Regex objNotNumberPattern = new Regex("[^0-9.-]");
			Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
			Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
			String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
			String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
			Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");
			return !objNotNumberPattern.IsMatch(strNumber) &&
			!objTwoDotPattern.IsMatch(strNumber) &&
			!objTwoMinusPattern.IsMatch(strNumber) &&
			objNumberPattern.IsMatch(strNumber);
		}

		/// <summary>
		/// For null returns false.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool IsNumber(this string s, bool allowSign, bool allowDots = false, bool allowCommas = false)
		{
			bool r = true;

			if (s == null)
			{
				r = false;
			}
			else
			{
				int i = 0;
				foreach (char c in s)
				{
					if (!Char.IsDigit(c))
					{
						if (allowSign && (c == '-' || c == '+') && i == 0)
						{
							//ok
						}
						else if (allowDots && c == '.')
						{
							//ok
						}
						else if (allowCommas && c == ',')
						{
							//ok
						}
						else
						{
							r = false;
							break;
						}
					}
					i++;
				}
			}
			return r;
		}

		/// <summary>
		/// For null returns false. For empty returns true.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool IsAllLetters(this string s)
		{
			bool r = true;

			if (s == null)
			{
				r = false;
			}
			else
			{
				int i = 0;
				foreach (char c in s)
				{
					if (!Char.IsLetter(c))
					{
						r = false;
						break;
					}
					i++;
				}
			}
			return r;
		}

		// Function To test for Alphabets.
		public static bool IsAlpha(this String strToCheck)
		{
			Regex objAlphaPattern = new Regex("[^a-zA-Z]");
			return !objAlphaPattern.IsMatch(strToCheck);
		}
		// Function to Check for AlphaNumeric.
		public static bool IsAlphaNumeric(this String strToCheck)
		{
			Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
			return !objAlphaNumericPattern.IsMatch(strToCheck);
		}

		/// <summary>
		/// Checks wether passed string is valid full url.
		/// </summary>
		/// <param name="strToCheck">Uri to check where protocol part must be included. Examples:
		/// '<example>http://www.mywebsite.com:8080/mywebapp/mydir/mypage.htm?mykey1=myvalue1&mykey2=myvalue2#myanchor</example>'
		/// '<example>http://www.mywebsite.com</example>'
		/// </param>
		/// <returns></returns>
		public static bool IsUri(this String strToCheck, bool uriWithoutHttpOrHttpsIsValid)
		{
			//Regex objAplhaPattern = new Regex(Pattern_Uri);
			//return objAplhaPattern.IsMatch(strToCheck);
			
			bool r;

			if (!uriWithoutHttpOrHttpsIsValid)
			{
				r = Uri.IsWellFormedUriString(strToCheck, UriKind.Absolute);
			}
			else
			{
				r = Uri.IsWellFormedUriString(strToCheck, UriKind.Absolute);
				if (r == false)
				{
					r = Uri.IsWellFormedUriString("http://"+strToCheck, UriKind.Absolute);
				}
			}

			return r;
		}

		/// <summary>
		/// method to determine is the absolute file path is a valid path
		/// </summary>
		/// <param name="path">the path we want to check</param>
		public static bool IsFilePath(this string path)
		{
			string pattern = @"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$";
			Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
			return reg.IsMatch(path);
		}

		
		public static bool IsEMail(this String strToCheck)
		{
			///Initial version: Was Laying about "03 9620 9611"!!! 
			//Regex objAlphaPattern = new Regex(Pattern_EMail);
			//return !objAlphaPattern.IsMatch(strToCheck);

			if (String.IsNullOrEmpty(strToCheck))
			{
				return false;
			}

			// Use IdnMapping class to convert Unicode domain names.
			strToCheck = Regex.Replace(strToCheck, @"(@)(.+)$", DomainMapper);
			if (invalid)
			{
				return false;
			}

			// Return true if strIn is in valid e-mail format. 
			return Regex.IsMatch(strToCheck, 
					  @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + 
					  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", 
					  RegexOptions.IgnoreCase);
		}
		private static bool  invalid = false;
	    private static string DomainMapper(Match match)
	    {
		   // IdnMapping class with default property values.
		   IdnMapping idn = new IdnMapping();
	   
		   string domainName = match.Groups[2].Value;
		   try {
		 	 domainName = idn.GetAscii(domainName);
		   }
		   catch (ArgumentException) {
		 	 invalid = true;      
		   }      
		   return match.Groups[1].Value + domainName;
	    }
	}
}
