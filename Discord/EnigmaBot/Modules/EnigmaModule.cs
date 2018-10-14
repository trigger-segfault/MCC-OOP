using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using EnigmaBot.Info;
using EnigmaMachine;

namespace EnigmaBot.Modules {
	[Name("Enigma Machine")]
	public class EnigmaModule : BotModuleBase {
		
		[Command("encipher")]
		[Alias("enc", "e")]
		[Parameters("<message...>")]
		[Example("Hello World!")]
		[Summary("Encipher text and post a decipherable message that sends a DM when the user reacts")]
		public async Task Post([Remainder] string text) {
			var msg = await ReplyAsync(null, false, Enigma.Post(text, Context.User));
			await msg.AddReactionAsync(BotReactions.ViewMessage);
		}
		
		[Command("sanitize")]
		[Alias("san", "s")]
		[Parameters("<text...>")]
		[Example("Sanitize **Hello**")]
		[Summary("Sanitizes the text so that it appears without formatting in Discord.")]
		public Task Sanitize([Remainder] string text) {
			return ReplyAsync(Format.Sanitize(text));
		}
	}
}
