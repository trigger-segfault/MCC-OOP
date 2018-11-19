using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Discord;

namespace TriggersTools.DiscordBots.Utils {
	public static class ColorUtils {

		public static Color Parse(string s) {
			bool hash = s.StartsWith("#");
			string hexStr = (hash ? s.Substring(1) : s);
			return new Color(uint.Parse(hexStr, NumberStyles.HexNumber, null));
		}

	}
}
