using System;
using System.Collections.Generic;
using System.Text;
using Discord;

namespace TriggersTools.DiscordBots {
	/// <summary>
	/// Additional enum values for <see cref="LogSeverity"/>.
	/// </summary>
	public static class LogSeverities {
		/// <summary>
		/// Debug messages from the Discord bot and not Discord.Net.
		/// </summary>
		public const LogSeverity Debug = (LogSeverity) 20;
		/// <summary>
		/// Trace messages from the Discord bot.
		/// </summary>
		public const LogSeverity Trace = (LogSeverity) 21;
		/// <summary>
		/// Print messages from the Discord bot.
		/// </summary>
		public const LogSeverity Print = (LogSeverity) 22;
		/// <summary>
		/// Notice that have their own log file.
		/// </summary>
		public const LogSeverity Notice = (LogSeverity) 23;
		/// <summary>
		/// User-submitted bug reports that also goto the Notice log file.
		/// </summary>
		public const LogSeverity Report = (LogSeverity) 24;
	}
}
