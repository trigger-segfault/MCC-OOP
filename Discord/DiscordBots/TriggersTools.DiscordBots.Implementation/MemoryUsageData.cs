using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.DiscordBots.Services {
	public struct MemoryUsageTime {
		public long Min { get; }
		public long Max { get; }
		public long Average { get; }
	}
	public class MemoryUsageData {
		/// <summary>
		/// Gets the total memory usage of the program.
		/// </summary>
		public MemoryUsageTime RamUsage { get; }
		/// <summary>
		/// Gets the total memory usage of the garbage collector.
		/// </summary>
		public MemoryUsageTime GCUsage { get; }
		/// <summary>
		/// Gets the time span this memory usage block covers.
		/// </summary>
		public TimeSpan TimeSpan { get; }
		/// <summary>
		/// Gets the time this memory usage block started at.
		/// </summary>
		public DateTime StartTime { get; }
	}
}
