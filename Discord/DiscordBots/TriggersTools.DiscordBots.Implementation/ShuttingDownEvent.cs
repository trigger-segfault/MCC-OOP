using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TriggersTools.DiscordBots {
	/// <summary>
	/// The types of shutdown modes a bot can be in.
	/// </summary>
	[Flags]
	public enum ShutdownMode {
		/// <summary>
		/// The program will shut down.
		/// </summary>
		Shutdown = 0,
		/// <summary>
		/// The program will restart in a new process.
		/// </summary>
		Restart = 1,
	}
	/// <summary>
	/// The event args for shutting down of the bot.
	/// </summary>
	public class ShuttingDownEventArgs {

		#region Fields

		/// <summary>
		/// Gets the UTC time the shutdown was initiated at.
		/// </summary>
		public DateTime InitiateTimeUtc { get; }
		/// <summary>
		/// Gets the optional command context for the restart.
		/// </summary>
		public ICommandContext Context { get; }
		/// <summary>
		/// Gets the shutdown mode of the bot.
		/// </summary>
		public ShutdownMode Mode { get; }
		/// <summary>
		/// Gets if the bot is also restarting after the shutdown.
		/// </summary>
		public bool IsRestarting => Mode.IsRestarting();

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="ShuttingDownEventArgs"/>.
		/// </summary>
		/// <param name="restarting">True if the bot is also restarting after the shutdown.</param>
		/// <param name="initator">The optional user that initiated the shutdown.</param>
		internal ShuttingDownEventArgs(ShutdownMode mode, ICommandContext context) {
			InitiateTimeUtc = DateTime.UtcNow;
			Mode = mode;
			Context = context;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the local time the shutdown was initiated at.
		/// </summary>
		public DateTime InitiateTime => InitiateTimeUtc.ToLocalTime();
		
		#endregion
	}

	/// <summary>
	/// The event handler to call the shutting down of the bot.
	/// </summary>
	/// <param name="restarting">The event args for shutting down.</param>
	public delegate Task ShuttingDownEventHandler(ShuttingDownEventArgs e);

	public static class ShutdownExtensions {

		/// <summary>
		/// Gets if the shutdown mode is a restart type.
		/// </summary>
		/// <param name="mode">The shutdown mode to check.</param>
		/// <returns>Try if the mode signifies restarting in some form.</returns>
		public static bool IsRestarting(this ShutdownMode mode) {
			return	mode == ShutdownMode.Restart /*||
					mode == ShutdownMode.RestartFull*/;
		}

		public static async Task InvokeAsync(this AsyncEvent<ShuttingDownEventHandler> eventHandler, ShuttingDownEventArgs e) {
			var subscribers = eventHandler.Subscriptions;
			for (int i = 0; i < subscribers.Count; i++)
				await subscribers[i].Invoke(e).ConfigureAwait(false);
		}
	}
}
