using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// Specifies the name used to identify the details command or module.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class DetailsNameAttribute : Attribute {
		/// <summary>
		/// Gets the details name for the command.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Constructs the details name attribute with the specified name.
		/// </summary>
		/// <param name="name">The details name for the command.</param>
		public DetailsNameAttribute(string name) {
			Name = name;
		}
	}
}
