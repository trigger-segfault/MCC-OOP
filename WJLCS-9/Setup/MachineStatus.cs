using System;
using WJLCS.Screens;

namespace WJLCS {
	/// <summary>
	/// A structure stating the setup status of the enigma machine.
	/// </summary>
	public struct MachineStatus {

		#region Fields

		/// <summary>
		/// True if the machine is setup.
		/// </summary>
		public bool IsSetup { get; set; }
		/// <summary>
		/// The letterset file.
		/// </summary>
		public string LetterSetFile { get; set; }
		/// <summary>
		/// True if the letterset is loaded.
		/// </summary>
		public bool LetterSetLoaded { get; set; }
		/// <summary>
		/// The total number of letters.
		/// </summary>
		public int LetterCount { get; set; }
		/// <summary>
		/// The hash of the letterset.
		/// </summary>
		public int LetterSetHash { get; set; }
		/// <summary>
		/// The plugboard file.
		/// </summary>
		public string PlugboardFile { get; set; }
		/// <summary>
		/// True if the plugboard is loaded.
		/// </summary>
		public bool PlugboardLoaded { get; set; }
		/// <summary>
		/// The hash of the steckering.
		/// </summary>
		public int PlugboardHash { get; set; }
		/// <summary>
		/// True if the rotor keys are loaded.
		/// </summary>
		public bool RotorKeysLoaded { get; set; }
		/// <summary>
		/// The rotor keys file.
		/// </summary>
		public string RotorKeysFile { get; set; }
		/// <summary>
		/// The hash of the rotor keys.
		/// </summary>
		public int RotorKeysHash { get; set; }
		/// <summary>
		/// The rotor count.
		/// </summary>
		public int RotorCount { get; set; }
		/// <summary>
		/// The array of rotor keys being used.
		/// </summary>
		public int[] RotorKeys { get; set; }

		#endregion

		#region Casting

		/// <summary>
		/// Casts the <see cref="MachineStatus"/> to a <see cref="bool"/>. Returns true if the machine is
		/// setup.
		/// </summary>
		/// <param name="status">The machine status to cast.</param>
		public static implicit operator bool(MachineStatus status) {
			return status.IsSetup;
		}

		#endregion

		#region Print

		/// <summary>
		/// Prints the status to the console.
		/// </summary>
		public void PrintStatus() {
			PrintHeader("CONFIGURATION STATUS:");
			PrintStatus(" Enigma Machine", IsSetup,
				"<Setup Complete>",
				"<Not Setup>");
			if (!LetterSetLoaded)
				PrintError(" Letterset File", $"{LetterSetFile} (Failed to load!)");
			else
				PrintStatus(" Letterset File", LetterSetFile != null,
					$"{LetterSetFile} (Hash: {LetterSetHash:X8}, Letters: {LetterCount})",
					"<Not Setup>");
			if (!PlugboardLoaded)
				PrintError(" Plugboard File", $"{PlugboardFile} (Failed to load!)");
			else
				PrintStatus(" Plugboard File", PlugboardFile != null,
					$"{PlugboardFile} (Hash: {PlugboardHash:X8})",
					"<Not Setup>");
			if (!RotorKeysLoaded)
				PrintError("Rotor Keys File", $"{RotorKeysFile} (Failed to load!)");
			else {
				PrintStatus("Rotor Keys File", RotorKeysFile != null,
					$"{RotorKeysFile} (Hash: {RotorKeysHash:X8}, Rotors: {RotorCount})",
					"<Not Setup>");
				if (RotorKeysFile != null) {
					PrintRotorKeys("     Rotor Keys");
				}
			}
		}

		#endregion

		#region Private Print

		private void PrintHeader(string line) {
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(line);
			Console.ResetColor();
		}
		private void PrintStatus(string label, bool condition, string ifTrue, string ifFalse) {
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(label + ": ");
			if (condition) {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(ifTrue);
			}
			else {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ifFalse);
			}
			Console.ResetColor();
		}
		private void PrintOK(string label, string value) {
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(label + ": ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(value);
			Console.ResetColor();
		}
		private void PrintRotorKeys(string label) {
			string keysStr = string.Join(" ", RotorKeys);
			const int maxLength = Screen.ScreenWidth - 20;
			if (keysStr.Length > maxLength) {
				// Cut off the extra characters by identifying the last space.
				int lastSpace = keysStr.LastIndexOf(' ', maxLength);
				keysStr = keysStr.Substring(0, lastSpace) + "..."; // Add ellipses
			}
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(label + ": ");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine(keysStr);
			Console.ResetColor();
		}
		private void PrintError(string label, string value) {
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(label + ": ");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(value);
			Console.ResetColor();
		}

		#endregion
	}
}
