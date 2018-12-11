using System;

namespace WJLCS.Enigma {
	/// <summary>
	/// The plugboard for remapping characters.
	/// </summary>
	internal class Plugboard {

		#region Fields
		
		/// <summary>
		/// The steckering used for the plugboard.
		/// </summary>
		private readonly Steckering steckering;

		#endregion

		#region Constructors
		
		/// <summary>
		/// Constructs the <see cref="Plugboard"/>.
		/// </summary>
		/// <param name="letterSet">The letterset to compare to the steckering.</param>
		/// <param name="steckering">The steckering to use.</param>
		public Plugboard(LetterSet letterSet, Steckering steckering) {
			if (letterSet == null)
				throw new ArgumentNullException(nameof(letterSet));
			this.steckering = steckering ?? throw new ArgumentNullException(nameof(steckering));
			if (steckering.Count != letterSet.Count)
				throw new ArgumentException($"Letterset and Steckering count do not match!\n" +
											$"Letterset: {letterSet.Count}, Steckering: {steckering.Count}",
											nameof(steckering));
		}

		#endregion

		#region Enciphering

		/// <summary>
		/// Enciphers the input character using the plugboard steckering.
		/// </summary>
		/// <param name="inputIndex">The character index being input through the plugboard.</param>
		/// <returns>The enciphered character index.</returns>
		public int Encipher(int inputIndex) {
			return steckering[inputIndex, false];
		}
		/// <summary>
		/// Deciphers the input character using the plugboard steckering.
		/// </summary>
		/// <param name="inputIndex">The character index being input through the plugboard.</param>
		/// <returns>The deciphered character index.</returns>
		public int Decipher(int inputIndex) {
			return steckering[inputIndex, true];
		}

		#endregion
	}
}
