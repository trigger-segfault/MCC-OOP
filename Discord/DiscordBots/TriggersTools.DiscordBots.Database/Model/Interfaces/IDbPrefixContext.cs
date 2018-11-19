using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A context type that has an assignable prefix.
	/// </summary>
	public interface IDbPrefixContext {
		/// <summary>
		/// The customized prefix for this context.
		/// </summary>
		string Prefix { get; set; }
	}
}
