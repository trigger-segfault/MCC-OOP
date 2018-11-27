using System;
using System.Text;

namespace WJLCS.Menus {
	/// <summary>
	/// The menu class for displaying the steckering.
	/// </summary>
	public class PlugboardMenu : Menu {

		#region Constants

		/// <summary>
		/// The margin between the steckering and Menu Width.
		/// </summary>
		private const int SteckeringMargin = 10;
		/// <summary>
		/// The width of each stecker when printed.
		/// </summary>
		private const int SteckerWidth = 6;
		/// <summary>
		/// This text in the menu file will be replaced with the steckering.
		/// </summary>
		private const string SteckeringToken = "STECKERING";

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="LetterSetMenu"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the menu text.</param>
		public PlugboardMenu(string filePath) : base(filePath) {
			AddToken(SteckeringToken, PrintSteckering);
		}

		#endregion

		#region Print Command

		/// <summary>
		/// Prints the steckering list.
		/// </summary>
		private void PrintSteckering(MenuDriver menuDriver) {
			string[] letters = menuDriver.Interpreter.GetEscapedLetterSet();
			int[] steckering = menuDriver.Interpreter.GetSteckering();
			if (letters == null) {
				PrintError($"No letterset is loaded!");
				return;
			}
			else if (steckering == null) {
				PrintError($"No steckering is loaded!");
				return;
			}

			// +1 because the last letter does not need a space after it.
			int steckersPerRow = (ScreenWidth - SteckeringMargin + 1) / SteckerWidth;

			Console.ForegroundColor = ConsoleColor.Magenta;
			int i = 0;
			while (i < steckering.Length) {
				StringBuilder str = new StringBuilder();
				for (int j = 0; i < steckering.Length && j < steckersPerRow; i++, j++) {
					str.Append(letters[i].PadLeft(2));
					str.Append("=");
					str.Append(letters[steckering[i]].PadRight(2));
					str.Append(" ");
				}
				PrintLine(str.ToString().TrimEnd());
			}
			Console.ResetColor();
		}

		#endregion
	}
}
