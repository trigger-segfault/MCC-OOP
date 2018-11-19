using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriggersTools.DiscordBots.Database.Configurations;
using TriggersTools.DiscordBots.Database.Converters;
using TriggersTools.DiscordBots.Database.Encrypters;
using TriggersTools.DiscordBots.Database.Model;
using TriggersTools.DiscordBots.Extensions;

namespace TriggersTools.DiscordBots.Database {
	public abstract class DbProvider : IDbProvider {
		
		#region Private Classes

		private class EntityInfo {
			/// <summary>
			/// Gets the type of the entity.
			/// </summary>
			public Type Type { get; }
			/// <summary>
			/// Gets the optional <see cref="IDbModelCreating.ModelCreating(ModelBuilder, DbContextEx)"/> function.
			/// </summary>
			public Action<ModelBuilder, DbContextEx> ModelCreating { get; }
			/// <summary>
			/// Gets the properties using converters.
			/// </summary>
			public PropertyConverterInfo[] ConverterProperties { get; }
			/// <summary>
			/// Gets the properties using encrypters.
			/// </summary>
			public PropertyEncrypterInfo[] EncrypterProperties { get; }

			public EntityInfo(Type type, PropertyConverterInfo[] converterProperties,
				PropertyEncrypterInfo[] encrypterProperties)
			{
				Type = type;
				if (typeof(IDbModelCreating).IsAssignableFrom(type))
					ModelCreating = ((IDbModelCreating) Activator.CreateInstance(type)).ModelCreating;
				ConverterProperties = converterProperties;
				EncrypterProperties = encrypterProperties;
			}
		}
		private class PropertyConverterInfoBase {
			/// <summary>
			/// The name of the property.
			/// </summary>
			public string Name { get; }
			/// <summary>
			/// The type of the property.
			/// </summary>
			public Type Type { get; }


			public PropertyConverterInfoBase(PropertyInfo prop) {
				Name = prop.Name;
				Type = prop.PropertyType;
			}
		}
		private class PropertyConverterInfo : PropertyConverterInfoBase {
			/// <summary>
			/// The value converter for the property.
			/// </summary>
			public ValueConverter Converter { get; set; }


			public PropertyConverterInfo(PropertyInfo prop, ValueConverter converter) : base(prop) {
				Converter = converter;
			}
		}
		private class PropertyEncrypterInfo : PropertyConverterInfoBase {
			/// <summary>
			/// The value converter for the property.
			/// </summary>
			public ValueEncrypters Encrypters { get; set; }
			/// <summary>
			/// Gets the default encrypt direction for the property.
			/// </summary>
			public EncryptDirection Direction { get; set; }
			
			public PropertyEncrypterInfo(PropertyInfo prop, ValueEncrypters encrypters,
				EncryptDirection direction)
				: base(prop)
			{
				Encrypters = encrypters;
				Direction = direction;
			}
		}

		private class ValueEncrypters {
			/// <summary>
			/// The four possible encryption directions as a value converter.
			/// </summary>
			private readonly ValueConverter[] encrypterDirections;

			/// <summary>
			/// Constructs the <see cref="ValueEncrypter{TModel, TProvider}"/> directions.
			/// </summary>
			/// <param name="type">The type of the value encrypter.</param>
			/// <param name="encrypter">The byte encrypter.</param>
			public ValueEncrypters(Type type, IByteEncrypter encrypter) {
				encrypterDirections = new ValueConverter[4];
				for (int i = 0; i < 4; i++) {
					encrypterDirections[i] = (ValueConverter)
						Activator.CreateInstance(type, encrypter, (EncryptDirection) i);
				}
			}
			
			/// <summary>
			/// Gets the encrypter for the specified encrypt direction.
			/// </summary>
			/// <param name="direction">The direction to get the encrypter for.</param>
			public ValueConverter this[EncryptDirection direction] {
				get => encrypterDirections[(int) direction];
			}
		}

		#endregion

		#region Fields

