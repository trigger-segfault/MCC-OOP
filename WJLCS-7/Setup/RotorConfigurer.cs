using System;
using System.IO;
using WJLCS.Enigma;

namespace WJLCS.Setup {
	/// <summary>
	/// The class for managing rotor setup.
	/// </summary>
	public class RotorConfigurer {

		#region Properties

		/// <summary>
		/// Gets the number of rotors in the Enigma Machine.
		/// </summary>
		public int RotorCount { get; private set; } = 3;
		/// <summary>
		/// Gets the path to the rotor count file, if one exists.
		/// </summary>
		public string File { get; private set; } = "Files/RotorCount.txt";

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the Enigma Machine <see cref="RotorConfigurer"/>.
		/// </summary>
		public RotorConfigurer() {
			try {
				if (System.IO.File.Exists(File))
					LoadFromFile(File);
			}
			catch { }
		}

		#endregion

		#region Configure

		/// <summary>
		/// Runs the rotor count configurer.
		/// </summary>
		/// 
		/// <exception cref="FormatException">
		/// An input rotor count is not an integer.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The input rotor count is less than one.
		/// </exception>
		public void ConfigureRotorCount(string input) {
			if (!int.TryParse(input, out int newRotorCount))
				throw new FormatException("Input is not a valid integer!");
			if (newRotorCount < 1)
				throw new ArgumentOutOfRangeException("rotor count", "Input is less than 1!");
			if (newRotorCount > SetupArgs.MaxRotorCount)
				throw new ArgumentOutOfRangeException("rotor count", $"Input is greater than the max " +
													 $"value of {SetupArgs.MaxRotorCount}!");
			SaveToFile(newRotorCount, File);
			RotorCount = newRotorCount;
		}

		#endregion
		
		#region File IO

		/// <summary>
		/// Reads the file and returns the rotor count.
		/// </summary>
		/// <param name="rotorCountFile">The file containing the rotor count.</param>
		/// 
		/// <exception cref="Exception">
		/// A file has invalid formatting, a parsed letter was invalid, or mismatched characters.
		/// </exception>
		private void LoadFromFile(string rotorCountFile) {
			string text = System.IO.File.ReadAllText(rotorCountFile);

			int newRotorCount = int.Parse(text.Trim());

			if (newRotorCount < 1)
				throw new Exception("Rotor count is less than 1!");
			if (newRotorCount > SetupArgs.MaxRotorCount)
				throw new Exception($"Rotor count is greater than the max count of " +
									$"{SetupArgs.MaxRotorCount}!");

			RotorCount = newRotorCount;
		}
		/// <summary>
		/// Writes the input rotor count to the file.
		/// </summary>
		/// <param name="rotorCount">The new number of rotors to save.</param>
		/// <param name="rotorCountFile">The rotor count file to save the count to.</param>
		private void SaveToFile(int rotorCount, string rotorCountFile) {
			Directory.CreateDirectory(Path.GetDirectoryName(File));
			System.IO.File.WriteAllText(rotorCountFile, rotorCount.ToString());
		}

		#endregion
	}
}
