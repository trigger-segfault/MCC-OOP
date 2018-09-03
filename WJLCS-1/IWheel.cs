
namespace WJLCS {
	/// <summary>
	/// A wheel used by the <see cref="IEnigmaMachine"/> to encipher characters.
	/// </summary>
	public interface IWheel {

		#region Properties

		/// <summary>
		/// Gets the name used to identify this wheel.
		/// </summary>
		string Label { get; }

		#endregion

		#region Enciphering
		
		/// <summary>
		/// Enciphers the input character using this wheel.
		/// </summary>
		/// <param name="position">The current rotation of the wheel.</param>
		/// <param name="input">The character being input into the wheel.</param>
		/// <param name="enciphered">The output enciphered character.</param>
		/// <param name="deciphered">The output deciphered character.</param>
		/// <returns>True if the turnover point was reached.</returns>
		void Encipher(int position, char input, out char enciphered, out char deciphered);

		#endregion

		#region Equality

		/// <summary>
		/// Enciphers the input character.
		/// </summary>
		/// <param name="input">The character being input into the wheel.</param>
		/// <param name="enciphered">The output enciphered character.</param>
		/// <param name="deciphered">The output deciphered character.</param>
		void Encipher(char input, out char enciphered, out char deciphered);

		#endregion
	}
}
