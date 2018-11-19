
namespace WJLCS.Screens {
	/// <summary>
	/// The screen for randomizing the plugboard.
	/// </summary>
	public class PlugboardRandomizeScreen : InterpreterScreen {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="PlugboardRandomizeScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		public PlugboardRandomizeScreen(string fileName) : base(fileName) { }

		#endregion

		#region RunScreen Override

		/// <summary>
		/// Runs the interpreter screen.
		/// </summary>
		protected override void RunScreen(Interpreter interpreter) {
			interpreter.RandomizePlugboard();
		}

		#endregion
	}
}
