using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnigmaBot.Context;
using EnigmaBot.Services;
using EnigmaBot.Utils;
using EnigmaBot.Info;

namespace EnigmaBot.Modules {
	[Name("Help")]
	public class HelpModule : BotModuleBase {


		[Command("help", RunMode = RunMode.Async)]
		[Summary("List all commands usable by you")]
		public async Task HelpAsycn() {
			await HelpBase();
		}

		[Command("helpg", RunMode = RunMode.Async)]
		[Parameters("<groupName>")]
		[Example("Enigma Machine")]
		[Summary("List all commands usable by you in the group")]
		public async Task HelpGroup([Remainder] string groupName) {
			string prefix = Config["prefix"];
			List<CommandInfo> results = await Help.GetUsableCommands(Context,
				c => !c.IsDuplicateFunctionality());
			CommandGroup group = Help.GetAllCommandGroups().FirstOrDefault(
				g => string.Compare(g.Name, groupName, true) == 0);

			if (group == null) {
				await ReplyAsync($"No group found matching **{groupName}**");
				return;
			}

			var builder = new EmbedBuilder() {
				Color = new Color(114, 137, 218),
			};

			string description = null;
			foreach (CommandInfo cmd in group) {
				string alias = cmd.Aliases.First();
				description += $"{prefix}{alias}";
				if (cmd.HasParameters())
					description += $" `{cmd.GetParameters()}`";
				description += "\n";
			}

			if (!string.IsNullOrWhiteSpace(description)) {
				builder.AddField(x => {
					x.Name = group.Name;
					x.Value = description;
					x.IsInline = false;
				});
			}
			
			await ReplyAsync("", false, builder.Build());
		}

		public async Task HelpBase(bool all = false, bool args = false) {
			var groups = await Help.GetUsableCommandGroups(
				Context, c => (!c.IsDuplicate() ||
				(args && !c.IsDuplicateFunctionality())));

			if (!all) {
				List<CommandGroup> groupsFinal = new List<CommandGroup>();
				foreach (var group in groups) {
					List<CommandInfo> commandsFinal = new List<CommandInfo>();
					HashSet<string> baseCommands = new HashSet<string>();
					foreach (var cmd in group) {
						if (baseCommands.Add(cmd.Aliases.First().Split(' ')[0])) {
							commandsFinal.Add(cmd);
						}
					}
					group.Clear();
					group.AddRange(commandsFinal);
				}
			}

			string prefix = Config["prefix"];
			var builder = new EmbedBuilder() {
				Color = new Color(114, 137, 218),
				Description = $"These are the commands you can use\nThe prefix is: `{prefix}`",
			};

			foreach (CommandGroup group in groups) {
				string description = null;
				foreach (CommandInfo cmd in group) {
					string alias = cmd.Aliases.First();
					if (!all) {
						if (!string.IsNullOrEmpty(description))
							description += " **|** ";
						description += $"{alias.Split(' ')[0]}";
					}
					else if (all) {
						if (args) {
							description += $"{prefix}{alias}";
							if (cmd.HasParameters())
								description += $" `{cmd.GetParameters()}`";
							description += "\n";
						}
						else {
							if (!string.IsNullOrEmpty(description))
								description += " **|** ";
							description += $"{alias}";
						}
					}
				}

				if (!string.IsNullOrWhiteSpace(description)) {
					builder.AddField(x => {
						x.Name = group.Name;
						x.Value = description;
						x.IsInline = false;
					});
				}
			}

			ITextChannel channel = Context.Channel as ITextChannel;
			if (args && !(Context.Channel is IDMChannel)) {
				IDMChannel dm = await Context.User.GetOrCreateDMChannelAsync();
				await dm.SendMessageAsync("", false, builder.Build());
				Context.IsSuccess = false;
			}
			else {
				await Context.Channel.SendMessageAsync("", false, builder.Build());
			}

		}

