
namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// A services for handling scoped wait contexts.
	/// </summary>
	public interface IWaitContextService {

		/// <summary>
		/// Gets the existing wait context for the user.
		/// </summary>
		/// <param name="userId">The snowflake entity Id of the user.</param>
		/// <returns>The wait context if one exists, otherwise null.</returns>
		IUserWaitContext GetWaitContext(ulong userId);
		/// <summary>
		/// Tries to add the wait context for the user.
		/// </summary>
		/// <param name="wait">The wait context to add.</param>
		/// <returns>True if the wait context was added.</returns>
		void AddWaitContext(IUserWaitContext wait);
		/// <summary>
		/// Tries to remove the wait context for the user.
		/// </summary>
		/// <param name="wait">The wait context to remove.</param>
		/// <returns>
		/// True if the wait context contained was the same as <paramref name="wait"/> and was removed.
		/// </returns>
		bool RemoveWaitContext(IUserWaitContext wait);
	}
}
