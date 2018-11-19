
namespace TriggersTools.DiscordBots.Database.Encrypters {
	/// <summary>
	/// An encryption value converter for <see cref="string"/> to base64 <see cref="string"/>.
	/// </summary>
	public class StringValueEncrypter : ValueEncrypter<string, string> {

		public StringValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d)) { }
		
		private static string Encrypt(string v, IByteEncrypter e, EncryptDirection d) {
			if (v == null)
				return null;
			if (v.Length == 0)
				return string.Empty;
			return EncryptString(v, e, d);
		}
		private static string Decrypt(string v, IByteEncrypter e, EncryptDirection d) {
			if (v == null)
				return null;
			if (v.Length == 0)
				return string.Empty;
			return DecryptString(v, e, d);
		}
	}
}
