using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Database.Converters {
	/// <summary>
	/// An encryption value converter for <see cref="StringList"/> to a string.
	/// </summary>
	public class StringListValueConverter : ValueConverter<StringList, string> {

		public StringListValueConverter()
			: base(v => Serialize(v), v => Deserialize(v)) { }
		
		private static string Serialize(StringList v) {
			return v?.Text;
		}
		private static StringList Deserialize(string v) {
			return new StringList(v);
		}
	}
}
