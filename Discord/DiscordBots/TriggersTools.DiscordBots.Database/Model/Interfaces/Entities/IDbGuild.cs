using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base database model for a guild.
	/// </summary>
	public interface IDbGuild
		: IDbSnowflake, IDbEndUserGuildData, IDbCommandContext { }
}
