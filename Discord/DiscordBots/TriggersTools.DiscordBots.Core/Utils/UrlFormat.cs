using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Utils {
	public static class UrlFormat {

		private const string UrlPattern = @"^(?:(?:(?:http|https|ftp):\/\/)?(?:[a-zA-Z0-9\-\.])+(?:\.)(?:[a-zA-Z0-9]){2,4}(?:[a-zA-Z0-9\/+=%&_\.~?\-]*))*$";
		private static readonly Regex UrlRegex = new Regex(UrlPattern, RegexOptions.IgnoreCase);

		/// <summary>
		/// Gets if the string is a proper Url, optionaly surrounded with angled braces.
		/// </summary>
		/// <param name="url">The string to check.</param>
		/// <returns>True if the string is a url.</returns>
		public static bool IsUrl(string url) {
			url = url.Trim();
			if (url.StartsWith("<") && url.EndsWith(">")) {
				string subUrl = url.Substring(1, url.Length - 2);
				if (UrlRegex.IsMatch(subUrl))
					return true;
			}
			return UrlRegex.IsMatch(url);
		}

		/// <summary>
		/// Trims and removes the angled braces from the url input.
		/// </summary>
		/// <param name="url">The url to cleanup.</param>
		/// <param name="isUrl">The output parameter that states if the string was a url.</param>
		/// <returns>The cleaned up url.</returns>
		public static string StripBraces(string url, out bool isUrl) {
			url = url.Trim();
			if (url.StartsWith("<") && url.EndsWith(">")) {
				string subUrl = url.Substring(1, url.Length - 2);
				if (UrlRegex.IsMatch(subUrl)) {
					isUrl = true;
					return subUrl;
				}
			}
			isUrl = UrlRegex.IsMatch(url);
			return url;
		}

	}
}
