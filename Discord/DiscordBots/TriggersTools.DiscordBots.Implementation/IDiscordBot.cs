using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots {
	/// <summary>
	/// The interface for a Discord bot that manages everything.
	/// </summary>
	public interface IDiscordBot {

		#region Properties

		/// <summary>
		/// Gets the Discord bot services container.
		/// </summary>
		DiscordBotServiceContainer Services { get; }
		/// <summary>
		/// Gets the run state of the Discord Bot.
		/// </summary>
		DiscordBotRunState RunState { get; }
		/// <summary>
		/// Gets the uptime for the bot, or the time that it has been running for and connected to Discord.
		/// </summary>
		TimeSpan Uptime { get; }
		/// <summary>
		/// Gets the total uptime of the bot across all executions.
		/// </summary>
		TimeSpan TotalUptime { get; }
		/// <summary>
		/// Gets the configuration information file. This must be assigned on <see cref="LoadConfig"/>.
		/// </summary>
		IConfigurationRoot Config { get; }
		/// <summary>
		/// Gets the total memory usage of the garbage collector.
		/// </summary>
		long GCUsage { get; }
		/// <summary>
		/// Gets the total memory usage of the program.
		/// </summary>
		/*long RamUsage { get; }*/
		/*/// <summary>
		/// Gets the default prefix used by the Discord Bot.
		/// </summary>
		string DefaultPrefix { get; }*/

		#endregion

		#region Events

		/// <summary>
		/// Raised when an exception occurs during startup.<para/>
		/// This event should no longer fire after <see cref="StartAsync"/> has finished calling.
		/// </summary>
		event StaticExceptionEventHandler StartupError;

		/// <summary>
		/// Called when the bot is first shutting down.
		/// </summary>
		event ShuttingDownEventHandler ShuttingDown;

		/// <summary>
		/// Raised when end user data of any type is being deleted.
		/// </summary>
		event EndUserDataEventHandler DeletingEndUserData;

		/// <summary>
		/// Raised ONCE when the configuration file is reloaded.
		/// </summary>
		event AsyncEventHandler ConfigReloaded;

		#endregion

		#region Configuration

		/// <summary>
		/// Loads the configuration files used for the bot.
		/// </summary>
		IConfigurationRoot LoadConfig();
		/// <summary>
		/// Loads the <see cref="DiscordSocketConfig"/> used to automatically setup the
		/// <see cref="DiscordSocketClient"/>.
		/// </summary>
		DiscordSocketConfig GetDiscordSocketConfig();
		/// <summary>
		/// Loads the <see cref="CommandServiceConfig"/> used to automatically setup the
		/// <see cref="CommandService"/>.
		/// </summary>
		CommandServiceConfig GetCommandServiceConfig();
		/// <summary>
		/// Loads the <see cref="ReliabilityConfig"/> used to automatically setup the
		/// <see cref="ReliabilityService"/>.
		/// </summary>
		ReliabilityConfig GetReliabilityConfig();

		/// <summary>
		/// Configures all required services for the Discord Bot.
		/// </summary>
		/// <param name="services">The services to configure.</param>
		/// <returns>Returns <paramref name="services"/>.</returns>
		IServiceCollection ConfigureServices(IServiceCollection services);
		/// <summary>
		/// Configures all required database services for the Discord Bot.
		/// </summary>
		/// <param name="services">The services to configure.</param>
		/// <returns>Returns <paramref name="services"/>.</returns>
		IServiceCollection ConfigureDatabase(IServiceCollection services);

		/*/// <summary>
		/// Creates the <see cref="DiscordBotServiceContainer"/> or an extended class.
		/// </summary>
		/// <param name="services">The service collection to initialize the service container with.</param>
		/// <returns>The newly created service container.</returns>
		DiscordBotServiceContainer CreateServiceContainer(IServiceCollection services);*/

		/// <summary>
		/// Gets the secret token used to control the Discord bot.
		/// </summary>
		/// <returns>The discord token.</returns>
		string GetDiscordToken();

		#endregion

		#region Management

		/// <summary>
		/// Asynchronously loads all related command modules.
		/// </summary>
		Task LoadCommandModulesAsync();
		/// <summary>
		/// Asynchronously initializes the Discord bot by giving it access to the server collection.
		/// </summary>
		/// <param name="services">The services to create the <see cref="DiscordBotServiceContainer"/> with.</param>
		Task<DiscordBotServiceContainer> InitializeAsync(IServiceCollection services);
		/// <summary>
		/// Asynchronously tells the Discord bot that everything has been setup.
		/// </summary>
		Task StartAsync();
		/// <summary>
		/// Asynchronously shuts down the bot.
		/// </summary>
		/// <param name="context">The context that the shutdown occurred in.</param>
		Task ShutdownAsync(ICommandContext context = null);
		/// <summary>
		/// Asynchronously restarts the bot by starting a new process.
		/// </summary>
		/// <param name="context">The context that the shutdown occurred in.</param>
		/// <param name="restartMessage">The message to display in the context channel after restarting.</param>
		Task RestartAsync(ICommandContext context = null, string restartMessage = null);

		Task<bool> DeleteEndUserDataAsync(DbContextEx db, ulong id, EndUserDataContents euds, EndUserDataType type);

		/// <summary>
		/// Reloads the configuration file and notifies all listening services.
		/// </summary>
		/// <returns></returns>
		Task ReloadConfigAsync();

		#endregion
	}
}
