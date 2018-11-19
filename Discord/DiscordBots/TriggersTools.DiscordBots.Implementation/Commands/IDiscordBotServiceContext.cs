
namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// A context for Discord bot commands, reactions, etc.
	/// </summary>
	public interface IDiscordBotServiceContext {
		/// <summary>
		/// Gets the container for all Discord bot services.
		/// </summary>
		DiscordBotServiceContainer Services { get; }
	}
}
