using System;
using System.Collections.Generic;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// The command wait context for Discord bot commands that require input over time.
	/// </summary>
	public class DiscordBotUserWaitContext : SocketUserWaitContext, IDiscordBotUserWaitContext, IDiscordBotServiceContainer {

		#region Fields

		/// <summary>
		/// Gets the container for all Discord bot services.
		/// </summary>
		public DiscordBotServiceContainer Services { get; }
		/// <summary>
		/// Gets the Discord bot running this program.
		/// </summary>
		public IDiscordBot DiscordBot => Services.DiscordBot;
		/// <summary>
		/// Gets the connected Discord socket client.
		/// </summary>
		public new DiscordSocketClient Client => Services.Client;
		/// <summary>
		/// The service that manages all Discord bot commands.
		/// </summary>
		public CommandServiceEx Commands => Services.Commands;
		/// <summary>
		/// Gets the set of command details after the service has been initialized.
		/// </summary>
		public CommandSetDetails CommandSet => Services.Commands.CommandSet;
		/// <summary>
		/// Gets the configuration information file.
		/// </summary>
		public IConfigurationRoot Config => Services.Config;
		/// <summary>
		/// The service for managing command contexts.
		/// </summary>
		public IContextingService Contexting => Services.Contexting;
		/// <summary>
		/// The service for logging messages to the console and to file.
		/// </summary>
		public ILoggingService Logging => Services.Logging;
		/// <summary>
		/// The service for random number generation.
		/// </summary>
		public Random Random => Services.Random;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="DiscordBotUserWaitContext"/>.
		/// </summary>
		/// <param name="services">The Discord bot service container.</param>
		/// <param name="client">The underlying client.</param>
		/// <param name="msg">The underlying message.</param>
		public DiscordBotUserWaitContext(DiscordBotServiceContainer services, DiscordSocketClient client,
			SocketUserMessage msg, string name, TimeSpan duration)
			: base(client, msg, services.Commands, name, duration)
		{
			Services = services;
		}
		/// <summary>
		/// Constructs the <see cref="DiscordBotUserWaitContext"/>.
		/// </summary>
		/// <param name="context">The underlying command context.</param>
		public DiscordBotUserWaitContext(DiscordBotCommandContext context, string name, TimeSpan duration)
			: base(context, context.Services.Commands, name, duration)
		{
			Services = context.Services;
		}

		#endregion

		#region Services

		/// <summary>
		/// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
		/// </summary>
		/// <typeparam name="T">The type of service object to get.</typeparam>
		/// <returns>A service object of type <typeparamref name="T"/>.</returns>
		/// 
		/// <exception cref="InvalidOperationException">
		/// There is no service of type <typeparamref name="T"/>.
		/// </exception>
		public T GetService<T>() {
			return Services.GetService<T>();
		}
		/// <summary>
		/// Gets all single services of base type <typeparamref name="T"/> in the collection.
		/// </summary>
		/// <typeparam name="T">The base type of the services to get.</typeparam>
		/// <returns>The collection of <typeparamref name="T"/> services.</returns>
		public IEnumerable<T> GetServices<T>() {
			return Services.GetServices<T>(ServiceLifetime.Singleton);
		}

		/// <summary>
		/// Gets the <see cref="DbContextEx"/> of type <typeparamref name="TDbContext"/>.
		/// </summary>
		/// <typeparam name="TDbContext">The type of the database context.</typeparam>
		/// <param name="configurationType">The optional database configuration type.</param>
		/// <returns>The constructed database context.</returns>
		public TDbContext GetDb<TDbContext>(string configurationType = null) where TDbContext : DbContextEx {
			return Services.GetDb<TDbContext>(configurationType);
		}
		/// <summary>
		/// Gets all bot database contexts.
		/// </summary>
		/// <returns>The collection of <see cref="DbContextEx"/>s.</returns>
		public IEnumerable<DbContextEx> GetDbContexts() {
			return Services.GetDbContexts();
		}

		#endregion
	}
}
