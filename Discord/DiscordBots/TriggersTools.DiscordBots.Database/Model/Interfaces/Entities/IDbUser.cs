
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a user.
	/// </summary>
	public interface IDbUser
		: IDbSnowflake, IDbEndUserData { }
}
