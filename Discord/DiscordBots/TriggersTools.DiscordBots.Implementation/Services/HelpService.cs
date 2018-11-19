using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Database.Model;
using TriggersTools.DiscordBots.Extensions;
using TriggersTools.DiscordBots.Reactions;
using TriggersTools.DiscordBots.Utils;

namespace TriggersTools.DiscordBots.Services {
	/// <summary>
	/// The result of a ping test.
	/// </summary>
	public struct PingResult {
		/// <summary>
		/// Gets or sets the latency.
		/// </summary>
		public int Latency { get; set; }
		/// <summary>
		/// Gets or sets the initial time it takes to send a message. null is unset.
		/// </summary>
		public int? Init { get; set; }
		/// <summary>
		/// Gets or sets the time it takes a sent message to return. null is unset, or calculating when
		/// <see cref="Init"/> is non-null.
		/// </summary>
		public int? Rtt { get; set; }

		public override string ToString() {
			string latency = $"{Latency}ms";
			string init = (Init.HasValue ? $"{Init.Value}ms" : "---");
			string rtt  = (Rtt.HasValue  ? $"{Rtt.Value}ms"  : (Init.HasValue ? "calculating" : "---"));
			return $"Heartbeat: {latency}, init: {init}, rtt: {rtt}";
		}
		public string Format() {
			string latency = $"{Latency}ms";
			string init = (Init.HasValue ? $"{Init.Value}ms" : "---");
			string rtt  = (Rtt.HasValue  ? $"{Rtt.Value}ms"  : (Init.HasValue ? "calculating" : "---"));
			return $"**Heartbeat:** `{latency}`, **init:** `{init}`, **rtt:** `{rtt}`";
		}
	}
	/// <summary>
	/// The services for getting information on commands and modules.
	/// </summary>
	public class HelpService : DiscordBotService {

		#region Constants

		public const string AboutTitle = "$NICKNAME$";
		public const string HelpTitle = "$NICKNAME$: Commands";
		public const string LockedTitle = "$NICKNAME$: Locked Commands";
		public const string LockableTitle = "$NICKNAME$: Lockable Commands";
		public const string ReactionsTitle = "$NICKNAME$: Reactions";
		public const string AboutDesc = null;
		public const string HelpDesc = "These are the commands you can use";
		public const string LockedDesc = "These are the commands that are locked and cannot be used. Items prefixed with `🔒` are locked directly";
		public const string LockableDesc = "These are the commands that can be locked. Items prefixed with `🔓` can be locked directly";
		public const string ReactionsDesc = "The following reactions are used to interact with the bot";

		#endregion

		#region Fields

		private readonly ConfigParserService configParser;
		private readonly ReactionService reactions;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="HelpService"/>.
		/// </summary>
		public HelpService(DiscordBotServiceContainer services,
						   ConfigParserService configParser,
						   ReactionService reactions)
			: base(services)
		{
			this.configParser = configParser;
			this.reactions = reactions;
		}

		#endregion

		/// <summary>
		/// Pings Discord with a message and uses those messages to determine the ping result.<para/>
		/// Note that this will not return valid values if the bot is currently being rate-limited.
		/// </summary>
		/// <param name="context">The command context to send the messages to.</param>
		/// <returns>The calculated ping result.</returns>
		public async Task<PingResult> PingResultAsync(ICommandContext context) {
			PingResult ping = new PingResult {
				Latency = Client.Latency,
			};
			RequestOptions rateLimited = new RequestOptions {
				RetryMode = RetryMode.AlwaysRetry & ~RetryMode.RetryRatelimit,
			};
			Stopwatch watch = Stopwatch.StartNew();
			try {
				IUserMessage message = await context.Channel.SendMessageAsync(ping.Format(), options: rateLimited).ConfigureAwait(false);
				ping.Init = (int) watch.ElapsedMilliseconds;
				await message.ModifyAsync(x => x.Content = ping.Format(), options: rateLimited).ConfigureAwait(false);
				ping.Rtt = (int) watch.ElapsedMilliseconds;
				watch.Stop();
				await message.ModifyAsync(x => x.Content = ping.Format(), options: rateLimited).ConfigureAwait(false);
			} catch (RateLimitedException) {
				await context.Channel.SendMessageAsync("**Ping Failed:** Rate limit was triggered").ConfigureAwait(false);
			}
			return ping;
		}

