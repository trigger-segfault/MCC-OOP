using System;
using System.Text;

namespace WJLCS.Menus {
	/// <summary>
	/// The menu class for displaying the letterset.
	/// </summary>
	public class LetterSetMenu : Menu {

		#region Constants
		
		/// <summary>
		/// The margin between the letterset and Menu Width.
		/// </summary>
		private const int LettersetMargin = 12;
		/// <summary>
		/// The width of each letter when printed.
		/// </summary>
		private const int LetterWidth = 3;
		/// <summary>
		/// This text in the menu file will be replaced with the letterset.
		/// </summary>
		private const string LettersetToken = "LETTERSET";

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructs the <see cref="LetterSetMenu"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the menu text.</param>
		public LetterSetMenu(string filePath) : base(filePath) {
			AddToken(LettersetToken, PrintLetterset);
		}

		#endregion

		#region Print Command
		
		/// <summary>
		/// Prints the letterset list.
		/// </summary>
		private void PrintLetterset(MenuDriver menuDriver) {
			string[] letters = menuDriver.Interpreter.GetEscapedLetterSet();
			if (letters == null) {
				PrintError($"No letterset is loaded!");
				return;
			}

			// +1 because the last letter does not need a space after it.
			int lettersPerRow = (ScreenWidth - LettersetMargin + 1) / LetterWidth;

			Console.ForegroundColor = ConsoleColor.DarkGreen;
			int i = 0;
			while (i < letters.Length) {
				StringBuilder str = new StringBuilder();
				for (int j = 0; i < letters.Length && j < lettersPerRow; i++, j++) {
					str.Append(letters[i].PadRight(LetterWidth));
				}
				PrintLine(str.ToString().TrimEnd());
			}
			Console.ResetColor();
		}

		#endregion
	}
}
