using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// An attribute stating the <see cref="CommandDetails"/> priority of a module.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ModulePriorityAttribute : Attribute {
		/// <summary>
		/// Gets the priority of the module. Defaults to zero when unset.
		/// </summary>
		public int Priority { get; }

		/// <summary>
		/// Constructs the module priority attribute with the specified priority.
		/// </summary>
		/// <param name="priority">The priority of the module. Defaults to zero when unset.</param>
		public ModulePriorityAttribute(int priority) {
			Priority = priority;
		}
	}
}
