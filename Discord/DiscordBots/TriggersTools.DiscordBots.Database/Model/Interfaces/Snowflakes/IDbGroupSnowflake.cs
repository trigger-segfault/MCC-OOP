using System;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A snowflake-identified database model for a subject within a group.
	/// </summary>
	public interface IDbGroupSnowflake : IDbSnowflake {
		/// <summary>
		/// The group snowflake Id key.
		/// </summary>
		ulong GroupId { get; set; }
	}
	/// <summary>
	/// A snowflake-identified database model for a subject within a group that may also exist in other
	/// groups.
	/// </summary>
	public interface IDbUniqueGroupSnowflake : IDbGroupSnowflake {
		/// <summary>
		/// The 128-bit group snowflake enity Id as a key.
		/// </summary>
		Guid Key { get; set; }
	}
}
