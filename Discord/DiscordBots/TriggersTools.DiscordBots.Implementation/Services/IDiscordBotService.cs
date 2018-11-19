using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace TriggersTools.DiscordBots.Services {
	public interface IDiscordBotService {
		
		#region Properties

		/// <summary>
		/// Gets the bot services provider.
		/// </summary>
		//DiscordBotServiceContainer Services { get; }

		#endregion

		#region Initialize

		/// <summary>
		/// Initializes the Discord Service after all other services have been setup.
		/// </summary>
		void Initialize();

		#endregion
	}
}
