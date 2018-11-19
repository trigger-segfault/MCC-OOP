using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Extensions;
using TriggersTools.DiscordBots.Utils;

namespace TriggersTools.DiscordBots.Services {
	/// <summary>
	/// The default <see cref="ILoggingService"/> implementation.
	/// </summary>
	public class DefaultLoggingService : ILoggingService, IDisposable {
		
		#region Fields

		private readonly bool logDebug;
		private readonly bool logTrace;
		private readonly bool logPrint;
		private readonly bool logNotice;

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructs the <see cref="DefaultLoggingService"/>.
		/// </summary>
		//public LoggingService(DiscordBotServiceContainer services) : base(services) {
		public DefaultLoggingService(DiscordSocketClient client,
							  CommandServiceEx commands,
							  IConfigurationRoot config)
		{
			AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
			client.Log += OnLogAsync;
			commands.Log += OnLogAsync;
			bool.TryParse(config["log:debug"], out logDebug);
			bool.TryParse(config["log:trace"], out logTrace);
			bool.TryParse(config["log:print"], out logPrint);
			bool.TryParse(config["log:notice"], out logNotice);
		}

		#endregion

		#region Properties

		private string BaseDirectory { get; } = Path.Combine(AppContext.BaseDirectory, "Logs");
		private string BaseFile => $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt";
		private string LogDirectory => Path.Combine(BaseDirectory, "Logs");
		private string ErrorDirectory => Path.Combine(BaseDirectory, "Errors");
		private string NoticeDirectory => Path.Combine(BaseDirectory, "Notices");
		private string LogFile => Path.Combine(LogDirectory, BaseFile);
		private string ErrorFile => Path.Combine(ErrorDirectory, BaseFile);
		private string NoticeFile => Path.Combine(NoticeDirectory, BaseFile);

		#endregion

		#region Logging

		/// <summary>
		/// Logs the log message.
		/// </summary>
		/// <param name="msg">The log message to log.</param>
		/// <param name="logFile">True if the log file should be written to.</param>
		/// <param name="errorFile">True if the error file should be written to.</param>
		/// <param name="noticeFile">True if the notice file should be written to.</param>
		public void Log(LogMessage msg, bool logFile = false, bool errorFile = false, bool noticeFile = false) {
			Task.Run(async () => await LogAsync(msg, logFile, errorFile, noticeFile).ConfigureAwait(false));
		}
		/// <summary>
		/// Asyncronously logs the log message.
		/// </summary>
		/// <param name="msg">The log message to log.</param>
		/// <param name="logFile">True if the log file should be written to.</param>
		/// <param name="errorFile">True if the error file should be written to.</param>
		public virtual async Task LogAsync(LogMessage msg, bool logFile = true, bool errorFile = false, bool noticeFile = false) {
			// Ignore these annoying messages
			if (msg.Exception?.InnerException is WebSocketException ||
				msg.Exception?.InnerException is WebSocketClosedException)
				return;

			if (logFile) {
				if (!Directory.Exists(LogDirectory))     // Create the log directory if it doesn't exist
					Directory.CreateDirectory(LogDirectory);
				if (!File.Exists(LogFile))               // Create today's log file if it doesn't exist
					File.Create(LogFile).Dispose();
			}
			if (errorFile || msg.Exception != null) {
				if (!Directory.Exists(ErrorDirectory))     // Create the error directory if it doesn't exist
					Directory.CreateDirectory(ErrorDirectory);
				if (!File.Exists(ErrorFile))               // Create today's error file if it doesn't exist
					File.Create(ErrorFile).Dispose();
			}
			if (noticeFile) {
				if (!Directory.Exists(NoticeDirectory))     // Create the log directory if it doesn't exist
					Directory.CreateDirectory(NoticeDirectory);
				if (!File.Exists(NoticeFile))               // Create today's log file if it doesn't exist
					File.Create(NoticeFile).Dispose();
			}

			string logText = FormatMessage(msg);

			if (logFile) {
				try {
					File.AppendAllText(LogFile, logText + Environment.NewLine);     // Write the log text to a file
				} catch (IOException) { }
			}
			if (errorFile || msg.Exception != null) {
				try {
					File.AppendAllText(ErrorFile, logText + Environment.NewLine);     // Write the error text to a file
				} catch (IOException) { }
			}
			if (noticeFile) {
				try {
					File.AppendAllText(NoticeFile, logText + Environment.NewLine);     // Write the notice text to a file
				} catch (IOException) { }
			}

			bool shouldPrint = msg.Severity <= LogSeverity.Info || msg.Source == "Command";
			switch (msg.Severity) {
			case LogSeverities.Debug: shouldPrint = logDebug; break;
			case LogSeverities.Trace: shouldPrint = logTrace; break;
			case LogSeverities.Print: shouldPrint = logPrint; break;
			case LogSeverities.Notice: shouldPrint = logNotice; break;
			case LogSeverities.Report: shouldPrint = true; break;
			}
			if (shouldPrint) {
				// Write the log text to the console
				ConsoleColor? severityColor = GetSeverityColor(msg.Severity);
				if (severityColor.HasValue) {
					using (ConsoleLock.Lock(severityColor))
						Console.WriteLine(logText);
				}
				else {
					await Console.Out.WriteLineAsync(logText).ConfigureAwait(false);
				}
			}
		}

