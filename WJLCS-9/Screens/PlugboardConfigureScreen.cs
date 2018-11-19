
namespace WJLCS.Screens {
	/// <summary>
	/// The screen for configuring the plugboard.
	/// </summary>
	public class PlugboardConfigureScreen : InterpreterScreen {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="PlugboardConfigureScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		public PlugboardConfigureScreen(string fileName) : base(fileName) { }

		#endregion

		#region RunScreen Override

		/// <summary>
		/// Runs the interpreter screen.
		/// </summary>
		protected override void RunScreen(Interpreter interpreter) {
			interpreter.ConfigurePlugboard();
		}

		#endregion
	}
}
