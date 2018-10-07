using System;
using System.Collections.Generic;
using System.Text;

namespace WJLCS.Screens {
	/// <summary>
	/// The screen for configuring the plugboard.
	/// </summary>
	public class GetPlugboardConfigureScreen : GetInterpreterScreen {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="GetPlugboardConfigureScreen"/>.
		/// </summary>
		/// <param name="filePath">The filepath of the screen text.</param>
		public GetPlugboardConfigureScreen(string fileName) : base(fileName) { }

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
