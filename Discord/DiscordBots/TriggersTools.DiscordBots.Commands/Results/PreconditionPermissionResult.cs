using System;
using System.Collections.Generic;
using System.Diagnostics;
using Discord;
using Discord.Commands;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// Represents a result type for command preconditions that contain missing permissions.
	/// </summary>
	public class PreconditionPermissionResult : PreconditionAttributeResult {
		/// <summary>
		/// The guild premissions that are required.
		/// </summary>
		public GuildPermission GuildPermissions { get; }
		/// <summary>
		/// The channel premissions that are required.
		/// </summary>
		public ChannelPermission ChannelPermissions { get; }

		/// <summary>
		///     Initializes a new <see cref="PreconditionPermissionResult" /> class with the command <paramref name="error"/> type
		///     and reason.
		/// </summary>
		/// <param name="error">The type of failure.</param>
		/// <param name="errorReason">The reason of failure.</param>
		protected PreconditionPermissionResult(CommandError? error, string errorReason, GuildPermission guildPermissions, ChannelPermission channelPermissions, Attribute precondition)
			: base(error, errorReason, precondition)
		{
			GuildPermissions = guildPermissions;
			ChannelPermissions = channelPermissions;
		}

		/// <summary>
		///     Returns a <see cref="PreconditionResult" /> with <see cref="CommandError.UnmetPrecondition" /> and the
		///     specified reason.
		/// </summary>
		/// <param name="reason">The reason of failure.</param>
		public static PreconditionResult FromError(string reason, GuildPermission guildPermissions, ChannelPermission channelPermissions, PreconditionAttribute precondition)
			=> new PreconditionPermissionResult(CommandError.UnmetPrecondition, reason, guildPermissions, channelPermissions, precondition);
		public static PreconditionResult FromError(Exception ex, GuildPermission guildPermissions, ChannelPermission channelPermissions, PreconditionAttribute precondition)
			=> new PreconditionPermissionResult(CommandError.Exception, ex.Message, guildPermissions, channelPermissions, precondition);
		/// <summary>
		///     Returns a <see cref="PreconditionResult" /> with the specified <paramref name="result"/> type.
		/// </summary>
		/// <param name="result">The result of failure.</param>
		public static PreconditionResult FromError(IResult result, IEmote reaction, GuildPermission guildPermissions, ChannelPermission channelPermissions, PreconditionAttribute precondition)
			=> new PreconditionPermissionResult(result.Error, result.ErrorReason, guildPermissions, channelPermissions, precondition);
		/// <summary>
		///     Returns a <see cref="PreconditionResult" /> with the specified <paramref name="result"/> type.
		/// </summary>
		/// <param name="result">The result of failure.</param>
		public static PreconditionResult FromError(IResult result, GuildPermission guildPermissions, ChannelPermission channelPermissions, PreconditionAttribute precondition)
			=> new PreconditionPermissionResult(result.Error, result.ErrorReason, guildPermissions, channelPermissions, precondition);
	}
}
