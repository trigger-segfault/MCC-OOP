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
	/// The default <see cref="IContextingService"/> implementation.
	/// </summary>
	public class DefaultContextingService : DiscordBotService, IContextingService {

		#region Constructors

		public DefaultContextingService(DiscordBotServiceContainer services) : base(services) { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets the default prefix used by the Discord Bot.
		/// </summary>
		public string DefaultPrefix => Config["prefix"];

		#endregion

		#region Command Contexts

		/// <summary>
		/// Constructs the required type of command context that is used by this Discord bot.
		/// </summary>
		/// <param name="msg">The message to construct the context from.</param>
		/// <returns>The created context.</returns>
		public IDiscordBotCommandContext CreateCommandContext(SocketUserMessage msg) {
			return new DiscordBotCommandContext(Services, Client, msg);
		}

		#endregion

		#region Database Contexts

		/// <summary>
		/// Gets if the current context supports custom prefixes.
		/// </summary>
		/// <param name="context">The command context to check.</param>
		/// <returns>True if custom prefixes are supported.</returns>
		public bool IsDbPrefixContext(ICommandContext context) => false;
		/// <summary>
		/// Gets if the current context supports lockable commands.
		/// </summary>
		/// <param name="context">The command context to check.</param>
		/// <returns>True if lockable commands are supported.</returns>
		public bool IsDbLockableContext(ICommandContext context) => false;
		/// <summary>
		/// Gets if the current context supports a manager Id.
		/// </summary>
		/// <param name="context">The command context to check.</param>
		/// <returns>True if a manager Id are supported.</returns>
		public bool IsDbManagerContext(ICommandContext context) => false;

		/// <summary>
		/// Gets the database used to locate the command context.
		/// </summary>
		/// <returns>The database context that must be disposed of.</returns>
		public DbContextEx GetCommandContextDb() => null;

		/// <summary>
		/// Finds the prefixable context in the database and optionally adds it.
		/// </summary>
		/// <param name="db">The database to search in.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="add">True if the context should be added if it's missing.</param>
		/// <returns>The context, or null if it does not exist, and <see cref="add"/> is false.</returns>
		public Task<IDbPrefixContext> FindDbPrefixContextAsync(DbContextEx db, ICommandContext context, bool add) {
			return Task.FromResult<IDbPrefixContext>(null);
		}
		/// <summary>
		/// Finds the lockable context in the database and optionally adds it.
		/// </summary>
		/// <param name="db">The database to search in.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="add">True if the context should be added if it's missing.</param>
		/// <returns>The context, or null if it does not exist, and <see cref="add"/> is false.</returns>
		public Task<IDbLockableContext> FindDbLockableContextAsync(DbContextEx db, ICommandContext context, bool add) {
			return Task.FromResult<IDbLockableContext>(null);
		}
		/// <summary>
		/// Finds the manager context in the database and optionally adds it.
		/// </summary>
		/// <param name="db">The database to search in.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="add">True if the context should be added if it's missing.</param>
		/// <returns>The context, or null if it does not exist, and <see cref="add"/> is false.</returns>
		public Task<IDbManagerContext> FindDbManagerContextAsync(DbContextEx db, ICommandContext context, bool add) {
			return Task.FromResult<IDbManagerContext>(null);
		}

		#endregion
	}
}
