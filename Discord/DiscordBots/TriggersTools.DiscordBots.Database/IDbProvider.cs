using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Database.Configurations;
using TriggersTools.DiscordBots.Database.Encrypters;

namespace TriggersTools.DiscordBots.Database {
	public interface IDbProvider {
		/// <summary>
		/// Adds the specified database configuration.
		/// </summary>
		/// <param name="configuration">The configuration to add.</param>
		void AddConfiguration(IDbConfiguration configuration);

		/// <summary>
		/// Registers a new <see cref="ValueConverter"/> type.
		/// </summary>
		/// <param name="valueConverterType">The type to register.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="valueEncrypterType"/> is not of type <see cref="ValueConverter{TModel, TProvider}"/>.
		/// Or the model type of the converter has already been registered.
		/// </exception>
		void AddConverter(Type valueConverterType);
		/// <summary>
		/// Registers a new <see cref="IValueEncrypter"/> type.
		/// </summary>
		/// <param name="valueEncrypterType">The type to register.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="valueEncrypterType"/> is not of type <see cref="ValueEncrypter{TModel, TProvider}"/>.
		/// Or the model type of the encrypter has already been registered.
		/// </exception>
		void AddEncrypter(Type valueEncrypterType);

		/// <summary>
		/// Configures setup of the model. <see cref="DbContext.OnModelCreating(ModelBuilder)"/>.
		/// </summary>
		/// <param name="modelBuilder">The model builder for the context.</param>
		/// <param name="db">The database model being created.</param>
		void ModelCreating(ModelBuilder modelBuilder, DbContextEx db);

		/// <summary>
		/// Configures the database connection.
		/// </summary>
		/// <typeparam name="TContext">The <see cref="BotDbContext"/> type to configure.</typeparam>
		/// <param name="optionsBuilder">
		/// The options builder from <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
		/// </param>
		/// <param name="db">The database being configured.</param>
		void Configure(DbContextOptionsBuilder optionsBuilder, DbContextEx db);
	}

	/// <summary>
	/// Extension methods for the see <see cref="IDbProvider"/> interface.
	/// </summary>
	public static class DbProviderExtensions {
		/// <summary>
		/// Registers a new <see cref="ValueConverter"/> type.
		/// </summary>
		/// <typeparam name="TConveter">The type to register.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <typeparamref name="TConverter"/> is not of type <see cref="ValueConverter{TModel, TProvider}"/>.
		/// Or the model type of the converter has already been registered.
		/// </exception>
		public static void AddConverter<TConveter>(this IDbProvider dbProvider)
			where TConveter : ValueConverter
		{
			dbProvider.AddConverter(typeof(TConveter));
		}
		/// <summary>
		/// Registers a new <see cref="IValueEncrypter"/> type.
		/// </summary>
		/// <typeparam name="TEncrypter">The type to register.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <typeparamref name="TEncrypter"/> is not of type <see cref="ValueEncrypter{TModel, TProvider}"/>.
		/// Or the model type of the encrypter has already been registered.
		/// </exception>
		public static void AddEncrypter<TEncrypter>(this IDbProvider dbProvider)
			where TEncrypter : IValueEncrypter
		{
			dbProvider.AddEncrypter(typeof(TEncrypter));
		}
	}
}
