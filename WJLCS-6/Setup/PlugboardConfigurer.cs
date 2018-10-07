using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WJLCS.Enigma;
using WJLCS.Utils;
using static WJLCS.Setup.LetterParser;

namespace WJLCS.Setup {
	/// <summary>
	/// The class for managing plugboard setup.
	/// </summary>
	public class PlugboardConfigurer {

		#region Properties

		/// <summary>
		/// Gets the loaded plugboard steckering.
		/// </summary>
		public Steckering Steckering { get; private set; }
		/// <summary>
		/// Gets the path to the letterset file, if one exists.
		/// </summary>
		public string File { get; private set; } = "Files/Plugboard.txt";

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the Enigma Machine <see cref="PlugboardConfigurer"/>.
		/// </summary>
		/// <param name="letterSet">The existing loaded letterset, or null.</param>
		public PlugboardConfigurer(LetterSet letterSet) {
			if (letterSet == null)
				return;
			try {
				if (System.IO.File.Exists(File))
					LoadFromFile(letterSet, File);
			}
			catch { }
		}

		#endregion

		#region Configure

		/// <summary>
		/// Runs the plugboard configurer.
		/// </summary>
		/// 
		/// <exception cref="FileNotFoundException">
		/// The input file was not found.
		/// </exception>
		/// <exception cref="LoadFailedException">
		/// An error occurred while loading the plugboard steckering.
		/// </exception>
		public string ConfigurePlugboard(LetterSet letterSet, string input) {
			if (letterSet == null)
				throw new LetterSetMissingException("Cannot configure Plugboard without a loaded letterset!");
			string file = input;
			try {
				if (!System.IO.File.Exists(file))
					throw new FileNotFoundException($"Input file \"{file}\" does not exist!");
			} catch (Exception ex) {
				throw new FileNotFoundException($"Input file \"{file}\" does not exist!\n" + ex.Message, ex);
			}
			try {
				LoadFromFile(letterSet, file);
				File = file;
			}
			catch (Exception ex) {
				throw new LoadFailedException(ex);
			}
			return null;
		}
		/// <summary>
		/// Runs the plugboard randomizer.
		/// </summary>
		/// 
		/// <exception cref="IOException">
		/// The input save path is invalid.
		/// </exception>
		/// <exception cref="SaveFailedException">
		/// Failed to save the new plugboard steckering.
		/// </exception>
		public void RandomizePlugboard(LetterSet letterSet, string input) {
			if (letterSet == null)
				throw new LetterSetMissingException("Cannot configure Plugboard without a loaded letterset!");
			string file = input;
			try {
				Path.GetFullPath(file);
				string directory = Path.GetDirectoryName(file);
				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
					Directory.CreateDirectory(directory);
			} catch (Exception ex) {
				throw new IOException($"Input file \"{file}\" has an invalid path!\n" + ex.Message, ex);
			}
			try {
				Steckering newSteckering = letterSet.RandomizeSteckering();
				SaveToFile(letterSet, newSteckering, file);
				Steckering = newSteckering;
				File = file;
			}
			catch (Exception ex) {
				throw new SaveFailedException(ex);
			}
		}
		/// <summary>
		/// Sets the steckering to null due a conflicting letterset.
		/// </summary>
		public void ResetSteckering() {
			Steckering = null;
		}

		#endregion

		#region File IO


		/// <summary>
		/// Reads the file and returns the <see cref="int[]"/> plugboard steckering.
		/// </summary>
		/// <param name="letterSet">The letterset to reference the index of the characters.</param>
		/// <param name="plugboardFile">The file containing the plugboard setup.</param>
		/// 
		/// <exception cref="Exception">
		/// A file has invalid formatting, a parsed letter was invalid, or mismatched characters.
		/// </exception>
		private void LoadFromFile(LetterSet letterSet, string plugboardFile) {
			string text = System.IO.File.ReadAllText(plugboardFile);
			string[] lines = text.SplitLines(true);
			// Keep track of the characters that are consumed.
			HashSet<char> inputCharacters = new HashSet<char>();
			HashSet<char> outputCharacters = new HashSet<char>();
			foreach (char c in letterSet)
				inputCharacters.Add(c);
			foreach (char c in letterSet)
				outputCharacters.Add(c);
			int[] steckering = new int[letterSet.Count];

			foreach (string line in lines) {
				int spaceIndex = line.IndexOf(' ', 1);
				if (spaceIndex == -1)
					throw new Exception($"Line \"{line}\" is missing space separator!");
				char inputChar = ParseLetter(line.Substring(0, spaceIndex), false).Value;
				char outputChar = ParseLetter(line.Substring(spaceIndex + 1), false).Value;
				if (!inputCharacters.Remove(inputChar))
					throw new Exception($"Line \"{line}\" input character \'{inputChar}\' has already " +
						$"been used or does not exist in the letterset!");
				if (!outputCharacters.Remove(inputChar))
					throw new Exception($"Line \"{line}\" output character \'{outputChar}\' has already " +
						$"been used or does not exist in the letterset!");
				int inputIndex = letterSet.IndexOf(inputChar);
				int outputIndex = letterSet.IndexOf(outputChar);
				steckering[inputIndex] = outputIndex;
			}

			foreach (char c in inputCharacters) {
				if (!outputCharacters.Remove(c))
					throw new Exception($"\'{c}\' is mapped to an output character but not an input " +
						$"character, cannot self-stecker!");
			}
			foreach (char c in outputCharacters) {
				throw new Exception($"\'{c}\' is mapped to an input character but not an output " +
					$"character, cannot self-stecker!");
			}
			Steckering = new Steckering(steckering);
		}
		/// <summary>
		/// Writes the input plugboard steckering to the file.
		/// </summary>
		/// <param name="letterSet">The letterset to map the steckering to.</param>
		/// <param name="steckering">The new steckering to save.</param>
		/// <param name="plugboardFile">The plugboard file to save the steckering to.</param>
		private void SaveToFile(LetterSet letterSet, Steckering steckering, string plugboardFile) {
			StringBuilder str = new StringBuilder();

			for (int inputIndex = 0; inputIndex < steckering.Count; inputIndex++) {
				int outputIndex = steckering[inputIndex];
				string input = EscapeLetter(letterSet[inputIndex]);
				string output = EscapeLetter(letterSet[outputIndex]);
				str.AppendLine($"{input} {output}");
			}
			System.IO.File.WriteAllText(plugboardFile, str.ToString());
		}

		#endregion
	}
}
