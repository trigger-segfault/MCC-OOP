using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TriggersTools.DiscordBots.Database.Configurations {
	/// <summary>
	/// The interface for configuration a <see cref="DiscordDbContext"/> connection.
	/// </summary>
	public interface IDbConfiguration {
		/// <summary>
		/// Gets the type name of the configuration.
		/// </summary>
		string ConfigurationType { get; }

		/// <summary>
		/// Configures the connection from <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
		/// </summary>
		/// <param name="optionsBuilder">The options builder for the context.</param>
		/// <param name="section">The configuration section for this type.</param>
		void Configure(DbContextOptionsBuilder optionsBuilder, IConfigurationSection section);
	}
}
