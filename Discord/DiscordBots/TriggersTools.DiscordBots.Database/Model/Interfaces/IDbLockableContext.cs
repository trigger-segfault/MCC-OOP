using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// An interface that allows directly locking commands and modules.
	/// </summary>
	public interface IDbLockableContext {
		/// <summary>
		/// The set of directly locked commands in this context.
		/// </summary>
		StringSet LockedCommands { get; }
		/// <summary>
		/// The set of directly locked modules in this context.
		/// </summary>
		StringSet LockedModules { get; }
	}
}
