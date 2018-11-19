using System;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace TriggersTools.DiscordBots.Services {
	/// <summary>
	/// A configuration class for the <see cref="ReliabilityService"/>.
	/// </summary>
	public class ReliabilityConfig {
		/// <summary>
		/// Returns a <see cref="ReliabilityService"/> configuration where the service is disabled.
		/// </summary>
		public static ReliabilityConfig Disabled => new ReliabilityConfig { Enabled = false };
		
		/// <summary>
		/// You can use this to disable the <see cref="ReliabilityService"/> without messing around with
		/// removing it from the service collection.
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// How long should we wait on the client to reconnect before resetting?<para/>
		/// Default is 30 seconds.
		/// </summary>
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
		/// <summary>
		/// Should we attempt to reset the client? Set this to false if your client is still locking up.<para/>
		/// Default is true.
		/// </summary>
		public bool AttemptReset { get; set; } = true;

		/// <summary>
		/// The <see cref="LogSeverity"/> to use for debug messages.
		/// </summary>
		public LogSeverity DebugSeverity { get; set; } = LogSeverity.Debug;
		/// <summary>
		/// The <see cref="LogSeverity"/> to use for info messages.
		/// </summary>
		public LogSeverity InfoSeverity { get; set; } = LogSeverity.Info;
		/// <summary>
		/// The <see cref="LogSeverity"/> to use for critical messages.
		/// </summary>
		public LogSeverity CriticalSeverity { get; set; } = LogSeverity.Critical;
	}
	/// <summary>
	/// This service requires that your bot is being run by a daemon that handles Exit Code 1 (or any exit
	/// code) as a restart.
	/// </summary>
	/// <remarks>
	/// If you do not have your bot setup to run in a daemon, this service will just terminate the process
	/// and the bot will not restart.<para/>
	/// 
	/// Links to daemons:<para/>
	/// [Powershell (Windows+Unix)] https://gitlab.com/snippets/21444 <para/>
	/// [Bash (Unix)] https://stackoverflow.com/a/697064
	/// </remarks>
	public sealed class ReliabilityService {

		#region Constants
		
		/// <summary>
		/// The source to display for log messages.
		/// </summary>
		private const string LogSource = "Reliability";

		#endregion
		
		#region Settings

		/// <summary>
		/// How long should we wait on the client to reconnect before resetting?
		/// </summary>
		public TimeSpan Timeout { get; }
		/// <summary>
		/// Should we attempt to reset the client? Set this to false if your client is still locking up.
		/// </summary>
		public bool AttemptReset { get; }

		/// <summary>
		/// The <see cref="LogSeverity"/> to use for debug messages.
		/// </summary>
		public LogSeverity DebugSeverity { get; }
		/// <summary>
		/// The <see cref="LogSeverity"/> to use for info messages.
		/// </summary>
		public LogSeverity InfoSeverity { get; }
		/// <summary>
		/// The <see cref="LogSeverity"/> to use for critical messages.
		/// </summary>
		public LogSeverity CriticalSeverity { get; }

		#endregion

		#region Fields

		private readonly DiscordSocketClient client;
		private readonly IDiscordBot discordBot;
		private readonly ILoggingService log;
		private CancellationTokenSource cancel;

		#endregion

		/// <summary>
		/// Construcst the <see cref="ReliabilityService"/> with the default configuration.
		/// </summary>
		public ReliabilityService(DiscordSocketClient client,
								  IDiscordBot discordBot,
								  ILoggingService log)
			: this(new ReliabilityConfig(), client, discordBot, log)
		{
		}
		/// <summary>
		/// Construcst the <see cref="ReliabilityService"/> with the specified configuration.
		/// </summary>
		public ReliabilityService(ReliabilityConfig reliabilityConfig,
								  DiscordSocketClient client,
								  IDiscordBot discordBot,
								  ILoggingService log)
		{
			Timeout = reliabilityConfig.Timeout;
			AttemptReset = reliabilityConfig.AttemptReset;
			DebugSeverity = reliabilityConfig.DebugSeverity;
			InfoSeverity = reliabilityConfig.InfoSeverity;
			CriticalSeverity = reliabilityConfig.CriticalSeverity;
			if (reliabilityConfig.Enabled) {
				this.client = client;
				this.discordBot = discordBot;
				this.log = log;
				cancel = new CancellationTokenSource();
				client.Connected += OnConnectedAsync;
				client.Disconnected += OnDisconnectedAsync;
			}
		}

		#region Event Handlers

		/// <summary>
		/// Called when connected again so that the service will not take any action.
		/// </summary>
		private Task OnConnectedAsync() {
			// Cancel all previous state checks and reset the CancelToken - client is back online
			_ = DebugAsync("Client reconnected, resetting cancel tokens...");
			cancel.Cancel();
			cancel = new CancellationTokenSource();
			_ = DebugAsync("Client reconnected, cancel tokens reset.");

			return Task.CompletedTask;
		}
		/// <summary>
		/// Called when a disconnection occurs so the service can begin waiting for reconnection.
		/// </summary>
		private Task OnDisconnectedAsync(Exception _e) {
			// Check the state after <timeout> to see if we reconnected
			_ = InfoAsync("Client disconnected, starting timeout task...");
			_ = Task.Delay(Timeout, cancel.Token).ContinueWith(async _ => {
				await DebugAsync("Timeout expired, continuing to check client state...").ConfigureAwait(false);
				await CheckStateAsync().ConfigureAwait(false);
				await DebugAsync("State came back okay").ConfigureAwait(false);
			});

			return Task.CompletedTask;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Checks the state of the task timeout or reconnection.
		/// </summary>
		private async Task CheckStateAsync() {
			// Client reconnected, no need to reset
			if (client.ConnectionState == ConnectionState.Connected) return;
			if (AttemptReset) {
				await InfoAsync("Attempting to reset the client").ConfigureAwait(false);

				var timeout = Task.Delay(Timeout);
				var connect = client.StartAsync();
				var task = await Task.WhenAny(timeout, connect).ConfigureAwait(false);

				if (task == timeout) {
					await CriticalAsync("Client reset timed out (task deadlocked?), killing process").ConfigureAwait(false);
					ForceRestart();
				}
				else if (connect.IsFaulted) {
					await CriticalAsync("Client reset faulted, killing process", connect.Exception).ConfigureAwait(false);
					ForceRestart();
				}
				else if (connect.IsCompleted && connect.Status == TaskStatus.RanToCompletion)
					await InfoAsync("Client reset succesfully!").ConfigureAwait(false);
				return;
			}

			await CriticalAsync("Client did not reconnect in time, killing process").ConfigureAwait(false);
			ForceRestart();
		}
		/// <summary>
		/// Forcefully restarts the program.
		/// </summary>
		private void ForceRestart() => Environment.Exit(0);

		#endregion

		#region Private Logging Helpers
		
		/// <summary>
		/// Logs a <see cref="DebugSeverity"/> message.
		/// </summary>
		/// <param name="message">The message to log.</param>
		private Task DebugAsync(string message)
			=> log.LogAsync(new LogMessage(DebugSeverity, LogSource, message));
		/// <summary>
		/// Logs an <see cref="InfoSeverity"/> message.
		/// </summary>
		/// <param name="message">The message to log.</param>
		private Task InfoAsync(string message)
			=> log.LogAsync(new LogMessage(InfoSeverity, LogSource, message));
		/// <summary>
		/// Logs a <see cref="CriticalSeverity"/> message.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="ex">The optional exception to log.</param>
		private Task CriticalAsync(string message, Exception ex = null)
			=> log.LogAsync(new LogMessage(CriticalSeverity, LogSource, message, ex));

		#endregion
	}
}