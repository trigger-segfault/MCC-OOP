
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a guild emote.
	/// </summary>
	public interface IDbGuildEmote
		: IDbGuildSnowflake, IDbEndUserGuildData { }
}
