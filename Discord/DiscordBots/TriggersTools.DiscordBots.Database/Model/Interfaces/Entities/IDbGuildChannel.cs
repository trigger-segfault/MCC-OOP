
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a guild channel.
	/// </summary>
	public interface IDbGuildChannel
		: IDbGuildSnowflake, IDbTextChannel, IDbEndUserGuildData, IDbCommandContext { }
}
