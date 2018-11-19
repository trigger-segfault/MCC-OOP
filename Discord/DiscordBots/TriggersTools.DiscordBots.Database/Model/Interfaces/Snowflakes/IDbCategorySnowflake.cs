
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A snowflake-identified database model for a subject within a category.
	/// </summary>
	public interface IDbCategorySnowflake : IDbGuildSnowflake {
		/// <summary>
		/// The category snowflake Id key.
		/// </summary>
		ulong CategoryId { get; set; }
	}
}
