using System;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// An attribute laying out all command usage in a friendly readable fashion.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class UsageAttribute : Attribute {
		/// <summary>
		/// Gets the usage of the command.
		/// </summary>
		public string Usage { get; }

		/// <summary>
		/// Constructs the <see cref="UsageAttribute"/> with the specified parameters usage.
		/// </summary>
		/// <param name="usage">The parameters usage string.</param>
		public UsageAttribute(string usage) {
			Usage = usage;
		}
	}
}
