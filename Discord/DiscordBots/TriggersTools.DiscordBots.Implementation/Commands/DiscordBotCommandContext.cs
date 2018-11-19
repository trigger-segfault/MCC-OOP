using System;
using System.Collections.Generic;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Database.Model;
using TriggersTools.DiscordBots.Extensions;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// The context for all Discord bot commands.
	/// </summary>
	public class DiscordBotCommandContext : SocketCommandContext, IDiscordBotCommandContext, IDiscordBotServiceContainer {

		#region Fields

		/// <summary>
		/// Gets the container for all Discord bot services.
		/// </summary>
		public DiscordBotServiceContainer Services { get; }
		/// <summary>
		/// Gets the lock context used to determine if commands are locked.
		/// </summary>
		public IDbLockableContext LockContext { get; }
		/// <summary>
		/// Gets the manager context used to determine if the user has a manager role.
		/// </summary>
		public IDbManagerContext ManageContext { get; }
		/// <summary>
		/// Gets the argument position in the command after the prefix.
		/// </summary>
		public int ArgPos { get; set; }
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
		/// Constructs the <see cref="DiscordBotCommandContext"/>.
		/// </summary>
		/// <param name="services">The Discord bot service container.</param>
		/// <param name="client">The underlying client.</param>
		/// <param name="msg">The underlying message.</param>
		public DiscordBotCommandContext(DiscordBotServiceContainer services, DiscordSocketClient client,
			SocketUserMessage msg)
			: base(client, msg)
		{
			Services = services;
			if (!Contexting.IsDbLockableContext(this) && !Contexting.IsDbManagerContext(this))
				return;
			using (var db = Contexting.GetCommandContextDb()) {
				LockContext = Contexting.FindDbLockableContextAsync(db, this, false).GetAwaiter().GetResult();
				if (LockContext is IDbManagerContext manageContext)
					ManageContext = manageContext;
				else
					ManageContext = Contexting.FindDbManagerContextAsync(db, this, false).GetAwaiter().GetResult();
			}
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
