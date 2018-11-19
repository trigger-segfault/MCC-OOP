
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a user DM.
	/// </summary>
	public interface IDbDM
		: IDbUserSnowflake, IDbTextChannel, IDbEndUserData, IDbCommandContext { }
}
