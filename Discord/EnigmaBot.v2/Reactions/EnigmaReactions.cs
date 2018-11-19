using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Discord;
using TriggersTools.DiscordBots;
using TriggersTools.DiscordBots.Reactions;

namespace EnigmaBot.Reactions {
	public class EnigmaReactions : ReactionService {

		#region Constants

		// Category Names
		private const string BotResponses = "Bot Responses";
		private const string BotActions = "Bot Actions";
		private const string UserActions = "User Actions";

		#endregion

		#region Constructors

		public EnigmaReactions(DiscordBotServiceContainer services) : base(services) { }

		#endregion

		#region Response Reactions
		
		[Description("The command was successful")]
		[Category(BotResponses)]
		public static Emoji Success { get; } = new Emoji("✅");
		[Description("The command was not found")]
		[Category(BotResponses)]
		public static Emoji UnknownCommand { get; } = new Emoji("❓");
		[Description("Not allowed to use the command")]
		[Category(BotResponses)]
		public static Emoji UnmetPrecondition { get; } = new Emoji("⛔");
		[Description("The command is or was locked")]
		[Category(BotResponses)]
		public static Emoji Locked { get; } = new Emoji("🔒");
		[Description("The command is or was unlocked")]
		[Category(BotResponses)]
		public static Emoji Unlocked { get; } = new Emoji("🔓");
		[Description("The command was incorrectly formatted")]
		[Category(BotResponses)]
		public static Emoji InvalidArgument { get; } = new Emoji("❌");
		[Description("An error occurred within the bot")]
		[Category(BotResponses)]
		public static Emoji Exception { get; } = new Emoji("⚠");
		[Description("The reply was sent to you as a direct message")]
		[Category(BotResponses)]
		public static Emoji DMSent { get; } = new Emoji("📨");

		#endregion

		#region Bot Action Reactions

		[Description("Click to view deciphered message")]
		[Category(BotActions)]
		public static Emoji ViewMessage { get; } = new Emoji("🔍");

		#endregion
	}
}
