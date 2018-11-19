using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// A result returned to denote that the origin message was deleted.
	/// </summary>
	[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
	public class DeletedResult : RuntimeResult {
		/// <summary>
		/// Initializes a new <see cref="DeletedResult" /> class with the type of error and reason.
		/// </summary>
		/// <param name="error">The type of failure, or <c>null</c> if none.</param>
		/// <param name="reason">The reason of failure.</param>
		public DeletedResult(CommandError? error, string reason) : base(error, reason) {

		}

		public static RuntimeResult FromSuccess()
			=> new DeletedResult(null, null);
		public static RuntimeResult FromError(CommandError error, string reason)
			=> new DeletedResult(error, reason);
		public static RuntimeResult FromError(Exception ex)
			=> new DeletedResult(CommandError.Exception, ex.Message);

		public override string ToString() => $"[Deleted] {(Reason ?? (IsSuccess ? "Successful" : "Unsuccessful"))}";
		private string DebuggerDisplay => $"[Deleted] {(IsSuccess ? $"Success: {Reason ?? "No Reason"}" : $"{Error}: {Reason}")}";
	}
}
