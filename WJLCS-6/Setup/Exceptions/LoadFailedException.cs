using System;
using System.Collections.Generic;
using System.Text;

namespace WJLCS.Setup {
	/// <summary>
	/// An exception thrown when loading something has failed for some reason.
	/// </summary>
	public class LoadFailedException : Exception {

		public LoadFailedException(string message) : base(message) { }

		public LoadFailedException(Exception ex) : base(ex.Message, ex) { }
	}
}
