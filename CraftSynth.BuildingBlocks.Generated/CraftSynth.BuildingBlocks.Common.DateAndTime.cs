using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftSynth.BuildingBlocks.Common
{
	public static class DateAndTime
	{
		public static string GetCurrentDateAndTimeInSortableFormat()
		{
			DateTime now = DateTime.Now;
			string currentDateAndTimeSortable =
				String.Format("{0}-{1}-{2} {3}:{4}:{5}",
				now.Year,
				now.Month.ToString("00"),
				now.Day.ToString("00"),
				now.Hour.ToString("00"),
				now.Minute.ToString("00"),
				now.Second.ToString("00")
				);
			return currentDateAndTimeSortable;
		}

		public static string GetCurrentDateInSortableFormat()
		{
			DateTime now = DateTime.Now;
			string currentDateAndTimeSortable =
				String.Format("{0}-{1}-{2}",
				now.Year,
				now.Month.ToString("00"),
				now.Day.ToString("00")
				);
			return currentDateAndTimeSortable;
		}

		public static string GetCurrentDateAndTimeInSortableFormatForFileSystem()
		{
			DateTime now = DateTime.Now;
			string currentDateAndTimeSortable =
				String.Format("{0}.{1}.{2}. {3}-{4}-{5}",
				now.Year,
				now.Month.ToString("00"),
				now.Day.ToString("00"),
				now.Hour.ToString("00"),
				now.Minute.ToString("00"),
				now.Second.ToString("00")
				);
			return currentDateAndTimeSortable;
		}

		public static string GetCurrentDateInSortableFormatForFileSystem()
		{
			DateTime now = DateTime.Now;
			string currentDateAndTimeSortable =
				String.Format("{0}.{1}.{2}.",
				now.Year,
				now.Month.ToString("00"),
				now.Day.ToString("00")
				);
			return currentDateAndTimeSortable;
		}

		public static string ToDateAndTimeInSortableFormatForFileSystem(this DateTime now)
		{
			string currentDateAndTimeSortable =
				String.Format("{0}.{1}.{2}. {3}-{4}-{5}",
				now.Year,
				now.Month.ToString("00"),
				now.Day.ToString("00"),
				now.Hour.ToString("00"),
				now.Minute.ToString("00"),
				now.Second.ToString("00")
				);
			return currentDateAndTimeSortable;
		}

		public static string ToDateAndTimeInSortableFormatForAzureBlob(this DateTime now)
		{
			string currentDateAndTimeSortable =
				String.Format("{0}-{1}-{2}-{3}-{4}-{5}",
				now.Year,
				now.Month.ToString("00"),
				now.Day.ToString("00"),
				now.Hour.ToString("00"),
				now.Minute.ToString("00"),
				now.Second.ToString("00")
				);
			return currentDateAndTimeSortable;
		}

		/// <summary>
		/// returns null if error occcured.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="throwErrorIfInInvalidFormat"></param>
		/// <param name="errorCaseResult"></param>
		/// <returns></returns>
		public static DateTime? ParseDateAndTimeInSortableFormatForAzureBlob(this string s, bool trimNonDigitsCharsAtEnd = false)
		{
			DateTime? r;
			try
			{
				//{0}-{1}-{2}-{3}-{4}-{5}
				string[] parts = s.Split('-');
				if (trimNonDigitsCharsAtEnd)
				{
					parts[5] = parts[5].TrimNonDigitsAtEnd();
				}
				r = new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[4]), int.Parse(parts[5]));
			}
			catch (Exception)
			{
				r = null;
			}
			return r;
		}

		/// <summary>
		/// Returns null if error occurred.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="allowCharsAfterDate"></param>
		/// <returns></returns>
		public static DateTime? ParseDateAndTimeAsYYYYMMDDHHMM(this string s, bool allowCharsAfterDate = false)
		{
			DateTime? r;
			try
			{
				//20140624000
				if (!allowCharsAfterDate && s.Length != 12)
				{
					throw new Exception("Invalid length.");
				}

				int year = int.Parse(s.Substring(0, 4));
				int month = int.Parse(s.Substring(4, 2));
				int day = int.Parse(s.Substring(6, 2));
				int hour = int.Parse(s.Substring(8, 2));
				int minute = int.Parse(s.Substring(10, 2));
				r = new DateTime(year, month, day, hour, minute, 0);
			}
			catch (Exception)
			{
				r = null;
			}
			return r;
		}

		public static string ToDateAndTimeAsYYYYMMDDHHMM(this DateTime now)
		{
			string r =
				String.Format("{0}{1}{2}{3}{4}",
				now.Year,
				now.Month.ToString("00"),
				now.Day.ToString("00"),
				now.Hour.ToString("00"),
				now.Minute.ToString("00")
				//,now.Second.ToString("00")
				);
			return r;
		}
	}
}