		private const string Locked = "`🔒` ";
		private const string NotLocked = "`❌` ";
		
		/// <summary>
		/// Formats a command to show the locked symbol when it or its module is locked.
		/// </summary>
		private string FormatLocked(IDbLockableContext lockContext, CommandDetails command) {
			return (command.IsLocked(lockContext) ? Locked : string.Empty);
		}
		/// <summary>
		/// Formats a module to show the locked symbol when it is locked.
		/// </summary>
		private string FormatLocked(IDbLockableContext lockContext, ModuleDetails module) {
			return (module.IsModuleLocked(lockContext) ? Locked : string.Empty);
		}
		/// <summary>
		/// Formats a command to show the locked or not locked symbol when it is locked.
		/// </summary>
		private string FormatCommandLocked(IDbLockableContext lockContext, CommandDetails command) {
			return (command.IsCommandLocked(lockContext) ? Locked : NotLocked);
		}
		/// <summary>
		/// Formats a module to show the locked or not locked symbol when it is locked.
		/// </summary>
		private string FormatModuleLocked(IDbLockableContext lockContext, ModuleDetails module) {
			return (module.IsModuleLocked(lockContext) ? Locked : NotLocked);
		}

		/// <summary>
		/// Formats the title for a command embed.
		/// </summary>
		private string FormatCommandTitle(string prefix, IDbLockableContext lockContext, CommandDetails cmd) {
			return $"{configParser.EmbedPrefix}{FormatLocked(lockContext, cmd)}**{prefix}{cmd.Alias}**";
		}
		/// <summary>
		/// Formats the list of command aliases.
		/// </summary>
		private string FormatCommandAliases(CommandDetails cmd) {
			return string.Join(", ", cmd.Aliases.Skip(1).Select(a => $"*{a}*"));
		}
		/// <summary>
		/// Formats the command usage.
		/// </summary>
		private string FormatCommandUsage(string prefix, CommandDetails cmd) {
			if (string.IsNullOrWhiteSpace(cmd.Usage))
				return FormatCommand($"{prefix}{cmd.Alias}");
			return FormatCommand($"{prefix}{cmd.Alias} {cmd.Usage}");
		}
		/// <summary>
		/// Formats the command with a markdown style.
		/// </summary>
		private string FormatCommand(string text) => $"*{text}*";

		/// <summary>
		/// Formats the list of command examples.
		/// </summary>
		private string FormatCommandExamples(string prefix, CommandDetails cmd) {
			string[] examples = new string[cmd.Examples.Count];
			for (int i = 0; i < cmd.Examples.Count; i++) {
				Example example = cmd.Examples[i];
				/*if (string.IsNullOrWhiteSpace(example.Execution)) {
					if (string.IsNullOrWhiteSpace(example.Explanation)) {
						examples[i] = $"‣ {FormatCommand($"{prefix}{cmd.Alias}")}";
					}
					else {
						examples[i] = $"‣ {FormatCommand($"{prefix}{cmd.Alias}")} **-** {example.Explanation}";
					}
				}
				else {*/
					if (string.IsNullOrWhiteSpace(example.Explanation)) {
						examples[i] = $"‣ {FormatCommand($"{prefix}{example.Execution}")}";
					}
					else {
						examples[i] = $"‣ {FormatCommand($"{prefix}{example.Execution}")} **-** {example.Explanation}";
					}
				//}
			}
			return string.Join("\n", examples);
		}
		/// <summary>
		/// Gets the prefix and lock context of the bot.
		/// </summary>
		private async Task<(string prefix, IDbLockableContext lockContext)> GetPrefixAndLockContextAsync(
			ICommandContext context)
		{
			using (var db = Contexting.GetCommandContextDb()) {
				IDbPrefixContext prefixContext = await Contexting.FindDbPrefixContextAsync(db, context, false).ConfigureAwait(false);
				string prefix = prefixContext.GetPrefix(Contexting);
				if (prefixContext is IDbLockableContext lockContext)
					return (prefix, lockContext);
				else
					return (prefix, await Contexting.FindDbLockableContextAsync(db, context, false).ConfigureAwait(false));
			}
		}

