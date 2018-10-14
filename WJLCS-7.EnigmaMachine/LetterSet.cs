using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WJLCS.Enigma.Utils;

namespace WJLCS.Enigma {
	/// <summary>
	/// A set of usable letters in the Enigma Machine.
	/// </summary>
	public class LetterSet : IReadOnlyList<char> {

		#region Fields
		
		/// <summary>
		/// The array of letters.
		/// </summary>
		private readonly char[] letters;
		/// <summary>
		/// The precalculated hash.
		/// </summary>
		private readonly int hash;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="LetterSet"/>
		/// </summary>
		/// <param name="letters">The array of usable characters.</param>
		public LetterSet(char[] letters) {
			if (letters == null)
				throw new ArgumentNullException(nameof(letters));
			if (letters.Length == 0)
				throw new ArgumentException(nameof(letters));
			HashSet<char> usedLetters = new HashSet<char>();
			foreach (char c in letters) {
				if (!usedLetters.Add(c))
					throw new ArgumentException(nameof(letters));
			}
			this.letters = new char[letters.Length];
			Array.Copy(letters, this.letters, letters.Length);
			hash = CalculateHash();
		}
		/// <summary>
		/// Calculates the hash code for the immutable steckering.
		/// </summary>
		/// <returns>The calculated hash code.</returns>
		private int CalculateHash() {
			int hash = letters.Length;
			int rotate = 1 + letters.Length % 30;
			for (int i = 0; i < letters.Length; i++) {
				hash ^= letters[i];
				hash = BitRotating.RotateRight(hash, rotate);
			}
			return hash;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of letters in the letterset.
		/// </summary>
		public int Count => letters.Length;
		/// <summary>
		/// Gets the character at the specified index in the letterset.
		/// </summary>
		/// <param name="index">The index of the character.</param>
		/// <returns>The character at the specified index.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		public char this[int index] => letters[index];

		#endregion

		#region Object Overrides

		/// <summary>
		/// Gets the hashcode for the letterset.
		/// </summary>
		public override int GetHashCode() => hash;

		#endregion

		#region IReadOnlyList Methods

		/// <summary>
		/// Gets the index of the specified character in the letterset.
		/// </summary>
		/// <param name="c">The character to get the index of.</param>
		/// <returns>The index of the character. Or -1 if it does not exist.</returns>
		public int IndexOf(char c) {
			return Array.IndexOf(letters, c);
		}
		/// <summary>
		/// Gets if this letterset contains the specified character.
		/// </summary>
		/// <param name="c">The character to check for.</param>
		/// <returns>True if the character exists.</returns>
		public bool Contains(char c) {
			return IndexOf(c) != -1;
		}
		/// <summary>
		/// Casts the letterset to a character array.
		/// </summary>
		public char[] ToArray() {
			char[] newLetters = new char[letters.Length];
			Array.Copy(letters, newLetters, letters.Length);
			return newLetters;
		}
		/// <summary>
		/// Gets the enumerator of the letterset.
		/// </summary>
		public IEnumerator<char> GetEnumerator() => letters.Cast<char>().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() =>GetEnumerator();

		#endregion

		#region Steckering

		/// <summary>
		/// Creates randomized steckering.
		/// </summary>
		/// <param name="seed">If nonzero, the seed will be used for RNG.</param>
		/// <returns>Returns the created <see cref="Steckering"/>.</returns>
		public Steckering RandomizeSteckering(int seed = 0) {
			Random random = (seed == 0 ? new Random() : new Random(seed));
			int[] steckering = new int[Count];
			// Use the label's hash code as the seed
			//List<char> remainingOutputs = new List<char>(letters);
			List<int> remainingOutputs = new List<int>(Count);
			for (int i = 0; i < Count; i++)
				remainingOutputs.Add(i);
			for (int i = 0; i < Count; i++) {
				int input = remainingOutputs[0];
				if (Count - i == 2) {
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
