using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// An encrypter interface that symmetrically converts binary data.
	/// </summary>
	public interface IByteEncrypter {
		/// <summary>
		/// Encrypts the data.
		/// </summary>
		/// <param name="unencrypted">The unencrypted data.</param>
		/// <returns>The encrypted data prefixed with a 16-byte initialization vector.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="unencrypted"/> is null.
		/// </exception>
		byte[] Encrypt(byte[] unencrypted);
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
		byte[] Decrypt(byte[] encrypted);
	}
}
