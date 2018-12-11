using System;
using System.IO;
using WJLCS.Utils;

namespace WJLCS.Menus {
	/// <summary>
	/// The menu that displays all missing files.
	/// </summary>
	public class ExceptionMenu : Menu {

		#region Constants
		
		/// <summary>
		/// This text in the menu file will be replaced with the exception.
		/// </summary>
		private const string ExceptionToken = "EXCEPTION";

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="ExceptionMenu"/>.
		/// </summary>
		public ExceptionMenu(string filePath) : base(filePath) {
			AddToken(ExceptionToken, PrintException);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the unhandled exception.
		/// </summary>
		public Exception Exception { get; set; }

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
				PrintLine("Unhandled Exception!");
				Console.WriteLine();
				PrintLine("The following error occurred while running the program:");
				Console.WriteLine();
				PrintException(menuDriver);
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
		private void PrintException(MenuDriver menuDriver) {
			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach (string line in Exception.ToString().SplitLines())
				PrintLine(line);
			Console.ForegroundColor = ConsoleColor.Red;
		}

		#endregion
	}
}
