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
	/// The command wait context for bot commands that require input over time.
	/// </summary>
	public class SocketUserWaitContext : SocketCommandContext, IUserWaitContext {

		#region Fields

		/// <summary>
		/// Gets the user friendly name of the wait context.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Gets the wait duration of the context.
		/// </summary>
		public TimeSpan Duration { get; }
		/// <summary>
		/// Gets the timer that counts down to expiration.
		/// </summary>
		public Timer ExpireTimer { get; set; }
		/// <summary>
		/// Gets the output message channel.
		/// </summary>
		public IMessageChannel OutputChannel { get; set; }
		/// <summary>
		/// Gets the wait context service
		/// </summary>
		public IWaitContextService WaitService { get; }
		/// <summary>
		/// Gets the message displayed when this wait context is overwritten by another wait context.
		/// </summary>
		public string OverwriteMessage { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="SocketUserWaitContext"/> with an existing <see cref="SocketCommandContext"/>.
		/// </summary>
		/// <param name="context">The underlying command context.</param>
		/// <param name="waitService">The wait service used to manage this context.</param>
		/// <param name="name">The friendly name of the wait context.</param>
		/// <param name="duration">The timeout duration of the wait context.</param>
		public SocketUserWaitContext(SocketCommandContext context, IWaitContextService waitService, string name,
			TimeSpan duration)
			: this(context.Client, context.Message, waitService, name, duration)
		{
		}
		/// <summary>
		/// Constructs the <see cref="SocketUserWaitContext"/> with no prior context.
		/// </summary>
		/// <param name="context">The underlying command context.</param>
		public SocketUserWaitContext(DiscordSocketClient client, SocketUserMessage msg, IWaitContextService waitService, string name,
			TimeSpan duration)
			: base(client, msg)
		{
			WaitService = waitService;
			Name = name;
			Duration = duration;
			OutputChannel = Channel;
			OverwriteMessage = $"**{name}:** Was canceled by another operation!";
		}

		#endregion

		#region Wait Context

		/// <summary>
		/// Marks the wait context as started.
		/// </summary>
		public async Task StartAsync() {
			WaitService.AddWaitContext(this);
			await startedEvent.InvokeAsync(this).ConfigureAwait(false);
		}
		/// <summary>
		/// Marks the wait context as expired.
		/// </summary>
		public async Task ExpireAsync() {
			await endedEvent.InvokeAsync(this).ConfigureAwait(false);
			await expiredEvent.InvokeAsync(this).ConfigureAwait(false);
		}
		/// <summary>
		/// Marks the wait context as canceled.
		/// </summary>
		public async Task<bool> CancelAsync() {
			if (WaitService.RemoveWaitContext(this)) {
				await endedEvent.InvokeAsync(this).ConfigureAwait(false);
				await canceledEvent.InvokeAsync(this).ConfigureAwait(false);
				return true;
			}
			return false;
		}
		/// <summary>
		/// Marks the wait context as finished.
		/// </summary>
		public async Task<bool> FinishAsync() {
			if (WaitService.RemoveWaitContext(this)) {
				await endedEvent.InvokeAsync(this).ConfigureAwait(false);
				await finishedEvent.InvokeAsync(this).ConfigureAwait(false);
				return true;
			}
			return false;
		}
		/// <summary>
		/// Called when a message has been received while this wait context is active.
		/// </summary>
		/// <param name="message">The user message that was received.</param>
		public Task MessageReceiveAsync(IUserMessage message) {
			return messageReceivedEvent.InvokeAsync(this, message);
		}

		#endregion

		#region Virtual Event Handlers
		
		/// <summary>
		/// Called when the wait context has started.
		/// </summary>
		public event Func<SocketUserWaitContext, Task> Started {
			add => startedEvent.Add(value);
			remove => startedEvent.Remove(value);
		}
		private readonly AsyncEvent<Func<SocketUserWaitContext, Task>> startedEvent = new AsyncEvent<Func<SocketUserWaitContext, Task>>();
		/// <summary>
		/// Called when the wait context has ended in any way.
		/// </summary>
		public event Func<SocketUserWaitContext, Task> Ended {
			add => endedEvent.Add(value);
			remove => endedEvent.Remove(value);
		}
		private readonly AsyncEvent<Func<SocketUserWaitContext, Task>> endedEvent = new AsyncEvent<Func<SocketUserWaitContext, Task>>();
		/// <summary>
		/// Called when the wait context has expired.
		/// </summary>
		public event Func<SocketUserWaitContext, Task> Expired {
			add => expiredEvent.Add(value);
			remove => expiredEvent.Remove(value);
		}
		private readonly AsyncEvent<Func<SocketUserWaitContext, Task>> expiredEvent = new AsyncEvent<Func<SocketUserWaitContext, Task>>();
		/// <summary>
		/// Called when the wait context has been canceled.
		/// </summary>
		public event Func<SocketUserWaitContext, Task> Canceled {
			add => canceledEvent.Add(value);
			remove => canceledEvent.Remove(value);
		}
		private readonly AsyncEvent<Func<SocketUserWaitContext, Task>> canceledEvent = new AsyncEvent<Func<SocketUserWaitContext, Task>>();
		/// <summary>
		/// Called when the wait context has finished.
		/// </summary>
		public event Func<SocketUserWaitContext, Task> Finished {
			add => finishedEvent.Add(value);
			remove => finishedEvent.Remove(value);
		}
		private readonly AsyncEvent<Func<SocketUserWaitContext, Task>> finishedEvent = new AsyncEvent<Func<SocketUserWaitContext, Task>>();
		/// <summary>
		/// Called when a message has been received while this wait context is active.
		/// </summary>
		/// <param name="message">The user message that was received.</param>
		public event Func<SocketUserWaitContext, IUserMessage, Task> MessageReceived {
			add => messageReceivedEvent.Add(value);
			remove => messageReceivedEvent.Remove(value);
		}
		private readonly AsyncEvent<Func<SocketUserWaitContext, IUserMessage, Task>> messageReceivedEvent = new AsyncEvent<Func<SocketUserWaitContext, IUserMessage, Task>>();

		#endregion

		#region IDisposable Implementation

		/// <summary>
		/// Disposes of the wait context.
		/// </summary>
		public virtual void Dispose() {

		}

		#endregion
	}
}
