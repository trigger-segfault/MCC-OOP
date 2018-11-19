using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Database.Encrypters {
	/// <summary>
	/// An encryption value converter for <see cref="StringSet"/> to <see cref="string"/>.
	/// </summary>
	public class StringSetValueEncrypter : ValueEncrypter<StringSet, string> {
		
		public StringSetValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d)) { }
		
		private static string Encrypt(StringSet v, IByteEncrypter e, EncryptDirection d) {
			if (v?.Text == null)
				return null;
			if (v.Text.Length == 0)
				return string.Empty;
			return EncryptString(v.Text, e, d);
		}
		private static StringSet Decrypt(string v, IByteEncrypter e, EncryptDirection d) {
			if (v == null || v.Length == 0)
				return new StringSet();
			return new StringSet(DecryptString(v, e, d));
		}
	}
}
