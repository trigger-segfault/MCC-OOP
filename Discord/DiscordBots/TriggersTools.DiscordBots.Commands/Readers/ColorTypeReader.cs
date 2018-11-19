using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TriggersTools.DiscordBots.Commands {
	public class ColorTypeReader : TypeReader {
		public ColorTypeReader() { }

		public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services) {
			bool hash = input.StartsWith("#");
			if (input.Length == (hash ? 7 : 6)) {
				string hexStr = (hash ? input.Substring(1) : input);
				if (uint.TryParse(hexStr, NumberStyles.HexNumber, null, out uint hexColor))
					return Task.FromResult(TypeReaderResult.FromSuccess(new Color(hexColor)));
				return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {nameof(Color)}."));
			}

			string[] rgbStrings = input.Split(',');
			byte[] rgb = new byte[3];
			if (rgbStrings.Length == 3) {
				for (int i = 0; i < 3; i++) {
					if (byte.TryParse(rgbStrings[i].Trim(), out byte channel)) {
						rgb[i] = channel;
						continue;
					}
					return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {nameof(Color)}."));
				}
				return Task.FromResult(TypeReaderResult.FromSuccess(new Color(rgb[0], rgb[1], rgb[2])));
			}
			return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {nameof(Color)}."));
		}
	}
}