		/*[Command("help all", RunMode = RunMode.Async)]
		[Summary("List every command in its entirety.")]
		public async Task HelpAllAsync() {
			await HelpBase(true);
		}

		[Command("help allargs", RunMode = RunMode.Async)]
		[Summary("List every command in its entirety, with parameters.")]
		public async Task HelpAllArgsAsync() {
			await HelpBase(true, true);
		}*/

		[Command("help", RunMode = RunMode.Async)]
		[Summary("Get help for any commands that start with the search term")]
		[Parameters("<command...|all>")]
		[Example("encipher")]
		[Remarks("Entering `all` as a search term will list every command in its entirety.\n" +
			"Entering `allargs` will list every command as well as the parameters for the commands.\n" +
			"Either of these options will DM the list to you.")]
		public async Task HelpSearch([Remainder]string searchTerm) {
			string prefix = Config["prefix"];

			if (string.Compare(searchTerm, "all", true) == 0) {
				await HelpBase(true);
				return;
			}
			/*else if (string.Compare(searchTerm, "allargs", true) == 0) {
				await HelpBase(true, true);
				return;
			}*/

			List<CommandInfo> results = await Help.GetUsableCommands(Context,
				c => !c.IsDuplicateFunctionality());
			Help.SearchCommands(results, searchTerm);

			if (!results.Any()) {
				await ReplyAsync($"No commands found matching **{searchTerm}**");
				return;
			}

			var builder = new EmbedBuilder() {
				Color = new Color(114, 137, 218),
				Description = $"Found {results.Count} commands matching **{searchTerm}**"
			};

			foreach (CommandInfo cmd in results) {
				string alias = cmd.Aliases.First();
				List<string> items = new List<string>();
				//if (cmd.Parameters.Any())
				//	items.Add($"__Parameters:__ {string.Join(", ", cmd.Parameters.Select(p => p.Name))}");
				if (cmd.HasParameters())
					items.Add($"__Usage:__ `{prefix}{alias} {cmd.GetParameters()}`");
				if (cmd.HasExample())
					items.Add($"__Example:__ `{prefix}{alias} {cmd.GetExample()}`"); 
				if (cmd.Summary != null)
					items.Add($"__Summary:__ {cmd.Summary}");
				//if (cmd.HasUsage())
				//	items.Add($"__Usage:__ {cmd.GetUsage()}");
				if (cmd.Remarks != null)
					items.Add($"__Remarks:__ {cmd.Remarks}");
				string value = string.Join('\n', items.ToArray());
				if (string.IsNullOrWhiteSpace(value))
					value = "No information available";
				string name = string.Join(", ", cmd.Aliases);
				builder.AddField(name, value);
			}

			await ReplyAsync("", false, builder.Build());
		}
		
		/*[Command("about")]
		[Summary("Gives some info on the bot")]
		public async Task About() {
			string name = Client.CurrentUser.GetName(Context.Guild);
			string prefix = await Settings.GetPrefix(Context);
			ulong creatorId = ulong.Parse(Config["creator_client_id"]);
			SocketUser creator = Client.GetUser(creatorId);
			var embed = new EmbedBuilder() {
				Color = new Color(114, 137, 218),
				Title = $"About {name}",
				Description =
					$"This bot is made by trigger_death for use in a small selection " +
					$"of Discord servers. " +
					$"It's selling features are spoilers that do not rely on gifs " +
					$"and a command which inserts claps between words. \n\n" +
					$"{name} is my first Discord bot as well as my first dive into " +
					$"heavy use of asynchonous functions and databases so things " +
					$"might be a little wonkey. \n\n" +
					$"Please be nice to {name}.  (ﾉ´ヮ`)ﾉ*: ･ﾟ\n",
			};
			embed.WithThumbnailUrl(@"https://i.imgur.com/Jep2ni2.png");
			embed.WithAuthor(creator);
			//embed.WithImageUrl(@"https://i.imgur.com/Jep2ni2.png");

			await ReplyAsync("", false, embed.Build());
		}*/
		
	}
}
