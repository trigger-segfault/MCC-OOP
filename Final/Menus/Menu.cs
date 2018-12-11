using System;
using System.Collections.Generic;
using WJLCS.Screens;

namespace WJLCS.Menus {
	/// <summary>
	/// The base class for all choice-driven menus.
	/// </summary>
	public class Menu : Screen {

		#region Fields

		/// <summary>
		/// The list of tokens that are interpreted as special print commands.
		/// </summary>
		private readonly Dictionary<string, Action<MenuDriver>> tokenCommands;
		/// <summary>
		/// True if the menu accepts no input as the last choice in the list.
		/// </summary>
		private readonly bool acceptNoInput;

		#endregion

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
		public Menu(string filePath, bool acceptNoInput = true) : base(filePath) {
			tokenCommands = new Dictionary<string, Action<MenuDriver>>();
			this.acceptNoInput = acceptNoInput;
		}

		#endregion

		#region Tokens

		/// <summary>
		/// Adds a tokens that is interpreted as special print commands.<para/>
		/// The added token is automatically surrounded with [! and !].
		/// </summary>
		public void AddToken(string token, Action<MenuDriver> printCommand) {
			tokenCommands.Add($"[!{token}!]", printCommand);
		}

		#endregion

		#region PrintScreen

		/// <summary>
		/// Prints the menu to the screen.
		/// </summary>
		protected override void PrintScreen(MenuDriver menuDriver) {
			Console.Clear();

			// Read the text for the menu
			string[] lines = ReadScreenFile(FilePath);
			foreach (string line in lines) {
				// Check for token commands
				if (tokenCommands.TryGetValue(line.Trim(), out var printCommand))
					printCommand(menuDriver);
				else
					PrintLine(line);
			}
			Console.WriteLine();
		}

		#endregion

		#region RunMenu

		/// <summary>
		/// Runs the menu and choice input.
		/// </summary>
		/// <returns>The choice index that was made.</returns>
		protected override MenuAction RunScreen(MenuDriver menuDriver) {
			if (Choices == null)
				throw new ArgumentNullException("Menu has no Choices to execute!");
			int index;
			PrintScreen(menuDriver);
			while (!ReadChoice(out index)) {
				PrintScreen(menuDriver);
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
			if (ChoiceCount == 1) {
				Console.Write("Press Enter: ");
			}
			else {
				Console.Write($"Enter choice ({1}-{ChoiceCount}): ");
				if (acceptNoInput)
					PrintWatermark(ChoiceCount.ToString());
			}
			return IsValidChoice(Console.ReadLine(), out index);
		}

		/// <summary>
		/// Tests if the choice is valid.
		/// </summary>
		/// <param name="choice">The input choice.</param>
		/// <param name="index">The output choice index. -1 if invalid.</param>
		/// <returns>True if the choice is valid.</returns>
		protected bool IsValidChoice(string choice, out int index) {
			index = -1;
			if (ChoiceCount == 1) {
				// No other choices means an enter will always succeed.
				index = 0;
			}
			else if (acceptNoInput && string.IsNullOrEmpty(choice)) {
				// zero-index the choice
				index = ChoiceCount - 1;
			}
			else if (int.TryParse(choice, out index) && index >= 1 && index <= ChoiceCount) {
				// zero-index the choice
				index--;
			}
			else {
				index = -1;
			}
			return index != -1;
		}

		#endregion

		#region GetMissingFiles

		/*/// <summary>
		/// Checks for any missing files required by this screen
		/// </summary>
		/// <returns>An array of missing files.</returns>
		public override string[] GetMissingFiles() {
			List<string> files = new List<string>();
			if (!File.Exists(FilePath))
				files.Add(FilePath);
			return files.ToArray();
		}*/

		#endregion
	}
}
