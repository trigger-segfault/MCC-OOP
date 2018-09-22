using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnigmaBot.Context {
	public class BotErrorResult : IBotErrorResult, IResult {

		public Exception Exception { get; set; }
		public CommandError? Error { get; set; }
		public string ErrorReason { get; set; }
		public bool IsSuccess { get; set; } = true;
	}

	public interface IBotErrorResult : IResult {
		Exception Exception { get; }
	}
}
