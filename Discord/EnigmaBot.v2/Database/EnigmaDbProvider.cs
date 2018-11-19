using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Database.Configurations;

namespace EnigmaBot.Database {
	public class EnigmaDbProvider : DbProvider {

		#region Constructors
		
		public EnigmaDbProvider(IConfigurationRoot config) : base(config) { }

		#endregion

		#region Override Methods

		protected override void AddConfigurations() {
			AddConfiguration(new SqliteConfiguration());
			AddConfiguration(new NpgsqlConfiguration());
		}

		#endregion
	}
}
