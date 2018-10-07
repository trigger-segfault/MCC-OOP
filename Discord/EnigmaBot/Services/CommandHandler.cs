using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System;
using Discord;
using Discord.Rest;
using System.Collections.Generic;
using EnigmaBot.Context;
using Discord.Commands.Builders;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;

namespace EnigmaBot.Services {
	public class CommandHandler : BotServiceBase {
		
		protected override void OnInitialized(ServiceProvider services) {
			base.OnInitialized(services);
			Client.MessageReceived += OnMessageReceivedAsync;
		}

		private async Task OnMessageReceivedAsync(SocketMessage s) { 
			if (!(s is SocketUserMessage msg))
				return;     // Ensure the message is from a user/bot
			if (s.Author.IsBot)
				return;     // Ignore bots and self when checking commands

			var context = new BotCommandContext(this, msg);     // Create the command context

			string prefix = Config["prefix"];
			int argPos = 0;     // Check if the message has a valid command prefix
			bool hasPrefix = msg.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase);
			bool hasMention = msg.HasMentionPrefix(Client.CurrentUser, ref argPos);
			if (hasPrefix || hasMention) {
				var result = await Commands.ExecuteAsync(context, argPos, Services);     // Execute the command

				if (!result.IsSuccess) {
					if (result.Error.HasValue && result.Error == CommandError.UnknownCommand)
						await msg.AddReactionAsync(new Emoji("❓"));
				}
			}
		}
	}
}
