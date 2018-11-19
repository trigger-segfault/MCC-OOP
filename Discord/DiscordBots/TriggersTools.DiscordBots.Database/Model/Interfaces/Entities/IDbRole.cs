
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a guild role.
	/// </summary>
	public interface IDbRole
		: IDbGuildSnowflake, IDbEndUserGuildData { }
}
