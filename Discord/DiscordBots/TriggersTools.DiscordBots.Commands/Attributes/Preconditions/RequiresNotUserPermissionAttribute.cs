using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Extensions;

namespace Discord.Commands {
	/// <summary>
	///     Requires the user invoking the command to not have a specified permission.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class RequiresNotUserPermissionAttribute : PreconditionAttribute {
		/// <summary>
		///     Gets the specified <see cref="Discord.GuildPermission" /> of the precondition.
		/// </summary>
		public GuildPermission? GuildPermission { get; }
		/// <summary>
		///     Gets the specified <see cref="Discord.ChannelPermission" /> of the precondition.
		/// </summary>
		public ChannelPermission? ChannelPermission { get; }

		/// <summary>
		///     Requires that the user invoking the command to not have a specific <see cref="Discord.GuildPermission"/>.
		/// </summary>
		/// <remarks>
		///     This precondition will always fail if the command is being invoked in a <see cref="IPrivateChannel"/>.
		/// </remarks>
		/// <param name="permission">
		///     The <see cref="Discord.GuildPermission" /> that the user must have. Multiple permissions can be
		///     specified by ORing the permissions together.
		/// </param>
		public RequiresNotUserPermissionAttribute(GuildPermission permission) {
			GuildPermission = permission;
			ChannelPermission = null;
		}
		/// <summary>
		///     Requires that the user invoking the command to not have a specific <see cref="Discord.ChannelPermission"/>.
		/// </summary>
		/// <param name="permission">
		///     The <see cref="Discord.ChannelPermission"/> that the user must have. Multiple permissions can be
		///     specified by ORing the permissions together.
		/// </param>
		public RequiresNotUserPermissionAttribute(ChannelPermission permission) {
			ChannelPermission = permission;
			GuildPermission = null;
		}

		/// <inheritdoc />
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) {
			var guildUser = context.User as IGuildUser;

			if (GuildPermission.HasValue) {
				if (guildUser == null)
					return Task.FromResult(PreconditionAttributeResult.FromError("Command must be used in a guild channel.", this));
				if (guildUser.GuildPermissions.Has(GuildPermission.Value))
					return Task.FromResult(PreconditionPermissionResult.FromError($"User requires guild permission {GuildPermission.Value}.", GuildPermission.Value & ~((GuildPermission) guildUser.GuildPermissions.RawValue), 0, this));
			}

			if (ChannelPermission.HasValue) {
				ChannelPermissions perms;
				if (context.Channel is IGuildChannel guildChannel)
					perms = guildUser.GetPermissions(guildChannel);
				else
					perms = ChannelPermissions.All(context.Channel);

				if (perms.Has(ChannelPermission.Value))
					return Task.FromResult(PreconditionPermissionResult.FromError($"User requires channel permission {ChannelPermission.Value}.", 0, ChannelPermission.Value & ~((ChannelPermission) perms.RawValue), this));
			}

			return Task.FromResult(PreconditionResult.FromSuccess());
		}
	}
}
