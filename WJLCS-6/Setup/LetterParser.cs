using System;
using System.Collections.Generic;
using System.Globalization;

namespace WJLCS.Setup {
	/// <summary>
	/// A static class for reading and writing escaped letters from files.
	/// </summary>
	public static class LetterParser {

		#region Constants

		/// <summary>
		/// The list of valid escape characters.
		/// </summary>
		private static readonly Dictionary<char, char> EscapeCharacters = new Dictionary<char, char> {
			// Characters that don't really need escaping
			//{ '\\', '\\' },
			//{ '\'', '\'' },
			//{ '"', '"' },

			{ '0', '\0' },
			{ 'a', '\a' },
			{ 'b', '\b' },
			{ 'f', '\f' },
			{ 'n', '\n' },
			{ 'r', '\r' },
			{ 't', '\t' },
			{ 'v', '\v' },
		};

		#endregion

		#region Parse/Escape

		/// <summary>
		/// Parses a single letter for use with the letterset and plugboard file.
		/// </summary>
		/// <param name="s">The string to parse.</param>
		/// <returns>A character if the line was not empty, otherwise null.</returns>
		/// 
		/// <exception cref="Exception">
		/// The input letter is invalid.
		/// </exception>
		public static char? ParseLetter(string s, bool allowEmpty) {
			if (s.Length == 0 && allowEmpty) {
				// Empty line: Ignore
				return null;
			}
			else if (s.Length == 1) {
				return s[0];
			}
			else if (s.Length == 2) {
				if (s[0] == '\\' && EscapeCharacters.TryGetValue(s[1], out char escaped))
					return escaped;
			}
			else if (s.Length == 6 && s.StartsWith(@"\u")) {
				string charCodeStr = s.Substring(2);
				if (int.TryParse(charCodeStr, NumberStyles.HexNumber, null, out int charCode))
					return (char) charCode;
				throw new Exception($"\"{charCodeStr}\" is not a valid unicode hex character code!");
			}
			throw new Exception($"\"{s}\" is not a valid letterset character!");
		}

		/// <summary>
		/// Escapes the specified letter so that it can be written to a file.
		/// </summary>
		/// <param name="c">The letter to escape.</param>
		/// <returns></returns>
		public static string EscapeLetter(char c) {
			// Check if this is an escape character
			foreach (var pair in EscapeCharacters) {
				if (pair.Value == c)
					return @"\" + pair.Key;
			}
			return new string(c, 1);
		}

		#endregion
	}
}
