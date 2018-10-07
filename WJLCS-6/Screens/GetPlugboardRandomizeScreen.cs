
namespace WJLCS.Screens {
	/// <summary>
	/// The screen for randomizing the plugboard.
	/// </summary>
	public class GetPlugboardRandomizeScreen : GetInterpreterScreen {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="GetPlugboardRandomizeScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		public GetPlugboardRandomizeScreen(string fileName) : base(fileName) { }

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
