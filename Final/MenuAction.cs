using System;
using WJLCS.Screens;

namespace WJLCS {
	/// <summary>
	/// An action to perform in the screen driver.
	/// </summary>
	public class MenuAction {

		#region Fields

		/// <summary>
		/// The action to run.
		/// </summary>
		private readonly Func<MenuDriver, Screen> action;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the screen action.
		/// </summary>
		/// <param name="action">The action to call.</param>
		public MenuAction(Func<Screen> action) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			this.action = (sd) => action();
		}
		/// <summary>
		/// Constructs the screen action.
		/// </summary>
		/// <param name="action">The action to call.</param>
		public MenuAction(Func<MenuDriver, Screen> action) {
			this.action = action ?? throw new ArgumentNullException(nameof(action));
		}

		#endregion

		#region Invoke

		/// <summary>
		/// Invokes the screen action and passes the menu driver.
		/// </summary>
		/// <param name="menuDriver">The driver for the screens.</param>
		/// <returns>The next screen to go to.</returns>
		public Screen Invoke(MenuDriver menuDriver) {
			return action(menuDriver);
		}

		#endregion

		#region Casting

		/// <summary>
		/// Casts a <see cref="Screen"/> to a <see cref="MenuAction"/>.
		/// </summary>
		/// <param name="screen">The screen to go to.</param>
		public static implicit operator MenuAction(Screen screen) {
			return new MenuAction(() => screen);
		}

		#endregion

		#region Standard Actions

		/// <summary>
		/// Gets a <see cref="MenuAction"/> that will return to the last screen.
		/// </summary>
		public static MenuAction LastScreen { get; } = new MenuAction((sd) => sd.LastScreen);
		/// <summary>
		/// Gets a <see cref="MenuAction"/> that will repeat the current screen.
		/// </summary>
		public static MenuAction CurrentScreen { get; } = new MenuAction((sd) => sd.CurrentScreen);
		/// <summary>
		/// Gets a <see cref="MenuAction"/> that exit the application.
		/// </summary>
		public static MenuAction Exit { get; } = new MenuAction((sd) => null);

		#endregion
	}
}
