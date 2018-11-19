using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TriggersTools.DiscordBots.Modules;
using TriggersTools.DiscordBots.Services;
//using EnigmaBot.Services;
using TriggersTools.DiscordBots.Utils;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Reactions;
using TriggersTools.DiscordBots.Extensions;
using EnigmaBot.Commands;
using System.IO;
using EnigmaBot.Database;
using Microsoft.EntityFrameworkCore;
using TriggersTools.DiscordBots;
using EnigmaBot.Services;
using WJLCS.Enigma.IO;

namespace EnigmaBot.Modules {
	[Name("Enigma Machine")]
	[AllowBots(false)]
	[IsLockable(false)]
	public class EnigmaModule : DiscordBotModule {
		
		private readonly EnigmaService enigma;
		private readonly ConfigParserService configParser;

		public EnigmaModule(DiscordBotServiceContainer services,
							EnigmaService enigma,
							ConfigParserService configParser)
			: base(services)
		{
			this.enigma = enigma;
			this.configParser = configParser;
		}

		[Name("encipher")]
		[Command("encipher"), Alias("e")]
		[Usage("<text...>")]
		[Summary("Enciphers the text and outputs a decipherable message and enciphered HTML file")]
		[Example("Hello World", @"Enciphers *Hello World* to the output text: */t?@zB}2p\`-*")]
		public async Task<RuntimeResult> Encipher([Remainder] string text) {
			try {
				await enigma.EncipherAsync(Context, text, false).ConfigureAwait(false);
				return NormalResult.FromSuccess();
			} catch (Exception ex) {
				await ReplyAsync($"**Error:** {ex.Message}").ConfigureAwait(false);
				return EmoteResults.FromInvalidArgument();
			}
		}

		[Name("encipherkeys")]
		[Command("encipherkeys"), Alias("encipherk", "ek")]
		[Usage("<{rotorKeys}> <text...>")]
		[Summary("Enciphers the text with the rotor keys and outputs a decipherable message and enciphered HTML file")]
		[Example("{587 673 11} Hello World", @"Enciphers *Hello World* using the rotor keys [587,673,11] to the output text: */t?@zB}2p\`-*")]
		public async Task<RuntimeResult> EncipherKeys([Remainder] string text) {
			try {
				await enigma.EncipherAsync(Context, text, true).ConfigureAwait(false);
				return NormalResult.FromSuccess();
			} catch (Exception ex) {
				await ReplyAsync($"**Error:** {ex.Message}").ConfigureAwait(false);
				return EmoteResults.FromInvalidArgument();
			}
		}
		[Name("decipher")]
		[Command("decipher"), Alias("d")]
		[Usage("<text...>|<htmlAttachment>")]
		[Summary("Deciphers the text or html attachment")]
		[Example(@"/t?@zB}2p\`-", @"Deciphers */t?@zB}2p\`-* to the output text: *Hello World*")]
		[Example("HTML attachment", "Deciphers the message within the HTML file")]
		public async Task<RuntimeResult> Decipher([Remainder] string text = null) {
			try {
				if (text == null && !Context.Message.Attachments.Any())
					return EmoteResults.FromInvalidArgument();
				await enigma.DecipherAsync(Context, text, false).ConfigureAwait(false);
				return NormalResult.FromSuccess();
			} catch (Exception ex) {
				await ReplyAsync($"**Error:** {ex.Message}").ConfigureAwait(false);
				return EmoteResults.FromInvalidArgument();
			}
		}
		[Name("decipherkeys")]
		[Command("decipherkeys"), Alias("decipherk", "dk")]
		[Usage("<{rotorKeys}> <text...>|<htmlAttachment>")]
		[Summary("Deciphers the text or html attachment with the rotor keys")]
		[Example(@"{587 673 11} /t?@zB}2p\`-", @"Deciphers */t?@zB}2p\`-* using the rotor keys [587,673,11] to the output text: *Hello World*")]
		[Example("{587 673 11} HTML attachment", @"Deciphers message within the HTML file using the rotor keys [587,673,11]")]
		public async Task<RuntimeResult> DecipherKeys([Remainder] string text) {
			try {
				if (text == null && !Context.Message.Attachments.Any())
					return EmoteResults.FromInvalidArgument();
				await enigma.DecipherAsync(Context, text, true).ConfigureAwait(false);
				return NormalResult.FromSuccess();
			} catch (Exception ex) {
				await ReplyAsync($"**Error:** {ex.Message}").ConfigureAwait(false);
				return EmoteResults.FromInvalidArgument();
			}
		}
		[Name("files")]
		[Command("files"), Alias("f")]
		[Summary("Posts the required Letterset and Plugboard files")]
		public Task PostFiles() {
			return enigma.PostFilesAsync(Context);
		}

		[Name("sanitize <text>")]
		[Command("sanitize"), Alias("s")]
		[Usage("<text...>")]
		[Example("**Hello**", @"Ouputs the text as \*\*Hello\*\*")]
		[Summary("Sanitizes the text so that it appears without formatting in Discord")]
		public Task Sanitize([Remainder] string text) {
			return ReplyAsync(Format.Sanitize(text));
		}

		[Name("rotorkeys [rotorCount]")]
		[Command("rotorkeys"), Alias("randomkeys", "keys")]
		[Usage("[rotorCount]")]
		[Example(@"Outputs 3 random prime number rotor keys")]
		[Example("100", @"Outputs 100 random prime number rotor keys")]
		[Summary("Outputs the specified number of random rotor prime number keys")]
		public async Task<RuntimeResult> RandomKeys(int rotorCount = 3) {
			if (rotorCount < 1)
				return EmoteResults.FromInvalidArgument();
			await enigma.RandomKeysAsync(Context, rotorCount).ConfigureAwait(false);
			return NormalResult.FromSuccess();
		}
	}
}
