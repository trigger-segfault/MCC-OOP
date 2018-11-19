using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WJLCS.Enigma {
	/// <summary>
	/// A symetrical encipherer that can encipher and decipher text.
	/// </summary>
	public interface IStringEncipherer {
		/// <summary>
		/// Enciphers the specified text.
		/// </summary>
		/// <param name="text">The text to encipher.</param>
		/// <returns>The enciphered text.</returns>
		string Encipher(string text);
		/// <summary>
		/// Deciphers the specified text.
		/// </summary>
		/// <param name="text">The text to decipher.</param>
		/// <returns>The deciphered text.</returns>
		string Decipher(string text);
	}
}
