using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots {
	/// <summary>
	/// A context for Discord bot services, modules, and command contexts.
	/// </summary>
	public interface IDiscordBotServiceContainer {

		#region Properties

		/// <summary>
		/// Gets the container for all Discord bot services.
		/// </summary>
		DiscordBotServiceContainer Services { get; }
		/// <summary>
		/// Gets the Discord bot running this program.
		/// </summary>
		IDiscordBot DiscordBot { get; }
		/// <summary>
		/// Gets the connected Discord socket client.
		/// </summary>
		DiscordSocketClient Client { get; }
		/// <summary>
		/// The service that manages all Discord bot commands.
		/// </summary>
		CommandServiceEx Commands { get; }
		/// <summary>
		/// Gets the configuration information file.
		/// </summary>
		IConfigurationRoot Config { get; }
		/// <summary>
		/// The service for managing command contexts.
		/// </summary>
		IContextingService Contexting { get; }
		/// <summary>
		/// The service for logging messages to the console and to file.
		/// </summary>
		ILoggingService Logging { get; }
		/// <summary>
		/// The service for random number generation.
		/// </summary>
		Random Random { get; }

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
		T GetService<T>();
		/// <summary>
		/// Gets all single services of base type <typeparamref name="T"/> in the collection.
		/// </summary>
		/// <typeparam name="T">The base type of the services to get.</typeparam>
		/// <returns>The collection of <typeparamref name="T"/> services.</returns>
		IEnumerable<T> GetServices<T>();

		/// <summary>
		/// Gets the <see cref="DbContextEx"/> of type <typeparamref name="TDbContext"/>.
		/// </summary>
		/// <typeparam name="TDbContext">The type of the database context.</typeparam>
		/// <param name="configurationType">The optional database configuration type.</param>
		/// <returns>The constructed database context.</returns>
		TDbContext GetDb<TDbContext>(string configurationType = null) where TDbContext : DbContextEx;
		/// <summary>
		/// Gets all bot database contexts.
		/// </summary>
		/// <returns>The collection of <see cref="DbContextEx"/>s.</returns>
		IEnumerable<DbContextEx> GetDbContexts();

		#endregion

	}
}
