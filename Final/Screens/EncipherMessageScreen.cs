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
		private readonly EnterMode mode;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="EncipherMessageScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		/// <param name="mode">The mode for how the screen receives input.</param>
		public EncipherMessageScreen(string fileName, EnterMode mode) : base(fileName) {
			if (mode == EnterMode.File)
				throw new ArgumentException("HTML mode is not supported for enciphering!", nameof(mode));
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