		/// <summary>
		/// The dictionary of configurations mapped to their names.
		/// </summary>
		private readonly Dictionary<string, IDbConfiguration> configurations = new Dictionary<string, IDbConfiguration>();
		/// <summary>
		/// The dictionary of setup information on database type entities. The Key is the
		/// <see cref="DbContextEx"/> type.
		/// </summary>
		private Dictionary<Type, EntityInfo[]> entityInfos = new Dictionary<Type, EntityInfo[]>();
		/// <summary>
		/// The default converters. The Type key represents the model type to convert to/from.
		/// </summary>
		private readonly Dictionary<Type, ValueConverter> converterDefaults = new Dictionary<Type, ValueConverter>();
		/// <summary>
		/// All mapped converters. The Type key represents the type of the converter, when used with
		/// <see cref="ConverterAttribute"/>.
		/// </summary>
		private readonly Dictionary<Type, ValueConverter> converterMap = new Dictionary<Type, ValueConverter>();
		/// <summary>
		/// The default encrypters. The Type key represents the model type to convert to/from.
		/// </summary>
		private readonly Dictionary<Type, ValueEncrypters> encrypterDefaults = new Dictionary<Type, ValueEncrypters>();
		/// <summary>
		/// All mapped encrypters. The Type key represents the type of the encrypter, when used with
		/// <see cref="EncryptedAttribute"/>.
		/// </summary>
		private readonly Dictionary<Type, ValueEncrypters> encrypterMap = new Dictionary<Type, ValueEncrypters>();
		/// <summary>
		/// The configuration information file.
		/// </summary>
		private readonly IConfigurationRoot config;

		/// <summary>
		/// The byte encrypter interface.
		/// </summary>
		private readonly IByteEncrypter encrypter;

		#endregion

		#region Constructors

		public DbProvider(IConfigurationRoot config) {
			this.config = config;
			encrypter = new ByteEncrypter(config["database:encryption"]);
			AddConfigurations();
			AddConverters();
		}

		#endregion
		
		#region Protected Virtual Methods

		/// <summary>
		/// Called to add the <see cref="IDbConfiguration"/> types to the provider.
		/// </summary>
		protected abstract void AddConfigurations();

		/// <summary>
		/// Called to add the <see cref="ValueConverter{TModel, TProvider}"/> and
		/// <see cref="ValueEncrypter{TModel, TProvider}"/> types.
		/// </summary>
		protected virtual void AddConverters() {
			// Converters
			AddConverter<DiscordColorValueConverter>();
			AddConverter<DiscordColorNullableValueConverter>();
			AddConverter<SnowflakeSetValueConverter>();
			AddConverter<StringListValueConverter>();
			AddConverter<StringSetValueConverter>();

			// Encrypters
			AddEncrypter<BinaryValueEncrypter>();
			AddEncrypter<DateTimeValueEncrypter>();
			AddEncrypter<DateTimeNullableValueEncrypter>();
			AddEncrypter<SnowflakeSetValueEncrypter>();
			AddEncrypter<StringListValueEncrypter>();
			AddEncrypter<StringSetValueEncrypter>();
			AddEncrypter<StringValueEncrypter>();
			AddEncrypter<TimeZoneInfoValueEncrypter>();
			AddEncrypter<TimeSpanValueEncrypter>();
			AddEncrypter<TimeSpanNullableValueEncrypter>();
		}

		#endregion

		#region Add

		/// <summary>
		/// Adds the specified database configuration.
		/// </summary>
		/// <param name="configuration">The configuration to add.</param>
		public void AddConfiguration(IDbConfiguration configuration) {
			configurations.Add(configuration.ConfigurationType, configuration);
		}

