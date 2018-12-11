
namespace WJLCS.Screens {
	/// <summary>
	/// The screen for deciphering a message.
	/// </summary>
	public class DecipherMessageScreen : InterpreterScreen {

		#region Fields
		
		/// <summary>
		/// How an enciphered message is received. Text, Paste, or HTML.
		/// </summary>
		private readonly EnterMode mode;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="DecipherMessageScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		/// <param name="mode">The mode for how the screen receives input.</param>
		public DecipherMessageScreen(string fileName, EnterMode mode) : base(fileName) {
			this.mode = mode;
		}

		#endregion

		#region RunScreen Override

		/// <summary>
		/// Runs the interpreter screen.
		/// </summary>
		protected override void RunScreen(Interpreter interpreter) {
			interpreter.RunDecipherer(mode);
		}

		#endregion
	}
}
