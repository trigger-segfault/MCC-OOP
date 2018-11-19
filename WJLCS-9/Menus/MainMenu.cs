using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WJLCS.Menus {
	/// <summary>
	/// The menu class for displaying the main welcome screen.
	/// </summary>
	public class MainMenu : Menu {

		#region Constants

		/// <summary>
		/// The padding to display the status from the left of the screen.
		/// </summary>
		private const int StatusMargin = 12;
		/// <summary>
		/// This text in the menu file will be replaced with the machine status.
		/// </summary>
		private const string StatusMarker = "[!ENIGMASTATUS!]";

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="MainMenu"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the menu text.</param>
		public MainMenu(string filePath) : base(filePath) { }

		#endregion

		#region PrintScreen Override

		/// <summary>
		/// Prints the main menu to the screen.
		/// </summary>
		protected override void PrintScreen(MenuDriver screenDriver) {
			Console.Clear();

			// Read the text for the menu
			string[] lines = ReadScreenFile(FilePath);
			foreach (string line in lines) {
				// Insert the letterset into the menu
				if (line.Trim() == StatusMarker)
					PrintStatus(screenDriver);
				else
					PrintLine(line);
			}
		}

		/// <summary>
		/// Prints the Enigma Machine status.
		/// </summary>
		private void PrintStatus(MenuDriver screenDriver) {
			screenDriver.Interpreter.PrintMachineStatus();
		}

		#endregion

		#region GetMissingFiles

		/*/// <summary>
		/// Checks for any missing files required by this screen
		/// </summary>
		/// <returns>An array of missing files.</returns>
		public override string[] GetMissingFiles() {
			List<string> files = new List<string>(base.GetMissingFiles());
			if (!File.Exists(LettersetFilePath))
				files.Add(LettersetFilePath);
			return files.ToArray();
		}*/

		#endregion
	}
}
