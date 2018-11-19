using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Extensions;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots {
	/// <summary>
	/// List services here that should be accessible from other services.
	/// </summary>
	/// <remarks>
	/// This class does make a mess of dependency injection, but it's helpful in reducing the typing load
	/// which is extremely important in my situation.
	/// </remarks>
	public class DiscordBotServiceContainer : IServiceProvider, IDisposable {

		#region Fields

		/// <summary>
		/// True if the <see cref="BotServiceBase"/> services have been initialized.
		/// </summary>
		private bool initialized = false;

		/// <summary>
		/// Gets the provider for the services.
		/// </summary>
		public IServiceProvider Provider { get; }
		/// <summary>
		/// Gets the collection for the services.
		/// </summary>
		public IServiceCollection Collection { get; }
		/// <summary>
		/// Gets the Discord bot running this program.
		/// </summary>
		public IDiscordBot DiscordBot { get; }
		/// <summary>
		/// Gets the connected Discord socket client.
		/// </summary>
		public DiscordSocketClient Client { get; }
		/// <summary>
		/// The service that manages all Discord bot commands.
		/// </summary>
		public CommandServiceEx Commands { get; }
		/// <summary>
		/// Gets the set of command details after the service has been initialized.
		/// </summary>
		public CommandSetDetails CommandSet => Commands.CommandSet;
		/// <summary>
		/// Gets the configuration information file.
		/// </summary>
		public IConfigurationRoot Config { get; }
		/// <summary>
		/// The service for managing command contexts.
		/// </summary>
		public IContextingService Contexting { get; }
		/// <summary>
		/// The service for logging messages to the console and to file.
		/// </summary>
		public ILoggingService Logging { get; }
		/// <summary>
		/// The service for random number generation.
		/// </summary>
		public Random Random { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="DiscordBotServiceContainer"/> with the specified services.
		/// </summary>
		/// <param name="services">The service collection to construct the provider from.</param>
		public DiscordBotServiceContainer(IServiceCollection services) {
			Collection = services;
			// Add all extended service types to the container
			Type serviceType = GetType();
			while (serviceType != null) {
				services.AddSingleton(serviceType, this);
				serviceType = serviceType.BaseType;
			}
			Provider = services.BuildServiceProvider();

			DiscordBot = GetService<IDiscordBot>();
			Client = GetService<DiscordSocketClient>();
			Commands = GetService<CommandServiceEx>();
			Config = GetService<IConfigurationRoot>();
			Contexting = GetService<IContextingService>();
			Logging = GetService<ILoggingService>();
			Random = GetService<Random>();

			AssignServices();

			// Enumerate over all singleton services once so that they
			// have been activated and any events have been setup.
			GetServices(ServiceLifetime.Singleton).Count();
		}

		#endregion

		#region Virtaul Methods

		/// <summary>
		/// Override this to assign global services that should be prepared before all other services are
		/// constructed.
		/// </summary>
		public virtual void AssignServices() { }

		#endregion

		#region Initialize

		/// <summary>
		/// Initializes the Discord bot services.
		/// </summary>
		public void InitializeServices() {
			if (!initialized) {
				initialized = false;
				foreach (IDiscordBotService service in GetServices<IDiscordBotService>(ServiceLifetime.Singleton))
					service.Initialize();
				foreach (DbContext context in GetDbContexts())
					context.Database.EnsureCreated();
			}
		}

		#endregion

		#region IServiceProvider Implementation

		/// <summary>
		/// Get service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <typeparam name="T">An object that specifies the type of service object to get.</typeparam>
		/// <returns>A service object of type <paramref name="serviceType"/>.</returns>
		/// 
		/// <exception cref="InvalidOperationException">
		/// There is no service of type <paramref name="serviceType"/>.
		/// </exception>
		public object GetService(Type serviceType) => Provider.GetRequiredService(serviceType);

		/// <summary>
		/// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
		/// </summary>
		/// <typeparam name="T">The type of service object to get.</typeparam>
		/// <returns>A service object of type <typeparamref name="T"/>.</returns>
		/// 
		/// <exception cref="InvalidOperationException">
		/// There is no service of type <typeparamref name="T"/>.
		/// </exception>
		public T GetService<T>() => Provider.GetRequiredService<T>();

		/*/// <summary>
		/// Gets all services types in the collection.
		/// </summary>
		/// <returns>The collection of service types.</returns>
		public IEnumerable<Type> GetServiceTypes() => Collection.Select(sd => sd.ServiceType);*/
		/// <summary>
		/// Gets all services in the collection.
		/// </summary>
		/// <param name="lifetime">The required lifetime of the service. Null for any type.</param>
		/// <returns>The collection of services.</returns>
		public IEnumerable<object> GetServices(ServiceLifetime lifetime) {
			return Collection.GetServices(Provider, lifetime);
		}
		/// <summary>
		/// Gets all services of base type <typeparamref name="T"/> in the collection.
		/// </summary>
		/// <typeparam name="T">The base type of the services to get.</typeparam>
		/// <param name="lifetime">The required lifetime of the service. Null for any type.</param>
		/// <returns>The collection of <typeparamref name="T"/> services.</returns>
		public IEnumerable<T> GetServices<T>(ServiceLifetime lifetime) {
			return Collection.GetServices<T>(Provider, lifetime);
		}

		/// <summary>
		/// Gets the <see cref="DbContextEx"/> of type <typeparamref name="TDbContext"/>.
		/// </summary>
		/// <typeparam name="TDbContext">The type of the database context.</typeparam>
		/// <param name="configurationType">The optional database configuration type.</param>
		/// <returns>The constructed database context.</returns>
		public TDbContext GetDb<TDbContext>(string configurationType = null) where TDbContext : DbContextEx {
			TDbContext db = Provider.GetRequiredService<TDbContext>();
			db.ConfigurationType = configurationType;
			return db;
		}
		/// <summary>
		/// Gets all bot database contexts.
		/// </summary>
		/// <returns>The collection of <see cref="DbContextEx"/>s.</returns>
		public IEnumerable<DbContextEx> GetDbContexts() {
			return Collection.GetServices<DbContextEx>(Provider, ServiceLifetime.Transient);
		}

		#endregion

		#region IDisposable Implementation

		/// <summary>
		/// Disposes of the <see cref="DiscordBotServiceContainer"/>.
		/// </summary>
		public void Dispose() {
			foreach (IDisposable service in GetServices<IDisposable>(ServiceLifetime.Singleton)) {
				service.Dispose();
			}
		}

		#endregion
	}
}
