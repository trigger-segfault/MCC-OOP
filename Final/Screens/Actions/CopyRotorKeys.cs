using System;

namespace WJLCS.Screens.Actions {
	/// <summary>
	/// An action for copying rotor keys and returning to the last screen.
	/// </summary>
	public static class CopyRotorKeys {

		/// <summary>
		/// Gets the actual action to populate a menu with.
		/// </summary>
		public static MenuAction Action => new MenuAction(CopyRotorKeysInternal);
		
		/// <summary>
		/// Copies the rotor keys and uses the same screen to state that the keys were copied.
		/// </summary>
		private static Screen CopyRotorKeysInternal(MenuDriver menuDriver) {
			TextCopy.Clipboard.SetText(string.Join(" ", menuDriver.Interpreter.GetRotorKeys()));
			Console.WriteLine();
			Console.Write("Rotor Keys Copied! Press Enter: ");
			Console.ReadLine();
			return menuDriver.MainMenu;
		}
	}
}
