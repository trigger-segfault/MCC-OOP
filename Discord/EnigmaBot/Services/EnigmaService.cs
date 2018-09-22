using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using EnigmaBot.Info;
using EnigmaMachine;
using Microsoft.Extensions.DependencyInjection;

namespace EnigmaBot.Services {
	public class EnigmaService : BotServiceBase {

		private readonly Dictionary<DateTime, Machine> machines = new Dictionary<DateTime, Machine>();
		private readonly SetupArgs setup;
		public Machine GetMachine(DateTime dateTime) {
			DateTime date = dateTime.ToUniversalTime().Date;
			if (!machines.TryGetValue(date, out Machine machine)) {
				setup.Steckering = setup.LetterSet.RandomizeSteckering(date.GetHashCode());
				machine = new Machine(setup);
				machines.Add(date, machine);
			}
			return machine;
		}
		
		private readonly object machineLock = new object();

		public EnigmaService() {
			char[] letters = new char[127 - 32 + 1];
			for (int i = 0; i < 127 - 32; i++) {
				letters[i] = (char) (i + 32);
			}
			letters[letters.Length - 1] = '\n';
			LetterSet letterSet = new LetterSet(letters);
			setup = new SetupArgs(letterSet) {
				Steckering = letterSet.RandomizeSteckering(),
				InvalidCharacter = '?',
				ResetAfter = 50,
				RotorCount = 3,
				RotateOnInvalid = true,
				UnmappedHandling = UnmappedHandling.Keep,
			};
		}

		protected override void OnInitialized(ServiceProvider services) {
			Client.ReactionAdded += OnReactionAdded;
		}

		private const string EncipheredTitle = "Enciphered:";
		private const string DecipheredTitle = "Deciphered:";
		private static readonly Color EmbedColor = new Color(255, 255, 0);
		private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3) {
			var msg = await arg1.DownloadAsync();
			var user = arg3.User.Value;
			var emote = arg3.Emote;

			if (BotReactions.ViewMessage.Equals(emote) && !user.IsBot) {
				if (msg.Author.Id == Client.CurrentUser.Id && msg.Embeds.Any()) {
					IEmbed encipheredEmbed = msg.Embeds.First();
					if (encipheredEmbed.Fields.Any()) {
						EmbedField field = encipheredEmbed.Fields[0];
						if (field.Name == EncipheredTitle) {
							DateTime dateTime = encipheredEmbed.Timestamp.Value.DateTime;
							string enciphered = Desanitize(field.Value);
							string deciphered = Decipher(enciphered, dateTime, false);

							var author = encipheredEmbed.Author.Value;
							EmbedBuilder embed = new EmbedBuilder();
							embed.WithColor(EmbedColor);
							embed.WithAuthor(author.Name, author.IconUrl, author.Url);
							embed.WithTimestamp(encipheredEmbed.Timestamp.Value);
							embed.AddField(DecipheredTitle, deciphered);
							var dm = await arg3.User.Value.GetOrCreateDMChannelAsync();
							await dm.SendMessageAsync(null, false, embed.Build());
						}
					}
				}
			}
		}

		public Embed Post(string text, IUser author) {
			DateTime dateTime = DateTime.UtcNow;
			string enciphered = Encipher(text, dateTime, true);
			EmbedBuilder embed = new EmbedBuilder();
			embed.WithColor(EmbedColor);
			embed.WithAuthor(author);
			embed.AddField(EncipheredTitle, enciphered);
			embed.Timestamp = new DateTimeOffset(dateTime);
			return embed.Build();
		}

		public string Encipher(string text, DateTime dateTime, bool sanitize) {
			lock (machineLock) {
				text = GetMachine(dateTime).Encipher(text);
			}
			if (sanitize)
				return Format.Sanitize(text);
			return text;
		}
		public string Decipher(string text, DateTime dateTime, bool sanitize) {
			lock (machineLock) {
				text = GetMachine(dateTime).Decipher(text);
			}
			if (sanitize)
				return Format.Sanitize(text);
			return text;
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
