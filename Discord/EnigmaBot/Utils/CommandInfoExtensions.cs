using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.Commands;
using EnigmaBot.Info;

namespace EnigmaBot.Utils {
	public static class CommandInfoExtensions {

		public static bool IsDuplicateFunctionality(this CommandInfo cmd) {
			var attr = cmd.Attributes.OfType<IsDuplicateAttribute>().FirstOrDefault();
			return (attr?.DuplicateFunctionality ?? false);
		}

		public static bool IsDuplicate(this CommandInfo cmd) {
			return cmd.Attributes.OfType<IsDuplicateAttribute>().Any();
		}

		public static ModuleInfo RootModule(this CommandInfo cmd) {
			ModuleInfo mod = cmd.Module;
			while (mod.Parent != null) {
				mod = mod.Parent;
			}
			return mod;
		}

		public static ModuleInfo RootModule(this ModuleInfo mod) {
			while (mod.Parent != null) {
				mod = mod.Parent;
			}
			return mod;
		}

		public static bool HasParameters(this CommandInfo cmd) {
			return cmd.Attributes.OfType<ParametersAttribute>().Any();
		}

		public static string GetParameters(this CommandInfo cmd) {
			var attr = cmd.Attributes.OfType<ParametersAttribute>().FirstOrDefault();
			return attr?.Parameters;
		}

		public static bool HasExample(this CommandInfo cmd) {
			return cmd.Attributes.OfType<ExampleAttribute>().Any();
		}

		public static string GetExample(this CommandInfo cmd) {
			var attr = cmd.Attributes.OfType<ExampleAttribute>().FirstOrDefault();
			return attr?.Example;
		}
	}
}
