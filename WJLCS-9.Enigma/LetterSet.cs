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
				throw new ArgumentException($"{nameof(letters)}.{nameof(letters.Length)} is zero!", nameof(letters));
			HashSet<char> usedLetters = new HashSet<char>();
			foreach (char c in letters) {
				if (!usedLetters.Add(c))
					throw new ArgumentException($"Letter '{c}' has already been used!", nameof(letters));
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
			int rotate = 1 + letters.Length % 31;
			for (int i = 0; i < letters.Length; i++) {
				hash ^= letters[i];
				hash = BitRotating.RotateLeft(hash, rotate);
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
	}
}
