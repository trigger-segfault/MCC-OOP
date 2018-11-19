using WJLCS.Enigma;

namespace WJLCS.Setup {
	/// <summary>
	/// The wrapper class for using the Enigma Machine to encipher and decipher.
	/// </summary>
	public class Encipherer {

		#region Enciphering

		/// <summary>
		/// Runs the Enigma Machine encipherer and copies the output to the clipboard.
		/// </summary>
		/// <returns>The enciphered string.</returns>
		public string Encipher(Machine machine, string input) {
			string enciphered = machine.Encipher(input);
			TextCopy.Clipboard.SetText(enciphered);
			return enciphered;
		}
		/// <summary>
		/// Runs the Enigma Machine decipherer and copies the output to the clipboard.
		/// </summary>
		/// <returns>The decipherer string.</returns>
		public string Decipher(Machine machine, string input) {
			string deciphered = machine.Decipher(input);
			TextCopy.Clipboard.SetText(deciphered);
			return deciphered;
		}

		#endregion
	}
}
