using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnigmaBot.Services {
	public abstract class BotServiceBase {

		public bool IsInitialized { get; private set; }

		public ServiceProvider Services { get; private set; }
		public DiscordSocketClient Client { get; private set; }
		public CommandService Commands { get; private set; }
		public IConfigurationRoot Config { get; private set; }
		public EnigmaService Enigma { get; private set; }
		public HelpService Help { get; private set; }
		public Random Random { get; private set; }

		public void Initialize(ServiceProvider services) {
			if (!IsInitialized) {
				Services = services;
				Client = services.GetService<DiscordSocketClient>();
				Commands = services.GetService<CommandService>();
				Config = services.GetService<IConfigurationRoot>();
				Enigma = services.GetService<EnigmaService>();
				Help = services.GetService<HelpService>();
				Random = services.GetService<Random>();
				OnInitialized(services);
				IsInitialized = true;
			}
		}

		protected virtual void OnInitialized(ServiceProvider services) { }

	}
}
