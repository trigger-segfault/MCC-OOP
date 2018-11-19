using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// A result returned when attempting to match commands and they're parameters.
	/// </summary>
	[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
	public class BestMatchResult : IResult {
		/// <summary>
		/// Gets the text of the command to execute.
		/// </summary>
		public string Text { get; }
		/// <summary>
		/// Gets the context of the best command match result.
		/// </summary>
		public ICommandContext Context { get; }
		/// <summary>
		/// Gets the service provider used for the best command match result.
		/// </summary>
		public IServiceProvider Services { get; }
		/// <summary>
		/// Gets the best command match.
		/// </summary>
		public CommandMatch Command { get; }
		/// <summary>
		/// Gets the <see cref="ParseResult"/> on success, otherwise the failure result returned from the
		/// highest scoring command.
		/// </summary>
		public IResult Result { get; }

		/// <inheritdoc/>
		public CommandError? Error { get; }
		/// <inheritdoc/>
		public string ErrorReason { get; }

		/// <inheritdoc/>
		public bool IsSuccess => !Error.HasValue;

		/// <summary>
		/// Constructs a successful command search result.
		/// </summary>
		/// <param name="text">The text of the command to execute.</param>
		/// <param name="commands">The valid command matches.</param>
		/// <param name="error"></param>
		/// <param name="errorReason"></param>
		private BestMatchResult(string text, ICommandContext context, IServiceProvider services, CommandMatch command, ParseResult parseResult) {
			Text = text;
			Context = context;
			Services = services;
			Command = command;
			Result = parseResult;
		}
		/// <summary>
		/// Constructs an unsuccessful command search result.
		/// </summary>
		/// <param name="text">The text of the command to execute.</param>
		/// <param name="failureResult">The failure result returned with the highest scoring command.</param>
		private BestMatchResult(string text, ICommandContext context, IServiceProvider services, IResult failureResult) {
			Text = text;
			Context = context;
			Services = services;
			Result = failureResult ?? throw new ArgumentNullException(nameof(failureResult));
			Error = failureResult.Error;
			ErrorReason = failureResult.ErrorReason;
		}

		public static BestMatchResult FromSuccess(string text, ICommandContext context, IServiceProvider services, CommandMatch command, ParseResult parseResult)
			=> new BestMatchResult(text, context, services, command, parseResult);
		public static BestMatchResult FromError(string text, ICommandContext context, IServiceProvider services, IResult failureResult)
			=> new BestMatchResult(text, context, services, failureResult);

		public override string ToString() => IsSuccess ? "Success" : $"{Result}";
		private string DebuggerDisplay => IsSuccess ? $"Success ({Command})" : $"{Result}";

		/// <summary>
		/// Executes the best command match asynchronously.
		/// </summary>
		/// <returns>The result of the command execution.</returns>
		public Task<IResult> ExecuteAsync() {
			if (!IsSuccess)
				throw new InvalidOperationException($"{nameof(BestMatchResult)} was not successful!");
			return Command.ExecuteAsync(Context, (ParseResult) Result, Services);
		}
	}
}
