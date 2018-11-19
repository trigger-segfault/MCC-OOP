using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Database;
using TriggersTools.DiscordBots.Services;

namespace EnigmaBot.Database {
	/// <summary>
	/// The design-time factory for <see cref="TriggerDbContext"/>.
	/// </summary>
	public class EnigmaDbContextFactory : DbContextExFactory<EnigmaDbProvider, EnigmaDbContext> {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="EnigmaDbContextFactory"/>.
		/// </summary>
		public EnigmaDbContextFactory() : base(new EnigmaMachineBot().LoadConfig()) { }

		#endregion

		#region Virtual Methods
		
		/// <summary>
		/// Configures additional required services.
		/// </summary>
		/// <param name="services">The service collection to add to.</param>
		/// <returns>The same server collection that was passed.</returns>
		protected override IServiceCollection ConfigureServices(IServiceCollection services) {
			return base.ConfigureServices(services)
				.AddEntityFrameworkNpgsql();
		}

		#endregion
	}
}
