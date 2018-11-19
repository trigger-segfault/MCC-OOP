using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using TriggersTools.Build;
using TriggersTools.DiscordBots.Extensions;
using TriggersTools.DiscordBots.Utils;

namespace TriggersTools.DiscordBots.Services {
	public class ConfigParserService : DiscordBotService {

		#region Constants

		public const string UsernameToken = "USERNAME";
		public const string NicknameToken = "NICKNAME";
		public const string UserIdToken = "USERID";
		public const string OwnerUsernameToken = "OWNED_USERNAME";
		public const string OwnerUserIdToken = "OWNED_USERID";
		public const string VersionToken = "VERSION";
		public const string BuildDateToken = "BUILD_DATE";

		public const string EmbedColorName = "embed";

		#endregion

		#region Fields

		private readonly Dictionary<string, Func<string>> tokens = new Dictionary<string, Func<string>>();
		private readonly Dictionary<string, Func<Color>> colors = new Dictionary<string, Func<Color>>();

		#endregion

		#region Constructors

		public ConfigParserService(DiscordBotServiceContainer services) : base(services) {
			AddToken(UsernameToken, () => Username);
			AddToken(NicknameToken, () => Nickname);
			AddToken(UserIdToken, () => UserId);
			AddToken(OwnerUsernameToken, () => OwnerUsername);
			AddToken(OwnerUserIdToken, () => OwnerUserId);
			AddToken(VersionToken, () => Version);
			AddToken(BuildDateToken, () => BuildDate);
			AddColor(EmbedColorName, () => EmbedColor);
		}

		#endregion


		protected void AddToken(string token, Func<string> getResult) {
			tokens[token] = getResult;
		}
		protected void AddColor(string path, Func<Color> getResult) {
			colors[path] = getResult;
		}

		#region Properties

		public string Username => Client.CurrentUser.Username;
		public string Nickname => Config[NicknamePath] ?? Username;
		public string Version => $"v{GetFileVersion().ProductVersion}{Config["verion_postfix"]}";
		public string UserId => Client.CurrentUser.Id.ToString();
		public string OwnerUsername => GetOwner().Username;
		public string OwnerUserId => GetOwner().Id.ToString();
		public string BuildDate => FormatBuildDate(Assembly.GetEntryAssembly().GetUtcBuildTime());

		public string EmbedPrefix => Config[EmbedPrefixPath];
		public Color EmbedColor => ColorUtils.Parse(Config[EmbedColorPath]);

		#endregion

		#region Protected Helpers

		protected IUser GetOwner() {
			return Client.GetApplicationInfoAsync().GetAwaiter().GetResult().Owner;
		}
		protected string GetDaySuffix(DateTime date) {
			switch (date.Day) {
			case 1:
			case 21:
			case 31:
				return "st";
			case 2:
			case 22:
				return "nd";
			case 3:
			case 23:
				return "rd";
			default:
				return "th";
			}
		}
		protected string Combine(string path, string name) {
			if (string.IsNullOrEmpty(path)) {
				path = name;
			}
			else if (path.EndsWith(":")) {
				if (name.StartsWith(":"))
					path += name.Substring(1);
				else
					path += name;
			}
			else {
				path += ":" + name;
			}
			return path;
		}
		protected FileVersionInfo GetFileVersion() {
			return FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
		}

		#endregion

		#region Protected Virtual Methods

		protected virtual string FormatBuildDate(DateTime date) {
			return $"{date:MMM} {date:%d}{GetDaySuffix(date)}, {date:yyyy}";
		}
		protected virtual string OnResolve(string text) {
			return text;
		}

		#endregion

		#region Protected Virtual Properties

		protected virtual string EmbedPrefixPath => "embed_prefix";
		protected virtual string DescriptionsPath => "";
		protected virtual string NicknamePath => "nickname";
		protected virtual string EmbedColorPath => "embed_color";

		#endregion


		public Color GetColor(string colorName) {
			return colors[colorName]();
		}

		public string Parse(string configPath, string configName, string defaultText = null) {
			return Resolve(Config[Combine(configPath, configName)] ?? defaultText);
		}

		public string Resolve(string text) {
			if (text == null)
				return null;
			foreach (var token in tokens) {
				text = text.Replace($"${token.Key}$", token.Value());
			}
			return OnResolve(text);
		}


		public string ParseTitle(string titleName, string defaultTitle = null) {
			return Parse(DescriptionsPath, titleName + "_title", defaultTitle);
		}
		public string ParseDescription(string descriptionName, string defaultDescription = null) {
			return Parse(DescriptionsPath, descriptionName + "_desc", defaultDescription);
		}
		public string ParseLinks(string linksName) {
			var links = Config.GetSection(Combine(DescriptionsPath, linksName + "_links"));
			List<string> linkList = new List<string>();
			if (links != null) {
				foreach (var child in links.GetChildren()) {
					string name = child["name"];
					string url = child["url"];
					if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(url)) {
						linkList.Add($"[{name}]({url})");
					}
				}
			}
			if (linkList.Any())
				return string.Join(" | ", linkList);
			return null;
		}

	}
}
