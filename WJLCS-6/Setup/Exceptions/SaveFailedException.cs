using System;
using System.Collections.Generic;
using System.Text;

namespace WJLCS.Setup {
	/// <summary>
	/// An exception thrown when saving something has failed for some reason.
	/// </summary>
	public class SaveFailedException : Exception {

		public SaveFailedException(string message) : base(message) { }

		public SaveFailedException(Exception ex) : base(ex.Message, ex) { }
	}
}
