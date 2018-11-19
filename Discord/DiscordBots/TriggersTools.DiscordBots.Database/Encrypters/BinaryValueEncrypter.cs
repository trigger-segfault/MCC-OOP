
namespace TriggersTools.DiscordBots.Database.Encrypters {
	/// <summary>
	/// An encryption value converter for <see cref="byte[]"/> to <see cref="byte[]"/>.
	/// </summary>
	public class BinaryValueEncrypter : ValueEncrypter<byte[], byte[]> {
		
		public BinaryValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d))
		{
		}

		private static byte[] Encrypt(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (v == null)
				return null;
			if (v.Length == 0)
				return new byte[0];
			return EncryptBytes(v, e, d);
		}
		private static byte[] Decrypt(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (v == null)
				return null;
			if (v.Length == 0)
				return new byte[0];
			return DecryptBytes(v, e, d);
		}
	}
}
