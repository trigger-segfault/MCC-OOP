using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A snowflake-identified database model.
	/// </summary>
	public abstract class DbSnowflake {
		/// <summary>
		/// The snowflake Id key.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public ulong Id { get; set; }
	}
}