		/// <summary>
		/// Builds an embed displaying command help.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <param name="command">The command to build the help for.</param>
		/// <returns>The built embed.</returns>
		public async Task<Embed> BuildCommandHelpAsync(ICommandContext context, CommandDetails command) {
			var (prefix, lockContext) = await GetPrefixAndLockContextAsync(context).ConfigureAwait(false);
			var embed = new EmbedBuilder {
				Color = configParser.EmbedColor,
				Title = FormatCommandTitle(prefix, lockContext, command),
				Description = command.Summary,
			};
			if (command.Aliases.Count > 1)
				embed.AddField("Aliases", FormatCommandAliases(command));
			
			embed.AddField("Usage", FormatCommandUsage(prefix, command));

			if (command.Examples.Any())
				embed.AddField("Examples", FormatCommandExamples(prefix, command));

			if (!string.IsNullOrWhiteSpace(command.Remarks))
				embed.AddField("Remarks", command.Remarks);

			if (!string.IsNullOrWhiteSpace(command.ImageUrl))
				embed.WithImageUrl(command.ImageUrl);
			
			return embed.Build();
		}

		private string FormatModuleTitle(IDbLockableContext lockContext, ModuleDetails module) {
			return $"{configParser.EmbedPrefix}{FormatLocked(lockContext, module)}**{module.Name} Module**";
		}

		/// <summary>
		/// Builds an embed displaying module help and command list.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <param name="module">The module to build the help for.</param>
		/// <returns>The built embed.</returns>
		public async Task<Embed> BuildModuleHelpAsync(ICommandContext context, ModuleDetails module, bool showAll) {
			var (prefix, lockContext) = await GetPrefixAndLockContextAsync(context).ConfigureAwait(false);
			var embed = new EmbedBuilder {
				Color = configParser.EmbedColor,
				Title = FormatModuleTitle(lockContext, module),
				Description = module.Summary,
			};

			List<CommandDetails> usableCommands = (showAll ? module : module.GetUsableCommands(context)).ToList();
			embed.AddField("Commands", usableCommands.Any() ? FormatCommandList(usableCommands) : "*(no usable commands)*");

			if (!string.IsNullOrWhiteSpace(module.Remarks))
				embed.AddField("Remarks", module.Remarks);
			

			return embed.Build();
		}

		private string FormatCommandList(IEnumerable<CommandDetails> commands) {
			return string.Join(" **|** ", commands.Select(c => c.Alias));
		}

		private string FormatDescription(string prefix) {
			StringBuilder str = new StringBuilder();
			str.AppendLine(configParser.ParseDescription("help", HelpDesc));
			str.Append($"The command prefix is: `{prefix}`");
			return str.ToString();
		}

		public async Task<Embed> BuildHelpListAsync(ICommandContext context, CommandSetDetails commandSet) {
			string prefix = await Contexting.GetPrefixAsync(context).ConfigureAwait(false);
			var embed = new EmbedBuilder() {
				Color = configParser.EmbedColor,
				Title = $"{configParser.EmbedPrefix}{configParser.ParseTitle("help", HelpTitle)}",
				Description = FormatDescription(prefix),
			};

			foreach (ModuleDetails module in commandSet) {
				List<CommandDetails> usableCommands = module.GetUsableCommands(context).ToList();
				if (usableCommands.Any())
					embed.AddField(module.Name, FormatCommandList(usableCommands));
			}

			//embed.WithThumbnailUrl(Client.CurrentUser.GetAvatarUrl());

			return embed.Build();
		}

