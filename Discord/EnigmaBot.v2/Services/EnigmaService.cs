using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EnigmaBot.Reactions;
using TriggersTools.DiscordBots;
using TriggersTools.DiscordBots.Services;
using TriggersTools.DiscordBots.Utils;
using WJLCS.Enigma;
using WJLCS.Enigma.IO;

namespace EnigmaBot.Services {
	public class EnigmaService : DiscordBotService {
		
		private readonly ConfigParserService configParser;
		private const string LetterSetFile = "Resources/Letterset.txt";
		private const string PlugboardFile = "Resources/Plugboard.txt";
		private const string RotorKeysFile = "Resources/RotorKeys.txt";
		private const string HtmlTemplateFile = "Resources/HtmlTemplate.html";
		private readonly LetterSet letterSet;
		private readonly Steckering steckering;
		private readonly RotorKeys rotorKeys;

		public EnigmaService(DiscordBotServiceContainer services,
							 ConfigParserService configParser) : base(services)
		{
			letterSet = LetterSetIO.Read(LetterSetFile);
			steckering = PlugboardIO.Read(letterSet.Count, PlugboardFile);
			rotorKeys = RotorIO.Read(RotorKeysFile);
			this.configParser = configParser;
			Client.ReactionAdded += OnReactionAddedAsync;
		}

