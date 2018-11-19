using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a guild emote.
	/// </summary>
	public abstract class DbGuildEmote : IDbGuildEmote {
		/// <summary>
		/// The snowflake Id key.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public ulong Id { get; set; }
		/// <summary>
		/// The guild snowflake Id.
		/// </summary>
		[Required]
		public ulong GuildId { get; set; }
		/// <summary>
		/// The End User (guild) Data snowflake Id key.
		/// </summary>
		[Required]
		public ulong EndUserGuildDataId { get => GuildId; set => GuildId = value; }

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
		public EntityType Type => EntityType.GuildEmote;
	}
}