		public Embed BuildLockableList(ICommandContext context, CommandSetDetails commandSet) {
			var embed = new EmbedBuilder() {
				Color = configParser.EmbedColor,
				Title = $"{configParser.EmbedPrefix}{configParser.ParseTitle("lockable", LockableTitle)}",
				Description = configParser.ParseDescription("lockable", LockableDesc),
			};
			foreach (ModuleDetails module in commandSet) {
				/*StringBuilder str = new StringBuilder();
				foreach (CommandDetails command in module) {
					string commandLocked = FormatCommandLocked(lockContext, command);
					if (!string.IsNullOrEmpty(moduleLocked) || !string.IsNullOrEmpty(commandLocked)) {
						str.AppendLine($"{commandLocked}{command.Alias}");
					}
				}*/
				string lockedCommands = FormatLockableCommandList(module.IsLockable, module);
				if (lockedCommands.Length > 0) {
					embed.AddField($"{FormatLockable(module)}{module.Name}", lockedCommands);
				}
			}
			return embed.Build();
		}
		public async Task<Embed> BuildLockedListAsync(ICommandContext context, CommandSetDetails commandSet) {
			IDbLockableContext lockContext = await Contexting.FindDbLockableContextAsync(context).ConfigureAwait(false);
			var embed = new EmbedBuilder() {
				Color = configParser.EmbedColor,
				Title = $"{configParser.EmbedPrefix}{configParser.ParseTitle("locked", LockedTitle)}",
				Description = configParser.ParseDescription("locked", LockedDesc),
			};
			foreach (ModuleDetails module in commandSet) {
				bool moduleLocked = module.IsModuleLocked(lockContext);
				/*StringBuilder str = new StringBuilder();
				foreach (CommandDetails command in module) {
					string commandLocked = FormatCommandLocked(lockContext, command);
					if (!string.IsNullOrEmpty(moduleLocked) || !string.IsNullOrEmpty(commandLocked)) {
						str.AppendLine($"{commandLocked}{command.Alias}");
					}
				}*/
				string lockedCommands = FormatLockedCommandList(lockContext, moduleLocked, module);
				if (lockedCommands.Length > 0) {
					embed.AddField($"{FormatModuleLocked(moduleLocked)}{module.Name}", lockedCommands);
				}
			}
			return embed.Build();
		}

		private IEnumerable<string> FormatLockedCommands(IDbLockableContext lockContext, bool moduleLocked, IEnumerable<CommandDetails> commands) {
			foreach (CommandDetails command in commands) {
				bool commandLocked = command.IsCommandLocked(lockContext);
				if (moduleLocked || commandLocked)
					yield return $"{FormatCommandLocked(commandLocked)}{command.Alias}";
			}
		}
		private string FormatLockedCommandList(IDbLockableContext lockContext, bool moduleLocked, IEnumerable<CommandDetails> commands) {
			return string.Join(" **|** ", FormatLockedCommands(lockContext, moduleLocked, commands));
		}

		private const string Lockable = "`🔓` ";
		private const string NotLockable = "`❌` ";

		private string FormatCommandLocked(bool locked) {
			return (locked ? Locked : NotLocked);
		}
		private string FormatModuleLocked(bool locked) {
			return (locked ? Locked : NotLocked);
		}
		private string FormatLockable(CommandDetails command) {
			return (command.IsLockable ? Lockable : NotLockable);
		}
		private string FormatLockable(ModuleDetails module) {
			return (module.IsLockable ? Lockable : NotLockable);
		}

