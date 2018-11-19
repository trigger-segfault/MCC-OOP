using System;
using System.Collections.Generic;
using System.IO;
using WJLCS.Screens;

namespace WJLCS.Menus {
	/// <summary>
	/// The base class for all choice-driven menus.
	/// </summary>
	public class Menu : Screen {
		
		#region Properties
		
		/// <summary>
		/// Gets the number of choices in the menu.
		/// </summary>
		public int ChoiceCount => Choices.Length;
		/// <summary>
		/// Gets the available screen action choices that can be performed.
		/// </summary>
		public MenuAction[] Choices { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="Menu"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the menu text.</param>
		/// <param name="publishVersion">
		/// True if there is a separate version of the menu file for production.
		/// </param>
		public Menu(string filePath/*, bool publishVersion*/) : base(filePath/*, publishVersion*/) { }

		#endregion

		#region PrintScreen

		/// <summary>
		/// Prints the menu to the screen.
		/// </summary>
		protected override void PrintScreen(MenuDriver screenDriver) {
			Console.Clear();

			// Read the text for the menu
			string[] lines = ReadScreenFile(FilePath);
			foreach (string line in lines)
				PrintLine(line);
		}

		#endregion

		#region RunMenu

		/// <summary>
		/// Runs the menu and choice input.
		/// </summary>
		/// <returns>The choice index that was made.</returns>
		protected override MenuAction RunScreen(MenuDriver screenDriver) {
			if (Choices == null)
				throw new ArgumentNullException("Menu has no Choices to execute!");
			int index;
			PrintScreen(screenDriver);
			while (!ReadChoice(out index)) {
				PrintScreen(screenDriver);
				PrintInvalidChoice();
			}
			return Choices[index];
		}

		/// <summary>
		/// Prints a message stating the last choice was invalid.
		/// </summary>
		protected void PrintInvalidChoice() {
			PrintError($"Invalid Choice! Must be between {1} and {ChoiceCount}!");
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
