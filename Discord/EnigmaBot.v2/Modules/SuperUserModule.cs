using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Database.Model;
using TriggersTools.DiscordBots.Modules;
using TriggersTools.DiscordBots.Services;
using EnigmaBot.Database;
using TriggersTools.DiscordBots.Utils;
using TriggersTools.DiscordBots.Commands;
using EnigmaBot.Commands;
using TriggersTools.DiscordBots;

namespace EnigmaBot.Modules {
	[Name("Superuser")]
	[Summary("You're not supposed to be here, you need to leave")]
	[RequiresSuperuser]
	[IsLockable(false)]
	public class SuperuserModule : DiscordBotModule {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="SuperuserModule"/>.
		/// </summary>
		public SuperuserModule(DiscordBotServiceContainer services) : base(services) { }

		#endregion

		[Name("shutdown")]
		[Command("shutdown")]
		[Summary("Shuts down the bot without restarting it")]
		public async Task Shutdown() {
			await ReplyAsync("`Shutting down...`").ConfigureAwait(false);
			await DiscordBot.ShutdownAsync(Context).ConfigureAwait(false);
		}

		[Name("restart")]
		[Command("restart")]
		[Summary("Restarts the bot to help fix any errors that have accumulated")]
		public async Task Restart() {
			await ReplyAsync("`Restarting...`").ConfigureAwait(false);
			await DiscordBot.RestartAsync(Context, "`Hello world!`").ConfigureAwait(false);
		}

		[Name("error")]
		[Command("error")]
		[Summary("Throw an error")]
		public Task Error() {
			throw new Exception();
		}

		[Name("reloadconfig")]
		[Command("reloadconfig")]
		[Summary("Reloads the configuration file. Services should respond to updated changes")]
		public async Task ReloadConfig() {
			var message = await ReplyAsync("`Reloading Config...`").ConfigureAwait(false);
			await DiscordBot.ReloadConfigAsync().ConfigureAwait(false);
			await message.ModifyAsync(c => c.Content = "`Reloading Config... Reloaded!`").ConfigureAwait(false);
		}

		[Name("status <status>")]
		[Command("status")]
		public Task SetStatus(ActivityType activity, [Remainder] string name) {
			return Client.SetGameAsync(name, type: activity);
		}

		[Name("visibility <visibility>")]
		[Command("visibility"), Alias("visible")]
		public Task SetVisibility(UserStatus status) {
			return Client.SetStatusAsync(status);
		}

		[Name("supersay <text>")]
		[Command("supersay")]
		[Usage("<text...>")]
		[Example("Hello World!", "Makes the bot send a message with *Hello World!*")]
		[Summary("Make the bot say something and then delete your command message")]
		public async Task SaySuperuser([Remainder] string text) {
			await Context.Message.DeleteAsync().ConfigureAwait(false);
			await ReplyAsync(text).ConfigureAwait(false);
			//return DeletedResult.FromSuccess();
		}

		[Name("react")]
		[Command("react")]
		[Summary("Reacts to this command")]
		public Task<RuntimeResult> React() {
			return Task.FromResult(EmoteResults.FromSuccess());
		}

		[Group("log")]
		[Usage("[warning|error] <message...>")]
		[Summary("Writes the text to the log files under the input category or *Notice*")]
		public class LogGroup : DiscordBotModule {
			
			#region Constructors

			/// <summary>
			/// Constructs the <see cref="LogGroup"/>.
			/// </summary>
			public LogGroup(DiscordBotServiceContainer services) : base(services) { }

			#endregion

			[Name("log <message>")]
			[Command("")]
			[Priority(0)]
			[Example("Check recent errors", "Write a Notice of *Check recent errors* to the log file")]
			public Task LogNotice([Remainder] string message) {
				return LogAsync(message, LogSeverities.Notice);
			}
			[Name("log warning <message>")]
			[Command("warning")]
			[Priority(1)]
			[Example("That last thing was acting strange", "Write a Warning of *That last thing was acting strange* to the log file")]
			public Task LogWarning([Remainder] string message) {
				return LogAsync(message, LogSeverity.Warning);
			}
			[Name("log error <message>")]
			[Command("error")]
			[Priority(1)]
			[Example("Something went very wrong", "Write an Error of *Something went very wrong* to the log file")]
			public Task LogError([Remainder] string message) {
				return LogAsync(message, LogSeverity.Error);
			}

			private Task LogAsync(string message, LogSeverity severity) {
				return Logging.LogAsync(new LogMessage(severity, $"{Context.User.Username}#{Context.User.Discriminator}", message), logFile: true, noticeFile: true);
			}
		}

		/*[Group("erase")]
		[Usage("<user|guild> <id>")]
		[Summary("Erases End User Data from the database")]
		public class EraseGroup : DiscordBotModule {

			#region Constructors

			/// <summary>
			/// Constructs the <see cref="EraseGroup"/>.
			/// </summary>
			public EraseGroup(DiscordBotServiceContainer services) : base(services) { }

			#endregion

			[Name("erase user <userId>")]
			[Command("user")]
			[Example("497125828120018945", "Erases the user with this Id")]
			public async Task EraseUser(ulong id) {
				var wait = new EraseEndUserDataWaitContext(Context, EndUserDataType.User, id);
				await wait.StartAsync().ConfigureAwait(false);
			}

			[Name("erase guild <guildId>")]
			[Command("guild")]
			[Example("436949335947870238", "Erases the guild with this Id")]
			public async Task EraseGuild(ulong id) {
				var wait = new EraseEndUserDataWaitContext(Context, EndUserDataType.Guild, id);
				await wait.StartAsync().ConfigureAwait(false);
			}
		}*/
	}
}
