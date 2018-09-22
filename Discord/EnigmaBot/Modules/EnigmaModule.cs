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
		
		/*[Command("post", RunMode = RunMode.Async)]
		[Alias("p")]
		[Summary("Post a decipherable message")]*/
		[Command("encipher", RunMode = RunMode.Async)]
		[Alias("enc", "e")]
		[Parameters("<message...>")]
		[Example("Hello World!")]
		[Summary("Encipher text and post a decipherable message that sends a DM when the user reacts")]
		public async Task Post([Remainder] string text) {
			var msg = await ReplyAsync(null, false, Enigma.Post(text, Context.User));
			await msg.AddReactionAsync(BotReactions.ViewMessage);
		}

		/*[Command("encipher", RunMode = RunMode.Async)]
		[Alias("enc", "e")]
		[Summary("Encipher text")]
		public async Task Encipher([Remainder] string text) {
			await ReplyAsync(Enigma.Encipher(text));
		}

		[Command("decipher", RunMode = RunMode.Async)]
		[Alias("dec", "d")]
		[Summary("Decipher text")]
		public async Task Decipher([Remainder] string text) {
			await ReplyAsync(Enigma.Decipher(text));
		}*/
	}
}
