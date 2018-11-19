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
	/// Extensions for the <see cref="IContextingService"/> and <see cref="IDbManagerContext"/> interfaces.
	/// </summary>
	public static class ManagerContextExtensions {
		/// <summary>
		/// Finds the managing context in the database.
		/// </summary>
		/// <param name="managing">The managing service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <returns>The context, or null if it does not exist.</returns>
		public static async Task<IDbManagerContext> FindDbManagerContextAsync(this IContextingService managing,
			ICommandContext context)
		{
			using (var db = managing.GetCommandContextDb())
				return await managing.FindDbManagerContextAsync(db, context, false).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets the manager role Id of the current command context.
		/// </summary>
		/// <param name="managing">The managing service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <returns>The context's manager role Id or 0 if the context or its role Id is unset.</returns>
		public static async Task<ulong> GetManagerRoleIdAsync(this IContextingService managing, ICommandContext context) {
			if (!managing.IsDbManagerContext(context))
				return 0UL;
			IDbManagerContext manageContext = await managing.FindDbManagerContextAsync(context).ConfigureAwait(false);
			return manageContext?.ManagerRoleId ?? 0UL;
		}
		/// <summary>
		/// Gets the manager role Id of the current command context.
		/// </summary>
		/// <param name="manageContext">The The manager context to look in.</param>
		/// <returns>The context's manager role Id or 0 if the context or its role Id is unset.</returns>
		public static ulong GetManagerRoleId(this IDbManagerContext manageContext) {
			return manageContext?.ManagerRoleId ?? 0UL;
		}
		/// <summary>
		/// Sets the manager role Id for the current command context.
		/// </summary>
		/// <param name="managing">The managing service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="newRoleId">The new manager role Id to set the context to.</param>
		/// <returns>
		/// True if the role Id was set, false if it was already set to this, and null if the role Id cannot
		/// be changed.
		/// </returns>
		public static async Task<bool?> SetManagerRoleIdAsync(this IContextingService managing, ICommandContext context, ulong newRoleId) {
			if (!managing.IsDbManagerContext(context))
				return null;
			using (var db = managing.GetCommandContextDb()) {
				IDbManagerContext manageContext = await managing.FindDbManagerContextAsync(db, context, true).ConfigureAwait(false);

				if (manageContext.ManagerRoleId != newRoleId) {
					manageContext.ManagerRoleId = newRoleId;
					db.ModifyOnly(manageContext, pc => pc.ManagerRoleId);
					await db.SaveChangesAsync().ConfigureAwait(false);
					return true;
				}
				return false;
			}
		}
		/// <summary>
		/// Resets the manager role Id for the current command context to the default.
		/// </summary>
		/// <param name="managing">The managing service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <returns>
		/// True if the role Id was set, false if it was already set to this, and null if the role Id cannot
		/// be changed.
		/// </returns>
		public static Task<bool?> ResetManagerRoleIdAsync(this IContextingService managing, ICommandContext context) {
			return managing.SetManagerRoleIdAsync(context, 0);
		}
	}
}
