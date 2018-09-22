using Discord;
using Discord.Commands;
using EnigmaBot.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaBot.Services {
	public enum MessageCountScope {
		Channel,
		Guild,
	}

	public enum MessageCountFilter {
		None,
		User,
	}

	public class CommandGroup : List<CommandInfo> {
		public string Name { get; }

		public CommandGroup() {
			Name = "Misc";
		}
		public CommandGroup(string name) {
			Name = name;
		}
	}

	public class HelpService : BotServiceBase {


		public List<CommandInfo> GetAllCommands() {
			return Commands.Commands.ToList();
		}

		public List<CommandGroup> GetAllCommandGroups() {
			Dictionary<string, CommandGroup> groups =
				new Dictionary<string, CommandGroup>();
			CommandGroup group = null;
			foreach (CommandInfo cmd in Commands.Commands) {
				// See if we're still using the same group
				if ((group == null || group.Name != cmd.RootModule().Name) &&
					!groups.TryGetValue(cmd.Module.Name, out group)) {
					group = new CommandGroup(cmd.Module.Name);
					groups.Add(group.Name, group);
				}
				group.Add(cmd);
			}
			return groups.Values.ToList();
		}

		public void FilterCommands(List<CommandInfo> commands,
			Func<CommandInfo, bool> shouldKeep) {
			for (int i = 0; i < commands.Count; i++) {
				CommandInfo cmd = commands[i];
				if (!shouldKeep.Invoke(cmd)) {
					commands.RemoveAt(i);
					i--;
				}
			}
		}

		public void FilterCommandGroups(List<CommandGroup> groups,
			Func<CommandInfo, bool> shouldKeep) {
			foreach (CommandGroup group in groups) {
				for (int i = 0; i < group.Count; i++) {
					CommandInfo cmd = group[i];
					if (!shouldKeep.Invoke(cmd)) {
						group.RemoveAt(i);
						i--;
					}
				}
			}
		}

		public async Task<List<CommandInfo>> GetUsableCommands(
			SocketCommandContext context, Func<CommandInfo, bool> shouldKeep = null) {
			List<CommandInfo> commands = GetAllCommands();
			for (int i = 0; i < commands.Count; i++) {
				CommandInfo cmd = commands[i];
				var result = await cmd.CheckPreconditionsAsync(context);
				if (!result.IsSuccess) {
					commands.RemoveAt(i);
					i--;
				}
			}
			if (shouldKeep != null)
				FilterCommands(commands, shouldKeep);
			return commands;
		}

		public async Task<List<CommandGroup>> GetUsableCommandGroups(
			SocketCommandContext context, Func<CommandInfo, bool> shouldKeep = null) {
			List<CommandGroup> groups = GetAllCommandGroups();
			foreach (CommandGroup group in groups) {
				for (int i = 0; i < group.Count; i++) {
					CommandInfo cmd = group[i];
					var result = await cmd.CheckPreconditionsAsync(context);
					if (!result.IsSuccess) {
						group.RemoveAt(i);
						i--;
					}
				}
			}
			if (shouldKeep != null)
				FilterCommandGroups(groups, shouldKeep);
			return groups;
		}

		public void SearchCommands(List<CommandInfo> commands, string search) {
			string[] searchArgs = search.Split(new char[] { ' ', '\t' },
				StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < commands.Count; i++) {
				CommandInfo cmd = commands[i];
				bool aliasMatched = false;
				foreach (string alias in cmd.Aliases) {
					aliasMatched = true;
					string[] cmdArgs = alias.Split(new char[] { ' ', '\t' },
						StringSplitOptions.RemoveEmptyEntries);
					if (searchArgs.Length > cmdArgs.Length) {
						aliasMatched = false;
						continue;
					}
					for (int j = 0; j < searchArgs.Length; j++) {
						if (string.Compare(searchArgs[j], cmdArgs[j], true) != 0) {
							aliasMatched = false;
							break;
						}
					}
					if (aliasMatched)
						break;
				}
				if (!aliasMatched) {
					commands.RemoveAt(i);
					i--;
				}
			}
		}

		public CommandInfo GetCommand(ref string command) {
			command = string.Join(' ', command.Split(new char[] { ' ', '\t' },
				StringSplitOptions.RemoveEmptyEntries));
			foreach (CommandInfo cmd in Commands.Commands) {
				foreach (string alias in cmd.Aliases) {
					if (string.Compare(command, alias, true) == 0)
						return cmd;
				}
			}
			return null;
		}

		public ModuleInfo GetModule(ref string module) {
			foreach (ModuleInfo mod in Commands.Modules) {
				if (string.Compare(module, mod.Name, true) == 0) {
					module = mod.Name;
					return mod;
				}
			}
			return null;
		}

		public async Task ListCommands(SocketCommandContext context, List<CommandGroup> groups) {
			string prefix = Config["prefix"];
			var builder = new EmbedBuilder() {
				Color = new Color(114, 137, 218),
				Description = "These are the commands you can use"
			};

			foreach (CommandGroup group in groups) {
				string description = null;
				foreach (CommandInfo cmd in group) {
					description += $"{prefix}{cmd.Aliases.First()}\n";
				}

				if (!string.IsNullOrWhiteSpace(description)) {
					builder.AddField(x => {
						x.Name = group.Name;
						x.Value = description;
						x.IsInline = false;
					});
				}
			}

			await context.Channel.SendMessageAsync("", false, builder.Build());
		}


		/*public async Task GetMessageCountTask(SocketCommandContext context,
			MessageCountScope scope, MessageCountFilter filter, ulong filterId = 0)
		{
			if (scope == MessageCountScope.Channel) {

			}
			else if (scope == MessageCountScope.Guild) {

			}
		}

		public async Task<List<Tuple<string, int>>> CountGuildMessages(SocketCommandContext context,
			MessageCountScope scope, MessageCountFilter filter, ulong filterId = 0)
		{
			List<Tuple<string, int>> channelCounts = new List<Tuple<string, int>>();
			foreach (IChannel baseChannel in context.Guild.Channels) {
				if (baseChannel is ITextChannel channel) {
					channelCounts.Add(await CountChannelMessages(context, channel, filter, filterId));
				}
			}
		}

		public async Task<Tuple<string, int>> CountChannelMessages(SocketCommandContext context,
			ITextChannel channel, MessageCountFilter filter,
			ulong filterId = 0)
		{
			int count = 0;
			IMessage last = null;
			var enumerable = await channel.GetMessagesAsync().FlattenAsync();
			while (enumerable.Any()) {
				if (filter == MessageCountFilter.None) {
					count += enumerable.Count();
					last = enumerable.Last();
				}
				else if (filter == MessageCountFilter.User) {
					foreach (IMessage msg in enumerable) {
						if (msg.Author.Id == filterId) {
							count++;
							last = msg;
						}
					}
				}
				
				enumerable = await channel.GetMessagesAsync(last, Direction.Before).FlattenAsync();
			}
			return new Tuple<string, int>(channel.Name, count);
		}

		public async Task MessageCountStatus

		[Command("msgcount"), Alias("messagecount")]
		[Summary("Gets the total message count for the current channel")]
		[RequireOwner]
		[RequireContext(ContextType.DM | ContextType.Group)]
		public async Task GetMessageCountOwner(SocketCommandContext context) {
			Stopwatch watch = Stopwatch.StartNew();
			IMessage message = await context.Channel.SendMessageAsync("Getting channel message count. This may take awhile...");
			int count = 0;
			var enumerable = await context.Channel.GetMessagesAsync().FlattenAsync();
			while (enumerable.Any()) {
				count += enumerable.Count();
				IMessage last = enumerable.Last();
				enumerable = await context.Channel.GetMessagesAsync(last, Direction.Before).FlattenAsync();
			}
			await message.DeleteAsync();
			await context.Channel.SendMessageAsync($"**Channel Message Count:** {count}\n\n*Took {watch.Elapsed.ToDHMSString()} to finish*");
		}
		
		public async Task GetGuildMessageCount(SocketCommandContext context) {
			Stopwatch watch = Stopwatch.StartNew();
			IMessage message = await context.Channel.SendMessageAsync("Getting guild message count. This may take awhile...");
			int count = 0;
			foreach (var baseChannel in context.Guild.Channels) {
				if (baseChannel is ITextChannel channel) {
					var enumerable = await channel.GetMessagesAsync().FlattenAsync();
					while (enumerable.Any()) {
						count += enumerable.Count();
						IMessage last = enumerable.Last();
						enumerable = await channel.GetMessagesAsync(last, Direction.Before).FlattenAsync();
					}
				}
			}
			await message.DeleteAsync();
			await context.Channel.SendMessageAsync($"**Total Message Count:** {count}\n\n*Took {watch.Elapsed.ToDHMSString()} to finish*");
		}*/
	}
}
