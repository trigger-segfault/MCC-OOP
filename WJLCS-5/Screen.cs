
namespace WJLCS {
	/// <summary>
	/// An abstract base class for a displayable screen.
	/// </summary>
	/// <remarks>
	/// This is an interface so it can be legally casted to a <see cref="ScreenAction"/>.
	/// </remarks>
	public abstract class Screen {

		/// <summary>
		/// Runs the screen.
		/// </summary>
		/// <returns>The action to perform after a screen choice.</returns>
		public abstract ScreenAction Run();

		/// <summary>
		/// Checks for any missing files required by this screen
		/// </summary>
		/// <returns>An array of missing files.</returns>
		public virtual string[] GetMissingFiles() => new string[0];
	}
}
