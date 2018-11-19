using System;

namespace WJLCS.Screens {
	/// <summary>
	/// The screen for enciphering a message.
	/// </summary>
	public class EncipherMessageScreen : InterpreterScreen {

		#region Fields

		/// <summary>
		/// How a deciphered message is received. Text or Paste.
		/// </summary>
		private readonly EncipherMode mode;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="EncipherMessageScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		/// <param name="paste">True if this decipher message should come from the clipboard.</param>
		public EncipherMessageScreen(string fileName, EncipherMode mode) : base(fileName) {
			if (mode == EncipherMode.HTML)
				throw new ArgumentException("HTML mode is not supported for enciphering, HTML mode is always on!", nameof(mode));
			this.mode = mode;
		}

		#endregion

		#region RunScreen Override

		/// <summary>
		/// Runs the interpreter screen.
		/// </summary>
		protected override void RunScreen(Interpreter interpreter) {
			interpreter.RunEncipherer(mode);
		}

		#endregion
	}
}
