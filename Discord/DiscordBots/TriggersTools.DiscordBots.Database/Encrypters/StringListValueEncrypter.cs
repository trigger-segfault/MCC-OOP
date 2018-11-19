using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Database.Encrypters {
	/// <summary>
	/// An encryption value converter for <see cref="StringList"/> to <see cref="string"/>.
	/// </summary>
	public class StringListValueEncrypter : ValueEncrypter<StringList, string> {
		
		public StringListValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d)) { }
		
		private static string Encrypt(StringList v, IByteEncrypter e, EncryptDirection d) {
			if (v?.Text == null)
				return null;
			if (v.Text.Length == 0)
				return string.Empty;
			return EncryptString(v.Text, e, d);
		}
		private static StringList Decrypt(string v, IByteEncrypter e, EncryptDirection d) {
			if (v == null || v.Length == 0)
				return new StringList();
			return new StringList(DecryptString(v, e, d));
		}
	}
}
