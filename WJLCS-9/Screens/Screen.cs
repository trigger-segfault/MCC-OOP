using System;
using System.Collections.Generic;
using System.IO;
using WJLCS.Utils;

namespace WJLCS.Screens {
	/// <summary>
	/// An abstract base class for a displayable screen.
	/// </summary>
	/// <remarks>
	/// This is an interface so it can be legally casted to a <see cref="MenuAction"/>.
	/// </remarks>
	public abstract class Screen {

		#region Constants

		/// <summary>
		/// The maximum width of a menu.
		/// </summary>
		public const int ScreenWidth = 80;
		/// <summary>
		/// The maximum height of a menu.
		/// </summary>
		public const int ScreenHeight = 24;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the filepath of the screen text.
		/// </summary>
		public string FilePath { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the base <see cref="Screen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		/// <param name="publishVersion">
		/// True if there is a separate version of the menu file for production.
		/// </param>
		protected Screen(string filePath/*, bool publishVersion*/) {
			FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reads all lines from the text file.
		/// </summary>
		/// <param name="filePath">The text file to read lines from.</param>
		/// <returns>The lines of the text file.</returns>
		protected string[] ReadScreenFile(string filePath) {
			return File.ReadAllLines(filePath);
		}
		/// <summary>
		/// Prints the centered line that conforms to screen width standards.
		/// </summary>
		/// <param name="line">The line to center and print.</param>
		protected void PrintLine(string line) {
			// Perform word ellipses to conform to screen length
			foreach (string ellipsesLine in line.WordEllipsesSplit(ScreenWidth, true)) {
				Console.WriteLine(ellipsesLine.Center(ScreenWidth));
			}
		}
		/// <summary>
		/// Prints an error line.
		/// </summary>
		/// <param name="error">The error to print.</param>
		protected void PrintError(string error) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(error);
			Console.ResetColor();
		}
		/// <summary>
		/// Prints a watermark in dark gray and backtracks to before it was entered.
		/// </summary>
		/// <param name="watermark">The text to print.</param>
		protected void PrintWatermark(string watermark) {
			ConsoleColor lastColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkGray;
			int left = Console.CursorLeft;
			int top = Console.CursorTop;
			Console.Write(watermark);
			Console.CursorLeft = left;
			Console.CursorTop = top;
			Console.ForegroundColor = lastColor;
		}

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Publishes and runs the screen.
		/// </summary>
		/// <returns>The action to perform after a screen choice.</returns>
		public virtual MenuAction Publish(MenuDriver menuDriver) {
			PrintScreen(menuDriver);
			return RunScreen(menuDriver);
		}
		/// <summary>
		/// Prints the menu to the screen.
		/// </summary>
		protected virtual void PrintScreen(MenuDriver menuDriver) {
			Console.Clear();

			// Read the text for the menu
			string[] lines = ReadScreenFile(FilePath);
			foreach (string line in lines)
				PrintLine(line);

			Console.WriteLine();
		}
		/// <summary>
		/// Checks for any missing files required by this screen
		/// </summary>
		/// <returns>An array of missing files.</returns>
		public virtual string[] GetMissingFiles() {
			List<string> files = new List<string>();
			if (!File.Exists(FilePath))
				files.Add(FilePath);
			return files.ToArray();
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Runs the screen.
		/// </summary>
		/// <returns>The action to perform after a screen choice.</returns>
		protected abstract MenuAction RunScreen(MenuDriver menuDriver);

		#endregion
	}
}
