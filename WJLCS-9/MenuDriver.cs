using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		public Interpreter Interpreter { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="MenuDriver"/> to control screen navigation.
		/// </summary>
		public MenuDriver() {
			// Setup the menu choices
			MainMenu.Choices = new MenuAction[] {
				EncryptionMenu,
				DecryptionMenu,
#if !PUBLISH
				PlugboardMenu,
#endif
				LettersetMenu,
				HTMLMenu,
				RotorDirectionsMenu,
				CreditsMenu,
				MenuAction.Exit,
			};
			LettersetMenu.Choices = new MenuAction[] {
#if !PUBLISH
				LetterSetConfigureScreen,
#endif
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
				DecipherMessageHtmlScreen,
				MainMenu,
			};
			PlugboardMenu.Choices = new MenuAction[] {
#if !PUBLISH
				PlugboardConfigureScreen,
				PlugboardRandomizeScreen,
#endif
				MainMenu,
			};
			RotorDirectionsMenu.Choices = new MenuAction[] {
#if !PUBLISH
				RotorConfigureScreen,
				RotorRandomizeScreen,
#endif
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
		public Menu MainMenu { get; } = new MainMenu("Files/Menus/MainMenu.txt");
		/// <summary>
		/// The letterset menu screen.
		/// </summary>
		public Menu LettersetMenu { get; } = new LettersetMenu("Files/Menus/LettersetMenu.txt");
		/// <summary>
		/// The encryption menu screen.
		/// </summary>
		public Menu EncryptionMenu { get; } = new Menu("Files/Menus/EncryptionMenu.txt");
		/// <summary>
		/// The decryption menu screen.
		/// </summary>
		public Menu DecryptionMenu { get; } = new Menu("Files/Menus/DecryptionMenu.txt");
		/// <summary>
		/// The plugboard menu screen.
		/// </summary>
		public Menu PlugboardMenu { get; } = new Menu("Files/Menus/PlugboardMenu.txt");
		/// <summary>
		/// The encryption directions menu screen.
		/// </summary>
		public Menu EncryptionDirectionsMenu { get; } = new Menu("Files/Menus/EncryptionDirectionsMenu.txt");
		/// <summary>
		/// The rotor directions menu screen.
		/// </summary>
		public Menu RotorDirectionsMenu { get; } = new Menu("Files/Menus/RotorDirectionsMenu.txt");
		/// <summary>
		/// The HTML menu screen.
		/// </summary>
		public Menu HTMLMenu { get; } = new Menu("Files/Menus/HTMLMenu.txt");
		/// <summary>
		/// The credits menu screen.
		/// </summary>
		public Menu CreditsMenu { get; } = new Menu("Files/Menus/CreditsMenu.txt");
		/// <summary>
		/// The missing files menu screen.
		/// </summary>
		public MissingFilesMenu MissingFilesMenu { get; } = new MissingFilesMenu("Files/Menus/MissingFilesMenu.txt");
		/// <summary>
		/// The unhandled exception menu screen.
		/// </summary>
		public ExceptionMenu ExceptionMenu { get; } = new ExceptionMenu("Files/Menus/ExceptionMenu.txt");

		#endregion

		#region Interpreter Screens

		/// <summary>
		/// The encipher message screen.
		/// </summary>
		public Screen EncipherMessageScreen { get; } = new EncipherMessageScreen("Files/Screens/EncipherMessageScreen.txt", EncipherMode.Input);
		/// <summary>
		/// The encipher message screen.
		/// </summary>
		public Screen EncipherMessagePasteScreen { get; } = new EncipherMessageScreen("Files/Screens/EncipherMessageScreen.txt", EncipherMode.Paste);
		/// <summary>
		/// The decipher message screen.
		/// </summary>
		public Screen DecipherMessageScreen { get; } = new DecipherMessageScreen("Files/Screens/DecipherMessageScreen.txt", EncipherMode.Input);
		/// <summary>
		/// The decipher message screen.
		/// </summary>
		public Screen DecipherMessagePasteScreen { get; } = new DecipherMessageScreen("Files/Screens/DecipherMessageScreen.txt", EncipherMode.Paste);
		/// <summary>
		/// The decipher message screen.
		/// </summary>
		public Screen DecipherMessageHtmlScreen { get; } = new DecipherMessageScreen("Files/Screens/DecipherMessageScreen.txt", EncipherMode.HTML);
		/// <summary>
		/// The letterset configuration screen.
		/// </summary>
		public Screen LetterSetConfigureScreen { get; } = new LetterSetConfigureScreen("Files/Screens/LetterSetConfigureScreen.txt");
		/// <summary>
		/// The plugboard configuration screen.
		/// </summary>
		public Screen PlugboardConfigureScreen { get; } = new PlugboardConfigureScreen("Files/Screens/PlugboardConfigureScreen.txt");
		/// <summary>
		/// The plugboard randomize screen.
		/// </summary>
		public Screen PlugboardRandomizeScreen { get; } = new PlugboardRandomizeScreen("Files/Screens/PlugboardRandomizeScreen.txt");
		/// <summary>
		/// The rotor configure screen.
		/// </summary>
		public Screen RotorConfigureScreen { get; } = new RotorConfigureScreen("Files/Screens/RotorConfigureScreen.txt");
		/// <summary>
		/// The rotor randomize screen.
		/// </summary>
		public Screen RotorRandomizeScreen { get; } = new RotorRandomizeScreen("Files/Screens/RotorRandomizeScreen.txt");

		#endregion

		#region Properties

		/// <summary>
		/// The current screen being displayed.
		/// </summary>
		public Screen CurrentScreen { get; private set; }
		/// <summary>
		/// The last screen to be visited.
		/// </summary>
		public Screen LastScreen { get; private set; }

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
					Screen nextScreen = nextAction?.Invoke(this);
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
			foreach (string file in Interpreter.GetMissingFiles())
				files.Add(file);
			// Enumerate over all screen properties
			foreach (PropertyInfo prop in this.GetType().GetProperties()) {
				if (typeof(Screen).IsAssignableFrom(prop.PropertyType)) {
					// Check if this property has a setter, skip non-constant properties
					if (prop.GetSetMethod(true) == null) {
						Screen screen = (Screen) prop.GetValue(this);
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
