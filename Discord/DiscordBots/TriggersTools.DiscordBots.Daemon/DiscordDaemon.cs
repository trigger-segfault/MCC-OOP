using System;
using System.Diagnostics;
using System.Linq;

namespace TriggersTools.DiscordBots {
	public static class DiscordDaemon {
		public static void RunDotNet(string dll, params string[] args) {
			Process process;
			do {
				Console.ResetColor();
				Console.Clear();
				ProcessStartInfo start = new ProcessStartInfo() {
					FileName = "dotnet",
					Arguments = $"{dll} {FormatArgs(args)}",
					UseShellExecute = false,
				};
				process = Process.Start(start);
				process.WaitForExit();
			} while (process.ExitCode != 0);
		}
		public static void RunExecutable(string exe, params string[] args) {
			Process process;
			do {
				Console.ResetColor();
				Console.Clear();
				ProcessStartInfo start = new ProcessStartInfo() {
					FileName = exe,
					Arguments = FormatArgs(args),
					UseShellExecute = false,
				};
				process = Process.Start(start);
				process.WaitForExit();
			} while (process.ExitCode != 0);
		}

		private static string FormatArgs(string[] args) {
			if (args.Any())
				return $"-daemon {string.Join(" ", args.Select(a => $"\"{a}\""))}";
			return "-daemon";
		}
	}
}
