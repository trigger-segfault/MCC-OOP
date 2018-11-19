using System;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// The directions data can be encrypted and decrypted in.
	/// </summary>
	[Flags]
	public enum EncryptDirection {
		/// <summary>Data is not encrypted or decrypted.</summary>
		None = 0,
		/// <summary>Data is encrypted.</summary>
		Encrypt = (1 << 0),
		/// <summary>Data is decrypted.</summary>
		Decrypt = (1 << 1),
		/// <summary>Data is encrypted and decrypted.</summary>
		Both = Encrypt | Decrypt,
	}
}
