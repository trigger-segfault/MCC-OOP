
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A snowflake-identified database model.
	/// </summary>
	public interface IDbSnowflake {
		/// <summary>
		/// The snowflake Id key.
		/// </summary>
		ulong Id { get; set; }

		/// <summary>
		/// Gets the entity type of this Discord model.
		/// </summary>
		EntityType Type { get; }
	}
}
