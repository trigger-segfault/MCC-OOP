
namespace WJLCS.Screens {
	/// <summary>
	/// The screen for configuring the letterset.
	/// </summary>
	public class LetterSetConfigureScreen : InterpreterScreen {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="LetterSetConfigureScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		public LetterSetConfigureScreen(string fileName) : base(fileName) { }

		#endregion

		#region RunScreen Override

		/// <summary>
		/// Runs the interpreter screen.
		/// </summary>
		protected override void RunScreen(Interpreter interpreter) {
			interpreter.ConfigureLetterSet();
		}

		#endregion
	}
}
