
namespace WJLCS.Enigma {
	/// <summary>
	/// A single live rotor in a rotor collection.
	/// </summary>
	internal class Rotor {
		
		#region Constructors

		/// <summary>
		/// Constructs the <see cref="Rotor"/>.
		/// </summary>
		/// <param name="letterSet">The letterset to get the count from.</param>
		/// <param name="key">The prime number key.</param>
		public Rotor(LetterSet letterSet, int key) {
			Base = letterSet.Count;
			InitialOffset = key - (Base * (key / Base));
			Offset = InitialOffset;
		}

		#endregion

		#region Private Properties
		
		/// <summary>
		/// Gets the length of the letterset.
		/// </summary>
		private int Base { get; }

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
