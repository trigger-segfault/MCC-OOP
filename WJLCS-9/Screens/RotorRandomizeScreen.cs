
namespace WJLCS.Screens {
	/// <summary>
	/// The screen for randomizing the rotor keys.
	/// </summary>
	public class RotorRandomizeScreen : InterpreterScreen {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="RotorRandomizeScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		public RotorRandomizeScreen(string fileName) : base(fileName) { }

		#endregion

		#region RunScreen Override

		/// <summary>
		/// Runs the interpreter screen.
		/// </summary>
		protected override void RunScreen(Interpreter interpreter) {
			interpreter.RandomizeRotors();
		}

		#endregion
	}
}
