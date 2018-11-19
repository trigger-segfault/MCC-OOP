
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a guild message.
	/// </summary>
	public interface IDbGuildMessage
		: IDbGuildSnowflake, IDbUserMessage, IDbEndUserGuildData { }
}
