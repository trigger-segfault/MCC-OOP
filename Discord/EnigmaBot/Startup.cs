using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EnigmaBot.Context;
using EnigmaBot.Services;

namespace EnigmaBot {
	public class Startup {
		public IConfigurationRoot Configuration { get; }

		public Startup(string[] args) {
			var builder = new ConfigurationBuilder()        // Create a new instance of the config builder
				.SetBasePath(AppContext.BaseDirectory)      // Specify the default location for the config file
				.AddJsonFile("Config.json");        // Add this (json encoded) file to the configuration
			Configuration = builder.Build();                // Build the configuration
		}

		public static async Task RunAsync(string[] args) {
			var startup = new Startup(args);
			await startup.RunAsync();
		}

		public async Task RunAsync() {
			var services = new ServiceCollection();             // Create a new instance of a service collection
			ConfigureServices(services);

			var provider = services.BuildServiceProvider();     // Build the service provider
			provider.GetRequiredService<CommandHandler>();      // Start the command handler service
			provider.GetRequiredService<LoggingService>();      // Start the logging service
			provider.GetRequiredService<EnigmaService>();      // Start the command handler service
			provider.GetRequiredService<HelpService>();      // Start the command handler service
			provider.GetRequiredService<IConfigurationRoot>();      // Start the command handler service

			await provider.GetRequiredService<StartupService>().StartAsync(provider, services);       // Start the startup service
			await Task.Delay(-1);                               // Keep the program alive
		}

		private void ConfigureServices(ServiceCollection services) {
			var config = new DiscordSocketConfig {
				// Add discord to the collection
				LogLevel = LogSeverity.Verbose,     // Tell the logger to give Verbose amount of info
				MessageCacheSize = 1000,             // Cache 1,000 messages per channel
				DefaultRetryMode = RetryMode.AlwaysRetry,
			};
			// Are we running on <= Windows 7?
			// Standard websocket only works for Windows 8+.
			if (WS4NetProvider.IsRequiredByOS) {
				config.WebSocketProvider = WS4NetProvider.Instance;
			}
			services.AddSingleton(new DiscordSocketClient(config))
			.AddSingleton(new CommandService(new CommandServiceConfig {                                       // Add the command service to the collection
				LogLevel = LogSeverity.Verbose,     // Tell the logger to give Verbose amount of info
				DefaultRunMode = RunMode.Async,     // Force all commands to run async by default
				CaseSensitiveCommands = false,       // Ignore case when executing commands
			}))
			.AddSingleton(Configuration)            // Add the configuration to the collection
			.AddSingleton<StartupService>()         // Add startupservice to the collection
			.AddSingleton<LoggingService>()         // Add loggingservice to the collection
			.AddSingleton<EnigmaService>()         // Add loggingservice to the collection
			.AddSingleton<CommandHandler>()
			.AddSingleton<HelpService>()
			.AddSingleton<Random>();                 // Add random to the collection
													 /*.AddDbContext<TriggerDatabaseContext>(options => {
														 options.UseSqlite("Data Source=trigger_chan.db");
													 }, ServiceLifetime.Transient);*/
		}
	}
}
