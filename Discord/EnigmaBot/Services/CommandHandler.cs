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
			var msg = s as SocketUserMessage;     // Ensure the message is from a user/bot
			if (msg == null)
				return;
			if (s.Author.IsBot)
				return;     // Ignore bots and self when checking commands

			var context = new BotCommandContext(this, msg);     // Create the command context

			string prefix = Config["prefix"];
			int argPos = 0;     // Check if the message has a valid command prefix
			bool hasPrefix = msg.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase);
			bool hasMention = msg.HasMentionPrefix(Client.CurrentUser, ref argPos);
			if (hasPrefix || hasMention) {
				var result = await Commands.ExecuteAsync(context, argPos, Services);     // Execute the command

				/*TestLockables();

				if (result is BotPreconditionResult precon) {
					await BotModuleBase.HandleResult(context, precon.Result);
				}

				CommandError? commandError = result.Error ?? context.Error;
				CustomCommandError? customError = context.CustomError;
				string errorReason = result.ErrorReason ?? context.ErrorReason;

				if (!result.IsSuccess || !context.IsSuccess) {    // If not successful, reply with the error.
																  // Don't react to unknown commands when mentioning
					if (hasMention && result.Error.HasValue &&
						result.Error == CommandError.UnknownCommand)
						return;

					bool errorStated = false;
					ReactionInfo reaction = null;
					if (commandError.HasValue)
						reaction = BotReactions.GetReaction(commandError.Value.ToString());
					else if (reaction == null && customError.HasValue)
						reaction = BotReactions.GetReaction(customError.Value.ToString());
					if (reaction != null) {
						await msg.AddReactionAsync(reaction.Emoji);
						errorStated = true;
					}
					if (!errorStated) {
						if (context.Exception != null) {
							await msg.AddReactionAsync(BotReactions.Exception);
							Console.WriteLine(context.Exception.ToString());
						}
						else if (result is ExecuteResult exResult) {
							Console.WriteLine(context.Exception.ToString());
						}
						//if (!result.IsSuccess)
						//	await context.Channel.SendMessageAsync(result.ToString());
						//else if (context.ErrorReason != null)
						//	await context.Channel.SendMessageAsync(context.ErrorReason);
					}
				}*/

			}
		}
	}
}
