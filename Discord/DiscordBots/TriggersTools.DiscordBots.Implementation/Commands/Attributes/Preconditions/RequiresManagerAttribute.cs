using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// A precondition, that requires the user has a role designated for bot managers.
	/// </summary>
	public class RequiresManagerAttribute : PreconditionAttribute {

		#region Precondition

		/// <summary>
		/// Checks if the <paramref name="cmd"/> has the sufficient permission to be executed.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <param name="cmd">The command being executed.</param>
		/// <param name="services">The service collection used for dependency injection.</param>
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext contextBase,
			CommandInfo cmd, IServiceProvider services)
		{
			IDiscordBotCommandContext context = (IDiscordBotCommandContext) contextBase;
			if (context.ManageContext == null)
				return Task.FromResult(PreconditionAttributeResult.FromError("Not a bot manager", this));

			CommandDetails command = context.Commands.CommandSet.FindCommand(cmd.GetDetailsName());
			ulong roleId = context.ManageContext.ManagerRoleId;
			if (roleId == 0 || !(context.User is IGuildUser guildUser && guildUser.RoleIds.Contains(roleId)))
				return Task.FromResult(PreconditionAttributeResult.FromError("Not a bot manager", this));

			return Task.FromResult(PreconditionResult.FromSuccess());
		}

		#endregion
	}
}
