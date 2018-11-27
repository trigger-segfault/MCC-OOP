using System;
using System.Text;

namespace WJLCS.Menus {
	/// <summary>
	/// The menu class for displaying the rotor keys.
	/// </summary>
	public class RotorMenu : Menu {

		#region Constants
		
		/// <summary>
		/// The margin between the rotor keys and Menu Width.
		/// </summary>
		private const int RotorKeysMargin = 12;
		/// <summary>
		/// The width of each rotor key when printed.
		/// </summary>
		private const int RotorKeyWidth = 4;
		/// <summary>
		/// This text in the menu file will be replaced with the rotor keys.
		/// </summary>
		private const string RotorKeysToken = "ROTORKEYS";

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="RotorMenu"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the menu text.</param>
		public RotorMenu(string filePath) : base(filePath) {
			AddToken(RotorKeysToken, PrintRotorKeys);
		}

		#endregion

		#region Print Command

		/// <summary>
		/// Prints the rotor key list.
		/// </summary>
		private void PrintRotorKeys(MenuDriver menuDriver) {
			int[] keys = menuDriver.Interpreter.GetRotorKeys();
			if (keys == null) {
				PrintError($"No rotor keys are loaded!");
				return;
			}

			// +1 because the last letter does not need a space after it.
			int keysPerRow = (ScreenWidth - RotorKeysMargin + 1) / RotorKeyWidth;

			Console.ForegroundColor = ConsoleColor.Blue;
			int i = 0;
			while (i < keys.Length) {
				StringBuilder str = new StringBuilder();
				for (int j = 0; i < keys.Length && j < keysPerRow; i++, j++) {
					str.Append(keys[i].ToString().PadLeft(RotorKeyWidth - 1).PadRight(RotorKeyWidth));
				}
				PrintLine(str.ToString().TrimEnd());
			}
			Console.ResetColor();
		}

		#endregion
	}
}
