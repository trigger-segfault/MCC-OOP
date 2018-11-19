using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TriggersTools.DiscordBots.Database.Configurations {
	/// <summary>
	/// The database configuration for Sqlite.
	/// </summary>
	public class SqliteConfiguration : IDbConfiguration {
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
			string file = section["file"];
			string password = section["password"];
			//optionsBuilder.UseSqlite($"Data Source='{file}'");
			optionsBuilder.UseSqlite(InitSqliteConnection(file, password));
		}
		/// <summary>
		/// Initializes the <see cref="SqliteConnection"/> with the specified file and password.
		/// </summary>
		/// <param name="file">The file path to the database.</param>
		/// <param name="password">The password if one exists; otherwise null.</param>
		/// <returns>The initialized connection.</returns>
		private SqliteConnection InitSqliteConnection(string file, string password) {
			var connection = new SqliteConnection($"Data Source='{file}'");
			connection.Open();

			if (password != null) {
				var command = connection.CreateCommand();
				command.CommandText = "SELECT quote($password);";
				command.Parameters.AddWithValue("$password", password);
				string quotedPassword = (string) command.ExecuteScalar();

				command.Parameters.Clear();
				command.CommandText = $"PRAGMA key={quotedPassword};";
				command.ExecuteNonQuery();
			}
			return connection;
		}
	}
}
