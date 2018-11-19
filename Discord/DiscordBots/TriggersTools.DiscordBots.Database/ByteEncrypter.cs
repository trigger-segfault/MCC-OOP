using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// An encrypter that symmetrically converts binary data.
	/// </summary>
	/// <remarks>
	/// See <a href="https://stackoverflow.com/a/26177005/7517185">source</a>.
	/// </remarks>
	public class ByteEncrypter : IByteEncrypter {

		#region Constants

		/// <summary>
		/// The length of the initialization vector.
		/// </summary>
		private const int IVLength = 16;

		#endregion

		#region Fields

		/// <summary>
		/// The random initialization vector generator.
		/// </summary>
		private readonly Random random = new Random();
		/// <summary>
		/// The encryption algorithm.
		/// </summary>
		private readonly SymmetricAlgorithm algorithm = new RijndaelManaged();
		/// <summary>
		/// The encryption key.
		/// </summary>
		private readonly byte[] key;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="ByteEncrypter"> with the specified base64 key.
		/// </summary>
		/// <param name="base64Key">The base64 string key.</param>
		public ByteEncrypter(string base64Key) {
			if (base64Key == null)
				throw new ArgumentNullException(nameof(base64Key));
			key = Convert.FromBase64String(base64Key);
		}
		/// <summary>
		/// Constructs the <see cref="ByteEncrypter"> with the specified binary key.
		/// </summary>
		/// <param name="binaryKey">The byte array key.</param>
		public ByteEncrypter(byte[] binaryKey) {
			if (binaryKey == null)
				throw new ArgumentNullException(nameof(binaryKey));
			key = new byte[binaryKey.Length];
			Array.Copy(binaryKey, key, binaryKey.Length);
		}

		#endregion

		#region Encrypt/Decrypt

		/// <summary>
		/// Encrypts the data.
		/// </summary>
		/// <param name="unencrypted">The unencrypted data.</param>
		/// <returns>The encrypted data prefixed with a 16-byte initialization vector.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="unencrypted"/> is null.
		/// </exception>
		public byte[] Encrypt(byte[] unencrypted) {
			if (unencrypted == null)
				throw new ArgumentNullException(nameof(unencrypted));
			using (MemoryStream output = new MemoryStream()) {

				byte[] vector = new byte[IVLength];
				random.NextBytes(vector);
				output.Write(vector, 0, vector.Length);

				var transform = algorithm.CreateEncryptor(key, vector);
				using (CryptoStream cs = new CryptoStream(output, transform, CryptoStreamMode.Write)) {
					cs.Write(unencrypted, 0, unencrypted.Length);
					cs.FlushFinalBlock();
					return output.ToArray();
				}
			}
		}
		/// <summary>
		/// Decrypts the data.
		/// </summary>
		/// <param name="encrypted">The encrypted data.</param>
		/// <returns>The decrypted data.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="encrypted"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The data is not prefixed with a 16-byte initialization vector.
		/// </exception>
		public byte[] Decrypt(byte[] encrypted) {
			if (encrypted == null)
				throw new ArgumentNullException(nameof(encrypted));
			if (encrypted.Length < IVLength)
				throw new ArgumentException("Not valid encrypted data", "encrypted");

			byte[] vector = new byte[IVLength];
			byte[] data = new byte[encrypted.Length - vector.Length];
			Array.Copy(encrypted, vector, vector.Length);
			Array.Copy(encrypted, vector.Length, data, 0, data.Length);

			var transform = algorithm.CreateDecryptor(key, vector);
			using (MemoryStream output = new MemoryStream())
			using (CryptoStream cs = new CryptoStream(output, transform, CryptoStreamMode.Write)) {
				cs.Write(data, 0, data.Length);
				cs.FlushFinalBlock();
				return output.ToArray();
			}
		}

		#endregion
	}
}
