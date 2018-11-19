using Discord;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TriggersTools.DiscordBots.Database.Converters {
	/// <summary>
	/// A value converter for <see cref="Color"/> to <see cref="uint"/>.
	/// </summary>
	public class DiscordColorValueConverter : ValueConverter<Color, uint> {

		public DiscordColorValueConverter()
			: base(v => Serialize(v), v => Deserialize(v)) { }
		
		private static uint Serialize(Color v) {
			return v.RawValue;
		}
		private static Color Deserialize(uint v) {
			return new Color(v);
		}
	}
	/// <summary>
	/// A value converter for <see cref="Color?"/> to <see cref="uint?"/>.
	/// </summary>
	public class DiscordColorNullableValueConverter : ValueConverter<Color?, uint?> {

		public DiscordColorNullableValueConverter()
			: base(v => Serialize(v), v => Deserialize(v)) { }
		
		private static uint? Serialize(Color? v) {
			return (v.HasValue ? v.Value.RawValue : (uint?) null);
		}
		private static Color? Deserialize(uint? v) {
			return (v.HasValue ? new Color(v.Value) : (Color?) null);
		}
	}
}