		/// <summary>
		/// Registers a new <see cref="ValueConverter"/> type.
		/// </summary>
		/// <param name="valueConverterType">The type to register.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="valueEncrypterType"/> is not of type <see cref="ValueConverter{TModel, TProvider}"/>.
		/// Or the model type of the converter has already been registered.
		/// </exception>
		public void AddConverter(Type valueConverterType) {
			Type type = valueConverterType;
			Type modelType = GetConverterModelType(type);
			ValueConverter converter =  (ValueConverter) Activator.CreateInstance(type);
			converterDefaults.Add(modelType, converter);
			converterMap.Add(type, converter);
		}
		/// <summary>
		/// Registers a new <see cref="ValueConverter{TModel, TProvider}"/> type.
		/// </summary>
		/// <typeparam name="TConverter">The type to register.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <typeparamref name="TConverter"/> is not of type <see cref="ValueConverter{TModel, TProvider}"/>.
		/// Or the model type of the converter has already been registered.
		/// </exception>
		public void AddConverter<TConverter>()
			where TConverter : ValueConverter, new()
		{
			AddConverter(typeof(TConverter));
		}
		/// <summary>
		/// Registers a new <see cref="IValueEncrypter"/> type.
		/// </summary>
		/// <param name="valueEncrypterType">The type to register.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="valueEncrypterType"/> is not of type <see cref="ValueEncrypter{TModel, TProvider}"/>.
		/// Or the model type of the encrypter has already been registered.
		/// </exception>
		public void AddEncrypter(Type valueEncrypterType) {
			Type type = valueEncrypterType;
			Type modelType = GetConverterModelType(type);
			ValueEncrypters encrypters = new ValueEncrypters(type, encrypter);
			encrypterDefaults.Add(modelType, encrypters);
			encrypterMap.Add(type, encrypters);
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
		public void AddEncrypter<TEncrypter>()
			where TEncrypter : IValueEncrypter
		{
			AddEncrypter(typeof(TEncrypter));
		}

		#endregion

		#region DbContext Setup

		/// <summary>
		/// Configures setup of the model. <see cref="DbContext.OnModelCreating(ModelBuilder)"/>.
		/// </summary>
		/// <param name="modelBuilder">The model builder for the context.</param>
		/// <param name="db">The database model being created.</param>
		public virtual void ModelCreating(ModelBuilder modelBuilder, DbContextEx db) {
			ApplyAttributes(modelBuilder, db);
		}
		/// <summary>
		/// Configures the database connection.
		/// </summary>
		/// <typeparam name="TContext">The <see cref="BotDbContext"/> type to configure.</typeparam>
		/// <param name="optionsBuilder">
		/// The options builder from <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
		/// </param>
		/// <param name="db">The database being configured.</param>
		public virtual void Configure(DbContextOptionsBuilder optionsBuilder, DbContextEx db) {
			var (section, configuration) = GetSection(db.ContextType, db.ConfigurationType);
			configuration.Configure(optionsBuilder, section);
		}

		#endregion

		#region Private Methods
		
		private (PropertyConverterInfo[], PropertyEncrypterInfo[]) GetConverterInfo(PropertyInfo[] properties) {
			List<PropertyConverterInfo> converterProperties = new List<PropertyConverterInfo>();
			List<PropertyEncrypterInfo> encrypterProperties = new List<PropertyEncrypterInfo>();
			for (int i = 0; i < properties.Length; i++) {
				PropertyInfo prop = properties[i];
				Type propType = prop.PropertyType;
				ValueEncrypters encrypters;
				ValueConverter converter;
				var converterAttr = prop.GetCustomAttribute<ConverterAttribute>();
				if (converterAttr == null && (DbNativeTypes.IsNativeType(propType) ||
					prop.IsDefined<InversePropertyAttribute>() || prop.IsDefined<ForeignKeyAttribute>()))
					continue;
				
				Type converterType = converterAttr?.ConverterType;
				if (converterAttr is EncryptedAttribute encryptedAttr) {
					if (converterType == null) {
						// No encrypted attribute, use default encrypter
						if (!encrypterDefaults.TryGetValue(propType, out encrypters))
							throw new ArgumentException($"Could not find default ValueEncrypter for type {propType.Name}!");
					}
					else if (!encrypterMap.TryGetValue(converterType, out encrypters)) {
						encrypters = new ValueEncrypters(converterType, encrypter);
						encrypterMap.Add(converterType, encrypters);
					}
					encrypterProperties.Add(new PropertyEncrypterInfo(prop, encrypters, encryptedAttr.Direction));
				}
				else {
					if (converterType == null) {
						// No converter attribute, use default converter
						if (converterDefaults.TryGetValue(propType, out converter))
							converterProperties.Add(new PropertyConverterInfo(prop, converter));
						/*if (!propType.IsEnum && !(Nullable.GetUnderlyingType(propType)?.IsEnum ?? false)) {
							if (!converterDefaults.TryGetValue(propType, out converter))
								throw new ArgumentException($"Could not find default ValueConverter for type {propType.Name}!");
						}*/
					}
					else if (!converterMap.TryGetValue(converterType, out converter)) {
						converter = (ValueConverter) Activator.CreateInstance(converterAttr.ConverterType);
						converterMap.Add(converterType, converter);
						converterProperties.Add(new PropertyConverterInfo(prop, converter));
					}
				}
			}
			return (converterProperties.ToArray(), encrypterProperties.ToArray());
		}

		private EntityInfo[] GetEntityInfo(Type dbType) {
			if (!entityInfos.TryGetValue(dbType, out var entityInfo)) {
				var entityProperties = DbReflection.GetEntityTypesAndMappedProperties(dbType);
				entityInfo = new EntityInfo[entityProperties.Count];
				int i = 0;
				foreach (var pair in entityProperties) {
					var (converterProperties, encrypterProperties) = GetConverterInfo(pair.Value);
					entityInfo[i] = new EntityInfo(pair.Key, converterProperties, encrypterProperties);
					i++;
				}
				entityInfos.Add(dbType, entityInfo);
			}
			return entityInfo;
		}

		private void ApplyAttributes(ModelBuilder modelBuilder, DbContextEx db) {
			EntityInfo[] entityInfos = GetEntityInfo(db.GetType());
			EncryptDirection direction = (!db.DisableEncryption ? EncryptDirection.Both : EncryptDirection.None);

			for (int i = 0; i < entityInfos.Length; i++) {
				EntityInfo entityInfo = entityInfos[i];
				var entity = modelBuilder.Entity(entityInfo.Type);

				entityInfo.ModelCreating?.Invoke(modelBuilder, db);

				PropertyConverterInfo[] converterProps = entityInfo.ConverterProperties;
				for (int j = 0; j < converterProps.Length; j++) {
					PropertyConverterInfo prop = converterProps[j];
					entity
						.Property(prop.Type, prop.Name)
						.HasConversion(prop.Converter);
				}
				PropertyEncrypterInfo[] encrypterProps = entityInfo.EncrypterProperties;
				for (int j = 0; j < encrypterProps.Length; j++) {
					PropertyEncrypterInfo prop = encrypterProps[j];
					entity
						.Property(prop.Type, prop.Name)
						.HasConversion(prop.Encrypters[prop.Direction | direction]);
				}
			}
		}
		
		private Type GetConverterModelType(Type converterType) {
			Type type = converterType;
			Type modelType = null;
			while (type != null) {
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueConverter<,>))
					modelType = type.GetGenericArguments()[0];
				type = type.BaseType;
			}
			if (modelType == null)
				throw new ArgumentException($"{type.Name} does not inherit from ValueConverter<TModel,TProvider>!");
			return modelType;
		}
		/// <summary>
		/// Gets the configuration and section with the specified context and name.
		/// </summary>
		/// <param name="contextType">The configuration context type.</param>
		/// <param name="configurationName">The configuration name inside the context.</param>
		/// <returns>The configuration section and interface.</returns>
		private (IConfigurationSection section, IDbConfiguration configuration) GetSection(
			string contextType, string configurationName) {
			IConfigurationSection contextSection = config.GetSection($"database:{contextType}");
			if (configurationName == null)
				configurationName = contextSection["configuration"];
			IConfigurationSection configurationSection = contextSection.GetSection(configurationName);
			string configurationType = configurationSection["type"];
			return (configurationSection, configurations[configurationType]);
		}

		#endregion
	}
}
