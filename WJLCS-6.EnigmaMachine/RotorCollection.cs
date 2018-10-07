using System;
using System.Collections.Generic;
using System.Linq;

namespace WJLCS.Enigma {
	public class RotorCollection {

		#region Static Fields
		
		private static readonly int[] PrimeNumbers;

		#endregion

		#region Initializer

		static RotorCollection() {
			var primeFinder = new PrimeLibrary.PrimeFinder();
			PrimeNumbers = primeFinder.GeneratePrimes(0, 1000).ToArray();
		}

		#endregion

		#region Fields

		private readonly LetterSet letterSet;
		private readonly List<Rotor> rotors;

		#endregion

		#region Constructors

		public RotorCollection(LetterSet letterSet, int rotorCount) {
			if (rotorCount < 1)
				throw new ArgumentException(nameof(rotorCount));
			this.letterSet = letterSet ?? throw new ArgumentNullException(nameof(letterSet));
			rotors = new List<Rotor>();
			for (int i = 0; i < rotorCount; i++) {
				rotors.Add(new Rotor(letterSet, PrimeNumbers[i]));
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
			for (int i = 0; i < rotors.Count; i++) {
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
			for (int i = rotors.Count - 1; i >= 0; i--) {
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
			for (int i = 0; i < rotors.Count; i++) {
				if (!rotors[i].Rotate())
					break;
			}
		}

		#endregion
	}
}
