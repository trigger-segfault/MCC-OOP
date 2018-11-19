using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// RUNTIME-SPECIFIC identifiers for database types.
	/// </summary>
	public enum EntityType {

		// Root Contexts
		Guild,
		DM,

		// Misc
		User,
		UserMessage,
		Emote,

		// Guild-Specific
		GuildChannel,
		GuildEmote,
		GuildMessage,
		GuildUser,
		CategoryChannel,
		VoiceChannel,
		Role,
	}

	/// <summary>
	/// The snowflake entity Id type.
	/// </summary>
	public enum SnowflakeType {
		Guild,
		Channel,
		CategoryChannel,
		VoiceChannel,
		User,
		Message,
		Emote,
		Role,
	}

	public static class EntityTypeExtensions {


		/// <summary>
		/// Gets if the entity is owned by a guild.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsGuildEntity(this EntityType type) {
			switch (type) {
			case EntityType.GuildChannel:
			case EntityType.GuildEmote:
			case EntityType.GuildMessage:
			case EntityType.GuildUser:
			case EntityType.CategoryChannel:
			case EntityType.VoiceChannel:
			case EntityType.Role:
				return true;
			default:
				return false;
			}
		}

		public static bool IsGuildSnowflake(this EntityType type) {
			switch (type) {
			case EntityType.GuildChannel:
			case EntityType.GuildEmote:
			case EntityType.GuildMessage:
			case EntityType.GuildUser:
				return true;
			default:
				return false;
			}
		}

		public static bool IsTextChannel(this EntityType type) {
			switch (type) {
			case EntityType.DM:
			case EntityType.GuildChannel:
				return true;
			default:
				return false;
			}
		}

		public static bool IsUser(this EntityType type) {
			switch (type) {
			case EntityType.User:
			case EntityType.GuildUser:
				return true;
			default:
				return false;
			}
		}
		public static bool IsEmote(this EntityType type) {
			switch (type) {
			case EntityType.Emote:
			case EntityType.GuildEmote:
				return true;
			default:
				return false;
			}
		}
		public static bool IsMessage(this EntityType type) {
			switch (type) {
			case EntityType.UserMessage:
			case EntityType.GuildMessage:
				return true;
			default:
				return false;
			}
		}
	}
}
