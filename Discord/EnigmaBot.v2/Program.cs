using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using TriggersTools.DiscordBots;

namespace EnigmaBot {
	class Program {
		/// <summary>
		/// Run the Discord Bot Program and setup assembly resolution.
		/// </summary>
		/// <param name="args">The program arguments.</param>
		/// <returns>The exit code.</returns>
		static async Task<int> Main(string[] args) {
			//Process.Start("java", "-jar Lavalink.jar").Dispose();
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
			return await Run(args).ConfigureAwait(false);
		}
		/// <summary>
		/// Run the Discord Bot.
		/// </summary>
		/// <param name="args">The program arguments.</param>
		/// <returns>The exit code.</returns>
		/// <remarks>
		/// We have this function so that we can resolve assemblies that this function requires.
		/// </remarks>
		static async Task<int> Run(string[] args) {
			Console.Title = "Enigma Machine - Discord Bot";
			return await DiscordStartup.RunAsync(args, () => new EnigmaMachineBot()).ConfigureAwait(false);
		}
		/// <summary>
		/// Resolves assemblies from the "libraries" folder.
		/// </summary>
		static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) {
			AssemblyName assemblyName = new AssemblyName(args.Name);
			string path = Path.Combine(AppContext.BaseDirectory, "libraries", assemblyName.Name + ".dll");
			return (File.Exists(path) ? Assembly.LoadFile(path) : null);
		}
	}
}
