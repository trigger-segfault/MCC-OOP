using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Database.Model;
using TriggersTools.DiscordBots.Extensions;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots {
	/// <summary>
	/// The base implementation of the <see cref="IDiscordBot"/> interface.
	/// </summary>
	public class DiscordBot : IDiscordBot, IDisposable {

		#region Constants

		/// <summary>
		/// The file to read from when displaying the *restarted* message.
		/// </summary>
		private const string RestartMessageFile = "DiscordBot.RestartMessage.txt";
		/// <summary>
		/// The file to read and write to, storing the total uptime of the bot.
		/// </summary>
		private const string TotalUptimeFile = "DiscordBot.TotalUptime.txt";

		#endregion

		#region Fields

		/// <summary>
		/// Gets the Discord bot services container.
		/// </summary>
		public DiscordBotServiceContainer Services { get; protected set; }
		/// <summary>
		/// Gets the run state of the Discord Bot.
		/// </summary>
		public DiscordBotRunState RunState { get; private set; } = DiscordBotRunState.Starting;
		/// <summary>
		/// Gets the configuration information file. This must be assigned on <see cref="LoadConfig"/>.
		/// </summary>
		public IConfigurationRoot Config { get; protected set; }
		/// <summary>
		/// The previous uptime added to the current ellapsed time since the bot last connected.
		/// </summary>
		private TimeSpan previousUptime;
		/// <summary>
		/// The time used to calculate the uptime since last connected.
		/// </summary>
		private DateTime lastConnected;
		/// <summary>
		/// The previous total uptime of the discord bot upon startup.
		/// </summary>
		private readonly TimeSpan startupUptime;
		/// <summary>
		/// The timer for saving the total uptime of the bot every so often.
		/// </summary>
		private readonly Timer saveTotalUptimeTimer;

		#endregion
		
		#region Properties

		/// <summary>
		/// Gets the connected Discord socket client.
		/// </summary>
		public DiscordSocketClient Client => Services.Client;
		/// <summary>
		/// The service that manages all Discord bot commands.
		/// </summary>
		public CommandServiceEx Commands => Services.Commands;
		/// <summary>
		/// Gets the uptime for the bot, or the time that it has been running for and connected to Discord.
		/// </summary>
		public TimeSpan Uptime {
			get {
				if (Client.ConnectionState == ConnectionState.Connected)
					return previousUptime + (DateTime.UtcNow - lastConnected);
				return previousUptime;
			}
		}
		/// <summary>
		/// Gets the total uptime of the bot across all executions.
		/// </summary>
		public TimeSpan TotalUptime => startupUptime + Uptime;
		/// <summary>
		/// Gets the total memory usage of the garbage collector.
		/// </summary>
		public long GCUsage => GC.GetTotalMemory(false);

		#endregion

		#region Virtual Properties

		/// <summary>
		/// The overridable interval for saving the total uptime.
		/// </summary>
		protected virtual TimeSpan SaveTotalUptimeInterval => TimeSpan.FromMinutes(1);

		#endregion

		#region Events

		/// <summary>
		/// Raised when an exception occurs during startup.
		/// </summary>
		public event StaticExceptionEventHandler StartupError;

		/// <summary>
		/// Raised when the bot is shutting down or restarting.
		/// </summary>
		public event ShuttingDownEventHandler ShuttingDown {
			add => shuttingDownEvent.Add(value);
			remove => shuttingDownEvent.Remove(value);
		}
		private readonly AsyncEvent<ShuttingDownEventHandler> shuttingDownEvent = new AsyncEvent<ShuttingDownEventHandler>();

		/// <summary>
		/// Raised when end user data of any type is being deleted.
		/// </summary>
		public event EndUserDataEventHandler DeletingEndUserData {
			add => deletingEndUserDataEvent.Add(value);
			remove => deletingEndUserDataEvent.Remove(value);
		}
		private readonly AsyncEvent<EndUserDataEventHandler> deletingEndUserDataEvent = new AsyncEvent<EndUserDataEventHandler>();
		
		/// <summary>
		/// Raised ONCE when the configuration file is reloaded.
		/// </summary>
		public event AsyncEventHandler ConfigReloaded {
			add => configReloadedEvent.Add(value);
			remove => configReloadedEvent.Remove(value);
		}
		private readonly AsyncEvent<AsyncEventHandler> configReloadedEvent = new AsyncEvent<AsyncEventHandler>();

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="DiscordBot"/>.
		/// </summary>
		public DiscordBot() {
			lastConnected = DateTime.UtcNow;
			previousUptime = TimeSpan.Zero;
			if (File.Exists(TotalUptimeFile)) {
				string text = File.ReadAllText(TotalUptimeFile);
				startupUptime = TimeSpan.Parse(text);
			}
			TimeSpan interval = SaveTotalUptimeInterval;
			saveTotalUptimeTimer = new Timer(OnSaveTotalUptime, null, interval, interval);

			// Let the bot display shutdown status when closed
			AppDomain.CurrentDomain.ProcessExit += OnAppDomainProcessExit;

			// Look for startup errors
			AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
			TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
		}

		#endregion

		#region Event Handlers


		/// <summary>
		/// Called to keep track of the bot's uptime.
		/// </summary>
		private Task OnConnectedAysnc() {
			lastConnected = DateTime.UtcNow;
			return Task.FromResult<object>(null);
		}
		/// <summary>
		/// Called to keep track of the bot's uptime.
		/// </summary>
		private Task OnDisconnectedAsync(Exception arg) {
			previousUptime += DateTime.UtcNow - lastConnected;
			return Task.FromResult<object>(null);
		}
		/// <summary>
		/// Called to output the restart message if one exists.
		/// </summary>
		private Task OnFirstReadyAysnc() {
			Client.Ready -= OnFirstReadyAysnc;
			return OutputRestartMessageAsync();
		}

		/// <summary>
		/// Sends a status stating the bot is shutting down.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAppDomainProcessExit(object sender, EventArgs e) {
			Client.SetStatusAsync(UserStatus.Invisible).ConfigureAwait(false).GetAwaiter().GetResult();
			Client.SetGameAsync("Shutting Down...").ConfigureAwait(false).GetAwaiter().GetResult();
		}
		/// <summary>
		/// Used to check for startup errors and raise <see cref="StartupError"/>.
		/// </summary>
		private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
			StartupError?.Invoke(e.ExceptionObject as Exception);
		}
		/// <summary>
		/// Used to check for startup errors and raise <see cref="StartupError"/>.
		/// </summary>
		private void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
			// Ignore this nonsense
			if (e.Exception is AggregateException)
				return;
			StartupError?.Invoke(e.Exception);
		}
		/// <summary>
		/// Called to save the total uptime every so often.
		/// </summary>
		private void OnSaveTotalUptime(object state = null) {
			if (RunState == DiscordBotRunState.Running || RunState == DiscordBotRunState.ShuttingDown)
				File.WriteAllText(TotalUptimeFile, TotalUptime.ToString());
		}

		#endregion

		#region Configuration

		/// <summary>
		/// Loads the configuration files used for the bot.
		/// </summary>
		public virtual IConfigurationRoot LoadConfig() {
			var builder = new ConfigurationBuilder()    // Create a new instance of the config builder
				.SetBasePath(AppContext.BaseDirectory)  // Specify the default location for the config file
				.AddJsonFile("Config.json");            // Add this (json encoded) file to the configuration
			return Config = builder.Build();                     // Build the configuration
		}
		/// <summary>
		/// Loads the <see cref="DiscordSocketConfig"/> used to automatically setup the
		/// <see cref="DiscordSocketClient"/>.
		/// </summary>
		public virtual DiscordSocketConfig GetDiscordSocketConfig() {
			return new DiscordSocketConfig {
				LogLevel = LogSeverity.Verbose,             // Tell the logger to give Verbose amount of info
				MessageCacheSize = 1000,                    // Cache 1,000 messages per channel
				DefaultRetryMode = RetryMode.AlwaysRetry,   // Don't give up when a request fails,
															// believe in your dreams and just do it!
			};
		}
		/// <summary>
		/// Loads the <see cref="CommandServiceConfig"/> used to automatically setup the
		/// <see cref="CommandService"/>.
		/// </summary>
		public virtual CommandServiceConfig GetCommandServiceConfig() {
			return new CommandServiceConfig {
				LogLevel = LogSeverity.Verbose,     // Tell the logger to give Verbose amount of info
				DefaultRunMode = RunMode.Async,     // Force all commands to run async by default
				CaseSensitiveCommands = false,      // Ignore case when executing commands
			};
		}
		/// <summary>
		/// Loads the <see cref="ReliabilityConfig"/> used to automatically setup the
		/// <see cref="ReliabilityService"/>.
		/// </summary>
		public virtual ReliabilityConfig GetReliabilityConfig() {
			return new ReliabilityConfig { };
		}

		/// <summary>
		/// Configures all required services for the Discord Bot.
		/// </summary>
		/// <param name="services">The services to configure.</param>
		/// <returns>Returns <paramref name="services"/>.</returns>
		public virtual IServiceCollection ConfigureServices(IServiceCollection services) {
			return services
				.AddSingleton<CommandHandlerService>()
				.AddSingleton<ILoggingService, DefaultLoggingService>()
				.AddSingleton<IContextingService, DefaultContextingService>()
				.AddSingleton<ResultHandlerService>()
				.AddSingleton<HelpService>()
				.AddSingleton<ReliabilityService>()
				.AddSingleton<Random>()
				;
		}
		/// <summary>
		/// Configures all required database services for the Discord Bot.
		/// </summary>
		/// <param name="services">The services to configure.</param>
		/// <returns>Returns <paramref name="services"/>.</returns>
		public virtual IServiceCollection ConfigureDatabase(IServiceCollection services) {
			return services;
		}
		
		/// <summary>
		/// Creates the <see cref="DiscordBotServiceContainer"/> or an extended class.
		/// </summary>
		/// <param name="services">The service collection to initialize the service container with.</param>
		/// <returns>The newly created service container.</returns>
		public virtual DiscordBotServiceContainer CreateServiceContainer(IServiceCollection services) {
			return new DiscordBotServiceContainer(services);
		}
		
		/// <summary>
		/// Gets the secret token used to control the Discord bot.
		/// </summary>
		/// <returns>The discord token.</returns>
		public virtual string GetDiscordToken() {
			string token = Config["tokens:discord"];
			if (string.IsNullOrWhiteSpace(token))
				throw new Exception("Please enter your bot's token into the `tokens:discord` in the config file!");
			return token;
		}

		#endregion

		#region Management

		/// <summary>
		/// Asynchronously initializes the Discord bot by giving it access to the server collection.
		/// </summary>
		/// <param name="services">The services to create the <see cref="DiscordBotServiceContainer"/> with.</param
		public async Task<DiscordBotServiceContainer> InitializeAsync(IServiceCollection services) {
			Services = CreateServiceContainer(services);
			Client.Connected += OnConnectedAysnc;
			Client.Disconnected += OnDisconnectedAsync;
			Client.Ready += OnFirstReadyAysnc;
			await OnInitializeAsync().ConfigureAwait(false);
			return Services;
		}
		/// <summary>
		/// Called to tell the Discord bot that everything has been setup.
		/// </summary>
		public async Task StartAsync() {
			RunState = DiscordBotRunState.Running;
			await OnStartAsync().ConfigureAwait(false);
			OnSaveTotalUptime();

			// Stop looking for startup errors
			AppDomain.CurrentDomain.UnhandledException -= OnAppDomainUnhandledException;
			TaskScheduler.UnobservedTaskException -= OnTaskSchedulerUnobservedTaskException;
		}
		/// <summary>
		/// Loads the commands and modules into the command service.
		/// </summary>
		public virtual Task LoadCommandModulesAsync() {
			return Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
		}
		/// <summary>
		/// Shuts down the bot.
		/// </summary>
		/// <param name="initiator">The user that initiated the shutdown.</param>
		public async Task ShutdownAsync(ICommandContext context = null) {
			OnSaveTotalUptime();
			await Client.SetGameAsync("Shutting Down...").ConfigureAwait(false);
			await Client.SetStatusAsync(UserStatus.Invisible).ConfigureAwait(false);
			RunState = DiscordBotRunState.ShuttingDown;
			await shuttingDownEvent.InvokeAsync(new ShuttingDownEventArgs(ShutdownMode.Shutdown, context)).ConfigureAwait(false);
		}
		/// <summary>
		/// Restarts the bot by starting a new process.
		/// </summary>
		/// <param name="initiator">The user that initiated the restart.</param>
		public async Task RestartAsync(ICommandContext context = null, string restartMessage = null) {
			OnSaveTotalUptime();
			await Client.SetGameAsync("Restarting...").ConfigureAwait(false);
			if (restartMessage != null && context != null)
				if (restartMessage != null && context != null)
					WriteRestartMessageAsync(context, restartMessage);
			RunState = DiscordBotRunState.Restarting;
			await shuttingDownEvent.InvokeAsync(new ShuttingDownEventArgs(ShutdownMode.Restart, context)).ConfigureAwait(false);
		}

		#endregion

		#region Virtual Management Methods

		/// <summary>
		/// Called at the end of the <see cref="InitializeAsync"/> method to perform additional
		/// initialization.
		/// </summary>
		protected virtual Task OnInitializeAsync() {
			return Task.FromResult<object>(null);
		}
		/// <summary>
		/// Called at the end of the <see cref="StartAsync"/> method to perform additional
		/// start initialization.
		/// </summary>
		protected virtual Task OnStartAsync() {
			return Task.FromResult<object>(null);
		}

		#endregion
		
		#region Private Restart Message

		/// <summary>
		/// Writes the restart message and the channel to send it to, to file.
		/// </summary>
		/// <param name="context">The context for restarting.</param>
		/// <param name="message">The restart message.</param>
		private void WriteRestartMessageAsync(ICommandContext context, string message) {
			StringBuilder str = new StringBuilder();
			ContextType contextType;// = ContextType.Guild;
			switch (context.Channel) {
			case IGuildChannel _: contextType = ContextType.Guild; break;
			case IGroupChannel _: contextType = ContextType.Group; break;
			case IDMChannel _:    contextType = ContextType.DM; break;
			default: return;
			}
			str.AppendLine(contextType.ToString());
			str.AppendLine((context.Guild?.Id ?? 0).ToString());
			str.AppendLine(context.Channel.Id.ToString());
			str.AppendLine(message);
			try {
				File.Create(RestartMessageFile).Dispose();
				File.WriteAllText(RestartMessageFile, str.ToString());
			} catch { }
		}

		/// <summary>
		/// Reads and outputs the restart message if one exists.
		/// </summary>
		private async Task OutputRestartMessageAsync() {
			if (!File.Exists(RestartMessageFile))
				return;
			try {
				string[] lines = File.ReadAllLines(RestartMessageFile);
				if (lines.Length < 4)
					return;

				ContextType context = (ContextType) Enum.Parse(typeof(ContextType), lines[0]);
				ulong guildId = ulong.Parse(lines[1]);
				ulong channelId = ulong.Parse(lines[2]);
				string message = string.Join("\n", lines.Skip(3));
				IMessageChannel channel = null;
				switch (context) {
				case ContextType.Guild:
					IGuild guild = Client.GetGuild(guildId);
					if (guild == null)
						return;
					channel = await guild.GetChannelAsync(channelId).ConfigureAwait(false) as IMessageChannel;
					/*if (channel == null) {
						async Task OnGuildAvailable(SocketGuild g) {
							if (g.Id == guild.Id) {
								Client.GuildAvailable -= OnGuildAvailable;
								channel = await guild.GetChannelAsync(channelId) as IMessageChannel;
								await channel.SendMessageAsync(message);
							}
						}
						Client.GuildAvailable += OnGuildAvailable;
					}*/
					break;
				case ContextType.Group:
					channel = await Client.GetGroupChannelAsync(channelId).ConfigureAwait(false) as IMessageChannel;
					break;
				case ContextType.DM:
					channel = await Client.GetDMChannelAsync(channelId).ConfigureAwait(false) as IMessageChannel;
					break;
				}
				if (channel == null)
					return;

				await channel.SendMessageAsync(message).ConfigureAwait(false);
			}
			finally {
				try {
					File.Delete(RestartMessageFile);
				} catch { }
			}
		}

		#endregion
		
		#region DeleteEndUserData

		public virtual async Task<bool> DeleteEndUserDataAsync(DbContextEx db, ulong id, EndUserDataContents euds, EndUserDataType type) {
			await deletingEndUserDataEvent.InvokeAsync(new EndUserDataEventArgs(id, type)).ConfigureAwait(false);
			if (DeleteEndUserDataDb(db, id, euds, type)) {
				await db.SaveChangesAsync().ConfigureAwait(false);
				return true;
			}
			return false;
		}

		private bool DeleteEndUserDataDb(DbContextEx db, ulong id, EndUserDataContents euds, EndUserDataType type) {
			bool anyDeleted = false;
			Type interfaceType = type.GetInterfaceType();
			foreach (PropertyInfo prop in db.GetType().GetProperties()) {
				Type propType = prop.PropertyType;
				if (IsEndUserDataTable(propType, interfaceType)) {
					Type entityType = propType.GetGenericArguments()[0];
					object table = prop.GetValue(db);
					string methodName = null;
					switch (type) {
					case EndUserDataType.User:
						methodName = nameof(DeleteEndUserDataTable);
						break;
					case EndUserDataType.Guild:
						methodName = nameof(DeleteEndUserGuildDataTable);
						break;
					}
					MethodInfo method = typeof(DiscordBot).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
					MethodInfo generic = method.MakeGenericMethod(entityType);
					anyDeleted |= (bool) generic.Invoke(this, new object[] { table, id, euds });
				}
			}
			return anyDeleted;
		}

		private static bool IsEndUserDataTable(Type propType, Type interfaceType) {
			return  propType.IsGenericType &&
					propType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
					interfaceType.IsAssignableFrom(propType.GetGenericArguments()[0]);
		}

		private bool DeleteEndUserDataTable<T>(DbSet<T> table, ulong id, EndUserDataContents euds)
			where T : class, IDbEndUserData
		{
			if (!euds.EraseAll) {
				T blankEntity = Activator.CreateInstance<T>();
				if (blankEntity.ShouldKeep(euds, EndUserDataType.User))
					return false;
			}

			var relatedEuds = table.Where(e => e.EndUserDataId == id);
			bool anyDeleted = relatedEuds.Any();
			table.RemoveRange(relatedEuds);

			return anyDeleted;
		}
		private bool DeleteEndUserGuildDataTable<T>(DbSet<T> table, ulong id, EndUserDataContents euds)
			where T : class, IDbEndUserGuildData
		{
			if (!euds.EraseAll) {
				T blankEntity = Activator.CreateInstance<T>();
				if (blankEntity.ShouldKeep(euds, EndUserDataType.Guild))
					return false;
			}

			var relatedEuds = table.Where(e => e.EndUserGuildDataId == id);
			bool anyDeleted = relatedEuds.Any();
			table.RemoveRange(relatedEuds);

			return anyDeleted;
		}

		/// <summary>
		/// Reloads the configuration file and notifies all listening services.
		/// </summary>
		/// <returns></returns>
		public async Task ReloadConfigAsync() {
			Config.Reload();
			await configReloadedEvent.InvokeAsync().ConfigureAwait(false);
		}

		#endregion

		#region IDisposable Implementation

		/// <summary>
		/// Disposes of the Discord bot.
		/// </summary>
		public void Dispose() {
			saveTotalUptimeTimer.Dispose();
		}

		#endregion
	}
}
