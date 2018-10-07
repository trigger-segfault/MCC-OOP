using System;
using System.Collections.Generic;
using System.Text;

namespace WJLCS.Setup {
	/// <summary>
	/// An exception thrown when something cannot be run due to the letterset being missing.
	/// </summary>
	public class LetterSetMissingException : Exception {

		public LetterSetMissingException(string message) : base(message) { }
	}
}
