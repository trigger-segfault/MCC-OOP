using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Extensions;

namespace TriggersTools.DiscordBots.Services {
	/// <summary>
	/// A service for checking for and executing commands.
	/// </summary>
	public class CommandHandlerService : DiscordBotService {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="CommandHandlerService"/>.
		/// </summary>
		public CommandHandlerService(DiscordBotServiceContainer services) : base(services) {
			Client.MessageReceived += OnMessageReceived;
		}

		#endregion

		#region Event Handlers

		private async Task OnMessageReceived(SocketMessage rawMessage) {
			// Ignore system messages, or messages from other bots
			if (!(rawMessage is SocketUserMessage message)) return;
			// Don't process my own output dumbass!
			if (message.Author.Id == Client.CurrentUser.Id) return;
			// Modules use the AllowBots precondition attribute, so let them pass... for now.
			if (message.Source != MessageSource.User && message.Source != MessageSource.Bot) return;

			var context = Contexting.CreateCommandContext(message);

			// Check if the message has a valid command prefix
			// This value holds the offset where the prefix ends
			int argPos = 0;
			string prefix = await Contexting.GetPrefixAsync(context).ConfigureAwait(false);
			bool hasMention = message.HasMentionPrefix(Client.CurrentUser, ref argPos);
			bool hasPrefix = message.HasStringPrefix(prefix, ref argPos, StringComparison.InvariantCultureIgnoreCase);
			if (!hasMention && !hasPrefix) return;

			context.ArgPos = argPos;
			if (!(await AllowCommandAsync(context).ConfigureAwait(false))) return;

			var result = await Commands.ExecuteAsync(context, argPos, Services).ConfigureAwait(false);
			
			/*if (result.Error.HasValue && !result.Reacted) {
				if (!string.IsNullOrWhiteSpace(result.ErrorReason))
					await context.Channel.SendMessageAsync(result.ErrorReason);
				else
					await context.Channel.SendMessageAsync(result.Error.ToString());
			}*/
		}

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Performs additional checks to see if the command is allowed to be processed.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <returns>True if the command can be processed.</returns>
		protected virtual Task<bool> AllowCommandAsync(IDiscordBotCommandContext context) {
			return Task.FromResult(true);
		}

		#endregion
	}
}
