using System;

namespace WJLCS.Screens {
	/// <summary>
	/// The screen for configuring the rotors.
	/// </summary>
	public class RotorConfigureScreen : InterpreterScreen {

		#region Fields

		/// <summary>
		/// How an rotor keys are received. Text or Paste.
		/// </summary>
		private readonly EnterMode mode;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="RotorConfigureScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		/// <param name="mode">The mode for how the screen receives input.</param>
		public RotorConfigureScreen(string fileName, EnterMode mode) : base(fileName) {
			if (mode == EnterMode.File)
				throw new ArgumentException("HTML mode is not supported for rotor configuration!", nameof(mode));
			this.mode = mode;
		}

		#endregion

		#region RunScreen Override

		/// <summary>
		/// Runs the interpreter screen.
		/// </summary>
		protected override void RunScreen(Interpreter interpreter) {
			interpreter.ConfigureRotors(mode);
		}

		#endregion
	}
}
