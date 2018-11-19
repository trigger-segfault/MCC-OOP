using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Services {
	/// <summary>
	/// A service interface for command contexts.
	/// </summary>
	public interface IContextingService {

		#region Properties

		/// <summary>
		/// Gets the default prefix used by the Discord Bot.
		/// </summary>
		string DefaultPrefix { get; }

		#endregion

		#region Command Contexts

		/// <summary>
		/// Constructs the required type of command context that is used by this Discord bot.
		/// </summary>
		/// <param name="msg">The message to construct the context from.</param>
		/// <returns>The created context.</returns>
		IDiscordBotCommandContext CreateCommandContext(SocketUserMessage msg);

		#endregion

		#region Database Contexts

		/// <summary>
		/// Gets if the current context supports custom prefixes.
		/// </summary>
		/// <param name="context">The command context to check.</param>
		/// <returns>True if custom prefixes are supported.</returns>
		bool IsDbPrefixContext(ICommandContext context);
		/// <summary>
		/// Gets if the current context supports lockable commands.
		/// </summary>
		/// <param name="context">The command context to check.</param>
		/// <returns>True if lockable commands are supported.</returns>
		bool IsDbLockableContext(ICommandContext context);
		/// <summary>
		/// Gets if the current context supports a manager Id.
		/// </summary>
		/// <param name="context">The command context to check.</param>
		/// <returns>True if a manager Id are supported.</returns>
		bool IsDbManagerContext(ICommandContext context);

		/// <summary>
		/// Gets the database used to locate the command context.
		/// </summary>
		/// <returns>The database context that must be disposed of.</returns>
		DbContextEx GetCommandContextDb();

		/// <summary>
		/// Finds the prefixable context in the database and optionally adds it.
		/// </summary>
		/// <param name="db">The database to search in.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="add">True if the context should be added if it's missing.</param>
		/// <returns>The context, or null if it does not exist, and <see cref="add"/> is false.</returns>
		Task<IDbPrefixContext> FindDbPrefixContextAsync(DbContextEx db, ICommandContext context, bool add);
		/// <summary>
		/// Finds the lockable context in the database and optionally adds it.
		/// </summary>
		/// <param name="db">The database to search in.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="add">True if the context should be added if it's missing.</param>
		/// <returns>The context, or null if it does not exist, and <see cref="add"/> is false.</returns>
		Task<IDbLockableContext> FindDbLockableContextAsync(DbContextEx db, ICommandContext context, bool add);
		/// <summary>
		/// Finds the manager context in the database and optionally adds it.
		/// </summary>
		/// <param name="db">The database to search in.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="add">True if the context should be added if it's missing.</param>
		/// <returns>The context, or null if it does not exist, and <see cref="add"/> is false.</returns>
		Task<IDbManagerContext> FindDbManagerContextAsync(DbContextEx db, ICommandContext context, bool add);

		#endregion
	}
}
