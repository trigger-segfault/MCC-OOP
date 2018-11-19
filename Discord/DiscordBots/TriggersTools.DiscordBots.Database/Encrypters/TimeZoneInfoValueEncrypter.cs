using System;

namespace TriggersTools.DiscordBots.Database.Encrypters {
	/// <summary>
	/// An encryption value converter for <see cref="TimeZoneInfo"/> to <see cref="byte[]"/>.
	/// </summary>
	public class TimeZoneInfoValueEncrypter : ValueEncrypter<TimeZoneInfo, byte[]> {

		public TimeZoneInfoValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d)) { }
		
		private static byte[] Encrypt(TimeZoneInfo v, IByteEncrypter e, EncryptDirection d) {
			if (v == null)
				return null;
			return EncryptFromString(v.ToSerializedString(), e, d);
		}
		private static TimeZoneInfo Decrypt(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (v == null || v.Length == 0)
				return null;
			return TimeZoneInfo.FromSerializedString(DecryptToString(v, e, d));
		}
	}
}
