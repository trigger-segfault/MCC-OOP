using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// States how a variable list of items should determine what is and isn't included.
	/// </summary>
	public enum Inclusion {
		/// <summary>
		/// Subjects need to be excluded from the list.
		/// </summary>
		Blacklist,
		/// <summary>
		/// Subjects need to be explicitly included in the list.
		/// </summary>
		Whitelist,
	}

	/// <summary>
	/// Extension methods for the <see cref="Inclusion"/> enum.
	/// </summary>
	public static class InclusionExtensions {

		
	}
}
