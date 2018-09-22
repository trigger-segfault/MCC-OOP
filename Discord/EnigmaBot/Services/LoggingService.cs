using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EnigmaBot.Services {
	public class LoggingService : BotServiceBase {

		private string LogDirectory { get; }
		private string LogFile => Path.Combine(LogDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");

		private static string ErrorDirectory { get; }
		private static string ErrorFile => Path.Combine(ErrorDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");

		static LoggingService() {
			ErrorDirectory = Path.Combine(AppContext.BaseDirectory, "Errors");
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
		}

		private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
			if (!(e.Exception is AggregateException aggr))
				WriteException(e.Exception);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			WriteException(e.ExceptionObject);
		}

		private static void WriteException(object ex) {
			try {
				Console.WriteLine($"Caught: { ex.GetType().Name}\n{ex.ToString()}");
				string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{ex.GetType().Name}] {ex.ToString()}";
				File.AppendAllText(ErrorFile, logText);
			}
			catch (IOException) { }
		}

		// DiscordSocketClient and CommandService are injected automatically from the IServiceProvider
		public LoggingService(DiscordSocketClient client, CommandService commands) {
			LogDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");

			client.Log += OnLogAsync;
			commands.Log += OnLogAsync;
		}

		protected override void OnInitialized(ServiceProvider services) {
			base.OnInitialized(services);
		}

		private async Task OnLogAsync(LogMessage msg) {
			if (!Directory.Exists(LogDirectory))     // Create the log directory if it doesn't exist
				Directory.CreateDirectory(LogDirectory);
			if (!File.Exists(LogFile))               // Create today's log file if it doesn't exist
				File.Create(LogFile).Dispose();

			string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
			try {
				File.AppendAllText(LogFile, logText + "\n");     // Write the log text to a file
			}
			catch (IOException) { }
			//if (msg.Severity < LogSeverity.Info || msg.Source.Contains("command"))
			await Console.Out.WriteLineAsync(logText);       // Write the log text to the console
		}
	}
}
