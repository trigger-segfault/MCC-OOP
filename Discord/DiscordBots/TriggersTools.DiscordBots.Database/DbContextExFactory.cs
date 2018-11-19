using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// The base design-time factory for <see cref="DbContextEx"/>s.
	/// </summary>
	/// <typeparam name="TDbProvider">
	/// The provider service for database creation and configuration.
	/// </typeparam>
	/// <typeparam name="TDbContext">The database context type.</typeparam>
	public abstract class DbContextExFactory<TDbProvider, TDbContext>
		: IDesignTimeDbContextFactory<TDbContext>, IDbContextExFactory
		where TDbProvider : class, IDbProvider
		where TDbContext : DbContextEx
	{
		#region Fields

		/// <summary>
		/// The service provider.
		/// </summary>
		public IServiceProvider Services { get; }
		/// <summary>
		/// The custom configuration type. Or null.
		/// </summary>
		public string ConfigurationType { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="DbContextExFactory{TBotDbConfigurationService, TContext}"
		/// /> with no custom configuration type.
		/// </summary>
		/// <param name="config">The configuration information file for the database.</param>
		protected DbContextExFactory(IConfigurationRoot config) : this(config, null) { }

		/// <summary>
		/// Constructs the <see cref="DbContextExFactory{TBotDbConfigurationService, TContext}"
		/// /> with a custom configuration type.
		/// </summary>
		/// <param name="config">The configuration information file for the database.</param>
		/// <param name="configurationName">
		/// The custom type to configure the database to in the config file.
		/// </param>
		protected DbContextExFactory(IConfigurationRoot config, string configurationName) {
			IServiceCollection services = new ServiceCollection()
				.AddSingleton(config)
				.AddSingleton<IDbProvider, TDbProvider>()
				.AddDbContext<TDbContext>(ServiceLifetime.Transient)
				;
			Services = ConfigureServices(services).BuildServiceProvider();
			ConfigurationType = configurationName;
		}

		#endregion

		#region CreateDbContext
		
		/// <summary>
		/// Creates a new instance of a derived database context.
		/// </summary>
		/// <param name="args">Arguments provided by the design-time service.</param>
		/// <returns>An instance of <typeparamref name="TDbContext"/>.</returns>
		public TDbContext CreateDbContext(string[] args = null) {
			TDbContext db = Services.GetRequiredService<TDbContext>();
			db.ConfigurationType = ConfigurationType;
			return db;
		}
		DbContextEx IDbContextExFactory.CreateDbContext(string[] args) => CreateDbContext(args);

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Configures additional required services.
		/// </summary>
		/// <param name="services">The service collection to add to.</param>
		/// <returns>The same server collection that was passed.</returns>
		protected virtual IServiceCollection ConfigureServices(IServiceCollection services) {
			return services;
		}

		#endregion
	}
}
