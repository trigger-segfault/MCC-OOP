using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots {
	/// <summary>
	/// Event args surrounding a change to End User Data.
	/// </summary>
	public class EndUserDataEventArgs {

		#region Fields

		/// <summary>
		/// Gets the type of End User Data that changed.
		/// </summary>
		public EndUserDataType Type { get; }
		/// <summary>
		/// Gets the snowflake entity Id of the End User Data type that changed.
		/// </summary>
		public ulong Id { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="EndUserDataEventArgs"/>.
		/// </summary>
		/// <param name="id">The Id of the End User Data type that changed.</param>
		/// <param name="type">The type of End User Data that changed.</param>
		public EndUserDataEventArgs(ulong id, EndUserDataType type) {
			Id = id;
			Type = type;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the interface type associated with this End User Data type.
		/// </summary>
		public Type AssociatedInterfaceType => Type.GetInterfaceType();

		#endregion
	}

	/// <summary>
	/// A delegate for changes made to an End User Data type.
	/// </summary>
	/// <param name="e">The event args surrounding a change to End User Data.</param>
	public delegate Task EndUserDataEventHandler(EndUserDataEventArgs e);

	public static class EndUserDataExtensions {
		public static async Task InvokeAsync(this AsyncEvent<EndUserDataEventHandler> eventHandler, EndUserDataEventArgs e) {
			var subscribers = eventHandler.Subscriptions;
			for (int i = 0; i < subscribers.Count; i++)
				await subscribers[i].Invoke(e).ConfigureAwait(false);
		}
	}
}
