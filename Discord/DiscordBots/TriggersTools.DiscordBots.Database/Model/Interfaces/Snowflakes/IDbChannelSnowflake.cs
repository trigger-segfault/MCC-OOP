
namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A snowflake-identified database model for a subject within a channel.
	/// </summary>
	public interface IDbChannelSnowflake : IDbSnowflake {
		/// <summary>
		/// The channel snowflake Id key.
		/// </summary>
		ulong ChannelId { get; set; }
	}
}
