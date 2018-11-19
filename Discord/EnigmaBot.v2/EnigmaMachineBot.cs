using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using EnigmaBot.Database;
using EnigmaBot.Reactions;
using EnigmaBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Reactions;
using TriggersTools.DiscordBots.Services;
using TriggersTools.DiscordBots.Utils;

namespace EnigmaBot {
	public class EnigmaMachineBot : DiscordBot {
		
		public override IServiceCollection ConfigureServices(IServiceCollection services) {
			return services
				.AddSingleton<CommandHandlerService>()
				.AddSingleton<ILoggingService, DefaultLoggingService>()
				.AddSingleton<IContextingService, DefaultContextingService>()
				.AddSingleton<ResultHandlerService>()
				.AddSingleton<HelpService>()
				.AddSingleton<ReactionService, EnigmaReactions>()
				.AddSingleton<StatusRotationService>()
				.AddSingleton<ConfigParserService>()
				.AddSingleton<EnigmaService>()

				.AddSingleton<Random>()
				;
		}

		public override IServiceCollection ConfigureDatabase(IServiceCollection services) {
			return services
				//.AddEntityFrameworkNpgsql()
				.AddSingleton<IDbProvider, EnigmaDbProvider>()
				.AddDbContext<EnigmaDbContext>(ServiceLifetime.Transient)
				;
		}
		
		public override IConfigurationRoot LoadConfig() {
			var builder = new ConfigurationBuilder()            // Create a new instance of the config builder
				.SetBasePath(AppContext.BaseDirectory)          // Specify the default location for the config file
				.AddJsonFile("Config.Public.json")              // Add this (json encoded) file to the configuration
				.AddJsonFile("Config.Private.json")             // Add this (json encoded) file to the configuration
				//.AddJsonFile(GetType(), "Config.Private.json");	// Add this embedded private (json encoded) file to the configuration
				;
			return Config = builder.Build();					// Build the configuration
		}

		public override DiscordSocketConfig GetDiscordSocketConfig() {
			return ConfigUtils.Parse<DiscordSocketConfig>(Config.GetSection("config"));
		}
		public override CommandServiceConfig GetCommandServiceConfig() {
			return ConfigUtils.Parse<CommandServiceConfig>(Config.GetSection("config"));
		}

		public override ReliabilityConfig GetReliabilityConfig() {
			return ConfigUtils.Parse<ReliabilityConfig>(Config.GetSection("config"));
		}

		public override async Task LoadCommandModulesAsync() {
			await base.LoadCommandModulesAsync().ConfigureAwait(false);
			//await Commands.AddModuleAsync<SpoilerModule>(Services).ConfigureAwait(false);
		}

		public override DiscordBotServiceContainer CreateServiceContainer(IServiceCollection services) {
			return new DiscordBotServiceContainer(services);
		}
	}
}
