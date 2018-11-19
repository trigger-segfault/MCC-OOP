using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Extensions;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// A precondition requiring that the command is invoked by a creator of the bot.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RequiresSuperuserAttribute : RequiresOwnerAttribute {
		/// <summary>
		/// Checks if the <paramref name="command"/> has the sufficient permission to be executed.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <param name="command">The command being executed.</param>
		/// <param name="services">The service collection used for dependency injection.</param>
		public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext contextBase,
			CommandInfo command, IServiceProvider services)
		{
			IDiscordBotCommandContext context = (IDiscordBotCommandContext) contextBase;
			var ownerResult = await base.CheckPermissionsAsync(context, command, services).ConfigureAwait(false);
			if (ownerResult.IsSuccess)
				return PreconditionResult.FromSuccess();
			string[] owners = context.Config.GetArray("ids:discord:superuseres");
			if (owners.Any(id => ulong.Parse(id) == context.User.Id))
				return PreconditionResult.FromSuccess();
			return PreconditionAttributeResult.FromError("You are not registed as a superuser of this bot", this);
		}
	}
}
