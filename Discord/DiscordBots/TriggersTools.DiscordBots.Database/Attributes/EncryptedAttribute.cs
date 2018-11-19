using System;
using TriggersTools.DiscordBots.Database.Encrypters;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// An attribute stating this value should use a <see cref="ValueEncrypter{TModel, TProvider}"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class EncryptedAttribute : ConverterAttribute {
		/// <summary>
		/// The encryption direction.
		/// </summary>
		public EncryptDirection Direction { get; }

		/// <summary>
		/// Constructs the <see cref="EncryptedAttribute"/> with the optional directional encryption.
		/// Uses the default encrypter for this type.
		/// </summary>
		/// <param name="direction">The encryption direction to use. (For changes during migration)</param>
		public EncryptedAttribute(EncryptDirection direction = EncryptDirection.Both) {
			Direction = direction;
		}
		/// <summary>
		/// Constructs the <see cref="EncryptedAttribute"/> with the optional directional encryption and
		/// customized encrypter type.
		/// </summary>
		/// <param name="direction">The encryption direction to use. (For changes during migration)</param>
		public EncryptedAttribute(Type converterType, EncryptDirection direction = EncryptDirection.Both)
			: base(converterType)
		{
			if (converterType == null)
				throw new ArgumentNullException(nameof(converterType));
			if (typeof(IValueEncrypter).IsAssignableFrom(converterType))
				throw new ArgumentException($"Converter type {converterType.Name} is not a ValueEncrypter!");
			Direction = direction;
		}

/*// Disable Obsolete complaining that the base function does not have the attribute.
#pragma warning disable CS0809
		/// <summary>
		/// An overrideable method to create the converter.<para/>
		/// DO NOT USE THIS, use <see cref="CreateConverter(IDbProvider, IByteEncrypter, EncryptDirection)"/>.
		/// </summary>
		/// <param name="dbProvider">The database provider.</param>
		/// <returns>The created <see cref="ValueConverter"/>.</returns>
		[Obsolete]
		public override ValueConverter CreateConverter(IDbProvider dbProvider) {
			throw new NotSupportedException("CreateConverter(IDbProvider, IByteEncrypter, EncryptDirection) must be used for EncryptedAttribute!");
		}
#pragma warning restore CS0809
		/// <summary>
		/// An overrideable method to create the encrypter.
		/// </summary>
		/// <param name="dbProvider">The database provider.</param>
		/// <param name="e">The required byte encrypter.</param>
		/// <param name="d">The forced encryption direction.</param>
		/// <returns>The created <see cref="ValueConverter"/>.</returns>
		public virtual ValueConverter CreateConverter(IDbProvider dbProvider, IByteEncrypter e, EncryptDirection d) {
			return (ValueConverter) Activator.CreateInstance(ConverterType, e, d);
		}*/
	}
}
