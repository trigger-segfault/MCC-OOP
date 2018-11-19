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
#if PUBLISH
			/*if (publishVersion) {
				// Append "-Publish" to the end of the file name
				string name = Path.GetFileNameWithoutExtension(filePath);
				string ext = Path.GetExtension(filePath);
				FilePath = $"{name}-Publish{ext}";
			}*/
#endif
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reads all lines from the text file.
		/// </summary>
		/// <param name="filePath">The text file to read lines from.</param>
		/// <returns>The lines of the text file.</returns>
		protected string[] ReadScreenFile(string filePath) {
			string text = File.ReadAllText(FilePath);
			return text.SplitLines();
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

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Publishes and runs the screen.
		/// </summary>
		/// <returns>The action to perform after a screen choice.</returns>
		public virtual MenuAction Publish(MenuDriver screenDriver) {
			PrintScreen(screenDriver);
			return RunScreen(screenDriver);
		}
		/// <summary>
		/// Prints the menu to the screen.
		/// </summary>
		protected virtual void PrintScreen(MenuDriver screenDriver) {
			Console.Clear();

			// Read the text for the menu
			string[] lines = ReadScreenFile(FilePath);
			foreach (string line in lines)
				PrintLine(line);
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
		protected abstract MenuAction RunScreen(MenuDriver screenDriver);

		#endregion
	}
}
