using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for Discord command contexts.
	/// </summary>
	public abstract class DbDiscordContext {
		/// <summary>
		/// The snowflake Id key.
		/// </summary>
		public abstract ulong Id { get; set; }

		/// <summary>
		/// Gets the context type of the database object.
		/// </summary>
		[NotMapped]
		public abstract DbEntityType Type { get; }
		/// <summary>
		/// Gets if this context is a root context and not contained in something like a guild.
		/// </summary>
		[NotMapped]
		public bool IsRoot => (Type != DbEntityType.GuildChannel);

		/// <summary>
		/// The custom prefix assigned to this context.
		/// </summary>
		public string Prefix { get; set; }
	}
}
