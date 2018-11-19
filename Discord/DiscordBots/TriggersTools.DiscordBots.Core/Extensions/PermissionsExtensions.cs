using Discord;

namespace TriggersTools.DiscordBots.Extensions {
	/// <summary>
	/// Extensions methods for <see cref="GuildPermission"/> and <see cref="ChannelPermission"/>.
	/// </summary>
	public static class PermissionsExtensions {
		/// <summary>
		/// Casts the <see cref="GuildPermissions"/> struct to the <see cref="GuildPermission"/> enum.
		/// </summary>
		/// <param name="perms">The <see cref="GuildPermissions"/> struct.</param>
		/// <returns>The <see cref="GuildPermission"/> enum</returns>
		public static GuildPermission ToEnum(this GuildPermissions perms) {
			GuildPermission outPerms = 0;
			foreach (GuildPermission perm in perms.ToList())
				outPerms |= perm;
			return outPerms;
		}
		/// <summary>
		/// Casts the <see cref="ChannelPermissions"/> struct to the <see cref="ChannelPermission"/> enum.
		/// </summary>
		/// <param name="perms">The <see cref="ChannelPermissions"/> struct.</param>
		/// <returns>The <see cref="ChannelPermission"/> enum</returns>
		public static ChannelPermission ToEnum(this ChannelPermissions perms) {
			ChannelPermission outPerms = 0;
			foreach (ChannelPermission perm in perms.ToList())
				outPerms |= perm;
			return outPerms;
		}

		/// <summary>
		/// Checks if the <see cref="GuildPermissions"/> has any of the <see cref="GuildPermission"/> flags.
		/// </summary>
		/// <param name="perms">The permissions struct to check.</param>
		/// <param name="perm">The permissions flags to check.</param>
		/// <returns>True if the permissions share any of their flags.</returns>
		public static bool HasAny(this GuildPermissions perms, GuildPermission perm) {
			return (perm & (GuildPermission) perms.RawValue) != 0;
		}
		/// <summary>
		/// Checks if the <see cref="ChannelPermissions"/> has any of the <see cref="ChannelPermission"/> flags.
		/// </summary>
		/// <param name="perms">The permissions struct to check.</param>
		/// <param name="perm">The permissions flags to check.</param>
		/// <returns>True if the permissions share any of their flags.</returns>
		public static bool HasAny(this ChannelPermissions perms, ChannelPermission perm) {
			return (perm & (ChannelPermission) perms.RawValue) != 0;
		}
	}
}
