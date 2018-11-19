using Discord;
using Discord.WebSocket;

namespace TriggersTools.DiscordBots {
	public static class UserExtensions {
		
		/// <summary>
		/// Gets the nickname or username of the Discord user.
		/// </summary>
		/// <param name="user">The user to get the name of.</param>
		/// <param name="guild">The optional guild to get the nickname from.</param>
		/// <param name="sanitize">True if the name should be sanitized.</param>
		/// <returns>The nickname of the guild user has one. Otherwise the username.</returns>
		public static string GetName(this IUser user, IGuild guild, bool sanitize) {
			string name = user.Username;
			if (guild != null) {
				IGuildUser gUser;
				if (user is IGuildUser)
					gUser = (IGuildUser) user;
				else if (guild is SocketGuild sGuild)
					gUser = sGuild.GetUser(user.Id);
				else
					gUser = guild.GetUserAsync(user.Id).GetAwaiter().GetResult();
				name = gUser.Nickname ?? name;
			}
			return (sanitize ? Format.Sanitize(name) : name);
		}

		/// <summary>
		/// Gets the nickname of the Discord user if they have one.
		/// </summary>
		/// <param name="user">The user to get the nickname of.</param>
		/// <param name="guild">The optional guild to get the nickname from.</param>
		/// <param name="sanitize">True if the nickname should be sanitized.</param>
		/// <returns>The nickname of the guild user has one. Otherwise null.</returns>
		public static string GetNickname(this IUser user, IGuild guild, bool sanitize) {
			if (guild != null) {
				IGuildUser gUser;
				if (user is IGuildUser)
					gUser = (IGuildUser) user;
				else if (guild is SocketGuild socketGuild)
					gUser = socketGuild.GetUser(user.Id);
				else
					gUser = guild.GetUserAsync(user.Id).GetAwaiter().GetResult();
				if (gUser.Nickname != null)
					return (sanitize ? Format.Sanitize(gUser.Nickname) : gUser.Nickname);
			}
			return null;
		}

		public static EmbedBuilder WithAuthorUsername(this EmbedBuilder builder, IUser user) {
			return builder.WithAuthor($"{user.Username}", user.GetAvatarUrl());
		}
		public static EmbedBuilder WithAuthorName(this EmbedBuilder builder, IUser user, IGuild guild) {
			IGuildUser gUser;
			if (user is IGuildUser)
				gUser = (IGuildUser) user;
			else if (guild is SocketGuild socketGuild)
				gUser = socketGuild.GetUser(user.Id);
			else
				gUser = guild.GetUserAsync(user.Id).GetAwaiter().GetResult();
			return builder.WithAuthor($"{(gUser.Nickname ?? user.Username)}", user.GetAvatarUrl());
		}
		public static EmbedBuilder WithAuthorName(this EmbedBuilder builder, IGuildUser user) {
			return builder.WithAuthor($"{(user.Nickname ?? user.Username)}", user.GetAvatarUrl());
		}
	}
}
