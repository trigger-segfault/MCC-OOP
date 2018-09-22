using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Discord;

namespace EnigmaBot.Info {
	public static class BotReactions {
		private const string Responses = "Responses";
		private const string Actions = "Actions";

		[Description("The command was successful")]
		[Category(Responses)]
		public static readonly Emoji Success = new Emoji("✅");

		[Description("Click to hear message")]
		[Category(Actions)]
		public static readonly Emoji ViewMessage = new Emoji("🔍");
	}
}
