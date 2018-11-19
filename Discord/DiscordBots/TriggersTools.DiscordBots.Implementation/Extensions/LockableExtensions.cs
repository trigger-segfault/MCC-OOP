using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using TriggersTools.DiscordBots.Commands;

namespace TriggersTools.DiscordBots.Extensions {
	public static class LockableExtensions {

		public static IsLockableAttribute GetDownwardLockable(this CommandInfo cmd) {
			var attr = cmd.Preconditions.OfType<IsLockableAttribute>().FirstOrDefault();
			if (attr == null)
				attr = cmd.Module.GetLockable();
			return attr;
		}


		public static IsLockableAttribute GetLockable(this CommandInfo cmd) {
			var attr = cmd.Preconditions.OfType<IsLockableAttribute>().FirstOrDefault();
			if (attr == null)
				attr = cmd.Module.GetLockable();
			return attr;
		}

		public static IsLockableAttribute GetLockable(this ModuleInfo mod) {
			mod = mod.GetRootModule();
			return mod.Preconditions.OfType<IsLockableAttribute>().FirstOrDefault();
		}

		public static bool IsLockable(this CommandInfo cmd) {
			return (cmd.GetLockable()?.IsLockable ?? false);
		}

		public static bool IsLockable(this ModuleInfo mod) {
			return (mod.GetLockable()?.IsLockable ?? false);
		}

		public static bool HasLockable(this CommandInfo cmd) {
			return GetLockable(cmd) != null;
		}

		public static bool HasLockable(this ModuleInfo mod) {
			return GetLockable(mod) != null;
		}
	}
}
