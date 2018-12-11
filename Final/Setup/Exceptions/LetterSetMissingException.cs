using System;

namespace WJLCS.Setup {
	/// <summary>
	/// An exception thrown when something cannot be run due to the letterset being missing.
	/// </summary>
	public class LetterSetMissingException : Exception {
		/// <summary>
		/// Constructs the <see cref="LetterSetMissingException"/>.
		/// </summary>
		/// <param name="message">The text message of the exception.</param>
		public LetterSetMissingException(string message) : base(message) { }
	}
}
