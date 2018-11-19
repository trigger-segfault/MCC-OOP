using System;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TriggersTools.DiscordBots.Database.Encrypters {
	/// <summary>
	/// The base value converter to use for encryption
	/// </summary>
	/// <typeparam name="TModel">The output model type of the encrypter.</typeparam>
	/// <typeparam name="TProvider">The database provider type of the converter</typeparam>
	public abstract class ValueEncrypter<TModel, TProvider>
		: ValueConverter<TModel, TProvider>, IValueEncrypter
	{
		protected ValueEncrypter(Expression<Func<TModel, TProvider>> convertToProvider,
							  Expression<Func<TProvider, TModel>> convertFromProvider)
			: base(convertToProvider, convertFromProvider) { }

		private static readonly Encoding UTF8 = Encoding.UTF8;

		/// <summary>
		/// Encrypts bytes. Null/empty checks should be handled BEFORE this function.
		/// </summary>
		/// <param name="v">The decrypted byte array.</param>
		/// <param name="e">The encryptor.</param>
		/// <param name="d">The encryption direction.</param>
		/// <returns>The encrypted bytes.</returns>
		protected static byte[] EncryptBytes(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (d.HasFlag(EncryptDirection.Encrypt))
				return e.Encrypt(v);
			return v;
		}
		/// <summary>
		/// Decrypts bytes. Null/empty checks should be handled BEFORE this function.
		/// </summary>
		/// <param name="v">The encrypted byte array.</param>
		/// <param name="e">The encryptor.</param>
		/// <param name="d">The encryption direction.</param>
		/// <returns>The decrypted bytes.</returns>
		protected static byte[] DecryptBytes(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (d.HasFlag(EncryptDirection.Decrypt))
				return e.Decrypt(v);
			return v;
		}
		/// <summary>
		/// Encrypts strings. Null/empty checks should be handled BEFORE this function.
		/// </summary>
		/// <param name="v">The decrypted string.</param>
		/// <param name="e">The encryptor.</param>
		/// <param name="d">The encryption direction.</param>
		/// <returns>The encrypted base64 string.</returns>
		protected static string EncryptString(string v, IByteEncrypter e, EncryptDirection d) {
			if (d.HasFlag(EncryptDirection.Encrypt))
				return Convert.ToBase64String(e.Encrypt(UTF8.GetBytes(v)));
			return v;
		}
		/// <summary>
		/// Decrypts strings. Null/empty checks should be handled BEFORE this function.
		/// </summary>
		/// <param name="v">The encrypted base64 string.</param>
		/// <param name="e">The encryptor.</param>
		/// <param name="d">The encryption direction.</param>
		/// <returns>The decrypted string.</returns>
		protected static string DecryptString(string v, IByteEncrypter e, EncryptDirection d) {
			if (d.HasFlag(EncryptDirection.Decrypt))
				return UTF8.GetString(e.Decrypt(Convert.FromBase64String(v)));
			return v;
		}
		/// <summary>
		/// Encrypts strings to bytes. Null/empty checks should be handled BEFORE this function.
		/// </summary>
		/// <param name="v">The decrypted string.</param>
		/// <param name="e">The encryptor.</param>
		/// <param name="d">The encryption direction.</param>
		/// <returns>The encrypted bytes.</returns>
		protected static byte[] EncryptFromString(string v, IByteEncrypter e, EncryptDirection d) {
			if (d.HasFlag(EncryptDirection.Encrypt))
				return e.Encrypt(UTF8.GetBytes(v));
			return UTF8.GetBytes(v);
		}
		/// <summary>
		/// Decrypts bytes to strings. Null/empty checks should be handled BEFORE this function.
		/// </summary>
		/// <param name="v">The encrypted byte array.</param>
		/// <param name="e">The encryptor.</param>
		/// <param name="d">The encryption direction.</param>
		/// <returns>The decrypted string.</returns>
		protected static string DecryptToString(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (d.HasFlag(EncryptDirection.Decrypt))
				return UTF8.GetString(e.Decrypt(v));
			return UTF8.GetString(v);
		}
	}
}
