using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TriggersTools.DiscordBots.Utils {
	/// <summary>
	/// A console lock that locks console output with color has been changed.<para/>
	/// This must be used in a using statement or be properly discosed of.
	/// </summary>
	public class ConsoleLock : IDisposable {

		#region Fields

		/// <summary>
		/// The global lock object.
		/// </summary>
		private static readonly object lockObj = new object();
		/// <summary>
		/// The Console color to use.
		/// </summary>
		public ConsoleColor? Color { get; }

		#endregion

		#region Constructors

		private ConsoleLock(ConsoleColor? color = null) {
			Color = color;
			Monitor.Enter(lockObj);
			if (Color.HasValue)
				Console.ForegroundColor = Color.Value;
		}

		#endregion

		#region Static Constructors

		/// <summary>
		/// Creates a console lock with no color and calls <see cref="Monitor.Enter"/>.
		/// </summary>
		/// <returns>The console lock that must be disposed of.</returns>
		public static ConsoleLock Lock() {
			return new ConsoleLock();
		}
		/// <summary>
		/// Creates a console lock with an optional color and calls <see cref="Monitor.Enter"/>.
		/// </summary>
		/// <returns>The console lock that must be disposed of.</returns>
		public static ConsoleLock Lock(ConsoleColor? color) {
			return new ConsoleLock(color);
		}

		#endregion

		#region IDisposable Implementation

		/// <summary>
		/// Disposes of the console lock and calls <see cref="Monitor.Exit"/>.
		/// </summary>
		public void Dispose() {
			if (Color.HasValue)
				Console.ResetColor();
			Monitor.Exit(lockObj);
		}

		#endregion
	}
}
