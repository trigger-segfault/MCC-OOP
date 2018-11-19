using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Commands;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots.Modules {
	/// <summary>
	/// The basic module for all Discord bot commands.
	/// </summary>
	public class DiscordBotBasicModule : ModuleBaseEx<DiscordBotCommandContext> {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="DiscordBotBasicModule"/>.
		/// </summary>
		public DiscordBotBasicModule() { }

		#endregion
	}
}
