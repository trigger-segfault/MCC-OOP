using System;

namespace WJLCS.Screens {
	/// <summary>
	/// The screen for enciphering a message.
	/// </summary>
	public class GetEncipherMessageScreen : GetInterpreterScreen {
		
		#region Fields

		/// <summary>
		/// True if this encipher message should come from the clipboard.
		/// </summary>
		private readonly bool paste;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="GetEncipherMessageScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		/// <param name="paste">True if this decipher message should come from the clipboard.</param>
		public GetEncipherMessageScreen(string fileName, bool paste) : base(fileName) {
			this.paste = paste;
		}

		#endregion

		#region RunScreen Override

		/// <summary>
		/// Runs the interpreter screen.
		/// </summary>
		protected override void RunScreen(Interpreter interpreter) {
			interpreter.RunEncipherer(paste);
		}

		#endregion
	}
}
