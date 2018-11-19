
namespace TriggersTools.DiscordBots.Database.Encrypters {
	/// <summary>
	/// An encryption value converter for <see cref="string"/> to <see cref="byte[]"/>.
	/// </summary>
	public class StringToByteValueEncrypter : ValueEncrypter<string, byte[]> {
		
		public StringToByteValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d)) { }
		
		private static byte[] Encrypt(string v, IByteEncrypter e, EncryptDirection d) {
			if (v == null)
				return null;
			if (v.Length == 0)
				return new byte[0];
			return EncryptFromString(v, e, d);
		}
		private static string Decrypt(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (v == null)
				return null;
			if (v.Length == 0)
				return string.Empty;
			return DecryptToString(v, e, d);
		}
	}
}
