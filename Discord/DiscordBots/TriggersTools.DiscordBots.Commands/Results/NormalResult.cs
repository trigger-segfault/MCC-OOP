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
	public class NormalResult : RuntimeResult {

		/// <summary>
		/// Initializes a new <see cref="NormalResult" /> class with the type of error and reason
		/// </summary>
		/// <param name="error">The type of failure, or <c>null</c> if none.</param>
		/// <param name="reason">The reason of failure.</param>
		public NormalResult(CommandError? error, string reason) : base(error, reason) { }
		
		public static RuntimeResult FromSuccess()
			=> new NormalResult(null, null);
		public static RuntimeResult FromError(CommandError error, string reason)
			=> new NormalResult(error, reason);
		public static RuntimeResult FromError(Exception ex)
			=> new NormalResult(CommandError.Exception, ex.Message);

		public override string ToString() => Reason ?? (IsSuccess ? "Successful" : "Unsuccessful");
		private string DebuggerDisplay => IsSuccess ? $"Success: {Reason ?? "No Reason"}" : $"{Error}: {Reason}";
	}
}
