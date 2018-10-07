using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WJLCS.Menus;
using WJLCS.Screens;

namespace WJLCS {
	/// <summary>
	/// The driver for controlling screens and navigation.
	/// </summary>
	public class MenuDriver {
		
		#region Fields

		/// <summary>
		/// Gets the interpreter that handles Enigma Machine functionality.
		/// </summary>
		public Interpreter Interpreter { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="MenuDriver"/> to control screen navigation.
		/// </summary>
		public MenuDriver() {
			MainMenu.Choices = new MenuAction[] {
				EncryptionMenu,
				DecryptionMenu,
				PlugboardMenu,
				LettersetMenu,
				HTMLMenu,
				RotorDirectionsMenu,
				CreditsMenu,
				MenuAction.Exit,
			};
			LettersetMenu.Choices = new MenuAction[] {
				LetterSetConfigureScreen,
				MainMenu,
			};
			CreditsMenu.Choices = new MenuAction[] {
				MainMenu,
			};
			EncryptionMenu.Choices = new MenuAction[] {
				EncryptionDirectionsMenu,
				EncipherMessageScreen,
				EncipherMessagePasteScreen,
				MainMenu,
			};
			EncryptionDirectionsMenu.Choices = new MenuAction[] {
				MainMenu,
			};
			DecryptionMenu.Choices = new MenuAction[] {
				DecipherMessageScreen,
				DecipherMessagePasteScreen,
				MainMenu,
			};
			PlugboardMenu.Choices = new MenuAction[] {
				PlugboardConfigureScreen,
				PlugboardRandomizeScreen,
				MainMenu,
			};
			RotorDirectionsMenu.Choices = new MenuAction[] {
				RotorConfigureScreen,
				MainMenu,
			};
			HTMLMenu.Choices = new MenuAction[] {
				MainMenu,
			};
			MissingFilesMenu.Choices = new MenuAction[] {
				MenuAction.Exit,
			};
			ExceptionMenu.Choices = new MenuAction[] {
				MenuAction.Exit,
			};
		}

		#endregion

		#region Menus

		/// <summary>
		/// The main menu screen.
		/// </summary>
		public GetMenu MainMenu { get; } = new GetMainMenu("Files/Menus/MainMenu.txt");
		/// <summary>
		/// The letterset menu screen.
		/// </summary>
		public GetMenu LettersetMenu { get; } = new GetLettersetMenu("Files/Menus/LettersetMenu.txt");
		/// <summary>
		/// The encryption menu screen.
		/// </summary>
		public GetMenu EncryptionMenu { get; } = new GetMenu("Files/Menus/EncryptionMenu.txt");
		/// <summary>
		/// The decryption menu screen.
		/// </summary>
		public GetMenu DecryptionMenu { get; } = new GetMenu("Files/Menus/DecryptionMenu.txt");
		/// <summary>
		/// The plugboard menu screen.
		/// </summary>
		public GetMenu PlugboardMenu { get; } = new GetMenu("Files/Menus/PlugboardMenu.txt");
		/// <summary>
		/// The encryption directions menu screen.
		/// </summary>
		public GetMenu EncryptionDirectionsMenu { get; } = new GetMenu("Files/Menus/EncryptionDirectionsMenu.txt");
		/// <summary>
		/// The rotor directions menu screen.
		/// </summary>
		public GetMenu RotorDirectionsMenu { get; } = new GetMenu("Files/Menus/RotorDirectionsMenu.txt");
		/// <summary>
		/// The HTML menu screen.
		/// </summary>
		public GetMenu HTMLMenu { get; } = new GetMenu("Files/Menus/HTMLMenu.txt");
		/// <summary>
		/// The credits menu screen.
		/// </summary>
		public GetMenu CreditsMenu { get; } = new GetMenu("Files/Menus/CreditsMenu.txt");
		/// <summary>
		/// The missing files menu screen.
		/// </summary>
		public GetMissingFilesMenu MissingFilesMenu { get; } = new GetMissingFilesMenu("Files/Menus/MissingFilesMenu.txt");
		/// <summary>
		/// The unhandled exception menu screen.
		/// </summary>
		public GetExceptionMenu ExceptionMenu { get; } = new GetExceptionMenu("Files/Menus/ExceptionMenu.txt");

		#endregion

		#region Screens

		/// <summary>
		/// The encipher message screen.
		/// </summary>
		public GetScreen EncipherMessageScreen { get; } = new GetEncipherMessageScreen("Files/Screens/EncipherMessageScreen.txt", false);
		/// <summary>
		/// The encipher message screen.
		/// </summary>
		public GetScreen EncipherMessagePasteScreen { get; } = new GetEncipherMessageScreen("Files/Screens/EncipherMessageScreen.txt", true);
		/// <summary>
		/// The decipher message screen.
		/// </summary>
		public GetScreen DecipherMessageScreen { get; } = new GetDecipherMessageScreen("Files/Screens/DecipherMessageScreen.txt", false);
		/// <summary>
		/// The decipher message screen.
		/// </summary>
		public GetScreen DecipherMessagePasteScreen { get; } = new GetDecipherMessageScreen("Files/Screens/DecipherMessageScreen.txt", true);
		/// <summary>
		/// The letterset configuration screen.
		/// </summary>
		public GetScreen LetterSetConfigureScreen { get; } = new GetLetterSetConfigureScreen("Files/Screens/LetterSetConfigureScreen.txt");
		/// <summary>
		/// The plugboard configuration screen.
		/// </summary>
		public GetScreen PlugboardConfigureScreen { get; } = new GetPlugboardConfigureScreen("Files/Screens/PlugboardConfigureScreen.txt");
		/// <summary>
		/// The plugboard randomize screen.
		/// </summary>
		public GetScreen PlugboardRandomizeScreen { get; } = new GetPlugboardRandomizeScreen("Files/Screens/PlugboardRandomizeScreen.txt");
		/// <summary>
		/// The rotor configure screen.
		/// </summary>
		public GetScreen RotorConfigureScreen { get; } = new GetRotorConfigureScreen("Files/Screens/RotorConfigureScreen.txt");

		#endregion

		#region Properties

		/// <summary>
		/// The current screen being displayed.
		/// </summary>
		public GetScreen CurrentScreen { get; private set; }
		/// <summary>
		/// The last screen to be visited.
		/// </summary>
		public GetScreen LastScreen { get; private set; }

		#endregion

		#region Run

		/// <summary>
		/// Runs the screen driver.
		/// </summary>
		public void Run() {
			try {
				Interpreter = new Interpreter();

				// Check for missing files before running
				string[] missingFiles = CheckForMissingFiles();
				if (missingFiles.Length > 0) {
					Log.LogMissingFiles(missingFiles);
					MissingFilesMenu.MissingFiles = missingFiles;
					CurrentScreen = MissingFilesMenu;
				}
				else {
					CurrentScreen = MainMenu;
				}
				LastScreen = CurrentScreen;

				// Run the screen driver loop
				do {
					MenuAction nextAction = CurrentScreen.Publish(this);
					GetScreen nextScreen = nextAction?.Invoke(this);
					if (nextScreen != CurrentScreen) {
						LastScreen = CurrentScreen;
						CurrentScreen = nextScreen;
					}
				} while (CurrentScreen != null);
			}
			catch (Exception ex) {
				Log.LogException(ex);
				// Oh no! Tell the user what went wrong so it can be reported!
				ExceptionMenu.Exception = ex;
				ExceptionMenu.Publish(this);
			}
		}

		/// <summary>
		/// Checks all classes for any missing required runtime files.
		/// </summary>
		/// <returns></returns>
		private string[] CheckForMissingFiles() {
			// Use a hashset to avoid duplicate file names.
			HashSet<string> files = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			// Enumerate over all screen properties
			foreach (PropertyInfo prop in this.GetType().GetProperties()) {
				if (typeof(GetScreen).IsAssignableFrom(prop.PropertyType)) {
					// Check if this property has a setter, skip non-constant properties
					if (prop.GetSetMethod(true) == null) {
						GetScreen screen = (GetScreen) prop.GetValue(this);
						foreach (string file in screen.GetMissingFiles())
							files.Add(file);
					}
				}
			}
			return files.ToArray();
		}
		
		#endregion
	}
}