		private RotorKeys ReadRotorKeys(ref string content) {
			List<CodeBlock> blocks = content.GetAllCodeBlocks();
			int left = content.IndexOfUnescaped(blocks, '{');
			int right = -1;
			if (left != -1) {
				right = content.IndexOfUnescaped(blocks, '}', left + 1);
				if (left < right) { // Implicit rightIndex != -1
					string keys = content.Substring(left + 1, right - 1 - left).Trim();
					if (!string.IsNullOrWhiteSpace(keys)) {
						content = content.Substring(right + 1).Trim();
						return ParseRotorKeys(keys);
					}
				}
			}
			throw new Exception("No rotor keys were found!");
		}
		private RotorKeys ParseRotorKeys(string s) {
			string[] keysStr = s.Split(new[] { ' ', '\t', '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries);
			int[] keys = new int[keysStr.Length];
			for (int i = 0; i < keys.Length; i++) {
				if (!int.TryParse(keysStr[i], out keys[i]))
					throw new Exception($"{keysStr[i]} is not an integer!");
			}
			try {
				return new RotorKeys(keys, false);
			} catch (Exception ex) {
				throw new Exception($"Failed to parse rotor keys: {ex.Message.Split('\n').First()}");
			}
		}

		public async Task EncipherAsync(ICommandContext context, string content, bool parseKeys) {
			var msg = context.Message;
			RotorKeys rotorKeys = (parseKeys ? ReadRotorKeys(ref content) : this.rotorKeys);
			Machine machine = new Machine(new SetupArgs {
				LetterSet = letterSet,
				Steckering = steckering,
				RotorKeys = rotorKeys,
			});
			if (content.Length > 1024)
				content = $"{content.Substring(0, 1021)}...";
			else if (content.Length == 0)
				throw new Exception("No content was specified!");
			string deciphered = content;
			string enciphered = machine.Encipher(content);
			/*if (msg.Attachments.Any()) {
				string htmlUrl = msg.Attachments.First().Url;
				using (HttpClient client = new HttpClient()) {
					string html = await client.GetStringAsync(htmlUrl).ConfigureAwait(false);
				}
			}*/
			EmbedBuilder embed = new EmbedBuilder {
				Color = configParser.EmbedColor,
				Title = EncipheredTitle,
				Timestamp = DateTime.UtcNow,
				Description = Format.Sanitize(enciphered),
			};
			embed.WithAuthorName(context.User, context.Guild);
			embed.AddField(RotorKeysTitle, string.Join(" ", rotorKeys));
			using (MemoryStream stream = new MemoryStream())
			using (StreamWriter writer = new StreamWriter(stream)) {
				string html = HtmlIO.WriteText(enciphered, HtmlTemplateFile);
				await writer.WriteAsync(html).ConfigureAwait(false);
				await writer.FlushAsync().ConfigureAwait(false);
				stream.Position = 0;
				await context.Channel.SendFileAsync(stream, "Message.html").ConfigureAwait(false);
				var message = await context.Channel.SendMessageAsync(embed: embed.Build()).ConfigureAwait(false);
				await message.AddReactionAsync(EnigmaReactions.ViewMessage).ConfigureAwait(false);
			}
		}

		public async Task DecipherAsync(ICommandContext context, string content, bool parseKeys) {
			var msg = context.Message;
			RotorKeys rotorKeys = (parseKeys ? ReadRotorKeys(ref content) : this.rotorKeys);
			Machine machine = new Machine(new SetupArgs {
				LetterSet = letterSet,
				Steckering = steckering,
				RotorKeys = rotorKeys,
			});
			if (content.Length > 1024)
				content = $"{content.Substring(0, 1021)}...";
			string enciphered = content;
			if (msg.Attachments.Any()) {
				string htmlUrl = msg.Attachments.First().Url;
				using (HttpClient client = new HttpClient()) {
					string html = await client.GetStringAsync(htmlUrl).ConfigureAwait(false);
					enciphered = HtmlIO.ReadText(html);
				}
			}
			else if (content.Length == 0) {
				throw new Exception("No content was specified!");
			}
			string deciphered = machine.Decipher(enciphered);
			EmbedBuilder embed = new EmbedBuilder {
				Color = configParser.EmbedColor,
				Title = DecipheredTitle,
				Timestamp = DateTime.UtcNow,
				Description = Format.Sanitize(deciphered),
			};
			embed.WithAuthorName(context.User, context.Guild);
			embed.AddField(RotorKeysTitle, string.Join(" ", rotorKeys));
			await context.Channel.SendMessageAsync(embed: embed.Build()).ConfigureAwait(false);
		}


		private const string EncipheredTitle = "Enciphered Message";
		private const string DecipheredTitle = "Deciphered Message";
		private const string RotorKeysTitle = "Rotor Keys";
		private async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3) {
			var msg = await arg1.DownloadAsync().ConfigureAwait(false);
			var user = arg3.User.Value;
			var emote = arg3.Emote;

			if (EnigmaReactions.ViewMessage.Equals(emote) && !user.IsBot) {
				if (msg.Author.Id == Client.CurrentUser.Id && msg.Embeds.Any()) {
					IEmbed encipheredEmbed = msg.Embeds.First();
					if (encipheredEmbed.Title == EncipheredTitle) {
						EmbedField field = encipheredEmbed.Fields.FirstOrDefault();
						RotorKeys rotorKeys = this.rotorKeys;
						if (encipheredEmbed.Fields.Any() && field.Name == RotorKeysTitle)
							rotorKeys = ParseRotorKeys(field.Value);
						Machine machine = new Machine(new SetupArgs {
							LetterSet = letterSet,
							Steckering = steckering,
							RotorKeys = rotorKeys,
						});
						string enciphered = Desanitize(encipheredEmbed.Description);
						string deciphered = machine.Decipher(enciphered);

						var author = encipheredEmbed.Author.Value;
						EmbedBuilder embed = new EmbedBuilder {
							Title = DecipheredTitle,
							Color = configParser.EmbedColor,
							Timestamp = encipheredEmbed.Timestamp.Value,
							Description = deciphered,
						};
						embed.WithAuthor(author.Name, author.IconUrl, author.Url);
						var dm = await arg3.User.Value.GetOrCreateDMChannelAsync().ConfigureAwait(false);
						await dm.SendMessageAsync(embed: embed.Build()).ConfigureAwait(false);
					}
				}
			}
		}

		public async Task PostFilesAsync(ICommandContext context) {
			await context.Channel.SendFileAsync(LetterSetFile, "**Letterset Configuration:**").ConfigureAwait(false);
			await context.Channel.SendFileAsync(PlugboardFile, "**Plugboard Configuration:**").ConfigureAwait(false);
			await context.Channel.SendFileAsync(RotorKeysFile, "**Default Rotor Keys:** *(Optional)*").ConfigureAwait(false);
		}
		
		public Task RandomKeysAsync(ICommandContext context, int count = 3) {
			return context.Channel.SendMessageAsync("{" + string.Join(" ", RotorIO.Generate(count)) + "}");
		}

		// Characters which need escaping
		private static readonly string[] SensitiveCharacters = { @"\", "*", "_", "~", "`" };

		/// <summary> Sanitizes the string, safely escaping any Markdown sequences. </summary>
		public static string Desanitize(string text) {
			foreach (string unsafeChar in SensitiveCharacters) {
				text = text.Replace(@"\" + unsafeChar, unsafeChar);
			}
			return text;
		}
	}
}
