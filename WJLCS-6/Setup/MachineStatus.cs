using System;

namespace WJLCS {
	/// <summary>
	/// A structure stating the setup status of the enigma machine.
	/// </summary>
	public struct MachineStatus {

		#region Fields

		public bool IsSetup { get; set; }
		public string LetterSetFile { get; set; }
		public bool LetterSetLoaded { get; set; }
		public int TotalLetters { get; set; }
		public int LetterSetHash { get; set; }
		public string PlugboardFile { get; set; }
		public bool PlugboardLoaded { get; set; }
		public int PlugboardHash { get; set; }
		public int RotorCount { get; set; }

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

		public void PrintStatus() {
			PrintHeader("CONFIGURATION STATUS:");
			PrintStatus("Enigma Machine", IsSetup,
				"<Setup Complete>",
				"<Not Setup>");
			if (!LetterSetLoaded)
				PrintError("Letterset File", $"{LetterSetFile} (Failed to load!)");
			else
				PrintStatus("Letterset File", LetterSetFile != null,
					$"{LetterSetFile} (Hash: {LetterSetHash:X8}, Letters: {TotalLetters})",
					"<Not Setup>");
			if (!PlugboardLoaded)
				PrintError("Letterset File", $"{PlugboardFile} (Failed to load!)");
			else
				PrintStatus("Plugboard File", PlugboardFile != null,
					$"{PlugboardFile} (Hash: {PlugboardHash:X8})",
					"<Not Setup>");
			PrintOK("   Rotor Count",
				RotorCount.ToString());
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
