using System.Collections.ObjectModel;

namespace WJLCS {
	/// <summary>
	/// The Enigma Machine that enciphers and deciphers text.
	/// </summary>
	public interface IEnigmaMachine {

		#region Properties

		/// <summary>
		/// Gets the central (in-use) wheels that encipher the characters.
		/// </summary>
		ICentralWheelCollection CentralWheels { get; }

		/// <summary>
		/// Gets the collection of wheels available for use.
		/// </summary>
		ReadOnlyCollection<IWheel> AvailableWheels { get; }

		/// <summary>
		/// Gets the steckerboard used to scramble the initial wiring.
		/// </summary>
		ISteckerboard Steckerboard { get; }

		/// <summary>
		/// Gets the available characters for use with this machine.
		/// </summary>
		ReadOnlyCollection<char> AvailableCharacters { get; }

		/// <summary>
		/// Gets the currently enciphered text to be sent.
		/// </summary>
		string EncipheredText { get; }

		/// <summary>
		/// Gets the currently deciphered text to be read.
		/// </summary>
		string DecipheredText { get; }

		/// <summary>
		/// Gets the length of the <see cref="EncipheredText"/> and <see cref="DecipheredText"/>.
		/// </summary>
		int Length { get; }

		/// <summary>
		/// The state of the Enigma Machine when being reset.
		/// </summary>
		EnigmaSetup SetupState { get; }

		#endregion

		#region Enciphering

		/// <summary>
		/// Enciphers the input string and then resets the machine state.
		/// </summary>
		/// <param name="input">The characters being input into the Enigma Machine.</param>
		/// <param name="enciphered">The output enciphered string.</param>
		/// <param name="deciphered">The output deciphered string.</param>
		void Encipher(string input, out string enciphered, out string deciphered);

		/// <summary>
		/// Enciphers the input character and changes the wheels' positions.
		/// </summary>
		/// <param name="input">The character being input into the Enigma Machine.</param>
		/// <param name="enciphered">The output enciphered character.</param>
		/// <param name="deciphered">The output deciphered character.</param>
		void Encipher(char input, out char enciphered, out char deciphered);

		/// <summary>
		/// Enciphers the input character without changing the wheel positions.
		/// </summary>
		/// <param name="input">The character being input into the Enigma Machine.</param>
		/// <param name="enciphered">The output enciphered character.</param>
		/// <param name="deciphered">The output deciphered character.</param>
		void Peek(char input, out char enciphered, out char deciphered);

		#endregion

		#region Setup

		/// <summary>
		/// Sets up the Enigma Machine using the new state.
		/// </summary>
		/// <param name="setup">The setup state to use.</param>
		void Setup(EnigmaSetup setup);

		/// <summary>
		/// Resets the Engima machine's state, by returning the wheel positions and clearing the text.
		/// </summary>
		void Reset();

		#endregion
	}
}
