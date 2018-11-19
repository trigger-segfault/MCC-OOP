
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A snowflake-identified database model for End User (guild) Data.
	/// </summary>
	public interface IDbEndUserGuildData : IDbEndUserDataBase {
		/// <summary>
		/// The End User (guild) Data snowflake Id key.
		/// </summary>
		ulong EndUserGuildDataId { get; set; }
	}
}
