using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Utils {
	public static class ConfigUtils {
		/// <summary>
		/// Attempts to parse the configuration type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of configuration.</typeparam>
		/// <param name="config">The file config information.</param>
		/// <param name="sectionKey">The name of the config section.</param>
		/// <returns>The parsed configuration type <typeparamref name="T"/>.</returns>
		public static T Parse<T>(IConfigurationRoot config, string sectionKey) where T : new() {
			return Parse<T>(config.GetSection(sectionKey));
		}
		/// <summary>
		/// Attempts to parse the configuration type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of configuration.</typeparam>
		/// <param name="section">The file config section information.</param>
		/// <returns>The parsed configuration type <typeparamref name="T"/>.</returns>
		public static T Parse<T>(IConfigurationSection section) where T : new() {
			section = section.GetSection(typeof(T).Name);
			T configuration = new T();
			if (section == null)
				return configuration;

			var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (PropertyInfo prop in props) {
				if (prop.SetMethod != null) {
					if (TryParse(prop.PropertyType, section[prop.Name], out object value))
						prop.SetValue(configuration, value);
				}
			}
			
			return configuration;
		}
		
		/// <summary>
		/// Tries to parse the specified type.
		/// </summary>
		/// <param name="type">The type to parse.</param>
		/// <param name="s">The string to parse.</param>
		/// <param name="value">The output value.</param>
		/// <returns>True if the parse was successful.</returns>
		private static bool TryParse(Type type, string s, out object value) {
			if (!string.IsNullOrEmpty(s)) {
				try {
					var converter = TypeDescriptor.GetConverter(type);
					if (converter != null) {
						value = converter.ConvertFromString(s);
						return true;
					}
				} catch (NotSupportedException) {
					Console.WriteLine($"Config type \"{type.Name}\" not supported");
				}
			}
			value = null;
			return false;
		}
	}
}
