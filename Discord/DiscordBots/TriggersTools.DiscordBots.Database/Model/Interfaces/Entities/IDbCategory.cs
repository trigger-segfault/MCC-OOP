
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a category.
	/// </summary>
	public interface IDbCategory
		: IDbGuildSnowflake, IDbCommandContext { }
}
