using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WJLCS.Enigma.IO {
	/// <summary>
	/// Static I/O methods for reading and writing letterset files.
	/// </summary>
	public static class LetterSetIO {

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

		#region File IO

		/// <summary>
		/// Reads the file and returns the <see cref="LetterSet"/>.
		/// </summary>
		/// <param name="letterSetFile">The file containing the letterset.</param>
		/// 
		/// <exception cref="FileNotFoundException">
		/// The letterset file was not found.
		/// </exception>
		/// <exception cref="LoadFailedException">
		/// A parsed letter is invalid, or an error occurred during loading.
		/// </exception>
		public static LetterSet Read(string letterSetFile) {
			try {
				string[] lines = File.ReadAllLines(letterSetFile);
				var chars = lines.Select(l => ParseLetter(l, false).Value);
				return new LetterSet(chars.ToArray());
			}
			catch (FileNotFoundException) {
				throw;
			}
			catch (Exception ex) {
				throw new LoadFailedException($"Failed to load the Letterset file!\n{ex.Message}");
			}
		}
		/// <summary>
		/// Writes the input letterset to the file.
		/// </summary>
		/// <param name="letterSet">The letterset to save.</param>
		/// <param name="letterSetFile">The letterset file to save the letters to.</param>
		/// 
		/// <exception cref="SaveFailedException">
		/// An error occurred while saving the file.
		/// </exception>
		public static void Write(LetterSet letterSet, string letterSetFile) {
			try {
				Directory.CreateDirectory(Path.GetDirectoryName(letterSetFile));
				string text = string.Join(Environment.NewLine, letterSet.Select(c => EscapeLetter(c)));
				File.WriteAllText(letterSetFile, text);
			}
			catch (Exception ex) {
				throw new SaveFailedException($"Failed to save the Letterset file!\n{ex.Message}");
			}
		}

		#endregion
	}
}
