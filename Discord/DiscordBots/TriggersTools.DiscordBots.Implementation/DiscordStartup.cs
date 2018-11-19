using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots {
	/// <summary>
	/// The static class for running a <see cref="IDiscordBot"/>.
	/// </summary>
	public class DiscordStartup {

		#region Constants

		/// <summary>
		/// The exit code signifying the bot is restarting.
		/// </summary>
		public const int RestartExitCode = 10;
		/// <summary>
		/// The argument used to specify a daemon is running the Discord bot.
		/// </summary>
		public const string DaemonArgument = "-daemon";

		#endregion

		#region Fields

		/// <summary>
		/// The Discord bot service provider.
		/// </summary>
		private DiscordBotServiceContainer services;
		/// <summary>
		/// The watch to count the startup time.
		/// </summary>
		private readonly Stopwatch startupWatch = Stopwatch.StartNew();
		/// <summary>
		/// The command line arguments.
		/// </summary>
		private readonly string[] args;
		/// <summary>
		/// The discord bot being run.
		/// </summary>
		private readonly IDiscordBot discordBot;
		/// <summary>
		/// The shutdown state, false if shutting down, true if restarting.
		/// </summary>
		//private bool isRestarting;
		/// <summary>
		/// The shutdown mode of the bot.
		/// </summary>
		private ShutdownMode shutdownMode;
		/// <summary>
		/// The wait event for bot shutdown.
		/// </summary>
		private readonly ManualResetEvent shutdownEvent = new ManualResetEvent(false);
		/// <summary>
		/// True if multiple instances of the Discord Bot token can run at the same time.
		/// </summary>
		private readonly bool allowMultipleInstances;
		/// <summary>
		/// True if <see cref="DaemonArgument"/> was passed, meaning another program is running this on a
		/// loop.
		/// </summary>
		private readonly bool isRunningFromDaemon;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="DiscordStartup"/> instance to run the bot.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		/// <param name="discordBot">The discord bot to run.</param>
		/// <param name="allowMultipleInstances">
		/// True if multiple instances of the Discord Bot token can run at the same time.
		/// </param>
		private DiscordStartup(string[] args, IDiscordBot discordBot, bool allowMultipleInstances) {
			this.args = args;
			this.discordBot = discordBot;
			this.allowMultipleInstances = allowMultipleInstances;
			if (args.Length > 0 && args[0] == DaemonArgument) {
				isRunningFromDaemon = true;
				// Be nice and remove the daemon argument.
				this.args = args.Skip(1).ToArray();
			}
			discordBot.StartupError += OnStartupError;
		}

		#endregion

		#region Static Properties

		/// <summary>
		/// Gets if the Operating system requires <see cref="WebSocket4Net"/> socket to function.
		/// </summary>
		private static bool RequiresWS4Net {
			get {
				return	Environment.OSVersion.Platform == PlatformID.Win32NT &&
						Environment.OSVersion.Version <= new Version(6, 1);
			}
		}

		#endregion

		#region Mutex

		/// <summary>
		/// The mutex used to identify a single instance of the bot.
		/// </summary>
		private Mutex mutex;
		/// <summary>
		/// True if the created mutex was new.
		/// </summary>
		private bool mutexCreatedNew;

		/// <summary>
		/// Checks if this Discord bot is the only instance of the application currently running.
		/// </summary>
		/// <returns>True if no other Discord bot with the same token is running.</returns>
		public bool SingleInstanceCheck() {
			if (allowMultipleInstances)
				return true;
			if (mutex == null)
				mutex = new Mutex(true, discordBot.GetDiscordToken(), out mutexCreatedNew);
			if (!mutexCreatedNew) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Startup Error: More than one instance of the Discord bot with this token is running!");
				Console.Beep();
				Console.Beep();
				Console.WriteLine("Press any key to exit...");
				Console.ResetColor();
				Console.ReadLine();
			}
			return mutexCreatedNew;
		}
		
		#endregion

		#region Run

		/// <summary>
		/// Runs the <see cref="IDiscordBot"/>.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		/// <param name="createDiscordBot">The discord bot to run.</param>
		/// <param name="allowMultipleInstances">
		/// True if multiple instances of the Discord Bot token can run at the same time.
		/// </param>
		/// <returns>The exit code of the program.</returns>
		public static Task<int> RunAsync(string[] args, Func<IDiscordBot> createDiscordBot,
			bool allowMultipleInstances = false)
		{
			if (createDiscordBot == null)
				throw new ArgumentNullException(nameof(createDiscordBot));
			DiscordStartup startup = new DiscordStartup(args, createDiscordBot(), allowMultipleInstances);
			return startup.RunAsync();
		}

		/// <summary>
		/// Runs the <see cref="IDiscordBot"/>.
		/// </summary>
		private async Task<int> RunAsync() {
			discordBot.LoadConfig();

			if (allowMultipleInstances || !SingleInstanceCheck())
				return 0;
			
			services = await discordBot.InitializeAsync(ConfigureServices()).ConfigureAwait(false);

			discordBot.ShuttingDown += OnShuttingDownAsync;

			// Start the startup service
			services.GetRequiredService<DiscordSocketClient>().Ready += OnReadyAsync;
			await services.GetRequiredService<StartupService>().StartAsync().ConfigureAwait(false);
			await discordBot.StartAsync().ConfigureAwait(false);

			// Keep the program alive until shutdown/restart
			shutdownEvent.WaitOne();

			// Wait a moment to prevent wonkey Discord message output
			//await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);

			// Allow time to go invisible
			await Task.Delay(TimeSpan.FromSeconds(1.5)).ConfigureAwait(false);
			await services.GetRequiredService<DiscordSocketClient>().StopAsync().ConfigureAwait(false);
			mutex?.Dispose();
			services.Dispose();

			if (shutdownMode == ShutdownMode.Restart) {
				if (isRunningFromDaemon)
					return RestartExitCode;
				
				// Restart right here, right now
				string arguments = string.Join(" ", args.Select(a => $"\"{a}\""));
#if NETSTANDARD2_0
				Process.Start("dotnet", $"{(Assembly.GetEntryAssembly().Location)} {arguments}");
#else
				Process.Start(Assembly.GetEntryAssembly().Location, arguments);
#endif

				// Double clear is needed to ensure *actually cleared* console. It's weird.
				Console.Clear();
				Console.Clear();
				Environment.Exit(0);
			}
			return 0;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Called the first time the Discord client is ready.
		/// </summary>
		private Task OnReadyAsync() {
			services.GetRequiredService<DiscordSocketClient>().Ready -= OnReadyAsync;
			return services.GetRequiredService<ILoggingService>()
				.LogAsync(new LogMessage(LogSeverity.Info, "Startup",
				$"Startup took {startupWatch.ElapsedMilliseconds}ms"));
		}
		/// <summary>
		/// Called when the bot is shutting down or restarting.
		/// </summary>
		private Task OnShuttingDownAsync(ShuttingDownEventArgs e) {
			shutdownMode = e.Mode;
			shutdownEvent.Set();
			return Task.FromResult<object>(null);
		}

		/// <summary>
		/// Called to print a startup error and hault execution.
		/// </summary>
		private void OnStartupError(Exception ex) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Startup Error: {ex}");
			Console.Beep();
			Console.Beep();
			Console.WriteLine("Press any key to exit...");
			Console.ResetColor();
			Console.ReadLine();
			Environment.Exit(0);
		}


		#endregion

		#region Configuration

		/// <summary>
		/// Configures the base services that are required for the bot to function.
		/// </summary>
		/// <returns>The service collection to reference for this bot service provider.</returns>
		private IServiceCollection ConfigureServices() {
			IConfigurationRoot config = discordBot.Config;
			DiscordSocketConfig discordConfig = discordBot.GetDiscordSocketConfig();
			CommandServiceConfig commandConfig = discordBot.GetCommandServiceConfig();
			ReliabilityConfig reliabilityConfig = discordBot.GetReliabilityConfig();

			// Are we running on <= Windows 7?
			// Standard websocket only works for Windows 8+.
			if (RequiresWS4Net) {
				discordConfig.WebSocketProvider = WS4NetProvider.Instance;
			}

			ServiceCollection services = new ServiceCollection();
			discordBot.ConfigureServices(services);
			discordBot.ConfigureDatabase(services);
			return services
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton<CommandServiceEx>()
				.AddSingleton<StartupService>()
				.AddSingleton<ReliabilityService>()
				.AddSingleton(config)
				.AddSingleton(discordConfig)
				.AddSingleton(commandConfig)
				.AddSingleton(reliabilityConfig)
				.AddSingleton(discordBot)
				.AddSingleton(args) // (Make startup args accessible)
				;
		}

		#endregion
	}
}
