using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TriggersTools.DiscordBots.Commands.Utils;
using TriggersTools.DiscordBots.Extensions;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// The extened command service that incorperates <see cref="CommandSetDetails"/> and <see cref="IWaitContextService"/>.
	/// </summary>
	public class CommandServiceEx : CommandService, IWaitContextService {

		#region Fields

		/// <summary>
		/// Gets the default run mode passed in the <see cref="CommandServiceConfig"/>.
		/// </summary>
		private readonly RunMode defaultRunMode;
		/// <summary>
		/// The dictionary of user-scoped wait contexts.
		/// </summary>
		private readonly ConcurrentDictionary<ulong, IUserWaitContext> userWaitContexts = new ConcurrentDictionary<ulong, IUserWaitContext>();
		/// <summary>
		/// Gets the set of command details after the service has been initialized.
		/// </summary>
		public CommandSetDetails CommandSet { get; private set; }

		#endregion

		#region Details

		/// <summary>
		/// Call this after modules have finished loading in order to contruct the command details.
		/// </summary>
		/// <param name="services">The services provider for the commands.</param>
		public void InitializeDetails(IServiceProvider services) {
			if (CommandSet != null)
				throw new InvalidOperationException($"{nameof(InitializeDetails)} has already been called!");
			CommandSet = new CommandSetDetails(this, services);
		}

		#endregion

		#region Events

		/*/// <summary>
		/// Called when a command returns a result, success, or failure.
		/// </summary>
		/// <remarks>
		/// <see cref="CommandInfo"/> is null when the service could not find a command match.
		/// </remarks>
		public event Func<CommandInfo, ICommandContext, IResult, Task> CommandResult {
			add => commandResultEvent.Add(value);
			remove => commandResultEvent.Remove(value);
		}
		private readonly AsyncEvent<Func<CommandInfo, ICommandContext, IResult, Task>> commandResultEvent
			= new AsyncEvent<Func<CommandInfo, ICommandContext, IResult, Task>>();*/

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new <see cref="CommandServiceEx"/> class with the default configuration.
		/// </summary>
		public CommandServiceEx(DiscordSocketClient client) : this(new CommandServiceConfig(), client) { }
		/// <summary>
		/// Initializes a new <see cref="CommandServiceEx"/> class with the provided configuration.
		/// </summary>
		/// <param name="config">The configuration class.</param>
		/// 
		/// <exception cref="InvalidOperationException">
		/// The Discord.Commands.RunMode cannot be set to <see cref="RunMode.Default"/>.
		/// </exception>
		public CommandServiceEx(CommandServiceConfig config, DiscordSocketClient client) : base(config) {
			defaultRunMode = config.DefaultRunMode;
			client.MessageReceived += OnMessageReceivedAsync;
			//Log += OnLogAsync;
			//CommandExecuted += OnCommandExecutedAsync;
		}

		#endregion


		#region WaitContext

		/// <summary>
		/// Gets the existing wait context for the user.
		/// </summary>
		/// <param name="userId">The snowflake entity Id of the user.</param>
		/// <returns>The wait context if one exists, otherwise null.</returns>
		public IUserWaitContext GetWaitContext(ulong userId) {
			userWaitContexts.TryGetValue(userId, out var wait);
			return wait;
		}
		/// <summary>
		/// Tries to add the wait context for the user.
		/// </summary>
		/// <param name="wait">The wait context to add.</param>
		/// <returns>True if the wait context was added.</returns>
		public void AddWaitContext(IUserWaitContext wait) {
			// Lock the wait context to prevent other actions on it during the process.
			lock (wait) {
				IUserWaitContext existingWait = userWaitContexts.GetOrAdd(wait.User.Id, wait);
				if (existingWait != wait) {
					userWaitContexts.TryRemove(wait.User.Id, out _);
					existingWait.OutputChannel.SendMessageAsync(existingWait.OverwriteMessage).GetAwaiter().GetResult();
				}
				userWaitContexts.TryAdd(wait.User.Id, wait);
				wait.ExpireTimer = new Timer((o) => OnWaitContextExpireAsync((IUserWaitContext) o).GetAwaiter().GetResult(),
											 wait, wait.Duration, Timeout.InfiniteTimeSpan);
			}
		}
		/// <summary>
		/// Tries to remove the wait context for the user.
		/// </summary>
		/// <param name="wait">The wait context to remove.</param>
		/// <returns>
		/// True if the wait context contained was the same as <paramref name="wait"/> and was removed.
		/// </returns>
		public bool RemoveWaitContext(IUserWaitContext wait) {
			// Lock the wait context to prevent other actions on it during the process.
			lock (wait) {
				if (userWaitContexts.TryRemoveValue(wait.User.Id, wait)) {
					wait.ExpireTimer?.Dispose();
					return true;
				}
			}
			return false;
		}

		#endregion

		#region ExecuteAsync

		/*/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <param name="argPos">The position of which the command starts at.</param>
		/// <param name="services">The service to be used in the command's dependency injection.</param>
		/// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
		/// <returns>
		/// A task that represents the asynchronous execution operation. The task result contains the result
		/// of the command execution.
		/// </returns>
		public new Task<IResult> ExecuteAsync(ICommandContext context, int argPos, IServiceProvider services, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception) {
			return ExecuteAsync(context, context.Message.Content.Substring(argPos), services, multiMatchHandling);
		}
		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <param name="input">The command string.</param>
		/// <param name="services">The service to be used in the command's dependency injection.</param>
		/// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
		/// <returns>
		/// A task that represents the asynchronous execution operation. The task result contains the result
		/// of the command execution.
		/// </returns>
		public new async Task<IResult> ExecuteAsync(ICommandContext context, string input, IServiceProvider services, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception) {
			IResult result;

			result = await base.ExecuteAsync(context, input, services, multiMatchHandling).ConfigureAwait(false);
			BestMatchResult bestMatchResult;
			bestMatchResult = await FindBestMatchAsync(context, input, services, multiMatchHandling);
			CommandInfo command = bestMatchResult.Command.Command;
			if (bestMatchResult.IsSuccess)
				result = await bestMatchResult.ExecuteAsync();
			else
				result = bestMatchResult.Result;

			if (result is ExecuteResult execute && IsCommandAsync(command)) {
				// Do nothing, this is not the result we're looking for.
				return result;
			}

			//await commandResultEvent.InvokeAsync(command, context, result);

			return result;
		}*/

		#endregion

		#region FindBestMatchAsync

		/// <summary>
		/// Performs a full search for the commands that match the input along with the parameters.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <param name="argPos">The position of which the command starts at.</param>
		/// <param name="services">The service to be used in the command's dependency injection.</param>
		/// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
		/// <returns>
		/// A task that represents the asynchronous match operation. The task result contains the result of the
		/// command search.
		/// </returns>
		public Task<BestMatchResult> FindBestMatchAsync(ICommandContext context, int argPos, IServiceProvider services, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception) {
			return FindBestMatchAsync(context, context.Message.Content.Substring(argPos), services, multiMatchHandling);
		}
		/// <summary>
		/// Performs a full search for the commands that match the input along with the parameters.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <param name="input">The command string.</param>
		/// <param name="services">The service to be used in the command's dependency injection.</param>
		/// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
		/// <returns>
		/// A task that represents the asynchronous match operation. The task result contains the result of the
		/// command search.
		/// </returns>
		public async Task<BestMatchResult> FindBestMatchAsync(ICommandContext context, string input, IServiceProvider services, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception) {
			services = services ?? EmptyServiceProvider.Instance;

			var searchResult = Search(input);
			if (!searchResult.IsSuccess)
				return BestMatchResult.FromError(input, context, services, searchResult);

			var commands = searchResult.Commands;
			var preconditionResults = new Dictionary<CommandMatch, PreconditionResult>();

			foreach (var match in commands) {
				preconditionResults[match] = await match.Command.CheckPreconditionsAsync(context, services).ConfigureAwait(false);
			}

			var successfulPreconditions = preconditionResults
				.Where(x => x.Value.IsSuccess)
				.ToArray();

			if (successfulPreconditions.Length == 0) {
				//All preconditions failed, return the one from the highest priority command
				var bestCandidate = preconditionResults
					.OrderByDescending(x => x.Key.Command.Priority)
					.FirstOrDefault(x => !x.Value.IsSuccess);
				return BestMatchResult.FromError(input, context, services, bestCandidate.Value);
			}

			//If we get this far, at least one precondition was successful.

			var parseResultsDict = new Dictionary<CommandMatch, ParseResult>();
			foreach (var pair in successfulPreconditions) {
				var parseResult = await pair.Key.ParseAsync(context, searchResult, pair.Value, services).ConfigureAwait(false);

				if (parseResult.Error == CommandError.MultipleMatches) {
					IReadOnlyList<TypeReaderValue> argList, paramList;
					switch (multiMatchHandling) {
					case MultiMatchHandling.Best:
						argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
						paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
						parseResult = ParseResult.FromSuccess(argList, paramList);
						break;
					}
				}

				parseResultsDict[pair.Key] = parseResult;
			}

			// Calculates the 'score' of a command given a parse result
			float CalculateScore(CommandMatch match, ParseResult parseResult) {
				float argValuesScore = 0, paramValuesScore = 0;

				if (match.Command.Parameters.Count > 0) {
					float argValuesSum = parseResult.ArgValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
					float paramValuesSum = parseResult.ParamValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

					argValuesScore = argValuesSum / match.Command.Parameters.Count;
					paramValuesScore = paramValuesSum / match.Command.Parameters.Count;
				}

				float totalArgsScore = (argValuesScore + paramValuesScore) / 2;
				return match.Command.Priority + totalArgsScore * 0.99f;
			}

			//Order the parse results by their score so that we choose the most likely result to return
			var parseResults = parseResultsDict
				.OrderByDescending(x => CalculateScore(x.Key, x.Value));

			var successfulParses = parseResults
				.Where(x => x.Value.IsSuccess)
				.ToArray();

			if (successfulParses.Length == 0) {
				//All parses failed, return the one from the highest priority command, using score as a tie breaker
				var bestMatch = parseResults
					.FirstOrDefault(x => !x.Value.IsSuccess);
				return BestMatchResult.FromError(input, context, services, bestMatch.Value);
			}

			//If we get this far, at least one parse was successful. Return the most likely overload.
			var chosenOverload = successfulParses[0];
			return BestMatchResult.FromSuccess(input, context, services, chosenOverload.Key, chosenOverload.Value);
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Called when a message is recieved to send to a wait context.
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		private async Task OnMessageReceivedAsync(SocketMessage msg) {
			if (!(msg is SocketUserMessage message)) return;

			IUserWaitContext waitContext = GetWaitContext(msg.Author.Id);
			if (waitContext != null)
				await waitContext.MessageReceiveAsync(message).ConfigureAwait(false);
		}
		/*/// <summary>
		/// Called when a command is logged. Checks if a async command threw an exception.
		/// </summary>
		/// <param name="msg">The log message.</param>
		private Task OnLogAsync(LogMessage msg) {
			if (msg.Exception is CommandException ex && IsCommandAsync(ex.Command)) {
				// TODO: Temp fix until async exceptions are supported
				// We found a valid result to a command.
				return commandResultEvent.InvokeAsync(ex.Command, ex.Context, ExecuteResult.FromError(ex.InnerException));
			}
			return Task.FromResult<object>(null);
		}
		/// <summary>
		/// Called when a command has successfully executed.
		/// </summary>
		/// <param name="cmd">The command that finished executing.</param>
		/// <param name="context">The context of the command execution.</param>
		/// <param name="result">The execution result.</param>
		private Task OnCommandExecutedAsync(Optional<CommandInfo> cmd, ICommandContext context, IResult result) {
			// TODO: Temp fix until async exceptions are supported
			return commandResultEvent.InvokeAsync(cmd.IsSpecified ? cmd.Value : null, context, result);
		}*/
		/// <summary>
		/// Called when a user wait context expires. Removes it afterwards.
		/// </summary>
		/// <param name="wait">The expiring wait context.</param>
		private async Task OnWaitContextExpireAsync(IUserWaitContext wait) {
			if (RemoveWaitContext(wait)) {
				wait.ExpireTimer?.Dispose();
				await wait.ExpireAsync().ConfigureAwait(false);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets if the command is asynchronously executed.
		/// </summary>
		/// <param name="cmd">The command to check.</param>
		/// <returns>True if the command is asynchronous.</returns>
		public bool IsCommandAsync(CommandInfo cmd) {
			return	cmd.RunMode == RunMode.Async ||
				   (cmd.RunMode == RunMode.Default &&
					defaultRunMode == RunMode.Async);
		}

		#endregion
	}
}
