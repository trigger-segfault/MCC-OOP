using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WJLCS.Enigma {
	/// <summary>
	/// An exception thrown when a non-prime number is thrown into the rotor keys.
	/// </summary>
	public class NotPrimeException : Exception {
		/// <summary>
		/// Constructs the <see cref="NotPrimeException"/>.
		/// </summary>
		/// <param name="message">The text message of the exception.</param>
		public NotPrimeException(string message) : base(message) { }
		/// <summary>
		/// Constructs the <see cref="NotPrimeException"/>.
		/// </summary>
		/// <param name="message">The inner exception of the exception.</param>
		public NotPrimeException(Exception ex) : base(ex.Message, ex) { }
		/// <summary>
		/// Constructs the <see cref="NotPrimeException"/>.
		/// </summary>
		/// <param name="message">The text message of the exception.</param>
		/// <param name="ex">The inner exception of the exception.</param>
		public NotPrimeException(string message, Exception ex) : base($"{message}\n{ex.Message}", ex) { }
	}
}
