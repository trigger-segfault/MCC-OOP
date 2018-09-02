using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WJLCS {
	/// <summary>
	/// A central wheel in use by the <see cref="IEnigmaMachine"/>.
	/// </summary>
	public interface ICentralWheel {

		#region Properties

		/// <summary>
		/// Gets the wheel information about the wheel being used.
		/// </summary>
		IWheel Wheel { get; }

		/// <summary>
		/// Gets or sets the current position for the wheel.
		/// </summary>
		int Position { get; set; }

		/// <summary>
		/// Gets or sets the turnover position for the wheel.
		/// </summary>
		int TurnoverPosition { get; set; }

		#endregion

		#region Enciphering

		/// <summary>
		/// Enciphers the input character using this wheel.
		/// </summary>
		/// <param name="input">The character being input into the wheel.</param>
		/// <param name="enciphered">The output enciphered character.</param>
		/// <param name="deciphered">The output deciphered character.</param>
		/// <returns>True if the turnover point was reached.</returns>
		bool Encipher(char input, out char enciphered, out char deciphered);

		#endregion
	}
}
