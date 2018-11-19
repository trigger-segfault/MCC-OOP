using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots.Utils {
	public static class ConfigUtils {

		public static ReliabilityConfig Parse(IConfigurationRoot config, string section) {
			ReliabilityConfig configuration = new ReliabilityConfig();

			foreach (PropertyInfo prop in typeof(ReliabilityConfig).GetProperties(BindingFlags.Instance | BindingFlags.Public)) {

			}


			return configuration;
		}
		public static T Parse<T>(IConfigurationRoot config, string sectionKey) where T : new() {
			return Parse<T>(config.GetSection(sectionKey));
		}
		public static T Parse<T>(IConfigurationSection section) where T : new() {
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

		private static bool TryParse<T>(string s, out T value) {
			if (!string.IsNullOrEmpty(s)) {
				try {
					var converter = TypeDescriptor.GetConverter(typeof(T));
					if (converter != null) {
						value = (T) converter.ConvertFromString(s);
						return true;
					}
				} catch (NotSupportedException) {
					Console.WriteLine($"Config type \"{typeof(T).Name}\" not supported");
				}
			}
			value = default(T);
			return false;
		}

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
