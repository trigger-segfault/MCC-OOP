using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// Require that the command is invoked in a channel not marked as NSFW.
	/// </summary>
	public class RequiresSfwAttribute : PreconditionAttribute {

		/// <summary>
		/// Checks if the <paramref name="command"/> has the sufficient permission to be executed.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <param name="command">The command being executed.</param>
		/// <param name="services">The service collection used for dependency injection.</param>
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
			CommandInfo command, IServiceProvider services)
		{
			if (context.Channel is ITextChannel text && !text.IsNsfw)
				return Task.FromResult(PreconditionResult.FromSuccess());
			return Task.FromResult(PreconditionAttributeResult.FromError("This command may only be invoked in a SFW channel.", this));
		}
	}
}
