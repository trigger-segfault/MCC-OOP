
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a user message.
	/// </summary>
	public interface IDbUserMessage
		: IDbUserSnowflake, IDbChannelSnowflake, IDbEndUserData { }
}
