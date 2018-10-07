using System;
using System.IO;
using WJLCS.Enigma;
using WJLCS.Setup;
using WJLCS.Utils;

namespace WJLCS {
	/// <summary>
	/// The interpreter class that runs the Enigma Machine.
	/// </summary>
	public class Interpreter {

		#region Constants

		/// <summary>
		/// Gets the path to the letterset file.
		/// </summary>
		//public const string LetterSetFile = "Files/Letterset.txt";
		/// <summary>
		/// Gets the path to the letterset file, if one exists.
		/// </summary>
		//public const string PlugboardFile = "Files/Plugboard.txt";

		#endregion

		#region Fields
		
		private readonly LetterSetConfigurer letterSetConfig;
		private readonly PlugboardConfigurer plugboardConfig;
		private readonly RotorConfigurer rotorConfig;
		private readonly MachineConfigurer machineConfig;

		private readonly Encipherer encipherer;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the Enigma Machine <see cref="Interpreter"/>.
		/// </summary>
		public Interpreter() {
			letterSetConfig = new LetterSetConfigurer();
			plugboardConfig = new PlugboardConfigurer(letterSetConfig.LetterSet);
			rotorConfig = new RotorConfigurer();
			machineConfig = new MachineConfigurer(letterSetConfig,
												  plugboardConfig,
												  rotorConfig);
			encipherer = new Encipherer();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the path to the letterset file.
		/// </summary>
		public string LetterSetFile => letterSetConfig.File;
		/// <summary>
		/// Gets the path to the plugboard steckering file.
		/// </summary>
		public string PlugboardFile => plugboardConfig.File;

		#endregion

		#region Status

		/// <summary>
		/// Gets the setup status of the machine and prints it to the console.
		/// </summary>
		public void PrintMachineStatus() {
			machineConfig.Status.PrintStatus();
		}

		#endregion

		#region Enciphering
		
		/// <summary>
		/// Runs the Enigma Machine encipherer.
		/// </summary>
		/// <returns>The enciphered string.</returns>
		public void RunEncipherer(bool paste) {
			try {
				if (!machineConfig.IsSetup) {
					PrintError("The Enigma Machine has not been fully configured!");
					return;
				}
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write("Deciphered: ");
				string line;
				if (!paste) {
					line = Console.ReadLine();
				}
				else {
					line = TextCopy.Clipboard.GetText();
					if (string.IsNullOrEmpty(line)) {
						PrintError("Clipboard has no text!");
						return;
					}
					Console.WriteLine(line);
				}
				line = line.Replace("\t", "    ");
				if (string.IsNullOrEmpty(line)) {
					Console.WriteLine();
					return;
				}
				string enciphered = encipherer.Encipher(machineConfig.Machine, line);
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write("Enciphered: ");
				Console.WriteLine(enciphered);
				Console.ResetColor();
				Console.WriteLine();
				TextCopy.Clipboard.SetText(enciphered);
				Console.Write("Copied Enciphered Text! ");
			}
			finally {
				Console.ResetColor();
			}
		}
		/// <summary>
		/// Runs the Enigma Machine decipherer.
		/// </summary>
		/// <returns>The decipherer string.</returns>
		public void RunDecipherer(bool paste) {
			try {
				if (!machineConfig.IsSetup) {
					PrintError("The Enigma Machine has not been fully configured!");
					return;
				}
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write("Enciphered: ");
				string line;
				if (!paste) {
					line = Console.ReadLine();
				}
				else {
					line = TextCopy.Clipboard.GetText();
					if (string.IsNullOrEmpty(line)) {
						PrintError("Clipboard has no text!");
						return;
					}
					Console.WriteLine(line);
				}
				line = line.Replace("\t", "    ");
				if (string.IsNullOrEmpty(line)) {
					Console.WriteLine();
					return;
				}
				string deciphered = encipherer.Decipher(machineConfig.Machine, line);
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write("Deciphered: ");
				Console.WriteLine(deciphered);
				Console.ResetColor();
				Console.WriteLine();
				TextCopy.Clipboard.SetText(deciphered);
				Console.Write("Copied Deciphered Text! ");
			}
			finally {
				Console.ResetColor();
			}
		}

		#endregion

		#region Configuring

		/// <summary>
		/// Runs the letterset configurer.
		/// </summary>
		public void ConfigureLetterSet() {
			Console.Write("Enter letterset configuration file: ");
			string file = Console.ReadLine();
			try {
				letterSetConfig.ConfigureLetterSet(file);
				try {
					plugboardConfig.ConfigurePlugboard(letterSetConfig.LetterSet, plugboardConfig.File);
					machineConfig.SetupMachineIfReady();
				}
				catch (Exception ex) {
					plugboardConfig.ResetSteckering();
					PrintWarning($"Could not use plugboard configuration from " +
								 $"\"{plugboardConfig.File}\"!\n\"{ex.Message}");
				}
			}
			catch (Exception ex) {
				PrintError(ex.Message);
			}
		}
		/// <summary>
		/// Runs the plugboard configurer.
		/// </summary>
		public void ConfigurePlugboard() {
			if (letterSetConfig.LetterSet == null) {
				PrintError("Cannot configure Plugboard without a loaded letterset!");
				return;
			}
			Console.Write("Enter plugboard configuration file: ");
			string file = Console.ReadLine();
			try {
				plugboardConfig.ConfigurePlugboard(letterSetConfig.LetterSet, file);
				machineConfig.SetupMachineIfReady();
			}
			catch (Exception ex) {
				PrintError(ex.Message);
			}
		}
		/// <summary>
		/// Runs the plugboard randomizer.
		/// </summary>
		public void RandomizePlugboard() {
			if (letterSetConfig.LetterSet == null) {
				PrintError("Cannot configure Plugboard without a loaded letterset!");
				return;
			}
			Console.Write("Enter the path to save the plugboard file to: ");
			string file = Console.ReadLine();
			try {
				plugboardConfig.RandomizePlugboard(letterSetConfig.LetterSet, file);
				machineConfig.SetupMachineIfReady();
			}
			catch (Exception ex) {
				PrintError(ex.Message);
			}
		}

		/// <summary>
		/// Runs the rotor configurer.
		/// </summary>
		public void ConfigureRotors() {
			Console.Write("Enter rotor count greater than zero: ");
			string count = Console.ReadLine();
			try {
				rotorConfig.ConfigureRotorCount(count);
			}
			catch (Exception ex) {
				PrintError(ex.Message);
			}
		}

		#endregion

		#region Private Print
		
		/// <summary>
		/// Prints an error line.
		/// </summary>
		/// <param name="warning">The error to print.</param>
		private void PrintError(string error) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(error);
			Console.ResetColor();
		}
		/// <summary>
		/// Prints an warning line.
		/// </summary>
		/// <param name="warning">The warning to print.</param>
		private void PrintWarning(string warning) {
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(warning);
			Console.ResetColor();
		}

		#endregion

	}
}
