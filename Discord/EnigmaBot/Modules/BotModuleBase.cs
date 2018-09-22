using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EnigmaBot.Context;
using EnigmaBot.Services;

namespace EnigmaBot.Modules {
	public class BotModuleBase : ModuleBase<BotCommandContext> {

		protected override void BeforeExecute(CommandInfo command) {
			base.BeforeExecute(command);

		}

		protected override void AfterExecute(CommandInfo command) {
			base.AfterExecute(command);
			//await HandleResult(Context, Context);
		}

		/*public static async Task HandleResult(BotCommandContext context, IBotErrorResult result) {
			if (!result.IsSuccess) {
				bool errorStated = false;
				ReactionInfo reaction = null;
				if (result.Error.HasValue)
					reaction = BotReactions.GetReaction(result.Error.Value.ToString());
				else if (reaction == null && result.CustomError.HasValue)
					reaction = BotReactions.GetReaction(result.CustomError.Value.ToString());
				if (reaction != null) {
					await context.Message.AddReactionAsync(reaction.Emoji);
					errorStated = true;
				}
				if (!errorStated) {
					if (result.Exception != null) {
						//await context.Message.AddReactionAsync(BotReactions.Exception);
						Console.WriteLine(result.Exception.ToString());
					}
					if (result.ErrorReason != null) {
						//await context.Channel.SendMessageAsync(result.ErrorReason);
					}
				}
			}
		}*/

		protected void InvalidArguments(string reason = "") {
			Context.IsSuccess = false;
			Context.ErrorReason = reason;
			//Context.CustomError = CustomCommandError.InvalidArgument;
		}

		public DiscordSocketClient Client => Context.Client;
		public CommandService Commands => Context.Commands;
		public IConfigurationRoot Config => Context.Config;
		public IServiceProvider Services => Context.Services;
		public HelpService Help => Context.Help;
		public EnigmaService Enigma => Context.Enigma;
		public Random Random => Context.Random;
	}
}
