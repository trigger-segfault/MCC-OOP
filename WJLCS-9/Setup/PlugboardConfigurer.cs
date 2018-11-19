using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WJLCS.Enigma;
using WJLCS.Enigma.IO;
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
		/// Gets the path to the plugboard steckering file, if one exists.
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
			Steckering oldSteckering = Steckering;
			try {
				LoadFromFile(letterSet, file);
				try {
					SaveToFile(Steckering, File);
				} catch (Exception ex) {
					Steckering = oldSteckering;
					throw new SaveFailedException(ex);
				}
			} catch (Exception ex) {
				throw new LoadFailedException(ex);
			}
			return null;
		}
		/// <summary>
		/// Runs the plugboard randomizer.
		/// </summary>
		/// 
		/// <exception cref="SaveFailedException">
		/// Failed to save the new plugboard steckering.
		/// </exception>
		public void RandomizePlugboard(LetterSet letterSet) {
			if (letterSet == null)
				throw new LetterSetMissingException("Cannot configure Plugboard without a loaded letterset!");
			try {
				Steckering newSteckering = PlugboardIO.Generate(letterSet.Count);
				SaveToFile(newSteckering, File);
				Steckering = newSteckering;
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
			Steckering = PlugboardIO.Read(letterSet.Count, plugboardFile);
		}
		/// <summary>
		/// Writes the input plugboard steckering to the file.
		/// </summary>
		/// <param name="steckering">The new steckering to save.</param>
		/// <param name="plugboardFile">The plugboard file to save the steckering to.</param>
		private void SaveToFile(Steckering steckering, string plugboardFile) {
			Directory.CreateDirectory(Path.GetDirectoryName(plugboardFile));
			PlugboardIO.Write(steckering, plugboardFile);
		}

		#endregion
	}
}
