using System;

namespace WJLCS {
	/// <summary>
	/// An action to perform in the screen driver.
	/// </summary>
	public class ScreenAction {

		#region Fields

		/// <summary>
		/// The action to run.
		/// </summary>
		private readonly Func<ScreenDriver, Screen> action;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the screen action.
		/// </summary>
		/// <param name="action">The action to call.</param>
		public ScreenAction(Func<Screen> action) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			this.action = (sd) => action();
		}
		/// <summary>
		/// Constructs the screen action.
		/// </summary>
		/// <param name="action">The action to call.</param>
		public ScreenAction(Func<ScreenDriver, Screen> action) {
			this.action = action ?? throw new ArgumentNullException(nameof(action));
		}

		#endregion

		#region Invoke

		/// <summary>
		/// Invokes the screen action and passes the menu driver.
		/// </summary>
		/// <param name="screenDriver">The driver for the screens.</param>
		/// <returns>The next screen to go to.</returns>
		public Screen Invoke(ScreenDriver screenDriver) {
			return action(screenDriver);
		}

		#endregion

		#region Casting

		/// <summary>
		/// Casts a <see cref="Screen"/> to a <see cref="ScreenAction"/>.
		/// </summary>
		/// <param name="screen">The screen to go to.</param>
		public static implicit operator ScreenAction(Screen screen) {
			return new ScreenAction(() => screen);
		}

		#endregion

		#region Standard Actions

		/// <summary>
		/// Gets a <see cref="ScreenAction"/> that will return to the last screen.
		/// </summary>
		public static ScreenAction LastScreen { get; } = new ScreenAction((sd) => sd.LastScreen);
		/// <summary>
		/// Gets a <see cref="ScreenAction"/> that will repeat the current screen.
		/// </summary>
		public static ScreenAction CurrentScreen { get; } = new ScreenAction((sd) => sd.CurrentScreen);
		/// <summary>
		/// Gets a <see cref="ScreenAction"/> that exit the application.
		/// </summary>
		public static ScreenAction Exit { get; } = new ScreenAction((sd) => null);

		#endregion
	}
}
