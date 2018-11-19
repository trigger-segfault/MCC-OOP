using System;
using System.Collections.Generic;
using System.Linq;

namespace WJLCS.Enigma {
	/// <summary>
	/// The collection of rotors that handles enciphering and deciphering through them.
	/// </summary>
	internal class RotorCollection {

		#region Fields
		
		/// <summary>
		/// The array of rotors in the collection.
		/// </summary>
		private readonly Rotor[] rotors;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="RotorCollection"/>.
		/// </summary>
		/// <param name="letterSet">The letterset used by the rotors for calculation.</param>
		/// <param name="rotorKeys">
		/// The list of prime number keys describing the offset and number of rotors.
		/// </param>
		public RotorCollection(LetterSet letterSet, RotorKeys rotorKeys) {
			if (letterSet == null)
				throw new ArgumentNullException(nameof(letterSet));
			if (rotorKeys == null)
				throw new ArgumentNullException(nameof(rotorKeys));
			rotors = new Rotor[rotorKeys.Count];
			for (int i = 0; i < rotorKeys.Count; i++) {
				rotors[i] = new Rotor(letterSet, rotorKeys[i]);
			}
		}

		#endregion

		#region Enciphering

		/// <summary>
		/// Enciphers the input character using the rotors.
		/// </summary>
		/// <param name="inputIndex">The character index being input into the rotor.</param>
		/// <param name="peek">True if the rotors should not be rotated.</param>
		/// <returns>The enciphered character index.</returns>
		public int Encipher(int inputIndex, bool peek = false) {
			for (int i = 0; i < rotors.Length; i++) {
				inputIndex = rotors[i].Encipher(inputIndex);
			}
			if (!peek)
				Rotate();
			return inputIndex;
		}
		/// <summary>
		/// Deciphers the input character using this rotors.
		/// </summary>
		/// <param name="inputIndex">The character index being input into the rotor.</param>
		/// <param name="peek">True if the rotors should not be rotated.</param>
		/// <returns>The deciphered character index.</returns>
		public int Decipher(int inputIndex, bool peek = false) {
			for (int i = rotors.Length - 1; i >= 0; i--) {
				inputIndex = rotors[i].Decipher(inputIndex);
			}
			if (!peek)
				Rotate();
			return inputIndex;
		}
		/// <summary>
		/// Resets the rotors' offsets to the initial offset.
		/// </summary>
		public void Reset() {
			foreach (Rotor rotor in rotors) {
				rotor.Reset();
			}
		}
		/// <summary>
		/// Rotates the rotors
		/// </summary>
		public void Rotate() {
			for (int i = 0; i < rotors.Length; i++) {
				if (!rotors[i].Rotate())
					break;
			}
		}

		#endregion
	}
}
