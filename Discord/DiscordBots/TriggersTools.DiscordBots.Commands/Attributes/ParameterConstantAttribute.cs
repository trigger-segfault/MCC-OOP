using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// An attribute stating the parameter must equate to at least one of the listed constants.
	/// </summary>
	public class ParameterConstantAttribute : ParameterPreconditionAttribute {

		/// <summary>
		/// The constants that the parameter name must match.
		/// </summary>
		public string[] Constants { get; }

		public bool IgnoreCase { get; set; }

		public ParameterConstantAttribute(params string[] constants) {
			if (constants.Length == 0)
				throw new ArgumentException($"{nameof(ParameterConstantAttribute)} must have at least one argument!");
			Constants = constants;
		}

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
			ParameterInfo parameter, object value, IServiceProvider services)
		{
			string s = value.ToString();
			if (Constants.Any(c => string.Compare(c, s, IgnoreCase) == 0))
				return Task.FromResult(PreconditionResult.FromSuccess());
			return Task.FromResult(PreconditionAttributeResult.FromError("The parameter does not match one of the required constants", this));
		}
	}
}
