using System;
using System.Collections.Generic;
using System.IO;

namespace WJLCS {
	/// <summary>
	/// The menu class for displaying the letterset.
	/// </summary>
	public class LettersetMenu : Menu {

		#region Constants
		
		/// <summary>
		/// The margin between the letterset and Menu Width.
		/// </summary>
		private const int LettersetMargin = 12;
		/// <summary>
		/// This text in the menu file will be replaced with the letterset.
		/// </summary>
		private const string LettersetMarker = "[!LETTERSET!]";

		#endregion

		#region Properties

		/// <summary>
		/// Gets the filepath of the letterset.
		/// </summary>
		public string LettersetFilePath { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="LettersetMenu"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the menu text.</param>
		/// <param name="lettersetFilePath">The filepath of the letterset.</param>
		public LettersetMenu(string filePath, string lettersetFilePath) : base(filePath) {
			LettersetFilePath = lettersetFilePath ??
				throw new ArgumentNullException(nameof(lettersetFilePath));
		}

		#endregion

		#region PrintMenu Override

		/// <summary>
		/// Prints the letterset menu to the screen.
		/// </summary>
		public override void PrintMenu() {
			Console.Clear();

			// Read the text for the menu
			string[] lines = ReadAllLines(FilePath);
			foreach (string line in lines) {
				// Insert the letterset into the menu
				if (line.Trim() == LettersetMarker)
					PrintLetterset();
				else
					PrintLine(line);
			}
		}

		/// <summary>
		/// Prints the letterset list.
		/// </summary>
		private void PrintLetterset() {
			string letterText = File.ReadAllText(LettersetFilePath);
			string[] letters = letterText.Replace("\r", "").Split('\n');

			// +1 because the last letter does not need a space after it.
			int lettersPerRow = (MenuWidth - LettersetMargin + 1) / 3;

			int i = 0;
			while (i < letters.Length) {
				string currentLine = string.Empty;
				for (int j = 0; i < letters.Length && j < lettersPerRow; i++, j++) {
					currentLine += letters[i].PadRight(3);
				}
				PrintLine(currentLine.TrimEnd());
			}
		}

		#endregion

		#region GetMissingFiles

		/// <summary>
		/// Checks for any missing files required by this screen
		/// </summary>
		/// <returns>An array of missing files.</returns>
		public override string[] GetMissingFiles() {
			List<string> files = new List<string>(base.GetMissingFiles());
			if (!File.Exists(LettersetFilePath))
				files.Add(LettersetFilePath);
			return files.ToArray();
		}

		#endregion
	}
}
