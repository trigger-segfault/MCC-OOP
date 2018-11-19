
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A snowflake-identified database model for End User (user) Data.
	/// </summary>
	public interface IDbEndUserData : IDbEndUserDataBase {
		/// <summary>
		/// The End User Data snowflake Id key.
		/// </summary>
		ulong EndUserDataId { get; set; }
	}
}
