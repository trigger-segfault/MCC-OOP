using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// An interface that contains the Id of a role that allows users administrative access to the Discord
	/// bot.
	/// </summary>
	public interface IDbManagerContext {
		/// <summary>
		/// The snowflake Id for the role that allows users administrative access to the Discord bot.
		/// </summary>
		ulong ManagerRoleId { get; set; }
	}
}
