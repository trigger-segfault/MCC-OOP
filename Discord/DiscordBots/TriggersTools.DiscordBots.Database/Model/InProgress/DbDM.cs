using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a user DM.
	/// </summary>
	public abstract class DbDM : IDbDM {
		/// <summary>
		/// The snowflake Id key.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public ulong Id { get; set; }
		/// <summary>
		/// The snowflake Id of the user this DM belongs to.
		/// </summary>
		[Required]
		public ulong UserId { get; set; }
		/// <summary>
		/// The End User Data snowflake Id key.
		/// </summary>
		[Required]
		public ulong EndUserDataId { get => Id; set => Id = value; }

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
		public EntityType Type => EntityType.DM;
	}
}
