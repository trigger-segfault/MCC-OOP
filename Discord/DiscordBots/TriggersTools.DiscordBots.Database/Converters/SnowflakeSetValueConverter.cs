using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Database.Converters {
	/// <summary>
	/// A value converter for <see cref="SnowflakeSet"/> to a string.
	/// </summary>
	public class SnowflakeSetValueConverter : ValueConverter<SnowflakeSet, string> {

		public SnowflakeSetValueConverter()
			: base(v => Serialize(v), v => Deserialize(v)) { }
		
		private static string Serialize(SnowflakeSet v) {
			return v?.Text;
		}
		private static SnowflakeSet Deserialize(string v) {
			return new SnowflakeSet(v);
		}
	}
}
