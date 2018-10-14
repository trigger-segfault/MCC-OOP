
namespace WJLCS.Screens {
	/// <summary>
	/// The screen for configuring the letterset.
	/// </summary>
	public class GetLetterSetConfigureScreen : GetInterpreterScreen {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="GetLetterSetConfigureScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		public GetLetterSetConfigureScreen(string fileName) : base(fileName) { }

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
