using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WJLCS.Enigma;

namespace WJLCS.Enigma.IO {
	/// <summary>
	/// Static I/O methods for reading and writing rotor key files.
	/// </summary>
	public static class RotorIO {

		#region File IO

		/// <summary>
		/// Reads the file and returns the rotor count.
		/// </summary>
		/// <param name="rotorKeysFile">The file containing the rotor keys.</param>
		/// 
		/// <exception cref="LoadFailedException">
		/// A file has invalid formatting, a parsed key was invalid, or not an in-range prime number.
		/// </exception>
		public static RotorKeys Read(string rotorKeysFile) {
			try {
				string[] lines = File.ReadAllLines(rotorKeysFile);
				var keys = lines.Select(l => int.Parse(l));
				return new RotorKeys(keys.ToArray(), false);
			}
			catch (Exception ex) {
				throw new LoadFailedException($"Failed to load the Rotor Keys file!\n{ex.Message}");
			}
		}
		/// <summary>
		/// Writes the input rotor count to the file.
		/// </summary>
		/// <param name="rotorKeys">The list of rotor prime number keys.</param>
		/// <param name="rotorKeysFile">The rotor count file to save the count to.</param>
		/// 
		/// <exception cref="SaveFailedException">
		/// An error occurred while saving the file.
		/// </exception>
		public static void Write(RotorKeys rotorKeys, string rotorKeysFile) {
			try {
				Directory.CreateDirectory(Path.GetDirectoryName(rotorKeysFile));
				string text = string.Join(Environment.NewLine, rotorKeys);
				File.WriteAllText(rotorKeysFile, text);
			}
			catch (Exception ex) {
				throw new SaveFailedException($"Failed to save the Rotor Keys file!\n{ex.Message}");
			}
		}

		#endregion

		#region Generate

		/// <summary>
		/// Creates randomized rotor keys.
		/// </summary>
		/// <param name="rotorCount">The number of rotors to use.</param>
		/// <param name="seed">If non-null, the seed will be used for RNG.</param>
		/// <returns>Returns the created <see cref="RotorKeys"/>.</returns>
		public static RotorKeys Generate(int rotorCount, int? seed = null) {
			if (rotorCount < 1)
				throw new ArgumentOutOfRangeException(nameof(rotorCount));
			Random random = (seed.HasValue ? new Random(seed.Value) : new Random());
			int[] keyIndexes = new int[rotorCount];
			for (int i = 0; i < rotorCount; i++) {
				// Get a random index between 0 and the total number of keys.
				keyIndexes[i] = random.Next(RotorKeys.TotalKeyCount);
			}
			return new RotorKeys(keyIndexes, true);
		}

		#endregion
	}
}
