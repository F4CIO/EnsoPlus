using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CraftSynth.BuildingBlocks.UI.WindowsForms
{
	public static class KeysHelper
	{
		public static List<Keys> ModifierKeysList = new List<Keys>() { 
            Keys.RControlKey,Keys.Control,Keys.ControlKey, Keys.LControlKey, 
            Keys.Shift,Keys.ShiftKey,Keys.LShiftKey, Keys.RShiftKey,
            Keys.Alt,
            Keys.LWin,Keys.RWin};

		public static string ToCommaSeparatedCodesString(this List<Keys> keys)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var key in keys)
			{
				if (sb.Length != 0)
				{
					sb.Append(',');
				}
				sb.Append((int)key);
			}
			return sb.ToString();
		}

		public static List<Keys> BuildKeyListFromCommaSeparatedCodesString(string codes)
		{
			List<Keys> result = new List<Keys>();
			var codesArray = codes.Split(',');
			foreach (var code in codesArray)
			{
				result.Add((Keys)int.Parse(code));
			}
			return result;
		}

		public static string ToUserFriendlyString(List<Keys> keyCombination)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var key in keyCombination)
			{
				if (sb.Length != 0)
				{
					sb.Append(" + ");
				}
				sb.Append(key.ToString());
			}
			return sb.ToString();
		}
	}
}
