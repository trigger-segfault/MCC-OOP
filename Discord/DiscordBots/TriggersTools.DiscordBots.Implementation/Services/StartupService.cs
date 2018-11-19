using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;
using TriggersTools.DiscordBots.Commands;

namespace TriggersTools.DiscordBots.Services {
	/// <summary>
	/// The startup service for Discord bots.
	/// </summary>
	internal sealed class StartupService {

		#region Fields

		/// <summary>
		/// True if the <see cref="BotServiceBase"/> services have been initialized.
		/// </summary>
		private bool initialized = false;
		
		/// <summary>
		/// Gets the bot services context.
		/// </summary>
		public DiscordBotServiceContainer Services { get; }
		/// <summary>
		/// Gets the Discord socket client used to communicate with the service.
		/// </summary>
		public DiscordSocketClient Client { get; }
		/// <summary>
		/// The service that manages all Discord bot commands.
		/// </summary>
		public CommandServiceEx Commands { get; }
		/// <summary>
		/// Gets the service for the Discord bot.
		/// </summary>
		public IDiscordBot DiscordBot { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="StartupService"/>
		/// </summary>
		public StartupService(DiscordBotServiceContainer services,
							  IConfigurationRoot config,
							  DiscordSocketClient client,
							  CommandServiceEx commands,
							  IDiscordBot discordBot)
		{
			Services = services;
			Client = client;
			Commands = commands;
			DiscordBot = discordBot;
		}

		#endregion

		#region Start

		/// <summary>
		/// Starts up the bot and provides the services.
		/// </summary>
		/// <param name="services">The Bot service provider.</param>
		public async Task StartAsync() {
			string discordToken = DiscordBot.GetDiscordToken(); // Get the discord token from the config file

			await Client.LoginAsync(TokenType.Bot, discordToken).ConfigureAwait(false);	// Login to discord
			await Client.StartAsync().ConfigureAwait(false);                              // Connect to the websocket
			Client.Connected += OnInitializeAsync;
		}

		/// <summary>
		/// Initializes the services and commands.
		/// </summary>
		/// <returns></returns>
		private async Task OnInitializeAsync() {
			if (!initialized) {
				initialized = true;
				await DiscordBot.LoadCommandModulesAsync().ConfigureAwait(false);
				Commands.InitializeDetails(Services);
				Services.InitializeServices();
			}
		}

		#endregion
	}
}
