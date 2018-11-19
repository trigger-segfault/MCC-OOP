using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using TriggersTools.DiscordBots.Commands;
using EnigmaBot.Reactions;

namespace EnigmaBot.Commands {
	public static class EmoteResults {
		public static RuntimeResult FromSuccess() {
			return ReactionResult.FromSuccess(EnigmaReactions.Success);
		}
		public static RuntimeResult FromInvalidArgument() {
			return ReactionResult.FromError(CommandError.ParseFailed, null, EnigmaReactions.InvalidArgument);
		}
		public static RuntimeResult FromInvalidArgument(string reason) {
			return ReactionResult.FromError(CommandError.ParseFailed, reason, EnigmaReactions.InvalidArgument);
		}
		public static RuntimeResult FromException() {
			return ReactionResult.FromError(CommandError.Exception, null, EnigmaReactions.Exception);
		}
		public static RuntimeResult FromUnmetPrecondition() {
			return ReactionResult.FromError(CommandError.UnmetPrecondition, null, EnigmaReactions.UnmetPrecondition);
		}
		public static RuntimeResult FromLocked() {
			return ReactionResult.FromError(CommandError.UnmetPrecondition, null, EnigmaReactions.Success, EnigmaReactions.Locked);
		}
		public static RuntimeResult FromUnlocked() {
			return ReactionResult.FromError(CommandError.UnmetPrecondition, null, EnigmaReactions.Success, EnigmaReactions.Unlocked);
		}
		public static RuntimeResult FromDMSent() {
			return ReactionResult.FromSuccess(EnigmaReactions.DMSent);
		}
	}
}
