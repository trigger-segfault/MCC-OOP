using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a guild.
	/// </summary>
	public abstract class DbGuild : IDbGuild {
		/// <summary>
		/// The snowflake Id key.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public ulong Id { get; set; }
		/// <summary>
		/// The End User (guild) Data snowflake Id key.
		/// </summary>
		[Required]
		public ulong EndUserGuildDataId { get => Id; set => Id = value; }

		/// <summary>
		/// The custom prefix assigned to this context.
		/// </summary>
		public string Prefix { get; set; }

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
		public EntityType Type => EntityType.Guild;
	}
}
