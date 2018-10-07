using System;
using System.Collections.Generic;
using System.Text;

namespace WJLCS.Enigma {
	public class Rotor {

		#region Fields

		private LetterSet letterSet;

		#endregion

		#region Constructors

		public Rotor(LetterSet letterSet, int key) {
			Key = key;
			this.letterSet = letterSet;
			InitialOffset = key - (Base * (Key / Base));
			Offset = InitialOffset;
		}

		#endregion

		#region Private Properties
		
		private int Base => letterSet.Count;
		private int Key { get; }

		#endregion

		#region Properties

		/// <summary>
		/// Gets the ofset of the rotor.
		/// </summary>
		public int Offset { get; private set; }

		/// <summary>
		/// The initial offset of the rotor.
		/// </summary>
		public int InitialOffset { get; }

		#endregion

		#region Enciphering

		/// <summary>
		/// Enciphers the input character using this rotor.
		/// </summary>
		/// <param name="inputIndex">The character index being input into the rotor.</param>
		/// <returns>The enciphered character index.</returns>
		public int Encipher(int inputIndex) {
			return (inputIndex + Offset) % Base;
		}

		/// <summary>
		/// Deciphers the input character using this rotor.
		/// </summary>
		/// <param name="inputIndex">The character index being input into the rotor.</param>
		/// <returns>The deciphered character index.</returns>
		public int Decipher(int inputIndex) {
			// + Base to prevent negative modulus result.
			return ((inputIndex - Offset) + Base) % Base;
		}

		/// <summary>
		/// Rotates the wheel offset.
		/// </summary>
		/// <returns>True if the turnover point was reached.</returns>
		public bool Rotate() {
			Offset = (Offset + 1) % Base;
			// Turnover position reached?
			return (Offset == InitialOffset);
		}

		/// <summary>
		/// Resets the rotor's offset to the initial offset.
		/// </summary>
		public void Reset() {
			Offset = InitialOffset;
		}

		#endregion
	}
}
