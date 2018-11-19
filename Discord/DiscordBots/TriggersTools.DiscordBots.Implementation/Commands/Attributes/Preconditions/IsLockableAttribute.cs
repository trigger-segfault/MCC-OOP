using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Commands.Base;
using TriggersTools.DiscordBots.Extensions;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// A precondition to check if a command or module is locked.
	/// </summary>
	public class IsLockableAttribute : IsLockableBaseAttribute {
		
		#region Constructors
		
		/// <summary>
		/// Constructs the <see cref="IsLockableAttribute"/>
		/// </summary>
		/// <param name="lockable">True if the command is lockable.</param>
		/// <param name="lockedByDefault">True if the commmand starts out locked.</param>
		public IsLockableAttribute(bool lockable, bool lockedByDefault = false)
			: base(lockable, lockedByDefault) { }

		#endregion

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
			if (context.LockContext == null)
				return Task.FromResult(PreconditionResult.FromSuccess());

			CommandDetails command = context.Commands.CommandSet.FindCommand(cmd.GetDetailsName());
			//IContextingService locking = services.GetService<IContextingService>();
			if (command.IsLocked(context.LockContext))
				return Task.FromResult(PreconditionAttributeResult.FromError("The command is locked", this));
			
			return Task.FromResult(PreconditionResult.FromSuccess());
		}

		#endregion
	}
}
