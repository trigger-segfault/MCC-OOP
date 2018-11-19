using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Database.Model;
using TriggersTools.DiscordBots.Extensions;
using TriggersTools.DiscordBots.Modules;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots.Services {
	/// <summary>
	/// A service interface for locking commands.
	/// </summary>
	public interface ILockingService {
		/// <summary>
		/// Gets if the current context supports lockable commands.
		/// </summary>
		/// <param name="context">The command context to check.</param>
		/// <returns>True if lockable commands are supported.</returns>
		bool IsDbLockableContext(ICommandContext context);

		/// <summary>
		/// Gets the lockable database context.
		/// </summary>
		/// <param name="context">The command context to get the lockable database context from.</param>
		/// <returns>The lockable database context.</returns>
		DbContextEx GetLockableContextDb();

		/// <summary>
		/// Finds the lockable context in the database and optionally adds it.
		/// </summary>
		/// <param name="dbBase">The database to search in.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="add">True if the context should be added if it's missing.</param>
		/// <returns>The context, or null if it does not exist, and <see cref="add"/> is false.</returns>
		Task<IDbLockableContext> FindDbLockableContextAsync(DbContextEx dbBase, ICommandContext context, bool add);
	}
}
