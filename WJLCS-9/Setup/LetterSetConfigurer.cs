using System;
using System.IO;
using WJLCS.Enigma;
using WJLCS.Enigma.IO;

namespace WJLCS.Setup {
	/// <summary>
	/// The class for managing letterset setup.
	/// </summary>
	public class LetterSetConfigurer {

		#region Properties

		/// <summary>
		/// Gets the loaded letterset.
		/// </summary>
		public LetterSet LetterSet { get; private set; }
		/// <summary>
		/// Gets the path to the letterset file.
		/// </summary>
		public string File { get; private set; } = "Files/Letterset.txt";

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the Enigma Machine <see cref="LetterSetConfigurer"/>.
		/// </summary>
		public LetterSetConfigurer() {
			try {
				if (System.IO.File.Exists(File))
					LoadFromFile(File);
			} catch { }
		}

		#endregion

		#region Configure

		/// <summary>
		/// Runs the letterset configurer.
		/// </summary>
		/// 
		/// <exception cref="FileNotFoundException">
		/// The input file was not found.
		/// </exception>
		/// <exception cref="LoadFailedException">
		/// An error occurred while loading the letterset.
		/// </exception>
		public void ConfigureLetterSet(string input) {
			string file = input;
			try {
				if (!System.IO.File.Exists(file))
					throw new FileNotFoundException($"Input file \"{file}\" does not exist!");
			} catch (Exception ex) {
				throw new FileNotFoundException($"Input file \"{file}\" does not exist!\n" + ex.Message, ex);
			}
			LetterSet oldLetterSet = LetterSet;
			try {
				LoadFromFile(file);
				try {
					SaveToFile(LetterSet, File);
				} catch (Exception ex) {
					LetterSet = oldLetterSet;
					throw new SaveFailedException(ex);
				}
			} catch (Exception ex) {
				throw new LoadFailedException(ex);
			}
		}

		#endregion

		#region File IO

		/// <summary>
		/// Reads the file and returns the <see cref="LetterSet"/>.
		/// </summary>
		/// <param name="letterSetFile">The file containing the letterset.</param>
		/// 
		/// <exception cref="Exception">
		/// A parsed letter is invalid.
		/// </exception>
		private void LoadFromFile(string letterSetFile) {
			LetterSet = LetterSetIO.Read(letterSetFile);
		}
		/// <summary>
		/// Writes the input letterset to the file.
		/// </summary>
		/// <param name="rotorCount">The new number of rotors to save.</param>
		/// <param name="letterSetFile">The letterset file to save the letters to.</param>
		private void SaveToFile(LetterSet letterSet, string letterSetFile) {
			Directory.CreateDirectory(Path.GetDirectoryName(letterSetFile));
			LetterSetIO.Write(letterSet, letterSetFile);
		}

		#endregion
	}
}