		/// <summary>
		/// Writes the debug line to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		public void Debug(string line) {
			Task.Run(async () => await DebugAsync(line).ConfigureAwait(false));
		}
		/// <summary>
		/// Asynchornously writes the debug line to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		public Task DebugAsync(string line) {
			return LogAsync(new LogMessage(LogSeverities.Debug, "", line), false);
		}
		/// <summary>
		/// Writes the debug trace to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		public void Trace(string line) {
			Task.Run(async () => await TraceAsync(line).ConfigureAwait(false));
		}
		/// <summary>
		/// Asynchornously writes the trace line to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		public Task TraceAsync(string line) {
			return LogAsync(new LogMessage(LogSeverities.Trace, "", line), false);
		}
		/// <summary>
		/// Writes the line to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		public void Write(string line) {
			Task.Run(async () => await WriteAsync(line).ConfigureAwait(false));
		}
		/// <summary>
		/// Asynchornously writes the line to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		public Task WriteAsync(string line) {
			return LogAsync(new LogMessage(LogSeverities.Print, "", line), false);
		}
		
		/// <summary>
		/// Gets the color of the severity.
		/// </summary>
		/// <param name="severity">The type of log severity.</param>
		/// <returns>The console color. Or null if the color is not different.</returns>
		public ConsoleColor? GetSeverityColor(LogSeverity severity) {
			switch (severity) {
			case LogSeverity.Critical:
				return ConsoleColor.DarkRed;
			case LogSeverity.Error:
				return ConsoleColor.Red;
			case LogSeverity.Warning:
				return ConsoleColor.Yellow;

			case LogSeverities.Debug:
				return ConsoleColor.Magenta;
			case LogSeverities.Trace:
				return ConsoleColor.Cyan;
			case LogSeverities.Print:
				return ConsoleColor.Green;
			case LogSeverities.Notice:
				return ConsoleColor.DarkCyan;
			case LogSeverities.Report:
				return ConsoleColor.DarkMagenta;

			default:
				return null;
			}
		}

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Formats the message into a string.
		/// </summary>
		/// <param name="msg">The log message to format.</param>
		/// <returns>The user friendly presentation of the message.</returns>
		protected virtual string FormatMessage(LogMessage msg) {
			string severity = msg.Severity.ToString();
			switch (msg.Severity) {
			case LogSeverities.Debug: severity = "Debug"; break;
			case LogSeverities.Trace: severity = "Trace"; break;
			case LogSeverities.Print: severity = "Print"; break;
			case LogSeverities.Notice: severity = "Notice"; break;
			case LogSeverities.Report: severity = "Report"; break;
			}
			StringBuilder str = new StringBuilder();
			str.Append($"{DateTime.UtcNow.ToString("hh:mm:ss")} [{severity}]");
			if (!string.IsNullOrWhiteSpace(msg.Source))
				str.Append($" {msg.Source}");
			str.Append($": {msg.Exception?.ToString() ?? msg.Message}");
			return str.ToString();
		}

		#endregion

		#region Exception Handlers

		/// <summary>
		/// Called to log Discord client and command messages.
		/// </summary>
		private Task OnLogAsync(LogMessage msg) => LogAsync(msg, true);
		/// <summary>
		/// Called when an unhandled exception occurs in the <see cref="AppDomain"/>.
		/// </summary>
		private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			Exception ex = e.ExceptionObject as Exception;
			string message = ex?.Message ?? e.ExceptionObject.ToString();
			Log(new LogMessage(LogSeverity.Error, "AppDomain.UnhandledException", message, ex), true, true);
		}
		/// <summary>
		/// Called when an unhandled exception occurs in the <see cref="TaskScheduler"/>.
		/// </summary>
		private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
			// Skip this, they're EVERYWHERE!
			if (e.Exception is AggregateException) return;

			Exception ex = e.Exception;
			string message = ex.Message;
			Log(new LogMessage(LogSeverity.Error, "TaskScheduler.UnobservedTaskException", message, ex), true, true);
		}

		#endregion

		#region IDisposable Implementation

		/// <summary>
		/// Disposes of the service.
		/// </summary>
		public void Dispose() {
			AppDomain.CurrentDomain.UnhandledException -= AppDomain_UnhandledException;
			TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
		}

		#endregion
	}
}
