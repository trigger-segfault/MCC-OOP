using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TriggersTools.DiscordBots.Commands {
	[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
	public class ReactionResult : RuntimeResult {
		/// <summary>
		/// The optional reactions to the command.
		/// </summary>
		public IEmote[] Reactions { get; }

		/// <summary>
		/// Initializes a new <see cref="ReactionResult" /> class with the type of error, reason, and optional reaction.
		/// </summary>
		/// <param name="error">The type of failure, or <c>null</c> if none.</param>
		/// <param name="reason">The reason of failure.</param>
		/// <param name="reaction">The optional reaction to the command message.</param>
		protected ReactionResult(CommandError? error, string reason, IEmote[] reactions) : base(error, reason) {
			Reactions = reactions;
		}

		public static RuntimeResult FromSuccess(params IEmote[] reactions)
			=> new ReactionResult(null, null, reactions);
		public static RuntimeResult FromError(CommandError error, string reason, params IEmote[] reactions)
			=> new ReactionResult(error, reason, reactions);
		public static RuntimeResult FromError(Exception ex, params IEmote[] reactions)
			=> new ReactionResult(CommandError.Exception, ex.Message, reactions);

		public override string ToString() => Reactions.Any() ? string.Join(" ", (object[]) Reactions) :
			(IsSuccess ? "Successful" : "Unsuccessful");
		private string DebuggerDisplay => Reactions.Any() ? string.Join(" ", (object[]) Reactions) :
			(IsSuccess ? $"Success: {Reason ?? "No Reason"}" : $"{Error}: {Reason}");
	}

	/// <summary>
	/// Extension methods for constructing <see cref="ReactionResult"/>s from <see cref="IEmote"/>s.
	/// </summary>
	public static class ReactionResultExtensions {
		/// <summary>
		/// Constructs a successful reaction result.
		/// </summary>
		public static RuntimeResult FromSuccess(this IEmote emote) {
			return ReactionResult.FromSuccess(emote);
		}
		/// <summary>
		/// Constructs an error reaction result.
		/// </summary>
		public static RuntimeResult FromError(this IEmote emote, CommandError error, string reason = null) {
			return ReactionResult.FromError(error, reason, emote);
		}
		/// <summary>
		/// Constructs an exception reaction result.
		/// </summary>
		public static RuntimeResult FromError(this IEmote emote, Exception ex) {
			return ReactionResult.FromError(ex, emote);
		}
	}
}
