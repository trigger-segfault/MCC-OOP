using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Net;

namespace TriggersTools.DiscordBots.Utils {
	public class MessageUpdater<T> : MessageUpdater {

		private static readonly Func<MessageUpdater, Task> EmptyUpdate = (mu) => Task.FromResult<object>(null);

		private readonly Func<MessageUpdater<T>, Task> updateCallback;
		public T State { get; }
		
		internal MessageUpdater(IUserMessage message, T state, TimeSpan interval, Func<MessageUpdater<T>, Task> update)
			: base(message, interval, EmptyUpdate)
		{
			State = state;
			updateCallback = update ?? throw new ArgumentNullException(nameof(update));
		}

		protected override void OnCallback(object state) {
			updateCallback(this).GetAwaiter().GetResult();
		}
	}

	public class MessageUpdater : IDisposable {

		public static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(5.0);

		private readonly Func<MessageUpdater, Task> updateCallback;
		private readonly object timerLock = new object();
		private Timer timer;

		public TimeSpan Interval { get; }
		public IUserMessage Message { get; private set; }

		internal MessageUpdater(IUserMessage message, TimeSpan interval, Func<MessageUpdater, Task> update) {
			Message = message;
			Interval = interval;
			updateCallback = update ?? throw new ArgumentNullException(nameof(update));
		}

		public static MessageUpdater Create(IUserMessage message, Func<MessageUpdater, Task> update) {
			return Create(message, DefaultInterval, update);
		}
		public static MessageUpdater Create(IUserMessage message, TimeSpan interval, Func<MessageUpdater, Task> update) {
			return new MessageUpdater(message, interval, update);
		}
		public static MessageUpdater<T> Create<T>(IUserMessage message, T state, Func<MessageUpdater, Task> update) {
			return Create(message, state, DefaultInterval, update);
		}
		public static MessageUpdater<T> Create<T>(IUserMessage message, T state, TimeSpan interval, Func<MessageUpdater, Task> update) {
			return new MessageUpdater<T>(message, state, interval, update);
		}
		public static MessageUpdater StartNew(IUserMessage message, Func<MessageUpdater, Task> update) {
			return StartNew(message, DefaultInterval, update);
		}
		public static MessageUpdater StartNew(IUserMessage message, TimeSpan interval, Func<MessageUpdater, Task> update) {
			MessageUpdater updater = Create(message, interval, update);
			updater.Start();
			return updater;
		}
		public static MessageUpdater<T> StartNew<T>(IUserMessage message, T state, Func<MessageUpdater, Task> update) {
			return StartNew(message, state, DefaultInterval, update);
		}
		public static MessageUpdater<T> StartNew<T>(IUserMessage message, T state, TimeSpan interval, Func<MessageUpdater, Task> update) {
			MessageUpdater<T> updater = Create(message, state, interval, update);
			updater.Start();
			return updater;
		}

		public bool IsRunning {
			get {
				lock (timerLock)
					return timer != null;
			}
			set {
				lock (timerLock) {
					if (IsRunning != value) {
						if (value)
							Start();
						else
							Stop();
					}
				}
			}
		}

		public void Start() {
			lock (timerLock) {
				if (timer == null)
					timer = new Timer(OnCallback, null, Interval, Interval);
			}
		}
		public void Restart() {
			lock (timerLock) {
				Stop();
				Start();
			}
		}

		public void Stop() {
			lock (timerLock) {
				timer?.Dispose();
				timer = null;
			}
		}

		public async Task UpdateAsync(string text = null, Embed embed = null, RequestOptions options = null) {
			try {
				await Message.ModifyAsync(p => {
					p.Content = text;
					p.Embed = embed;
				}, options).ConfigureAwait(false);
			} catch (HttpException ex) {
				if (ex.HttpCode == HttpStatusCode.NotFound) {
					// Deleted, post a new message
					try {
						Message = await Message.Channel.SendMessageAsync(text, embed: embed, options: options).ConfigureAwait(false);
					} catch { }
				}
			}
		}

		protected virtual void OnCallback(object state) {
			updateCallback(this).GetAwaiter().GetResult();
		}

		public void Dispose() {
			timer?.Dispose();
		}
	}
}
