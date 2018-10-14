using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WJLCS.Enigma.Utils;

namespace WJLCS.Enigma {
	/// <summary>
	/// A set of remapped indexes for use with the Enigma Machine.
	/// </summary>
	public class Steckering : IReadOnlyList<int> {

		#region Fields

		/// <summary>
		/// The array of remapped indexes.
		/// </summary>
		private readonly int[] steckering;
		/// <summary>
		/// The reverse array of remapped indexes.
		/// </summary>
		private readonly int[] decipherSteckering;
		/// <summary>
		/// The precalculated hash.
		/// </summary>
		private readonly int hash;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="Steckering"/>.
		/// </summary>
		/// <param name="steckering">The array of remapped indexes.</param>
		public Steckering(int[] steckering) {
			if (steckering == null)
				throw new ArgumentNullException(nameof(steckering));
			if (steckering.Length == 0)
				throw new ArgumentException(nameof(steckering));
			//HashSet<int> usedIndexes = new HashSet<int>();
			bool[] usedIndexes = new bool[steckering.Length];
			for (int inputIndex = 0; inputIndex < steckering.Length; inputIndex++) {
				int outputIndex = steckering[inputIndex];
				if (usedIndexes[outputIndex])
				//if (!usedIndexes.Add(outputIndex))
					throw new ArgumentException(nameof(steckering));
				if (outputIndex < 0 || outputIndex >= steckering.Length)
					throw new ArgumentOutOfRangeException(nameof(steckering));
				usedIndexes[outputIndex] = true;
			}
			this.steckering = new int[steckering.Length];
			Array.Copy(steckering, this.steckering, steckering.Length);
			decipherSteckering = new int[steckering.Length];
			for (int i = 0; i < steckering.Length; i++) {
				decipherSteckering[i] = Array.IndexOf(steckering, i);
			}
			hash = CalculateHash();
		}
		/// <summary>
		/// Calculates the hash code for the immutable steckering.
		/// </summary>
		/// <returns>The calculated hash code.</returns>
		private int CalculateHash() {
			int hash = steckering.Length;
			int rotate = 1 + steckering.Length % 31;
			for (int i = 0; i < steckering.Length; i++) {
				hash ^= steckering[i];
				hash = BitRotating.RotateLeft(hash, rotate);
			}
			return hash;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of remapped indexes in the steckering.
		/// </summary>
		public int Count => steckering.Length;
		/// <summary>
		/// Gets the remapped index at the specified index.
		/// </summary>
		/// <param name="index">The index to get the remapped index at.</param>
		/// <returns>The new index.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		public int this[int index] => steckering[index];
		/// <summary>
		/// Gets index of the remapped index or vice versa.
		/// </summary>
		/// <param name="index">The index to get the remapped index of.</param>
		/// <param name="decipher">
		/// True if the input index is the remapped index and the output index is the original index.
		/// </param>
		/// <returns>The remapped index.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		public int this[int index, bool decipher] => (decipher ? decipherSteckering : steckering)[index];

		#endregion

		#region Object Overrides

		/// <summary>
		/// Gets the hash code for the steckering.
		/// </summary>
		public override int GetHashCode() => hash;

		#endregion

		#region IReadOnlyList Methods

		/// <summary>
		/// Gets the index of the remap index.
		/// </summary>
		/// <param name="outputIndex">The remap index to get the index of.</param>
		/// <returns></returns>
		public int IndexOf(int outputIndex) {
			return Array.IndexOf(steckering, outputIndex);
		}
		/// <summary>
		/// Returns true if the steckering contains the specified remap index.
		/// </summary>
		/// <param name="outputIndex">The remap index to check for.</param>
		/// <returns>True if the remap index exists.</returns>
		public bool Contains(int outputIndex) {
			return IndexOf(outputIndex) != -1;
		}
		/// <summary>
		/// Casts the steckering to an integer array.
		/// </summary>
		public int[] ToArray() {
			int[] newSteckering = new int[steckering.Length];
			Array.Copy(steckering, newSteckering, steckering.Length);
			return newSteckering;
		}
		/// <summary>
		/// Gets the enumerator for the steckering.
		/// </summary>
		public IEnumerator<int> GetEnumerator() => steckering.Cast<int>().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		/*#region Steckering

		/// <summary>
		/// Creates a steckering in the reverse direction.
		/// </summary>
		/// <returns>A copy of the steckering in reverse.</returns>
		public Steckering Reverse() {
			int[] reverseSteckering = new int[steckering.Length];
			for (int i = 0; i < steckering.Length; i++) {
				reverseSteckering[i] = Array.IndexOf(steckering, i);
			}
			return new Steckering(reverseSteckering);
		}

		#endregion*/
	}
}
