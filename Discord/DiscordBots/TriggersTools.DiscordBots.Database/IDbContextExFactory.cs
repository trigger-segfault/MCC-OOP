using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// The interface for design-time factory for <see cref="DbContextEx"/>s.
	/// </summary>
	public interface IDbContextExFactory {
		/// <summary>
		/// The custom configuration type. Or null.
		/// </summary>
		string ConfigurationType { get; }
		
		/// <summary>
		/// Creates a new instance of a derived database context.
		/// </summary>
		/// <param name="args">Arguments provided by the design-time service.</param>
		/// <returns>An instance of <typeparamref name="TDbContext"/>.</returns>
		DbContextEx CreateDbContext(string[] args = null);
	}
}
