
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a guild user.
	/// </summary>
	public interface IDbGuildUser
		: IDbUniqueGuildSnowflake, IDbEndUserData, IDbEndUserGuildData { }
}
