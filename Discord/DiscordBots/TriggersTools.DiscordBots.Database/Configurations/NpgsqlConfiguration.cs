using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Database.Configurations {
	/// <summary>
	/// The database configuration for Npgsql.
	/// </summary>
	public class NpgsqlConfiguration : IDbConfiguration {
		/// <summary>
		/// Gets the name of the configuration.
		/// </summary>
		public string ConfigurationType => GetType().Name;
		/// <summary>
		/// Configures the connection from <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
		/// </summary>
		/// <param name="optionsBuilder">The options builder for the context.</param>
		/// <param name="section">The configuration section for this type.</param>
		public void Configure(DbContextOptionsBuilder optionsBuilder, IConfigurationSection section) {
			section = section.GetSection("connection_string");
			var connectionVars = section.GetChildren().Select(c => $"{c.Key}='{c.Value}'");
			string connectionString = string.Join(";", connectionVars);

			optionsBuilder.UseNpgsql(connectionString);
		}
	}
}
