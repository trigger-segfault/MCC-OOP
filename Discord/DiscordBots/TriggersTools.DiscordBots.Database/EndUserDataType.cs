using System;
using System.Collections.Generic;
using System.Text;
using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// The types of End User Data that can be erased.
	/// </summary>
	public enum EndUserDataType {
		/// <summary>The data is related to a user.</summary>
		User,
		/// <summary>The data is related to a guild.</summary>
		Guild,
	}

	/// <summary>
	/// Extension methods for the <see cref="EndUserDataType"/> enum.
	/// </summary>
	public static class EndUserDataTypeExtensions {
		/// <summary>
		/// Gets the interface type used to identify this End User Data.
		/// </summary>
		/// <param name="type">The End User Data type.</param>
		/// <returns>Either <see cref="IDbEndUserData"/> or <see cref="IDbEndUserGuildData"/>.</returns>
		public static Type GetInterfaceType(this EndUserDataType type) {
			switch (type) {
			case EndUserDataType.User: return typeof(IDbEndUserData);
			case EndUserDataType.Guild: return typeof(IDbEndUserGuildData);
			}
			return null;
		}
	}
}
