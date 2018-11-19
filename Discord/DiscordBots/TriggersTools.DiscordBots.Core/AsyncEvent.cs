using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots {
	/// <summary>
	/// An event that can properly be invoked asynchronously.
	/// </summary>
	/// <typeparam name="T">The delegate type of the event.</typeparam>
	public class AsyncEvent<T> where T : class {

		#region Fields

		/// <summary>
		/// The lock used while adding or removing event handlers.
		/// </summary>
		private readonly object subLock = new object();
		/// <summary>
		/// The immutable array used to store the subscriptions.
		/// </summary>
		private ImmutableArray<T> subscriptions = ImmutableArray.Create<T>();

		#endregion

		#region Properties

		/// <summary>
		/// Gets if this event has any functions subscribed to it.
		/// </summary>
		public bool HasSubscribers => subscriptions.Length != 0;
		/// <summary>
		/// Gets the functions that are subscribed to this event.
		/// </summary>
		public IReadOnlyList<T> Subscriptions => subscriptions;

		#endregion

		#region Subscribing

		/// <summary>
		/// Subscribes the function to the event.
		/// </summary>
		/// <param name="subscriber">The function to subscribe.</param>
		public void Add(T subscriber) {
			if (subscriber == null)
				throw new ArgumentNullException(nameof(subscriber));
			lock (subLock)
				subscriptions = subscriptions.Add(subscriber);
		}
		/// <summary>
		/// Unsubscribes the function to the event.
		/// </summary>
		/// <param name="subscriber">The function to unsubscribe.</param>
		public void Remove(T subscriber) {
			if (subscriber == null)
				throw new ArgumentNullException(nameof(subscriber));
			lock (subLock)
				subscriptions = subscriptions.Remove(subscriber);
		}

		#endregion
	}

	/// <summary>
	/// An asynchronous event handler with no arguments.
	/// </summary>
	/// <returns></returns>
	public delegate Task AsyncEventHandler();

	/// <summary>
	/// An asynchronous event with no arguments.
	/// </summary>
	public class AsyncEvent : AsyncEvent<AsyncEventHandler> { }

	/// <summary>
	/// Extension methods for asynchronous events, making them easier to call.
	/// </summary>
	public static class AsyncEventExtensions {
		public static async Task InvokeAsync(this AsyncEvent<AsyncEventHandler> eventHandler) {
			var subscribers = eventHandler.Subscriptions;
			for (int i = 0; i < subscribers.Count; i++)
				await subscribers[i].Invoke().ConfigureAwait(false);
		}
		public static async Task InvokeAsync(this AsyncEvent<Func<Task>> eventHandler) {
			var subscribers = eventHandler.Subscriptions;
			for (int i = 0; i < subscribers.Count; i++)
				await subscribers[i].Invoke().ConfigureAwait(false);
		}
		public static async Task InvokeAsync<T>(this AsyncEvent<Func<T, Task>> eventHandler, T arg) {
			var subscribers = eventHandler.Subscriptions;
			for (int i = 0; i < subscribers.Count; i++)
				await subscribers[i].Invoke(arg).ConfigureAwait(false);
		}
		public static async Task InvokeAsync<T1, T2>(this AsyncEvent<Func<T1, T2, Task>> eventHandler, T1 arg1, T2 arg2) {
			var subscribers = eventHandler.Subscriptions;
			for (int i = 0; i < subscribers.Count; i++)
				await subscribers[i].Invoke(arg1, arg2).ConfigureAwait(false);
		}
		public static async Task InvokeAsync<T1, T2, T3>(this AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3) {
			var subscribers = eventHandler.Subscriptions;
			for (int i = 0; i < subscribers.Count; i++)
				await subscribers[i].Invoke(arg1, arg2, arg3).ConfigureAwait(false);
		}
		public static async Task InvokeAsync<T1, T2, T3, T4>(this AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
			var subscribers = eventHandler.Subscriptions;
			for (int i = 0; i < subscribers.Count; i++)
				await subscribers[i].Invoke(arg1, arg2, arg3, arg4).ConfigureAwait(false);
		}
		public static async Task InvokeAsync<T1, T2, T3, T4, T5>(this AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) {
			var subscribers = eventHandler.Subscriptions;
			for (int i = 0; i < subscribers.Count; i++)
				await subscribers[i].Invoke(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
		}
	}
}
