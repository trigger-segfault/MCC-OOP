using System;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A snowflake-identified database model for a subject within a guild.
	/// </summary>
	public interface IDbGuildSnowflake : IDbSnowflake {
		/// <summary>
		/// The guild snowflake Id key.
		/// </summary>
		ulong GuildId { get; set; }
	}
	/// <summary>
	/// A snowflake-identified database model for a subject within a guild that may also exist in other
	/// groups.
	/// </summary>
	public interface IDbUniqueGuildSnowflake : IDbGuildSnowflake {
		/// <summary>
		/// The 128-bit guild snowflake enity Id as a key.
		/// </summary>
		Guid Key { get; set; }
	}
}
