using System;
using System.Collections.Generic;
using System.IO;

namespace WJLCS.Enigma.IO {
	/// <summary>
	/// Static I/O methods for reading and writing plugboard steckering files.
	/// </summary>
	public static class PlugboardIO {

		#region File IO

		/// <summary>
		/// Reads the file and returns the plugboard steckering.
		/// </summary>
		/// <param name="letterCount">The number of letters in the letterset..</param>
		/// <param name="plugboardFile">The file containing the plugboard setup.</param>
		/// 
		/// <exception cref="FileNotFoundException">
		/// The Plugboard file was not found.
		/// </exception>
		/// <exception cref="LoadFailedException">
		/// An error occurred while loading the file.<para/>
		/// I.E. The file has invalid formatting, a parsed letter was invalid, or mismatched characters.
		/// </exception>
		public static Steckering Read(int letterCount, string plugboardFile) {
			try {
				string[] lines = File.ReadAllLines(plugboardFile);

				int index = 0;
				int[] steckering = new int[letterCount];
				// Keep track of the indexes that are consumed.
				bool[] usedIndexes = new bool[steckering.Length];
				foreach (string line in lines) {
					// Empty line, we done bois!
					if (line.Length == 0)
						break;
					int newIndex = int.Parse(line.Trim());
					if (newIndex >= steckering.Length)
						throw new ArgumentOutOfRangeException($"Index of {newIndex} is greater than or " +
															  $"equal to size of letterset!");
					if (usedIndexes[newIndex])
						throw new Exception($"Index of {newIndex} has already been used!");
					usedIndexes[newIndex] = true;
					steckering[index] = newIndex;
					index++;
				}
				if (index != steckering.Length)
					throw new Exception($"Insufficient number of plugboard indexes to match letterset ({index})!");

				return new Steckering(steckering);
			}
			catch (FileNotFoundException) {
				throw;
			}
			catch (Exception ex) {
				throw new LoadFailedException($"Failed to load the Plugboard file!\n{ex.Message}");
			}
		}
		/// <summary>
		/// Writes the input plugboard steckering to the file.
		/// </summary>
		/// <param name="steckering">The new steckering to save.</param>
		/// <param name="plugboardFile">The plugboard file to save the steckering to.</param>
		/// 
		/// <exception cref="SaveFailedException">
		/// An error occurred while saving the file.
		/// </exception>
		public static void Write(Steckering steckering, string plugboardFile) {
			try {
				Directory.CreateDirectory(Path.GetDirectoryName(plugboardFile));
				string text = string.Join(Environment.NewLine, steckering);
				File.WriteAllText(plugboardFile, text);
			}
			catch (Exception ex) {
				throw new SaveFailedException($"Failed to save the Plugboard file!\n{ex.Message}");
			}
		}

		#endregion

		#region Steckering

		/// <summary>
		/// Creates randomized steckering.
		/// </summary>
		/// <param name="letterCount">The number of letters in the letterset.</param>
		/// <param name="seed">If non-null, the seed will be used for RNG.</param>
		/// <returns>Returns the created <see cref="Steckering"/>.</returns>
		public static Steckering Generate(int letterCount, int? seed = null) {
			if (letterCount < 1)
				throw new ArgumentOutOfRangeException(nameof(letterCount));
			Random random = (seed.HasValue ? new Random(seed.Value) : new Random());
			int[] steckering = new int[letterCount];
			List<int> remainingOutputs = new List<int>(letterCount);
			for (int i = 0; i < letterCount; i++)
				remainingOutputs.Add(i);
			for (int i = 0; i < letterCount; i++) {
				int input = remainingOutputs[0];
				if (letterCount - i == 2) {
					// Last two characters: Do this right so we don't end up stuck
					// with the last character being equal to the input character.
					int index = 0;
					if (i == remainingOutputs[0])
						index = 1;
					steckering[i] = remainingOutputs[index];
					steckering[i + 1] = remainingOutputs[1 - index];
					break;
				}
				else {
					// Avoid encountering the same character and having to repeat the RNG.
					int invalidIndex = remainingOutputs.IndexOf(input);
					int max = remainingOutputs.Count - (invalidIndex != -1 ? 1 : 0);

					// Pick a random output index from the remaining characters
					int outputIndex = random.Next(max);

					// Skip the index of the input character
					if (outputIndex >= invalidIndex)
						outputIndex++;

					steckering[i] = remainingOutputs[outputIndex];
					remainingOutputs.RemoveAt(outputIndex);
				}
			}
			return new Steckering(steckering);
		}

		#endregion
	}
}
