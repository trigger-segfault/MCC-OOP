using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WJLCS {
	/// <summary>
	/// The driver for controlling screens and navigation.
	/// </summary>
	public class ScreenDriver {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="ScreenDriver"/> to control screen navigation.
		/// </summary>
		public ScreenDriver() {
			MainMenu.Choices = new ScreenAction[] {
				ScreenAction.CurrentScreen, // Template
				ScreenAction.CurrentScreen, // Template
				ScreenAction.CurrentScreen, // Template
				LettersetMenu,
				ScreenAction.CurrentScreen, // Template
				ScreenAction.CurrentScreen, // Template
				CreditsMenu,
				ScreenAction.Exit,
			};
			LettersetMenu.Choices = new ScreenAction[] {
				ScreenAction.LastScreen,
			};
			CreditsMenu.Choices = new ScreenAction[] {
				ScreenAction.LastScreen,
			};
		}

		#endregion

		#region Menus

		/// <summary>
		/// The main menu screen.
		/// </summary>
		public Menu MainMenu { get; } = new Menu("Menus/MainMenu.txt");
		/// <summary>
		/// The letterset menu screen.
		/// </summary>
		public Menu LettersetMenu { get; } = new LettersetMenu("Menus/LettersetMenu.txt", "Letterset.txt");
		/// <summary>
		/// The credits menu screen.
		/// </summary>
		public Menu CreditsMenu { get; } = new Menu("Menus/CreditsMenu.txt");
		/// <summary>
		/// The missing files menu screen.
		/// </summary>
		public MissingFilesMenu MissingFilesMenu { get; } = new MissingFilesMenu("Menus/MissingFilesMenu.txt");
		/// <summary>
		/// The unhandled exception menu screen.
		/// </summary>
		public ExceptionMenu ExceptionMenu { get; } = new ExceptionMenu("Menus/ExceptionMenu.txt");

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
					ScreenAction nextAction = CurrentScreen.Run();
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
				ExceptionMenu.Run();
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
