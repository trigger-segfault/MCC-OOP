using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Database.Model;
using TriggersTools.DiscordBots.Modules;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots.Extensions {
	/// <summary>
	/// Extensions for the <see cref="IContextingService"/> and <see cref="IDbLockableContext"/> interfaces.
	/// </summary>
	public static class LockingContextExtensions {
		/// <summary>
		/// Finds the lockable context in the database.
		/// </summary>
		/// <param name="locking">The locking service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <returns>The context, or null if it does not exist.</returns>
		public static async Task<IDbLockableContext> FindDbLockableContextAsync(this IContextingService locking, ICommandContext context) {
			using (var db = locking.GetCommandContextDb())
				return await locking.FindDbLockableContextAsync(db, context, false).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets if the specified command or its module is locked.
		/// </summary>
		/// <param name="locking">The locking service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="command">The command to check.</param>
		/// <returns>True if the command or its module is locked.</returns>
		public static async Task<bool> IsLockedAsync(this IContextingService locking, ICommandContext context, CommandDetails command) {
			IDbLockableContext lockContext = await locking.FindDbLockableContextAsync(context).ConfigureAwait(false);
			if (lockContext == null)
				return command.IsLockedByDefault || command.Module.IsLockedByDefault;
			return command.IsLocked(lockContext);
		}
		/// <summary>
		/// Gets if the specified command is locked.
		/// </summary>
		/// <param name="locking">The locking service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="command">The command to check.</param>
		/// <returns>True if the command is locked.</returns>
		public static async Task<bool> IsCommandLockedAsync(this IContextingService locking, ICommandContext context, CommandDetails command) {
			IDbLockableContext lockContext = await locking.FindDbLockableContextAsync(context).ConfigureAwait(false);
			return command.IsCommandLocked(lockContext);
		}
		/// <summary>
		/// Gets if the specified module is locked.
		/// </summary>
		/// <param name="locking">The locking service to work with.</param>
		/// <param name="context">The command context to look for.</param>
		/// <param name="module">The module to check.</param>
		/// <returns>True if the module is locked.</returns>
		public static async Task<bool> IsModuleLockedAsync(this IContextingService locking, ICommandContext context, ModuleDetails module) {
			IDbLockableContext lockContext = await locking.FindDbLockableContextAsync(context).ConfigureAwait(false);
			return module.IsModuleLocked(lockContext);
		}

		/// <summary>
		/// Gets if the specified command or its module is locked.
		/// </summary>
		/// <param name="command">The command to check.</param>
		/// <param name="lockContext">The lockable context to work with.</param>
		/// <returns>True if the command or its module is locked.</returns>
		public static bool IsLocked(this CommandDetails command, IDbLockableContext lockContext) {
			if (lockContext == null)
				return command.IsLockedByDefault || command.Module.IsLockedByDefault;
			return (command.IsCommandLocked(lockContext) || command.Module.IsModuleLocked(lockContext));
		}
		/// <summary>
		/// Gets if the specified command is locked.
		/// </summary>
		/// <param name="command">The command to check.</param>
		/// <param name="lockContext">The lockable context to work with.</param>
		/// <returns>True if the command is locked.</returns>
		public static bool IsCommandLocked(this CommandDetails command, IDbLockableContext lockContext) {
			if (lockContext == null)
				return command.IsLockedByDefault;
			return (command.IsLockable && command.IsLockedByDefault !=
					lockContext.LockedCommands.Contains(command.Alias));
		}
		/// <summary>
		/// Gets if the specified module is locked.
		/// </summary>
		/// <param name="module">The module to check.</param>
		/// <param name="lockContext">The lockable context to work with.</param>
		/// <returns>True if the module is locked.</returns>
		public static bool IsModuleLocked(this ModuleDetails module, IDbLockableContext lockContext) {
			if (lockContext == null)
				return module.IsLockedByDefault;
			return (module.IsLockable && module.IsLockedByDefault !=
					lockContext.LockedModules.Contains(module.Name));
		}
		/*/// <summary>
		/// Gets if the specified command or its module is locked.
		/// </summary>
		/// <param name="lockContext">The lockable context to work with.</param>
		/// <param name="command">The command to check.</param>
		/// <returns>True if the command or its module is locked.</returns>
		public static bool IsLocked(this IDbLockableContext lockContext, CommandDetails command) {
			if (lockContext == null)
				return command.IsLockedByDefault || command.Module.IsLockedByDefault;
			return (lockContext.IsCommandLocked(command) || lockContext.IsModuleLocked(command.Module));
		}
		/// <summary>
		/// Gets if the specified command is locked.
		/// </summary>
		/// <param name="lockContext">The lockable context to work with.</param>
		/// <param name="command">The command to check.</param>
		/// <returns>True if the command is locked.</returns>
		public static bool IsCommandLocked(this IDbLockableContext lockContext, CommandDetails command) {
			if (lockContext == null)
				return command.IsLockedByDefault;
			return (command.IsLockable && command.IsLockedByDefault !=
					lockContext.LockedCommands.Contains(command.Alias));
		}
		/// <summary>
		/// Gets if the specified module is locked.
		/// </summary>
		/// <param name="lockContext">The lockable context to work with.</param>
		/// <param name="module">The module to check.</param>
		/// <returns>True if the module is locked.</returns>
		public static bool IsModuleLocked(this IDbLockableContext lockContext, ModuleDetails module) {
			if (lockContext == null)
				return module.IsLockedByDefault;
			return (module.IsLockable && module.IsLockedByDefault !=
					lockContext.LockedModules.Contains(module.Name));
		}*/

		public static CommandDetails ResolveCommand(this DiscordBotService service, string commandName) {
			return service.Commands.CommandSet.FindCommand(commandName);
		}
		public static CommandDetails ResolveCommand(this DiscordBotService service, CommandInfo cmd) {
			return service.ResolveCommand(cmd.GetDetailsName());
		}
		public static ModuleDetails ResolveModule(this DiscordBotService service, string moduleName) {
			return service.Commands.CommandSet.FindModule(moduleName);
		}
		public static ModuleDetails ResolveModule(this DiscordBotService service, ModuleInfo mod) {
			return service.ResolveModule(mod.GetDetailsName());
		}
		public static CommandDetails ResolveCommand<T>(this DiscordBotModule<T> module, string commandName)
			where T : class, IDiscordBotCommandContext
		{
			return module.Commands.CommandSet.FindCommand(commandName);
		}
		public static CommandDetails ResolveCommand<T>(this DiscordBotModule<T> module, CommandInfo cmd)
			where T : class, IDiscordBotCommandContext
		{
			return module.ResolveCommand(cmd.GetDetailsName());
		}
		public static ModuleDetails ResolveModule<T>(this DiscordBotModule<T> module, string moduleName)
			where T : class, IDiscordBotCommandContext
		{
			return module.Commands.CommandSet.FindModule(moduleName);
		}
		public static ModuleDetails ResolveModule<T>(this DiscordBotModule<T> module, ModuleInfo mod)
			where T : class, IDiscordBotCommandContext
		{
			return module.ResolveModule(mod.GetDetailsName());
		}
		/// <summary>
		/// Attempts to lock the command in the specified context.
		/// </summary>
		/// <param name="locking">The locking service to work with.</param>
		/// <param name="context">The command context to look for the command from.</param>
		/// <param name="command">The command to look for.</param>
		/// <returns>
		/// True if the command was locked, false if it was already locked, and null if the command cannot
		/// be locked.
		/// </returns>
		public static async Task<bool?> LockCommandAsync(this IContextingService locking, ICommandContext context, CommandDetails command) {
			if (!locking.IsDbLockableContext(context) || !command.IsLockable)
				return null;
			using (var db = locking.GetCommandContextDb()) {
				var lockContext = await locking.FindDbLockableContextAsync(db, context, true).ConfigureAwait(false);
				
				if ((!command.IsLockedByDefault && lockContext.LockedCommands.Add(command.Alias)) ||
					( command.IsLockedByDefault && lockContext.LockedCommands.Remove(command.Alias)))
				{
					db.ModifyOnly(lockContext, lc => lc.LockedCommands);
					await db.SaveChangesAsync().ConfigureAwait(false);
					return true;
				}
				return false;
			}
		}
		/// <summary>
		/// Attempts to unlock the command in the specified context.
		/// </summary>
		/// <param name="locking">The locking service to work with.</param>
		/// <param name="context">The command context to look for the command from.</param>
		/// <param name="command">The command to look for.</param>
		/// <returns>
		/// True if the command was unlocked, false if it was already unlocked, and null if the command cannot
		/// be locked.
		/// </returns>
		public static async Task<bool?> UnlockCommandAsync(this IContextingService locking, ICommandContext context, CommandDetails command) {
			if (!locking.IsDbLockableContext(context) || !command.IsLockable)
				return null;
			using (var db = locking.GetCommandContextDb()) {
				var lockContext = await locking.FindDbLockableContextAsync(db, context, true).ConfigureAwait(false);
				
				if ((!command.IsLockedByDefault && lockContext.LockedCommands.Remove(command.Alias)) ||
					( command.IsLockedByDefault && lockContext.LockedCommands.Add(command.Alias)))
				{
					db.ModifyOnly(lockContext, lc => lc.LockedCommands);
					await db.SaveChangesAsync().ConfigureAwait(false);
					return true;
				}
				return false;
			}
		}
		/// <summary>
		/// Attempts to lock the module in the specified context.
		/// </summary>
		/// <param name="locking">The locking service to work with.</param>
		/// <param name="context">The module context to look for the module from.</param>
		/// <param name="module">The module to look for.</param>
		/// <returns>
		/// True if the module was locked, false if it was already locked, and null if the module cannot
		/// be locked.
		/// </returns>
		public static async Task<bool?> LockModuleAsync(this IContextingService locking, ICommandContext context, ModuleDetails module) {
			if (!locking.IsDbLockableContext(context) || !module.IsLockable)
				return null;
			using (var db = locking.GetCommandContextDb()) {
				var lockContext = await locking.FindDbLockableContextAsync(db, context, true).ConfigureAwait(false);
				
				if ((!module.IsLockedByDefault && lockContext.LockedModules.Add(module.Name)) ||
					( module.IsLockedByDefault && lockContext.LockedModules.Remove(module.Name)))
				{
					db.ModifyOnly(lockContext, lc => lc.LockedModules);
					await db.SaveChangesAsync().ConfigureAwait(false);
					return true;
				}
				return false;
			}
		}
		/// <summary>
		/// Attempts to unlock the module in the specified context.
		/// </summary>
		/// <param name="locking">The locking service to work with.</param>
		/// <param name="context">The module context to look for the module from.</param>
		/// <param name="module">The module to look for.</param>
		/// <returns>
		/// True if the module was unlocked, false if it was already unlocked, and null if the module cannot
		/// be locked.
		/// </returns>
		public static async Task<bool?> UnlockModuleAsync(this IContextingService locking, ICommandContext context, ModuleDetails module) {
			if (!locking.IsDbLockableContext(context) || !module.IsLockable)
				return null;
			using (var db = locking.GetCommandContextDb()) {
				var lockContext = await locking.FindDbLockableContextAsync(db, context, true).ConfigureAwait(false);

				if ((!module.IsLockedByDefault && lockContext.LockedModules.Remove(module.Name)) ||
					( module.IsLockedByDefault && lockContext.LockedModules.Add(module.Name)))
				{
					db.ModifyOnly(lockContext, lc => lc.LockedModules);
					await db.SaveChangesAsync().ConfigureAwait(false);
					return true;
				}
				return false;
			}
		}
	}
}
