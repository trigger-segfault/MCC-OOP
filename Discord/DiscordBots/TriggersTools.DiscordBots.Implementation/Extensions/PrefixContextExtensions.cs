using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Database.Model;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots.Extensions {
	/// <summary>
	/// Extensions for the <see cref="IContextingService"/> and <see cref="IDbPrefixContext"/> interfaces.
	/// </summary>
	public static class PrefixContextExtensions {
		/// <summary>
		/// Finds the prefix context in the database.
		/// </summary>
		/// <param name="prefixing">The prefixing service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <returns>The context, or null if it does not exist.</returns>
		public static async Task<IDbPrefixContext> FindDbPrefixContextAsync(this IContextingService prefixing,
			ICommandContext context)
		{
			using (var db = prefixing.GetCommandContextDb())
				return await prefixing.FindDbPrefixContextAsync(db, context, false).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets the prefix of the current command context.
		/// </summary>
		/// <param name="prefixing">The prefixing service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <returns>The context's prefix or the default prefix if the context or its prefix is unset.</returns>
		public static async Task<string> GetPrefixAsync(this IContextingService prefixing, ICommandContext context) {
			if (!prefixing.IsDbPrefixContext(context))
				return prefixing.DefaultPrefix;
			IDbPrefixContext prefixContext = await prefixing.FindDbPrefixContextAsync(context).ConfigureAwait(false);
			return prefixContext?.Prefix ?? prefixing.DefaultPrefix;
		}
		/// <summary>
		/// Gets the prefix of the current prefix context.
		/// </summary>
		/// <param name="prefixContext">The prefix context to look in.</param>
		/// <param name="prefixing">The prefixing context to get the default prefix from.</param>
		/// <returns>The context's prefix or the default prefix if the context or its prefix is unset.</returns>
		public static string GetPrefix(this IDbPrefixContext prefixContext, IContextingService prefixing) {
			return prefixContext?.Prefix ?? prefixing.DefaultPrefix;
		}
		/// <summary>
		/// Sets the prefix for the current command context.
		/// </summary>
		/// <param name="prefixing">The prefixing service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="newPrefix">The new prefix to set the context to.</param>
		/// <returns>
		/// True if the prefix was set, false if it was already set to this, and null if the prefix cannot be
		/// changed.
		/// </returns>
		public static async Task<bool?> SetPrefixAsync(this IContextingService prefixing, ICommandContext context, string newPrefix) {
			if (!prefixing.IsDbPrefixContext(context))
				return null;
			using (var db = prefixing.GetCommandContextDb()) {
				IDbPrefixContext prefixContext = await prefixing.FindDbPrefixContextAsync(db, context, true).ConfigureAwait(false);

				if (prefixContext.Prefix != newPrefix) {
					prefixContext.Prefix = newPrefix;
					db.ModifyOnly(prefixContext, pc => pc.Prefix);
					await db.SaveChangesAsync().ConfigureAwait(false);
					return true;
				}
				return false;
			}
		}
		/// <summary>
		/// Resets the prefix for the current command context to the default.
		/// </summary>
		/// <param name="prefixing">The prefixing service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <returns>
		/// True if the prefix was set, false if it was already set to this, and null if the prefix cannot be
		/// changed.
		/// </returns>
		public static Task<bool?> ResetPrefixAsync(this IContextingService prefixing, ICommandContext context) {
			return prefixing.SetPrefixAsync(context, null);
		}
	}
}
