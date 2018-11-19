
namespace TriggersTools.DiscordBots.Database.Model.Interfaces {
	/// <summary>
	/// The base database model for the bot's self.
	/// </summary>
	public interface IDbSelf {
		/// <summary>
		/// Gets the snowflake entity Id for the self.
		/// </summary>
		ulong SelfId { get; set; }
	}
}
