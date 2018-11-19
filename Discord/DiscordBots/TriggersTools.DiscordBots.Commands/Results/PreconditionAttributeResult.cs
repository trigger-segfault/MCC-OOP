using System;
using System.Collections.Generic;
using System.Diagnostics;
using Discord.Commands;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// Represents a result type for command preconditions that contains the calling attribute.
	/// </summary>
	[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
	public class PreconditionAttributeResult : PreconditionResult {
		/// <summary>
		/// Gets the precondition that failed. Null on success.
		/// </summary>
		public Attribute Precondition { get; }
		
		/// <summary>
		/// Initializes a new <see cref="PreconditionAttributeResult" /> class with the command <paramref name="error"/> type,
		/// reason and associated attribute.
		/// </summary>
		/// <param name="error">The type of failure.</param>
		/// <param name="errorReason">The reason of failure.</param>
		/// <param name="precondition">The precondition that triggered this error.</param>
		protected PreconditionAttributeResult(CommandError? error, string errorReason, Attribute precondition)
			: base(error, errorReason)
		{
			Precondition = precondition;
		}

		/// <summary>
		///     Returns a <see cref="PreconditionAttributeResult" /> with no errors.
		/// </summary>
		//public new static PreconditionResult FromSuccess()
		//	=> new PreconditionAttributeResult(null, null, null);
		/// <summary>
		///     Returns a <see cref="PreconditionAttributeResult" /> with <see cref="CommandError.UnmetPrecondition" /> and the
		///     specified reason and failed attribute.
		/// </summary>
		/// <param name="reason">The reason of failure.</param>
		public static PreconditionResult FromError(string reason, Attribute precondition)
			=> new PreconditionAttributeResult(CommandError.UnmetPrecondition, reason, precondition);
		public static PreconditionResult FromError(Exception ex, Attribute precondition)
			=> new PreconditionAttributeResult(CommandError.Exception, ex.Message, precondition);
		/// <summary>
		///     Returns a <see cref="PreconditionAttributeResult" /> with the specified <paramref name="result"/> type.
		/// </summary>
		/// <param name="result">The result of failure.</param>
		public static PreconditionResult FromError(IResult result, Attribute precondition)
			=> new PreconditionAttributeResult(result.Error, result.ErrorReason, precondition);

		/// <summary>
		/// Returns a string indicating whether the <see cref="PreconditionAttributeResult"/> is successful.
		/// </summary>
		public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
		private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
	}
}
