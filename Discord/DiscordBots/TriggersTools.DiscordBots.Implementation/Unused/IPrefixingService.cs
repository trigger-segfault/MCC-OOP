using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Services {
	/// <summary>
	/// A service interface for context prefixes.
	/// </summary>
	public interface IPrefixingService {
		/// <summary>
		/// Gets the default prefix used by the Discord Bot.
		/// </summary>
		string DefaultPrefix { get; }

		/// <summary>
		/// Gets if the current context supports custom prefixes.
		/// </summary>
		/// <param name="context">The command context to check.</param>
		/// <returns>True if custom prefixes are supported.</returns>
		bool IsDbPrefixContext(ICommandContext context);

		/// <summary>
		/// Gets the database used to locate the prefixable context.
		/// </summary>
		/// <returns>The database context that must be disposed of.</returns>
		DbContextEx GetPrefixContextDb();

		/// <summary>
		/// Finds the prefixable context in the database and optionally adds it.
		/// </summary>
		/// <param name="dbBase">The database to search in.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="add">True if the context should be added if it's missing.</param>
		/// <returns>The context, or null if it does not exist, and <see cref="add"/> is false.</returns>
		Task<IDbPrefixContext> FindDbPrefixContextAsync(DbContextEx dbBase, ICommandContext context, bool add);
	}
}
