using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Extensions;

namespace TriggersTools.DiscordBots.Services {
	/// <summary>
	/// A service for overriding command result reactions and command responses.
	/// </summary>
	public class ResultHandlerService : DiscordBotService {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="ResultHandlerService"/>.
		/// </summary>
		public ResultHandlerService(DiscordBotServiceContainer services) : base(services) {
			Commands.CommandExecuted += OnCommandResultAsync;
		}

		#endregion
		
		#region Event Handlers

		private async Task OnCommandResultAsync(Optional<CommandInfo> cmd, ICommandContext context, IResult result) {
			HandleResultEventArgs e = new HandleResultEventArgs(cmd.GetValueOrDefault(), context, result);
			await OnHandleResultAsync(e).ConfigureAwait(false);
			if (e.Result is ReactionResult reactionResult && reactionResult.Reactions.Any()) {
				foreach (IEmote reaction in reactionResult.Reactions) {
					try {
						await e.Context.Message.AddReactionAsync(reaction).ConfigureAwait(false);
					} catch { }
				}
			}
			else if (e.Reaction != null && !(e.Result is DeletedResult)) {
				try {
					await context.Message.AddReactionAsync(e.Reaction).ConfigureAwait(false);
				} catch { }
			}
		}
		/// <summary>
		/// Handles responding to a command.
		/// </summary>
		protected virtual async Task OnHandleResultAsync(HandleResultEventArgs e) {
			int argPos = 0;
			CommandDetails command = (e.Command != null ? this.ResolveCommand(e.Command) : null);
			if (command != null && await Contexting.IsLockedAsync(e.Context, command).ConfigureAwait(false)) {
				e.Set("🔒", 100);
			}
			if (e.Result.Error.HasValue && e.Result.Error == CommandError.UnknownCommand) {
				// Don't spellcheck mention commands
				if (!e.Context.Message.HasMentionPrefix(Client.CurrentUser, ref argPos))
					e.Set("❓", int.MaxValue);
			}
			if (e.Result.IsSuccess) {

			}
			else if (e.Result is PreconditionResult preconditionResult) {
				List<PreconditionResult> preconditionResults = new List<PreconditionResult>();
				if (e.Result is PreconditionGroupResult groupResult) {
					preconditionResults.AddRange(groupResult.PreconditionResults);
				}
				else {
					preconditionResults.Add(preconditionResult);
				}
				foreach (var result in preconditionResults) {
					if (!(result is PreconditionAttributeResult attributeResult))
						continue;
					switch (attributeResult.Precondition) {
					case RequiresNsfwAttribute _:
						e.Set("🔞", 1); break;
					case RequiresUserPermissionAttribute _:
					case RequiresSuperuserAttribute _:
					case RequiresOwnerAttribute _:
					case RequiresContextAttribute _:
						e.Set("⛔", 2); break;
					case AllowBotsAttribute _:
						e.Set("🤖", 2); break;
					case ParameterConstantAttribute _:
						e.Set("❌", 2); break;
					case RequiresBotPermissionAttribute _:
						e.Set("⚠", 2); break;
					}
				}
			}
			else if (e.Result is ParseResult || e.Result is TypeReaderResult) {
				e.Set("❌");
			}
			else if (e.Result is ExecuteResult executeResult) {
				if (executeResult.Exception != null)
					e.Set("⚠", 100);
			}
			else {
				if (!e.Context.Message.HasMentionPrefix(Client.CurrentUser, ref argPos))
					e.Set("❌");
			}
			/*if (e.Result.Error.HasValue && !e.Result.Reacted) {
				if (!string.IsNullOrWhiteSpace(e.Result.ErrorReason))
					await e.Context.Channel.SendMessageAsync(e.Result.ErrorReason);
				else
					await e.Context.Channel.SendMessageAsync(e.Result.Error.ToString());
			}*/
			//return Task.FromResult<object>(null);
		}

		#endregion
	}
}
