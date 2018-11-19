
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The database model interface for a Discord context where a command can be executed.
	/// </summary>
	public interface IDbCommandContext : IDbPrefixContext, IDbLockableContext, IDbManagerContext { }
}
