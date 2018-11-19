using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Database.Encrypters {
	/// <summary>
	/// An encryption value converter for <see cref="SnowflakeSet"/> to <see cref="string"/>.
	/// </summary>
	public class SnowflakeSetValueEncrypter : ValueEncrypter<SnowflakeSet, string> {
		
		public SnowflakeSetValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d)) { }
		
		private static string Encrypt(SnowflakeSet v, IByteEncrypter e, EncryptDirection d) {
			if (v?.Text == null)
				return null;
			if (v.Text.Length == 0)
				return string.Empty;
			return EncryptString(v.Text, e, d);
		}
		private static SnowflakeSet Decrypt(string v, IByteEncrypter e, EncryptDirection d) {
			if (v == null || v.Length == 0)
				return new SnowflakeSet();
			return new SnowflakeSet(DecryptString(v, e, d));
		}
	}
}
