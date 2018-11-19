using System.Linq;
using Microsoft.Extensions.Configuration;

namespace TriggersTools.DiscordBots.Extensions {
	/// <summary>
	/// Extension methods or <see cref="IConfiguration"/>.
	/// </summary>
	public static class ConfigurationExtensions {
		/// <summary>
		/// Gets an array of values from the specified key.
		/// </summary>
		/// <param name="config">The configuration root.</param>
		/// <param name="key">The key of the configuration array.</param>
		/// <returns>The array of values.</returns>
		public static string[] GetArray(this IConfiguration config, string key) {
			return config.GetSection(key).GetChildren().Select(c => c.Value).ToArray();
		}
	}
}