		private IEnumerable<string> FormatLockableCommands(bool moduleLockable, IEnumerable<CommandDetails> commands) {
			foreach (CommandDetails command in commands) {
				if (moduleLockable || command.IsLockable)
					yield return $"{FormatLockable(command)}{command.Alias}";
			}
		}
		private string FormatLockableCommandList( bool moduleLockable, IEnumerable<CommandDetails> commands) {
			return string.Join(" **|** ", FormatLockableCommands(moduleLockable, commands));
		}


		#region Build Reactions

		/// <summary>
		/// Builds an embed with a list of all reactions and their information.
		/// </summary>
		public Embed BuildReactionList() {
			var embed = new EmbedBuilder() {
				Color = configParser.EmbedColor,
				Title = $"{configParser.EmbedPrefix}{configParser.ParseTitle("reactions", ReactionsTitle)}",
				Description = configParser.ParseDescription("reactions", ReactionsDesc),
			};

			foreach (ReactionCategoryInfo category in reactions.Categories) {
				string name = category.Name;
				string description = string.Join("\n", category.Reactions.Select(r => $"‣ {r.Emote} **-** {r.Description}"));

				embed.AddField(category.Name, description);
			}
			return embed.Build();
		}

		#endregion

		#region Build About

		public async Task<Embed> BuildAboutEmbedAsync(ICommandContext context, Func<EmbedBuilder, Task> addFields) {
			var embed = new EmbedBuilder {
				Color = configParser.EmbedColor,
				Title = $"{configParser.EmbedPrefix}{configParser.ParseTitle("about", AboutTitle)}",
			};
			//embed.WithThumbnailUrl(Client.CurrentUser.GetAvatarUrl());

			embed.Description = configParser.ParseLinks("about");
			string aboutDesc = configParser.ParseDescription("about", AboutDesc);
			if (aboutDesc != null)
				embed.AddField("About", aboutDesc);

			string prefix = await Contexting.GetPrefixAsync(context).ConfigureAwait(false);
			embed.AddField("Prefix", $"The command prefix is `{prefix}`");// is present on **{Client.Guilds.Count}** servers!");

			/*TimeSpan uptime = DiscordBot.Uptime;
			int d = (int) uptime.TotalDays;
			int h = uptime.Hours;
			int m = uptime.Minutes;
			int s = uptime.Seconds;
			uptime = DiscordBot.TotalUptime;
			int d2 = (int) uptime.TotalDays;
			int h2 = uptime.Hours;
			int m2 = uptime.Minutes;
			int s2 = uptime.Seconds;
			int guilds = Client.Guilds.Count;
			long spoilers = await this.spoilers.GetSpoilerCountAsync().ConfigureAwait(false);
			long spoiledUsers = await this.spoilers.GetSpoiledUserCountAsync().ConfigureAwait(false);
			long members = Client.Guilds.Sum(g => g.Users.Count);

			embed.AddField("Stats", $"**{spoilers}** spoiler{Plural(spoilers)} have been revealed " +
									$"**{spoiledUsers}** time{Plural(spoiledUsers)}");
			embed.AddField("Uptime", $"`Current:` " +
									 $"**{d}** day{Plural(d)}, " +
									 $"**{h}** hour{Plural(h)}, " +
									 $"**{m}** minute{Plural(m)}, and " +
									 $"**{s}** second{Plural(s)}\n" +
									 $"`Total:` " +
									 $"**{d2}** day{Plural(d)}, " +
									 $"**{h2}** hour{Plural(h)}, " +
									 $"**{m2}** minute{Plural(m)}, and " +
									 $"**{s2}** second{Plural(s)}");
			embed.AddField("Servers", $"Active on **{guilds}** server{Plural(guilds)} with " +
									  $"**{members}** member{Plural(members)}");*/
			await addFields(embed).ConfigureAwait(false);
			return embed.Build();
		}

		#endregion
	}
}
