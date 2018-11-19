using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A snowflake-identified database model for a subject within a guild.
	/// </summary>
	public abstract class DbGuildSnowflake : IDbDiscordEntity {
		/// <summary>
		/// The 128-bit guild snowflake enity Id as a key.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Guid Key {
			get => Int128Converter.ToGuid(GuildId, Id);
			set {
				var (guildId, id) = Int128Converter.ToUInt64s(value);
				GuildId = guildId;
				Id = id;
			}
		}

		/// <summary>
		/// The guild snowflake Id key.
		/// </summary>
		[Required]
		public ulong GuildId { get; set; }
		/// <summary>
		/// The subject snowflake Id key.
		/// </summary>
		[Required]
		public ulong Id { get; set; }

		/// <summary>
		/// Gets the entity type of this Discord model.
		/// </summary>
		[NotMapped]
		public abstract DbDiscordType Type { get; }
	}
}
