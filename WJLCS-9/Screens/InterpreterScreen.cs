using System;

namespace WJLCS.Screens {
	/// <summary>
	/// A screen that runs makes use of the <see cref="Interpreter"/> class.
	/// </summary>
	public abstract class InterpreterScreen : Screen {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="InterpreterScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		protected InterpreterScreen(string filePath) : base(filePath) { }

		#endregion

		#region Override RunScreen

		/// <summary>
		/// Runs the screen.
		/// </summary>
		/// <returns>The action to perform after a screen choice.</returns>
		protected override sealed MenuAction RunScreen(MenuDriver menuDriver) {
			RunScreen(menuDriver.Interpreter);
			Console.Write("Press Enter: ");
			Console.ReadLine();
			return menuDriver.MainMenu;
		}

		#endregion

		#region Abstract RunScreen

		/// <summary>
		/// Runs the interpreter screen.
		/// </summary>
		protected abstract void RunScreen(Interpreter interpreter);

		#endregion
	}
}
