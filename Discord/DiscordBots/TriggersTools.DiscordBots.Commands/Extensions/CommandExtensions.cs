using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.Commands;
using TriggersTools.DiscordBots.Commands.Base;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// Extensions methods for <see cref="CommandInfo"/>, <see cref="ModuleInfo"/>, and
	/// <see cref="ParameterInfo"/> classes.
	/// </summary>
	public static class CommandExtensions {

		#region Attributes

		/// <summary>
		/// Gets if the command has a <see cref="CommandInfo"/>.
		/// </summary>
		/// <param name="cmd">The command to check.</param>
		/// <returns>True if a preview attribute exists.</returns>
		public static bool HasPreview(this CommandInfo cmd) {
			return cmd.Attributes.OfType<PreviewAttribute>().Any();
		}
		/// <summary>
		/// Gets if the module has a <see cref="PreviewAttribute"/>.
		/// </summary>
		/// <param name="mod">The module to check.</param>
		/// <returns>True if a preview attribute exists.</returns>
		public static bool HasPreview(this ModuleInfo mod) {
			return mod.Attributes.OfType<PreviewAttribute>().Any();
		}
		/// <summary>
		/// Gets the command's <see cref="PreviewAttribute.ImageUrl"/> if it has one.
		/// </summary>
		/// <param name="cmd">The command to check.</param>
		/// <returns>The usage if it exists, otherwise null.</returns>
		public static string GetPreview(this CommandInfo cmd) {
			return cmd.Attributes.OfType<PreviewAttribute>().FirstOrDefault()?.ImageUrl;
		}
		/// <summary>
		/// Gets the module's <see cref="PreviewAttribute.ImageUrl"/> if it has one.
		/// </summary>
		/// <param name="mod">The module to check.</param>
		/// <returns>The preview image Url if it exists, otherwise null.</returns>
		public static string GetPreview(this ModuleInfo mod) {
			return mod.Attributes.OfType<PreviewAttribute>().FirstOrDefault()?.ImageUrl;
		}
		/// <summary>
		/// Gets if the command has a <see cref="UsageAttribute"/>.
		/// </summary>
		/// <param name="cmd">The command to check.</param>
		/// <returns>True if a usage attribute exists.</returns>
		public static bool HasUsage(this CommandInfo cmd) {
			return cmd.Attributes.OfType<UsageAttribute>().Any();
		}
		/// <summary>
		/// Gets if the module has a <see cref="UsageAttribute"/>.
		/// </summary>
		/// <param name="mod">The module to check.</param>
		/// <returns>True if a usage attribute exists.</returns>
		public static bool HasUsage(this ModuleInfo mod) {
			return mod.Attributes.OfType<UsageAttribute>().Any();
		}
		/// <summary>
		/// Gets the command's <see cref="UsageAttribute.Usage"/> if it has one.
		/// </summary>
		/// <param name="cmd">The command to check.</param>
		/// <returns>The usage if it exists, otherwise null.</returns>
		public static string GetUsage(this CommandInfo cmd) {
			return cmd.Attributes.OfType<UsageAttribute>().FirstOrDefault()?.Usage;
		}
		/// <summary>
		/// Gets the module's <see cref="UsageAttribute.Usage"/> if it has one.
		/// </summary>
		/// <param name="mod">The module to check.</param>
		/// <returns>The usage if it exists, otherwise null.</returns>
		public static string GetUsage(this ModuleInfo mod) {
			return mod.Attributes.OfType<UsageAttribute>().FirstOrDefault()?.Usage;
		}
		/// <summary>
		/// Gets the command has any <see cref="ExampleAttribute"/>s.
		/// </summary>
		/// <param name="cmd">The command to check.</param>
		/// <returns>True if the command has any examples.</returns>
		public static bool HasExamples(this CommandInfo cmd) {
			return cmd.Attributes.OfType<ExampleAttribute>().Any();
		}
		/// <summary>
		/// Gets the module has any <see cref="ExampleAttribute"/>s.
		/// </summary>
		/// <param name="mod">The module to check.</param>
		/// <returns>True if the module has any examples.</returns>
		public static bool HasExamples(this ModuleInfo mod) {
			return mod.Attributes.OfType<ExampleAttribute>().Any();
		}
		/// <summary>
		/// Gets the command's <see cref="ExampleAttribute.Example"/>s.
		/// </summary>
		/// <param name="cmd">The command to check.</param>
		/// <returns>An enumerable of command examples.</returns>
		public static IEnumerable<Example> GetExamples(this CommandInfo cmd) {
			return cmd.Attributes.OfType<ExampleAttribute>().Select(a => a.Example.AddAlias(cmd.Aliases.First()));
		}
		/// <summary>
		/// Gets the module's <see cref="ExampleAttribute.Example"/>s.
		/// </summary>
		/// <param name="mod">The module to check.</param>
		/// <returns>An enumerable of module examples.</returns>
		public static IEnumerable<Example> GetExamples(this ModuleInfo mod) {
			return mod.Attributes.OfType<ExampleAttribute>().Select(a => a.Example.AddAlias(mod.Aliases.First()));
		}
		/// <summary>
		/// Gets if the command has a <see cref="HideAttribute"/>.
		/// </summary>
		/// <param name="cmd">The command to check.</param>
		/// <returns>True if a hide attribute exists.</returns>
		public static bool IsHidden(this CommandInfo cmd) {
			return cmd.Attributes.OfType<HideAttribute>().Any();
		}
		/// <summary>
		/// Gets if the module has a <see cref="HideAttribute"/>.
		/// </summary>
		/// <param name="mod">The module to check.</param>
		/// <returns>True if a hide attribute exists.</returns>
		public static bool IsHidden(this ModuleInfo mod) {
			return mod.Attributes.OfType<HideAttribute>().Any();
		}

		#endregion

		#region RootModule

		/// <summary>
		/// Gets the root module containing this command.
		/// </summary>
		/// <param name="cmd">The command to check the modules of.</param>
		/// <returns>The root module of the command.</returns>
		public static ModuleInfo GetRootModule(this CommandInfo cmd) {
			ModuleInfo mod = cmd.Module;
			while (mod.Parent != null) {
				mod = mod.Parent;
			}
			return mod;
		}
		/// <summary>
		/// Gets the root module containing this module. May return this module.
		/// </summary>
		/// <param name="mod">The module to check the parent modules of.</param>
		/// <returns>The root module of the module if any parents exist, otherwise this module.</returns>
		public static ModuleInfo GetRootModule(this ModuleInfo mod) {
			while (mod.Parent != null) {
				mod = mod.Parent;
			}
			return mod;
		}

		#endregion

		#region DetailsName
		
		public static string GetDetailsName(this CommandInfo cmd) {
			var mod = cmd.GetRootAliasModule();
			if (mod != null)
				return mod.Attributes.OfType<DetailsNameAttribute>().FirstOrDefault()?.Name ?? mod.Group;
			return cmd.Attributes.OfType<DetailsNameAttribute>().FirstOrDefault()?.Name ?? cmd.Aliases.First();
		}
		public static string GetDetailsName(this ModuleInfo mod) {
			mod = mod.GetRootAliasModule();
			return mod.Attributes.OfType<DetailsNameAttribute>().FirstOrDefault()?.Name ?? mod.Group;
		}

		#endregion

		#region ModulePriority

		public static int GetPriority(this ModuleInfo mod) {
			mod = mod.GetRootAliasModule();
			return mod.Attributes.OfType<ModulePriorityAttribute>().FirstOrDefault()?.Priority ?? 0;
		}

		#endregion

		#region BaseAlias

		/// <summary>
		/// Gets the base aliases for the command. If the command has an alias module then it will use the
		/// root alias module's aliases.
		/// </summary>
		/// <param name="cmd">The comand to check.</param>
		/// <returns>The root alias module's aliases, otherwise this command's aliases.</returns>
		public static IEnumerable<string> GetBaseAliases(this CommandInfo cmd) {
			ModuleInfo mod = cmd.GetRootAliasModule();
			return mod?.Aliases ?? cmd.Aliases;
		}
		/// <summary>
		/// Gets the base alias for the command. If the command has an alias module then it will use the root
		/// alias module's alias.
		/// </summary>
		/// <param name="cmd">The comand to check.</param>
		/// <returns>The root alias module's alias, otherwise this alias.</returns>
		public static string GetBaseAlias(this CommandInfo cmd) {
			ModuleInfo mod = cmd.GetRootAliasModule();
			return mod.Attributes.OfType<DetailsNameAttribute>().FirstOrDefault()?.Name ?? mod?.Group ??
				   cmd.Attributes.OfType<DetailsNameAttribute>().FirstOrDefault()?.Name ?? cmd.Aliases.First();
		}
		/*/// <summary>
		/// Gets the base alias for the alias module. If the module has an alias module then it will use the root
		/// alias module's alias.
		/// </summary>
		/// <param name="cmd">The module to check.</param>
		/// <returns>The root alias module's alias, otherwise null.</returns>
		public static string GetBaseAlias(this ModuleInfo mod) {
			mod = mod.GetRootAliasModule();
			if (mod == null)
				return null;
			return mod.Attributes.OfType<DetailsNameAttribute>().FirstOrDefault()?.Name ?? mod?.Group;
		}*/

		#endregion

		#region RootAliasModule

		/// <summary>
		/// Gets this command's root module that uses an alias, meaning it has a <see cref="GroupAttribute"/>.
		/// </summary>
		/// <param name="cmd">The command to check.</param>
		/// <returns>The root module using an alias, or null if none were found.</returns>
		public static ModuleInfo GetRootAliasModule(this CommandInfo cmd) {
			return cmd.Module.GetRootAliasModule();
		}
		/// <summary>
		/// Gets this module's root module that uses an alias, meaning it has a <see cref="GroupAttribute"/>.
		/// </summary>
		/// <param name="mod">The module to check.</param>
		/// <returns>The root module using an alias, or null if none were found.</returns>
		public static ModuleInfo GetRootAliasModule(this ModuleInfo mod) {
			while (mod.Parent?.Group != null)
				mod = mod.Parent;
			return (mod.Group != null ? mod : null);
		}
		/// <summary>
		/// Checks if this command has an alias parent module, meaning it that module has a
		/// <see cref="GroupAttribute"/>.
		/// </summary>
		/// <param name="cmd">The command to check.</param>
		/// <returns>True if a module's <see cref="ModuleInfo.Group"/> parameter is not non-null.</returns>
		public static bool HasAliasModule(this CommandInfo cmd) {
			return cmd.GetRootAliasModule() != null;
		}
		/// <summary>
		/// Checks if this or any parent modules has an alias, meaning it that module has a
		/// <see cref="GroupAttribute"/>.
		/// </summary>
		/// <param name="mod">The module to check.</param>
		/// <returns>True if a module's <see cref="ModuleInfo.Group"/> parameter is not non-null.</returns>
		public static bool HasAliasModule(this ModuleInfo mod) {
			return mod.GetRootAliasModule() != null;
		}
		/// <summary>
		/// Checks if this command module is an alias module, meaning it has a <see cref="GroupAttribute"/>.
		/// </summary>
		/// <param name="mod">The module to check.</param>
		/// <returns>True if the <see cref="ModuleInfo.Group"/> parameter is non-null.</returns>
		public static bool IsAliasModule(this ModuleInfo mod) {
			return mod.Group != null;
		}

		#endregion

		#region IsLockable

		/// <summary>
		/// Gets the farthest up command or module lockable attribute.
		/// </summary>
		/// <param name="cmd">The command to start from.</param>
		/// <returns>The lockable attribute, or null if none exists.</returns>
		public static IsLockableBaseAttribute GetLockable(this CommandInfo cmd) {
			var attr = cmd.Preconditions.OfType<IsLockableBaseAttribute>().FirstOrDefault();
			return attr ?? cmd.Module.GetLockable();
		}
		/// <summary>
		/// Gets the farthest up modulelockable attribute.
		/// </summary>
		/// <param name="mod">The module to start from.</param>
		/// <returns>The lockable attribute, or null if none exists.</returns>
		public static IsLockableBaseAttribute GetLockable(this ModuleInfo mod) {
			IsLockableBaseAttribute attr = null;
			do {
				attr = mod.Preconditions.OfType<IsLockableBaseAttribute>().FirstOrDefault();
				mod = mod.Parent;
			} while (mod != null && attr == null);
			return attr;
		}
		/// <summary>
		/// Gets the farthest up command or module lockable attribute and checks if it's true.
		/// </summary>
		/// <param name="cmd">The command to start from.</param>
		/// <returns>True if the lockable attribute exists and is true, or false if none exists.</returns>
		public static bool IsLockable(this CommandInfo cmd) {
			return (cmd.GetLockable()?.IsLockable ?? false);
		}
		/// <summary>
		/// Gets the farthest up module lockable attribute and checks if it's true.
		/// </summary>
		/// <param name="mod">The module to start from.</param>
		/// <returns>True if the lockable attribute exists and is true, or false if none exists.</returns>
		public static bool IsLockable(this ModuleInfo mod) {
			return (mod.GetLockable()?.IsLockable ?? false);
		}
		/// <summary>
		/// Gets the farthest up command or module lockable attribute and checks if it's locked by default.
		/// </summary>
		/// <param name="cmd">The command to start from.</param>
		/// <returns>True if the lockable attribute exists and it's locked by default, or false if none exists.</returns>
		public static bool IsLockedByDefault(this CommandInfo cmd) {
			return (cmd.GetLockable()?.IsLockedByDefault ?? false);
		}
		/// <summary>
		/// Gets the farthest up module lockable attribute and checks if it's locked by default.
		/// </summary>
		/// <param name="mod">The module to start from.</param>
		/// <returns>True if the lockable attribute exists and it's locked by default, or false if none exists.</returns>
		public static bool IsLockedByDefault(this ModuleInfo mod) {
			return (mod.GetLockable()?.IsLockedByDefault ?? false);
		}
		/// <summary>
		/// Checks if this command module tree has a lockable attibute.
		/// </summary>
		/// <param name="cmd">The command to start from.</param>
		/// <returns>True if the lockable attribute exists.</returns>
		public static bool HasLockable(this CommandInfo cmd) {
			return GetLockable(cmd) != null;
		}
		/// <summary>
		/// Checks if this command module tree has a lockable attibute.
		/// </summary>
		/// <param name="mod">The module to start from.</param>
		/// <returns>True if the lockable attribute exists.</returns>
		public static bool HasLockable(this ModuleInfo mod) {
			return GetLockable(mod) != null;
		}

		#endregion
	}
}
