
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A snowflake-identified database model for a subject owned by a user.
	/// </summary>
	public interface IDbUserSnowflake : IDbSnowflake {
		/// <summary>
		/// The user snowflake Id key.
		/// </summary>
		ulong UserId { get; set; }
	}
}
