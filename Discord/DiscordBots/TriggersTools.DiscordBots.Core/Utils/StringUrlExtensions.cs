using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TriggersTools.DiscordBots.Utils {
	/// <summary>
	/// String extension methods for Urls.
	/// </summary>
	public static class StringUrlExtensions {

		/// <summary>
		/// The regex for matching Urls.
		/// </summary>
		private static readonly Regex UrlRegex =
			new Regex(@"(?:<\S+:\/\/\S+>)|(?:https?:\/\/\S(?:\S|\/)\S*)", RegexOptions.Multiline);
		
		/// <summary>
		/// Gets all Urls inside the string.
		/// </summary>
		/// <param name="s">The string to search in.</param>
		/// <returns>A list of all found Urls.</returns>
		public static List<StringUrl> GetUrls(this string s) {
			List<string> literals = new List<string>();
			List<StringUrl> urls = new List<StringUrl>();
			int index = 0;

			foreach (CodeBlock block in s.GetAllCodeBlocks()) {
				if (block.Start > index)
					literals.Add(s.Substring(index, block.Start - index));
				index = block.End;
			}
			if (index < s.Length)
				literals.Add(s.Substring(index));

			// For each split literal between code blocks
			foreach (string literal in literals) {
				index = 0;
				Match match;
				// Find as many urls in each literal as possible
				do {
					match = UrlRegex.Match(literal, index);
					if (match.Success) {
						// This expression has a side effect of matching 
						urls.Add(new StringUrl {
							Url = match.Value,
							Start = match.Index,
							Length = match.Length,
						});
						index = match.Index + match.Length;
					}
				} while (match.Success && index < literal.Length);
			}

			return urls;
		}


		/// <summary>
		/// Returns true if the string ends with an image extension.
		/// </summary>
		/// <param name="url">The url string to check.</param>
		/// <returns>True if the url string is an image.</returns>
		public static bool IsImageUrl(this string url) {
			switch (Path.GetExtension(url).ToLower()) {
			case ".png":
			case ".jpg":
			case ".jpeg":
			case ".gif":
				return true;
			}
			return false;
		}
	}
}
