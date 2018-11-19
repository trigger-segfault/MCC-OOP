using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Database.Converters {
	/// <summary>
	/// A value converter for <see cref="StringSet"/> to a string.
	/// </summary>
	public class StringSetValueConverter : ValueConverter<StringSet, string> {

		public StringSetValueConverter()
			: base(v => Serialize(v), v => Deserialize(v)) { }
		
		private static string Serialize(StringSet v) {
			return v?.Text;
		}
		private static StringSet Deserialize(string v) {
			return new StringSet(v);
		}
	}
}
