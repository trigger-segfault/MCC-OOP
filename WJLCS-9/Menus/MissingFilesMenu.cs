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
		private const string MissingFilesMarker = "[!MISSING_FILES!]";

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="MissingFilesMenu"/>.
		/// </summary>
		public MissingFilesMenu(string filePath) : base(filePath) { }

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
		protected override void PrintScreen(MenuDriver screenDriver) {
			Console.Clear();

			Console.ForegroundColor = ConsoleColor.Red;
			if (!File.Exists(FilePath)) {
				// Missing MissingFilesMenu text file, print a hardcoded menu instead
				Console.WriteLine();
				PrintLine("Missing Files!");
				Console.WriteLine();
				PrintLine("The following files required for runtime are missing, including the file to display this error menu!");
				Console.WriteLine();
				PrintMissingFiles();
				Console.WriteLine();
			}
			else {
				// Read the text for the menu
				string[] lines = ReadScreenFile(FilePath);
				foreach (string line in lines) {
					// Insert the letterset into the menu
					if (line.Trim() == MissingFilesMarker)
						PrintMissingFiles();
					else
						PrintLine(line);
				}
			}
			Console.ResetColor();
		}

		/// <summary>
		/// Prints the missing file list.
		/// </summary>
		private void PrintMissingFiles() {
			foreach (string file in MissingFiles)
				PrintLine(file);
		}

		#endregion
	}
}
