using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EnigmaBot.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnigmaBot.Context {
	public class BotCommandContext : SocketCommandContext, IBotErrorResult {

		private BotServiceBase serviceBase;

		public BotCommandContext(BotServiceBase serviceBase, SocketUserMessage msg)
			: base(serviceBase.Client, msg) {
			this.serviceBase = serviceBase;
			IsSuccess = true;
		}

		public Exception Exception { get; set; }
		public CommandError? Error { get; set; }
		public string ErrorReason { get; set; }
		public bool IsSuccess { get; set; }

		public CommandService Commands => serviceBase.Commands;
		public IServiceProvider Services => serviceBase.Services;
		public IConfigurationRoot Config => serviceBase.Config;
		public HelpService Help => serviceBase.Help;
		public EnigmaService Enigma => serviceBase.Enigma;
		public Random Random => serviceBase.Random;
	}
}
