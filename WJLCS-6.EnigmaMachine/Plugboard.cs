using System;

namespace WJLCS.Enigma {
	public class Plugboard {

		#region Fields

		private readonly LetterSet letterSet;
		private readonly Steckering steckering;
		private readonly Steckering reverseSteckering;

		#endregion

		#region Constructors

		public Plugboard(LetterSet letterSet, Steckering steckering) {
			this.letterSet = letterSet ?? throw new ArgumentNullException(nameof(letterSet));
			this.steckering = steckering ?? throw new ArgumentNullException(nameof(steckering));
			if (steckering.Count != letterSet.Count)
				throw new ArgumentException(nameof(steckering));
			reverseSteckering = steckering.Reverse();
			/*reverseSteckering = new int[steckering.Length];
			for (int i = 0; i < steckering.Length; i++) {
				reverseSteckering[i] = Array.IndexOf(steckering, i);
			}*/
		}

		#endregion

		#region Enciphering

		/// <summary>
		/// Enciphers the input character using the plugboard steckering.
		/// </summary>
		/// <param name="inputIndex">The character index being input through the plugboard.</param>
		/// <returns>The enciphered character index.</returns>
		public int Encipher(int inputIndex) {
			return steckering[inputIndex];
		}

		/// <summary>
		/// Deciphers the input character using the plugboard steckering.
		/// </summary>
		/// <param name="inputIndex">The character index being input through the plugboard.</param>
		/// <returns>The deciphered character index.</returns>
		public int Decipher(int inputIndex) {
			return reverseSteckering[inputIndex];
		}

		#endregion
	}
}
