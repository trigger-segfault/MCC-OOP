using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// The entity property is converted with a <see cref="ValueConverter"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ConverterAttribute : Attribute {
		/// <summary>
		/// The type of <see cref="ValueConverter"/> to use.
		/// </summary>
		public Type ConverterType { get; }
		/// <summary>
		/// Gets if this converter uses a custom type.
		/// </summary>
		public bool HasConverterType => ConverterType != null;

		/// <summary>
		/// Constructs the overridden converter attribute.
		/// </summary>
		protected ConverterAttribute() {
			ConverterType = null;
		}
		/// <summary>
		/// Constructs the converter with the customized type.
		/// </summary>
		/// <param name="converterType">
		/// The customized converter type. Must be of type <see cref="ValueConverter"/>.
		/// </param>
		public ConverterAttribute(Type converterType) {
			if (converterType == null)
				throw new ArgumentNullException(nameof(converterType));
			if (typeof(ValueConverter).IsAssignableFrom(converterType))
				throw new ArgumentException($"Converter type {converterType.Name} is not a ValueConverter!");
			ConverterType = converterType;
		}

		/*/// <summary>
		/// An overrideable method to create the converter.
		/// </summary>
		/// <param name="dbProvider">The database provider.</param>
		/// <returns>The created <see cref="ValueConverter"/>.</returns>
		public virtual ValueConverter CreateConverter(IDbProvider dbProvider) {
			return (ValueConverter) Activator.CreateInstance(ConverterType);
		}*/
	}
	/*/// <summary>
	/// The entity property is converter with a <see cref="StringSetValueConverter"/>.
	/// </summary>
	public class ConverterStringSetAttribute : ConvertedAttribute {
		public ConverterStringSetAttribute()
			: base(typeof(StringSetValueConverter)) { }
	}
	/// <summary>
	/// The entity property is converter with a <see cref="StringListValueConverter"/>.
	/// </summary>
	public class ConverterStringSearchCollectionAttribute : ConverterAttribute {
		public ConverterStringSearchCollectionAttribute()
			: base(typeof(ConverterStringSearchCollectionAttribute)) { }
	}
	/// <summary>
	/// The entity property is converter with a <see cref="DiscordColorNullableValueConverter"/>.
	/// </summary>
	public class ConverterNullableDiscordColorAttribute : ConverterAttribute {
		public ConverterNullableDiscordColorAttribute()
			: base(typeof(DiscordColorNullableValueConverter)) { }
	}
	/// <summary>
	/// The entity property is encrypted with a <see cref="StringValueEncrypter"/>.
	/// </summary>
	public class EncryptedStringAttribute : EncryptedAttribute {
		public EncryptedStringAttribute(EncryptDirection direction = EncryptDirection.Both)
			: base(typeof(StringValueEncrypter), direction) { }
	}
	/// <summary>
	/// The entity property is encrypted with a <see cref="BinaryValueEncrypter"/>.
	/// </summary>
	public class EncryptedBytesAttribute : EncryptedAttribute {
		public EncryptedBytesAttribute(EncryptDirection direction = EncryptDirection.Both)
			: base(typeof(BinaryValueEncrypter), direction) { }
	}
	/// <summary>
	/// The entity property is encrypted with a <see cref="StringToByteValueEncrypter"/>.
	/// </summary>
	public class EncryptedStringToBytesAttribute : EncryptedAttribute {
		public EncryptedStringToBytesAttribute(EncryptDirection direction = EncryptDirection.Both)
			: base(typeof(StringToByteValueEncrypter), direction) { }
	}
	/// <summary>
	/// The entity property is encrypted with a <see cref="DateTimeValueEncrypter"/>.
	/// </summary>
	public class EncryptedDateTimeAttribute : EncryptedAttribute {
		public EncryptedDateTimeAttribute(EncryptDirection direction = EncryptDirection.Both)
			: base(typeof(DateTimeValueEncrypter), direction) { }
	}
	/// <summary>
	/// The entity property is encrypted with a <see cref="DateTimeNullableValueEncrypter"/>.
	/// </summary>
	public class EncryptedDateTimeNullableAttribute : EncryptedAttribute {
		public EncryptedDateTimeNullableAttribute(EncryptDirection direction = EncryptDirection.Both)
			: base(typeof(DateTimeNullableValueEncrypter), direction) { }
	}
	/// <summary>
	/// The entity property is encrypted with a <see cref="TimeZoneValueEncrypter"/>.
	/// </summary>
	public class EncryptedTimeZoneAttribute : EncryptedAttribute {
		public EncryptedTimeZoneAttribute(EncryptDirection direction = EncryptDirection.Both)
			: base(typeof(TimeZoneValueEncrypter), direction) { }
	}
	/// <summary>
	/// The entity property is encrypted with a <see cref="StringSetValueEncrypter"/>.
	/// </summary>
	public class EncryptedStringSearchSetAttribute : EncryptedAttribute {
		public EncryptedStringSearchSetAttribute(EncryptDirection direction = EncryptDirection.Both)
			: base(typeof(StringSetValueEncrypter), direction) { }
	}
	/// <summary>
	/// The entity property is encrypted with a <see cref="StringListValueEncrypter"/>.
	/// </summary>
	public class EncryptedStringSearchCollectionAttribute : EncryptedAttribute {
		public EncryptedStringSearchCollectionAttribute(EncryptDirection direction = EncryptDirection.Both)
			: base(typeof(StringListValueEncrypter), direction) { }
	}*/
	/*/// <summary>
	/// The entity property is encrypted with a <see cref="StringBase64EncryptionValueConverter"/>.
	/// </summary>
	public class EncryptedBase64Attribute : EncryptedAttribute {
		public EncryptedBase64Attribute(EncryptionDirection direction = EncryptionDirection.Both)
			: base(typeof(StringBase64EncryptionValueConverter), direction) { }
	}
	/// <summary>
	/// The entity property is encrypted with a <see cref="StringBlobEncryptionValueConverter"/>.
	/// </summary>
	public class EncryptedBlobAttribute : EncryptedAttribute {
		public EncryptedBlobAttribute(EncryptionDirection direction = EncryptionDirection.Both)
			: base(typeof(StringBlobEncryptionValueConverter), direction) { }
	}*/
}
