using System;
using System.Collections.Generic;
using System.IO;
using WJLCS.Utils;

namespace WJLCS {
	/// <summary>
	/// The base class for all choice-driven menus.
	/// </summary>
	public class Menu : Screen {

		#region Constants

		/// <summary>
		/// The maximum width of a menu.
		/// </summary>
		public const int MenuWidth = 80;
		/// <summary>
		/// The maximum height of a menu.
		/// </summary>
		public const int MenuHeight = 24;

		#endregion
		
		#region Properties

		/// <summary>
		/// Gets the filepath of the menu text.
		/// </summary>
		public string FilePath { get; }
		/// <summary>
		/// Gets the number of choices in the menu.
		/// </summary>
		public int ChoiceCount => Choices.Length;
		/// <summary>
		/// Gets the available screen action choices that can be performed.
		/// </summary>
		public ScreenAction[] Choices { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="Menu"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the menu text.</param>
		public Menu(string filePath) {
			FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		#endregion

		#region PrintMenu

		/// <summary>
		/// Prints the menu to the screen.
		/// </summary>
		public virtual void PrintMenu() {
			Console.Clear();

			// Read the text for the menu
			string[] lines = ReadAllLines(FilePath);
			foreach (string line in lines)
				PrintLine(line);
		}

		/// <summary>
		/// Reads all lines from the text file.
		/// </summary>
		/// <param name="filePath">The text file to read lines from.</param>
		/// <returns>The lines of the text file.</returns>
		protected string[] ReadAllLines(string filePath) {
			string text = File.ReadAllText(FilePath);
			return text.SplitLines();
		}

		/// <summary>
		/// Prints the centered line that conforms to menu width standards.
		/// </summary>
		/// <param name="line">The line to center and print.</param>
		protected void PrintLine(string line) {
			// Perform word ellipses to conform to menu length
			foreach (string ellipsesLine in line.WordEllipsesSplit(MenuWidth, true)) {
				Console.WriteLine(ellipsesLine.Center(MenuWidth));
			}
		}

		#endregion

		#region RunMenu
		
		/// <summary>
		/// Runs the menu and choice input.
		/// </summary>
		/// <returns>The choice index that was made.</returns>
		public override ScreenAction Run() {
			if (Choices == null)
				throw new ArgumentNullException("Menu has no Choices to execute!");
			int index;
			PrintMenu();
			while (!ReadChoice(out index)) {
				PrintMenu();
				PrintInvalidChoice();
			}
			return Choices[index];
		}

		/// <summary>
		/// Prints a message stating the last choice was invalid.
		/// </summary>
		protected void PrintInvalidChoice() {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Invalid Choice! Must be between {1} and {ChoiceCount}!");
			Console.ResetColor();
		}

		/// <summary>
		/// Reads the user's choice input.
		/// </summary>
		/// <param name="index">The output choice index. -1 if invalid.</param>
		/// <returns>True if the choice was valid.</returns>
		protected bool ReadChoice(out int index) {
			if (ChoiceCount == 1)
				Console.Write("Press Enter: ");
			else
				Console.Write($"Enter choice ({1}-{ChoiceCount}): ");
			return IsValidChoice(Console.ReadLine(), out index);
		}

		/// <summary>
		/// Tests if the choice is valid.
		/// </summary>
		/// <param name="choice">The input choice.</param>
		/// <param name="index">The output choice index. -1 if invalid.</param>
		/// <returns>True if the choice is valid.</returns>
		protected bool IsValidChoice(string choice, out int index) {
			// No other choices means an enter will always succeed.
			if (ChoiceCount == 1) {
				index = 0;
				return true;
			}
			if (int.TryParse(choice, out index) && index >= 1 && index <= ChoiceCount) {
				// zero-index the index
				index--;
				return true;
			}
			index = -1;
			return false;
		}

		#endregion

		#region GetMissingFiles

		/// <summary>
		/// Checks for any missing files required by this screen
		/// </summary>
		/// <returns>An array of missing files.</returns>
		public override string[] GetMissingFiles() {
			List<string> files = new List<string>();
			if (!File.Exists(FilePath))
				files.Add(FilePath);
			return files.ToArray();
		}

		#endregion
	}
}
