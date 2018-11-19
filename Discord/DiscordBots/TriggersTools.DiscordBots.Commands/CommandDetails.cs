using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// Details about a single command, or multiple commands that share the same name.
	/// </summary>
	public class CommandDetails {

		#region Builder

		/// <summary>
		/// The <see cref="CommandDetails"/> builder class.
		/// </summary>
		public class Builder {

			public List<CommandInfo> CommandInfos { get; } = new List<CommandInfo>();
			public List<ModuleInfo> ModuleInfos { get; } = new List<ModuleInfo>();
			public List<Example> Examples { get; } = new List<Example>();
			public List<string> Aliases { get; } = new List<string>();
			
			public IServiceProvider Services { get; set; }

			public ModuleDetails Module { get; set; }
			public bool IsLockable { get; set; }
			public bool IsLockedByDefault { get; set; }
			public bool IsModule { get; set; }
			public string FriendlyName { get; set; }
			public string Alias { get; set; }
			public string Usage { get; set; }
			public string Summary { get; set; }
			public string Remarks { get; set; }
			public int Priority { get; set; }
			public string ImageUrl { get; set; }
		}

		#endregion

		#region Fields

		/// <summary>
		/// Gets the service provider used to calculate usability.
		/// </summary>
		public IServiceProvider Services { get; }

		/// <summary>
		/// Gets if this command is a module info. If false, this is a single command info.
		/// </summary>
		public bool IsModule { get; }

		/// <summary>
		/// Gets the parent module of the command.
		/// </summary>
		public ModuleDetails Module { get; }
		/// <summary>
		/// Gets the friendly name of the command.
		/// </summary>
		public string FriendlyName { get; }
		/// <summary>
		/// Gets the primary alias of the command.
		/// </summary>
		public string Alias { get; }
		/// <summary>
		/// Gets the usage of the command.
		/// </summary>
		public string Usage { get; }
		/// <summary>
		/// Gets the summary for the command.
		/// </summary>
		public string Summary { get; }
		/// <summary>
		/// Gets the remarks for the command.
		/// </summary>
		public string Remarks { get; }
		/// <summary>
		/// Gets the Url of the image to display for the command.
		/// </summary>
		public string ImageUrl { get; }
		/// <summary>
		/// Gets the priority of the command.
		/// </summary>
		public int Priority { get; }
		/// <summary>
		/// Gets if the command can be locked directly.
		/// </summary>
		public bool IsLockable { get; }
		/// <summary>
		/// Gets if the command is first locked, when no settings exist.
		/// </summary>
		public bool IsLockedByDefault { get; set; }
		/// <summary>
		/// Gets the aliases of the command.
		/// </summary>
		public IReadOnlyList<string> Aliases { get; }
		/// <summary>
		/// Gets the examples for the command.
		/// </summary>
		public IReadOnlyList<Example> Examples { get; }

		/// <summary>
		/// Gets the module infos that this command encompasses.
		/// </summary>
		public IReadOnlyList<ModuleInfo> ModuleInfos { get; }
		/// <summary>
		/// Gets the command infos that this command encompasses.
		/// </summary>
		public IReadOnlyList<CommandInfo> CommandInfos { get; }

		#endregion

		#region Constructors
		
		/// <summary>
		/// Constructs the <see cref="CommandDetails"/>.
		/// </summary>
		public CommandDetails(Builder builder) {
			Services = builder.Services ?? throw new ArgumentNullException(nameof(Builder.Services));
			IsModule = builder.IsModule;
			Module = builder.Module;
			FriendlyName = builder.FriendlyName ?? builder.Alias;
			Alias = builder.Alias ?? throw new ArgumentNullException(nameof(Builder.Alias));
			Usage = builder.Usage;
			Summary = builder.Summary;
			Remarks = builder.Remarks;
			ImageUrl = builder.ImageUrl;
			Priority = builder.Priority;
			IsLockable = builder.IsLockable;
			IsLockedByDefault = builder.IsLockedByDefault;
			Aliases = builder.Aliases.ToImmutableArray();
			Examples = builder.Examples.ToImmutableArray();
			ModuleInfos = builder.ModuleInfos.ToImmutableArray();
			CommandInfos = builder.CommandInfos.ToImmutableArray();
		}

		#endregion

		#region IsUsable

		/// <summary>
		/// Gets if the command is usable in the specified context.
		/// </summary>
		/// <param name="context">The command context.</param>
		/// <returns>True if the command is usable.</returns>
		public bool IsUsable(ICommandContext context) {
			return CommandInfos.Any(c => c.CheckPreconditionsAsync(context, Services).GetAwaiter().GetResult().IsSuccess);
		}

		#endregion
	}

	/// <summary>
	/// Details about a command module.
	/// </summary>
	public class ModuleDetails : IReadOnlyList<CommandDetails> {

		#region Builder

		/// <summary>
		/// The <see cref="ModuleDetails"/> builder class.
		/// </summary>
		public class Builder {
			public List<CommandDetails.Builder> Commands { get; } = new List<CommandDetails.Builder>();
			public CommandSetDetails CommandService { get; set; }
			public string Name { get; set; }
			public string Summary { get; set; }
			public string Remarks { get; set; }
			public bool IsLockable { get; set; }
			public bool IsLockedByDefault { get; set; }
			public ModuleInfo ModuleInfo { get; set; }
		}

		#endregion

		#region Fields
		
		/// <summary>
		/// Gets the name of this module.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Gets the summary for this module.
		/// </summary>
		public string Summary { get; }
		/// <summary>
		/// Gets the remarks for this module.
		/// </summary>
		public string Remarks { get; }
		/// <summary>
		/// Gets if the module can be locked directly.
		/// </summary>
		public bool IsLockable { get; }
		/// <summary>
		/// Gets if the module is first locked, when no settings exist.
		/// </summary>
		public bool IsLockedByDefault { get; set; }

		/// <summary>
		/// Gets the module info assocaited with this module.
		/// </summary>
		public ModuleInfo ModuleInfo { get; }
		/// <summary>
		/// Gets the list of command details in this module.
		/// </summary>
		public IReadOnlyList<CommandDetails> Commands { get; }
		/// <summary>
		/// Gets the commands that are lockable in this module.
		/// </summary>
		public IEnumerable<CommandDetails> LockableCommands => Commands.Where(c => c.IsLockable);
		/// <summary>
		/// Gets the commands that are locked by default in this module.
		/// </summary>
		public IEnumerable<CommandDetails> LockedByDefaultCommands => Commands.Where(c => c.IsLockedByDefault);

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="ModuleDetails"/>.
		/// </summary>
		public ModuleDetails(Builder builder) {
			var commands = builder.Commands.ToList();
			commands.Sort((a, b) => string.Compare(a.Alias, b.Alias, true));
			foreach (var commandBuilder in commands)
				commandBuilder.Module = this;
			Commands = commands.Select(b => new CommandDetails(b)).ToImmutableArray();

			ModuleInfo = builder.ModuleInfo;
			Name = builder.Name ?? throw new ArgumentNullException(nameof(Builder.Name));
			Summary = builder.Summary;
			Remarks = builder.Remarks;
			IsLockable = builder.IsLockable;
			IsLockedByDefault = builder.IsLockedByDefault;
		}

		#endregion

		#region Find

		/// <summary>
		/// Finds the command with the specified command info.
		/// </summary>
		/// <param name="cmd">The command info to search with.</param>
		/// <param name="context">The optional command context to search with permissions.</param>
		/// <returns>The located command, or null.</returns>
		public CommandDetails Find(CommandInfo cmd, ICommandContext context = null) {
			return Find(cmd.GetDetailsName(), context);
			/*return Commands.Where(c => c.CommandInfos.Contains(cmd))
						   .Where(c => context == null || c.IsUsable(context))
						   .FirstOrDefault();*/
		}
		/// <summary>
		/// Finds the command with the specified alias.
		/// </summary>
		/// <param name="alias">The alias to search with.</param>
		/// <param name="context">The optional command context to search with permissions.</param>
		/// <returns>The located command, or null.</returns>
		public CommandDetails Find(string alias, ICommandContext context = null) {
			return Commands.Where(c => c.Aliases.Any(a => string.Compare(alias, a, true) == 0))
						   .Where(c => context == null || c.IsUsable(context))
						   .OrderByDescending(c => c.Priority)
						   .FirstOrDefault();
		}
		/// <summary>
		/// Finds the command with the specified predicate
		/// </summary>
		/// <param name="match">The predicate to search with.</param>
		/// <param name="context">The optional command context to search with permissions.</param>
		/// <returns>The located command, or null.</returns>
		public CommandDetails Find(Predicate<CommandDetails> match, ICommandContext context = null) {
			return Commands.Where(c => match(c))
						   .Where(c => context == null || c.IsUsable(context))
						   .OrderByDescending(c => c.Priority)
						   .FirstOrDefault();
		}

		#endregion

		#region GetUsableCommands

		/// <summary>
		/// Enumerates all usable commands in the module.
		/// </summary>
		/// <param name="context">The command context.</param>
		/// <returns>An enumerable of usable commands.</returns>
		public IEnumerable<CommandDetails> GetUsableCommands(ICommandContext context) {
			return Commands.Where(c => c.IsUsable(context));
		}

		#endregion

		#region List Implementation

		/// <summary>
		/// Gets the number of commands in the module.
		/// </summary>
		public int Count => Commands.Count;
		/// <summary>
		/// Gets the command at the specified index in the module list.
		/// </summary>
		/// <param name="index">The index of the command.</param>
		/// <returns>The command at the specified index.</returns>
		public CommandDetails this[int index] => Commands[index];
		/// <summary>
		/// Gets the enumerator for the command module.
		/// </summary>
		public IEnumerator<CommandDetails> GetEnumerator() => Commands.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Commands.GetEnumerator();

		#endregion
	}

	/// <summary>
	/// A collection of module details and command details available in a command service.
	/// </summary>
	public class CommandSetDetails : IReadOnlyList<ModuleDetails> {
		
		#region Fields
		
		/// <summary>
		/// The list of modules available in the bot.
		/// </summary>
		public IReadOnlyList<ModuleDetails> Modules { get; }
		/// <summary>
		/// Gets the command service containing these modules.
		/// </summary>
		public CommandService CommandService { get; }

		/// <summary>
		/// Gets the commands that are lockable in this command set.
		/// </summary>
		public IEnumerable<CommandDetails> LockableCommands {
			get => Modules.SelectMany(m => m.Commands).Where(c => c.IsLockable);
		}
		/// <summary>
		/// Gets the modules that are lockable in this command set.
		/// </summary>
		public IEnumerable<ModuleDetails> LockableModules {
			get => Modules.Where(m => m.IsLockable);
		}
		/// <summary>
		/// Gets the commands that are locked by default in this command set.
		/// </summary>
		public IEnumerable<CommandDetails> LockedByDefaultCommands {
			get => Modules.SelectMany(m => m.Commands).Where(c => c.IsLockedByDefault);
		}
		/// <summary>
		/// Gets the modules that are locked by default in this command set.
		/// </summary>
		public IEnumerable<ModuleDetails> LockedByDefaultModules {
			get => Modules.Where(m => m.IsLockedByDefault);
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="CommandSetDetails"/>.
		/// </summary>
		/// <param name="commands">The command service to get the commands from.</param>
		/// <param name="services">The service provider.</param>
		public CommandSetDetails(CommandService commands, IServiceProvider services) {
			CommandService = commands;
			Modules = Build(commands, services);
		}

		#endregion

		#region Find

		/// <summary>
		/// Finds the command with the specified command info.
		/// </summary>
		/// <param name="cmd">The command info to search with.</param>
		/// <param name="context">The optional command context to search with permissions.</param>
		/// <returns>The located command, or null.</returns>
		public CommandDetails FindCommand(CommandInfo cmd, ICommandContext context = null) {
			return Modules.Select(g => g.Find(cmd, context))
						  .Where(c => c != null)
						  .FirstOrDefault();
		}
		/// <summary>
		/// Finds the command with the specified alias.
		/// </summary>
		/// <param name="alias">The alias to search with.</param>
		/// <param name="context">The optional command context to search with permissions.</param>
		/// <returns>The located command, or null.</returns>
		public CommandDetails FindCommand(string alias, ICommandContext context = null) {
			return Modules.Select(g => g.Find(alias, context))
						  .Where(c => c != null)
						  .OrderByDescending(c => c.Priority)
						  .FirstOrDefault();
		}
		/// <summary>
		/// Finds the command with the specified predicate
		/// </summary>
		/// <param name="match">The predicate to search with.</param>
		/// <param name="context">The optional command context to search with permissions.</param>
		/// <returns>The located command, or null.</returns>
		public CommandDetails FindCommand(Predicate<CommandDetails> match, ICommandContext context = null) {
			return Modules.Select(g => g.Find(match, context))
						  .Where(c => c != null)
						  .OrderByDescending(c => c.Priority)
						  .FirstOrDefault();
		}
		/// <summary>
		/// Finds the module with the specified module info.
		/// </summary>
		/// <param name="mod">The module info to search with.</param>
		/// <param name="context">The optional command context to search with permissions.</param>
		/// <returns>The located module, or null.</returns>
		public ModuleDetails FindModule(ModuleInfo mod, ICommandContext context = null) {
			return Modules.Where(g => g.ModuleInfo == mod)
						  .Where(g => context == null || g.GetUsableCommands(context).Any())
						  .FirstOrDefault();
		}
		/// <summary>
		/// Finds the module with the specified name.
		/// </summary>
		/// <param name="name">The name to search with.</param>
		/// <param name="context">The optional module context to search with permissions.</param>
		/// <returns>The located module, or null.</returns>
		public ModuleDetails FindModule(string name, ICommandContext context = null) {
			return Modules.Where(g => string.Compare(name, g.Name, true) == 0)
						  .Where(g => context == null || g.GetUsableCommands(context).Any())
						  .FirstOrDefault();
		}
		/// <summary>
		/// Finds the module with the specified predicate
		/// </summary>
		/// <param name="match">The predicate to search with.</param>
		/// <param name="context">The optional module context to search with permissions.</param>
		/// <returns>The located module, or null.</returns>
		public ModuleDetails FindModule(Predicate<ModuleDetails> match, ICommandContext context = null) {
			return Modules.Where(m => match(m))
						  .Where(g => context == null || g.GetUsableCommands(context).Any())
						  .FirstOrDefault();
		}

		#endregion

		#region List Implementation

		/// <summary>
		/// Gets the number of modules in the command service.
		/// </summary>
		public int Count => Modules.Count;
		/// <summary>
		/// Gets the module at the specified index in the command service.
		/// </summary>
		/// <param name="index">The index of the module.</param>
		/// <returns>The module at the specified index.</returns>
		public ModuleDetails this[int index] => Modules[index];
		/// <summary>
		/// Gets the enumerator for the command service.
		/// </summary>
		public IEnumerator<ModuleDetails> GetEnumerator() => Modules.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		#region Build

		/// <summary>
		/// Builds the list of modules.
		/// </summary>
		/// <param name="commands">The command service to get the modules from.</param>
		/// <param name="services"></param>
		/// <returns>The built list of modules.</returns>
		private IReadOnlyList<ModuleDetails> Build(CommandService commands, IServiceProvider services) {
			var modules = new List<ModuleDetails>();
			foreach (ModuleInfo mod in commands.Modules) {
				if (!mod.IsSubmodule && !mod.IsHidden())
					modules.Add(BuildModule(mod, services));
			}

			modules.Sort((a, b) => string.Compare(a.Name, b.Name, true));
			return modules.ToImmutableArray();
		}
		private ModuleDetails BuildModule(ModuleInfo mod, IServiceProvider services) {
			var builder = new ModuleDetails.Builder {
				ModuleInfo = mod,
				CommandService = this,
				Name = mod.Name,
				Summary = mod.Summary,
				Remarks = mod.Remarks,
				IsLockable = mod.IsLockable(),
				IsLockedByDefault = mod.IsLockedByDefault(),
			};
			foreach (ModuleInfo submod in mod.Submodules) {
				if (submod.HasAliasModule() && !submod.IsHidden())
					builder.Commands.Add(BuildCommand(submod, services));
			}
			foreach (CommandInfo cmd in mod.Commands) {
				if (!cmd.HasAliasModule() && !cmd.IsHidden())
					builder.Commands.Add(BuildCommand(cmd, services));
			}
			builder.Commands.Sort((a, b) => string.Compare(a.Alias, b.Alias, true));
			return new ModuleDetails(builder);
		}

		private CommandDetails.Builder BuildCommand(CommandInfo cmd, IServiceProvider services) {
			var builder = new CommandDetails.Builder {
				Services = services,
				Alias = cmd.GetDetailsName(),
				Summary = cmd.Summary,
				Remarks = cmd.Remarks,
				ImageUrl = cmd.GetPreview(),
				Priority = cmd.Priority,
				Usage = cmd.GetUsage(),
				IsLockable = cmd.IsLockable(),
				IsLockedByDefault = cmd.IsLockedByDefault(),
				IsModule = false,
			};
			builder.Examples.AddRange(cmd.GetExamples());
			builder.Aliases.AddRange(cmd.Aliases);
			builder.CommandInfos.Add(cmd);
			return builder;
		}
		private CommandDetails.Builder BuildCommand(ModuleInfo submod, IServiceProvider services) {
			var builder = new CommandDetails.Builder {
				Services = services,
				Alias = submod.GetDetailsName(),
				Summary = submod.Summary,
				Remarks = submod.Remarks,
				ImageUrl = submod.GetPreview(),
				Priority = submod.GetPriority(),
				Usage = submod.GetUsage(),
				IsLockable = submod.IsLockable(),
				IsLockedByDefault = submod.IsLockedByDefault(),
				IsModule = true,
			};
			builder.Examples.AddRange(submod.GetExamples());
			builder.Aliases.AddRange(submod.Aliases);
			BuildCommandSubmodules(builder, submod);
			return builder;
		}
		private void BuildCommandSubmodules(CommandDetails.Builder builder, ModuleInfo mod) {
			builder.ModuleInfos.Add(mod);
			foreach (ModuleInfo submod in mod.Submodules) {
				BuildCommandSubmodules(builder, submod);
			}
			foreach (CommandInfo cmd in mod.Commands) {
				builder.Examples.AddRange(cmd.GetExamples());
				builder.CommandInfos.Add(cmd);
				//builder.Priority = Math.Min(builder.Priority, cmd.Priority);
			}
		}

		#endregion
	}
}
