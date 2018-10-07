using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WJLCS.Enigma.Utils;

namespace WJLCS.Enigma {
	public class Steckering : IReadOnlyList<int> {

		#region Fields

		private readonly int[] steckering;
		private readonly int hash;

		#endregion

		#region Constructors

		public Steckering(int[] steckering) {
			if (steckering == null)
				throw new ArgumentNullException(nameof(steckering));
			if (steckering.Length == 0)
				throw new ArgumentException(nameof(steckering));
			HashSet<int> usedIndexes = new HashSet<int>();
			for (int inputIndex = 0; inputIndex < steckering.Length; inputIndex++) {
				int outputIndex = steckering[inputIndex];
				if (!usedIndexes.Add(outputIndex))
					throw new ArgumentException(nameof(steckering));
				else if (outputIndex < 0 || outputIndex >= steckering.Length)
					throw new ArgumentOutOfRangeException(nameof(steckering));
			}
			this.steckering = new int[steckering.Length];
			Array.Copy(steckering, this.steckering, steckering.Length);
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

		public int Count => steckering.Length;

		public int this[int index] => steckering[index];

		#endregion

		#region Object Overrides

		public override int GetHashCode() => hash;

		#endregion

		#region IReadOnlyList Methods

		public int IndexOf(int outputIndex) {
			return Array.IndexOf(steckering, outputIndex);
		}

		public bool Contains(int outputIndex) {
			return IndexOf(outputIndex) != -1;
		}

		public int[] ToArray() {
			int[] newSteckering = new int[steckering.Length];
			Array.Copy(steckering, newSteckering, steckering.Length);
			return newSteckering;
		}

		public IEnumerator<int> GetEnumerator() {
			return steckering.Cast<int>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion

		#region Steckering

		public Steckering Reverse() {
			int[] reverseSteckering = new int[steckering.Length];
			for (int i = 0; i < steckering.Length; i++) {
				reverseSteckering[i] = Array.IndexOf(steckering, i);
			}
			return new Steckering(reverseSteckering);
		}

		#endregion
	}
}
