using Discord.Commands;
using TriggersTools.DiscordBots.Database.Model;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// The context for a bot command.
	/// </summary>
	public interface IDiscordBotCommandContext : ICommandContext, IDiscordBotServiceContainer {
		/// <summary>
		/// Gets the lockable context used to determine if commands are locked.
		/// </summary>
		IDbLockableContext LockContext { get; }
		/// <summary>
		/// Gets the manager context used to determine if the user has a manager role.
		/// </summary>
		IDbManagerContext ManageContext { get; }
		/// <summary>
		/// Gets the argument position in the command after the prefix.
		/// </summary>
		int ArgPos { get; set; }
	}
}
