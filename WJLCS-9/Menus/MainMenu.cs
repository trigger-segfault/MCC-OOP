
namespace WJLCS.Menus {
	/// <summary>
	/// The menu class for displaying the main welcome screen.
	/// </summary>
	public class MainMenu : Menu {

		#region Constants
		
		/// <summary>
		/// This text in the menu file will be replaced with the machine status.
		/// </summary>
		private const string StatusToken = "ENIGMASTATUS";

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="MainMenu"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the menu text.</param>
		public MainMenu(string filePath) : base(filePath, false) {
			// false to disable no input as exiting the program
			AddToken(StatusToken, PrintStatus);
		}

		#endregion

		#region Print Command
		
		/// <summary>
		/// Prints the Enigma Machine status.
		/// </summary>
		private void PrintStatus(MenuDriver menuDriver) {
			menuDriver.Interpreter.PrintMachineStatus();
		}

		#endregion
	}
}
