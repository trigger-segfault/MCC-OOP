using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// A precondition specifying if bots are allowed to use this context.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AllowBotsAttribute : PreconditionAttribute {

		#region Fields

		/// <summary>
		/// Gets if bots are allowed to use this command.
		/// </summary>
		public bool Allow { get; }

		#endregion

		#region Constructors

		/*/// <summary>
		/// Constructs the <see cref="AllowBotsAttribute"/> where bots are allowed to use this context.
		/// </summary>
		public AllowBotsAttribute() {
			Allow = true;
		}*/
		/// <summary>
		/// Constructs the <see cref="AllowBotsAttribute"/> with the specified setting.
		/// </summary>
		/// <param name="allowBots">True if bots are allowed to use this context.</param>
		public AllowBotsAttribute(bool allowBots) {
			Allow = allowBots;
		}

		#endregion

		#region Precondition

		/// <summary>
		/// Checks if the <paramref name="command"/> has the sufficient permission to be executed.
		/// </summary>
		/// <param name="context">The context of the command.</param>
		/// <param name="command">The command being executed.</param>
		/// <param name="services">The service collection used for dependency injection.</param>
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
			CommandInfo command, IServiceProvider services)
		{
			if (!context.User.IsBot)
				return Task.FromResult(PreconditionResult.FromSuccess());
			if (Allow)
				return Task.FromResult(PreconditionResult.FromSuccess());
			return Task.FromResult(PreconditionAttributeResult.FromError("Bots cannot use this command", this));
		}

		#endregion
	}
}
