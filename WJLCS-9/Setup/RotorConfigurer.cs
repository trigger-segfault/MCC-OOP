using System;
using System.IO;
using WJLCS.Enigma;
using WJLCS.Enigma.IO;

namespace WJLCS.Setup {
	/// <summary>
	/// The class for managing rotor setup.
	/// </summary>
	public class RotorConfigurer {

		#region Properties

		/// <summary>
		/// Gets the rotor keys setup in the Enigma Machine.
		/// </summary>
		public RotorKeys RotorKeys { get; private set; }
		/// <summary>
		/// Gets the path to the rotor count file, if one exists.
		/// </summary>
		public string File { get; private set; } = "Files/RotorKeys.txt";

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
		/// <exception cref="FileNotFoundException">
		/// The input file was not found.
		/// </exception>
		/// <exception cref="LoadFailedException">
		/// An error occurred while loading the rotor keys.
		/// </exception>
		public void ConfigureRotorKeys(string input) {
			string file = input;
			try {
				if (!System.IO.File.Exists(file))
					throw new FileNotFoundException($"Input file \"{file}\" does not exist!");
			} catch (Exception ex) {
				throw new FileNotFoundException($"Input file \"{file}\" does not exist!\n" + ex.Message, ex);
			}
			RotorKeys oldRotorKeys = RotorKeys;
			try {
				LoadFromFile(file);
				try {
					SaveToFile(RotorKeys, File);
				} catch (Exception ex) {
					RotorKeys = oldRotorKeys;
					throw new SaveFailedException(ex);
				}
			} catch (Exception ex) {
				throw new LoadFailedException(ex);
			}
		}
		/// <summary>
		/// Runs the rotor keys randomizer.
		/// </summary>
		/// 
		/// <exception cref="SaveFailedException">
		/// Failed to save the new rotor keys.
		/// </exception>
		public void RandomizeRotorKeys(string input) {
			if (!int.TryParse(input, out int rotorCount))
				throw new FormatException("Input is not a valid integer!");
			if (rotorCount < 1)
				throw new ArgumentOutOfRangeException("rotor count", "Input is less than 1!");
			try {
				RotorKeys newRotorKeys = RotorIO.Generate(rotorCount);
				SaveToFile(newRotorKeys, File);
				RotorKeys = newRotorKeys;
			} catch (Exception ex) {
				throw new SaveFailedException(ex);
			}
		}

		#endregion

		#region File IO

		/// <summary>
		/// Reads the file and returns the sets up the rotor keys.
		/// </summary>
		/// <param name="rotorKeysFile">The file containing the rotor keys.</param>
		/// 
		/// <exception cref="Exception">
		/// A file has invalid formatting, a parsed letter was invalid, or mismatched characters.
		/// </exception>
		private void LoadFromFile(string rotorKeysFile) {
			RotorKeys = RotorIO.Read(rotorKeysFile);
		}
		/// <summary>
		/// Writes the input rotor count to the file.
		/// </summary>
		/// <param name="rotorKeys">The new number of rotors to save.</param>
		/// <param name="rotorKeysFile">The rotor count file to save the count to.</param>
		private void SaveToFile(RotorKeys rotorKeys, string rotorKeysFile) {
			Directory.CreateDirectory(Path.GetDirectoryName(rotorKeysFile));
			RotorIO.Write(rotorKeys, rotorKeysFile);
		}

		#endregion
	}
}
