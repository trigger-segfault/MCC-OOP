using System.Collections.Generic;

namespace WJLCS {
	/// <summary>The wiring board with the initial remapping of characters.</summary>
	public interface ISteckerboard {

		#region Testing

		/// <summary>
		/// Gets if the character is self-steckered, and not remapped.
		/// </summary>
		/// <param name="input">The input character to check.</param>
		/// <returns>True if the character maps to itself.</returns>
		bool IsSelfSteckered(char input);

		#endregion

		#region Enciphering
		
		/// <summary>
		/// Enciphers the input character.
		/// </summary>
		/// <param name="input">The character being input into the steckerboard.</param>
		/// <param name="enciphered">The output enciphered character.</param>
		/// <param name="deciphered">The output deciphered character.</param>
		void Encipher(char input, out char enciphered, out char deciphered);

		#endregion

		#region Setup

		/// <summary>
		/// Sets up the steckerboard to use the specified rewired characters.
		/// </summary>
		/// <param name="rewiredCharacters">The rewired characters to use.</param>
		/// <remarks>
		/// Characters that are self-steckered do not need to be included.
		/// </remarks>
		void Setup(IEnumerable<KeyValuePair<char, char>> rewiredCharacters);

		#endregion
	}
}
