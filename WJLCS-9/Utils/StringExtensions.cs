using System;
using System.Collections.Generic;

namespace WJLCS.Utils {
	/// <summary>
	/// Helper functions for strings.
	/// </summary>
	public static class StringExtensions {
		
		/// <summary>
		/// Centers the line by padding it with spaces.
		/// </summary>
		/// <param name="line">The line to center.</param>
		/// <param name="width">The width of the area to center the line in.</param>
		/// <returns>The centered line.</returns>
		public static string Center(this string line, int width) {
			if (string.IsNullOrWhiteSpace(line))
				return string.Empty;

			int offset = (width - line.Length) / 2;
			return new string(' ', offset) + line;
		}

		/// <summary>
		/// Splits the line into multiple lines by word.
		/// </summary>
		/// <param name="line">The line to split.</param>
		/// <param name="maxWidth">The max width of a line.</param>
		/// <param name="trim">True if whitespace should be trimmed upon word ellipses.</param>
		/// <returns>The split lines.</returns>
		public static string[] WordEllipsesSplit(this string line, int maxWidth, bool trim) {
			List<string> lines = new List<string>();

			// Only trim lines when word ellipses is performed
			if (line.Length > maxWidth && trim)
				line = line.Trim();

			// Split the text into multiple lines until they are all <= MenuWidth
			while (line.Length > maxWidth) {
				int index;
				for (index = maxWidth; index > 0; index--) {
					if (char.IsWhiteSpace(line[index]))
						break;
				}
				if (index == 0) {
					// Word is too long, perform character ellipses
					lines.Add(line.Substring(0, maxWidth - 1) + "-");
					line = line.Substring(maxWidth - 1);
				}
				else {
					// Whitespace found, perform word ellipses
					// Only trim lines when word ellipses is performed
					string lineToAdd = line.Substring(0, index);
					line = line.Substring(index).TrimStart();
					if (trim) {
						lineToAdd = lineToAdd.Trim();
					}
					lines.Add(lineToAdd);
				}
			}

			// Add the final line
			if (string.IsNullOrEmpty(line) && lines.Count > 0) {
				// Line is empty and word ellipses was performed, add nothing
			}
			else if (string.IsNullOrWhiteSpace(line))
				lines.Add(string.Empty);
			else
				lines.Add(line);

			return lines.ToArray();
		}

		/// <summary>
		/// Splits the lines and accounts for carriage returns.
		/// </summary>
		/// <param name="text">The text to split into lines.</param>
		/// <returns>The array of split lines.</returns>
		public static string[] SplitLines(this string text, bool removeEmptyLines = false) {
			var options = (removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
			return text.Replace("\t", "    ").Replace("\r\n", "\n").Split(new[] { '\n', '\r' }, options);
		}
	}
}
