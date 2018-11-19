using Microsoft.EntityFrameworkCore;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A model interface that adds its own arguments to <see cref="DbContext.OnModelCreating(ModelBuilder)"/>.
	/// </summary>
	public interface IDbModelCreating {
		/// <summary>
		/// Configures setup of the model from <see cref="DbContext.OnModelCreating(ModelBuilder)"/>.
		/// </summary>
		/// <param name="modelBuilder">The model builder for the database context.</param>
		void ModelCreating(ModelBuilder modelBuilder, DbContextEx db);
	}
}
