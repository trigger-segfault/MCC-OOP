using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WJLCS {
	/// <summary>
	/// The central wheel collection that operates the in-use wheels.
	/// </summary>
	public interface ICentralWheelCollection {

		#region Properties

		/// <summary>
		/// Gets the central wheels used by the machine.
		/// </summary>
		ReadOnlyCollection<ICentralWheel> Wheels { get; }

		#endregion

		#region Enciphering

		/// <summary>
		/// Enciphers the input character and changes the wheels' positions.
		/// </summary>
		/// <param name="input">The character being input into the wheel collection.</param>
		/// <param name="enciphered">The output enciphered character.</param>
		/// <param name="deciphered">The output deciphered character.</param>
		void Encipher(char input, out char enciphered, out char deciphered);

		/// <summary>
		/// Enciphers the input character without changing the wheel positions.
		/// </summary>
		/// <param name="input">The character being input into the wheel collection.</param>
		/// <param name="enciphered">The output enciphered character.</param>
		/// <param name="deciphered">The output deciphered character.</param>
		void Peek(char input, out char enciphered, out char deciphered);
		
		#endregion

		#region Setup

		/// <summary>
		/// Sets up the central wheels based on the supplied wheel states.
		/// </summary>
		/// <param name="rewiredCharacters">The rewired characters to use.</param>
		/// <remarks>
		/// Characters that are self-steckered do not need to be included.
		/// </remarks>
		void Setup(IEnumerable<WheelSetup> wheels);

		#endregion
	}
}
