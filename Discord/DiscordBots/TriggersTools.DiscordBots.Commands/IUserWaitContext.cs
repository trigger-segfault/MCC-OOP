using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// A context for waiting for additional command input after the initial execution.
	/// </summary>
	public interface IUserWaitContext : ICommandContext, IDisposable {
		
		/// <summary>
		/// Gets the user friendly name of the wait context.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the wait duration of the context.
		/// </summary>
		TimeSpan Duration { get; }
		/// <summary>
		/// Gets the timer that counts down to expiration.
		/// </summary>
		Timer ExpireTimer { get; set; }
		/// <summary>
		/// Gets the output message channel.
		/// </summary>
		IMessageChannel OutputChannel { get; }
		/// <summary>
		/// Gets the message displayed when this wait context is overwritten by another wait context.
		/// </summary>
		string OverwriteMessage { get; }

		/// <summary>
		/// Marks the wait context as started.
		/// </summary>
		Task StartAsync();
		/// <summary>
		/// Marks the wait context as expired.
		/// </summary>
		Task ExpireAsync();
		/// <summary>
		/// Marks the wait context as canceled.
		/// </summary>
		Task<bool> CancelAsync();
		/// <summary>
		/// Marks the wait context as finished.
		/// </summary>
		Task<bool> FinishAsync();

		/// <summary>
		/// Called when a message has been received while this wait context is active.
		/// </summary>
		/// <param name="message">The user message that was received.</param>
		Task MessageReceiveAsync(IUserMessage message);
	}
}
