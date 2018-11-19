
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a text channel.
	/// </summary>
	public interface IDbTextChannel
		: IDbSnowflake, IDbCommandContext { }
}
