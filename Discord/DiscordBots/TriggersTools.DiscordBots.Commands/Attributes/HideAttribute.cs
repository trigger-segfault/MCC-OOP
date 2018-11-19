using System;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// An attribute, stating the command should be visually hidden from the search list.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class HideAttribute : Attribute { }
}
