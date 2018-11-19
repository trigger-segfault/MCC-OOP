using System;

namespace WJLCS.Enigma.IO {
	/// <summary>
	/// An exception thrown when saving something has failed for some reason.
	/// </summary>
	public class SaveFailedException : Exception {
		/// <summary>
		/// Constructs the <see cref="SaveFailedException"/>.
		/// </summary>
		/// <param name="message">The text message of the exception.</param>
		public SaveFailedException(string message) : base(message) { }
		/// <summary>
		/// Constructs the <see cref="SaveFailedException"/>.
		/// </summary>
		/// <param name="ex">The inner exception of the exception.</param>
		public SaveFailedException(Exception ex) : base(ex.Message, ex) { }
		/// <summary>
		/// Constructs the <see cref="SaveFailedException"/>.
		/// </summary>
		/// <param name="message">The text message of the exception.</param>
		/// <param name="ex">The inner exception of the exception.</param>
		public SaveFailedException(string message, Exception ex) : base($"{message}\n{ex.Message}", ex) { }
	}
}
