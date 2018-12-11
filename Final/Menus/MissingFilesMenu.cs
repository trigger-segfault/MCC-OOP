using System;
using System.IO;

namespace WJLCS.Menus {
	/// <summary>
	/// The menu that displays all missing files.
	/// </summary>
	public class MissingFilesMenu : Menu {

		#region Constants
		
		/// <summary>
		/// This text in the menu file will be replaced with the missing files.
		/// </summary>
		private const string MissingFilesToken = "MISSINGFILES";

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="MissingFilesMenu"/>.
		/// </summary>
		public MissingFilesMenu(string filePath) : base(filePath) {
			AddToken(MissingFilesToken, PrintMissingFiles);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the array of missing files.
		/// </summary>
		public string[] MissingFiles { get; set; }

		#endregion

		#region PrintScreen Override

		/// <summary>
		/// Prints the missing files menu to the screen.
		/// </summary>
		protected override void PrintScreen(MenuDriver menuDriver) {

			Console.ForegroundColor = ConsoleColor.Red;
			if (!File.Exists(FilePath)) {
				Console.Clear();
				// Missing MissingFilesMenu text file, print a hardcoded menu instead
				Console.WriteLine();
				PrintLine("Missing Files!");
				Console.WriteLine();
				PrintLine("The following files required for runtime are missing, including the file to display this error menu!");
				Console.WriteLine();
				PrintMissingFiles(menuDriver);
				Console.WriteLine();
			}
			else {
				base.PrintScreen(menuDriver);
			}
			Console.ResetColor();
			Console.WriteLine();
		}

		/// <summary>
		/// Prints the missing file list.
		/// </summary>
		private void PrintMissingFiles(MenuDriver menuDriver) {
			foreach (string file in MissingFiles)
				PrintLine(file);
		}

		#endregion
	}
}
