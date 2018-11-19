using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace TriggersTools.DiscordBots.Commands.Base {
	/// <summary>
	/// A base precondition to check if a command or module is locked.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public abstract class IsLockableBaseAttribute : PreconditionAttribute {
		/// <summary>
		/// Gets if the command is lockable.
		/// </summary>
		public bool IsLockable { get; }
		/// <summary>
		/// Gets if the commmand starts out locked.
		/// </summary>
		public bool IsLockedByDefault { get; }

		/// <summary>
		/// Constructs the <see cref="IsLockableBaseAttribute"/>
		/// </summary>
		/// <param name="lockable">True if the command is lockable.</param>
		/// <param name="lockedByDefault">True if the commmand starts out locked.</param>
		public IsLockableBaseAttribute(bool lockable, bool lockedByDefault = false) {
			IsLockable = lockable;
			IsLockedByDefault = lockedByDefault;
			if (!lockable && lockedByDefault)
				throw new ArgumentException("Cannot make a command or module locked by default without being lockable!");
		}
	}
}
