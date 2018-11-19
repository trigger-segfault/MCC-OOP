using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a guild user.
	/// </summary>
	public abstract class DbGuildUser : IDbGuildUser {
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
		/// The End User (guild) Data snowflake Id key.
		/// </summary>
		[Required]
		public ulong EndUserGuildDataId { get => GuildId; set => GuildId = value; }
		/// <summary>
		/// The End User Data snowflake Id key.
		/// </summary>
		[Required]
		public ulong EndUserDataId { get => Id; set => Id = value; }

		/// <summary>
		/// Checks if the user has asked this information to be deleted.
		/// </summary>
		/// <param name="euds">The info to that the user requested to be deleted.</param>
		/// <returns>True if the data should be deleted.</returns>
		public abstract bool ShouldKeep(EndUserDataContents euds, EndUserDataType type);


		/// <summary>
		/// Gets the entity type of this Discord model.
		/// </summary>
		[NotMapped]
		public EntityType Type => EntityType.GuildUser;
	}
}
