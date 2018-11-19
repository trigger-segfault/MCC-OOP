using System;

namespace TriggersTools.DiscordBots.Database.Encrypters {
	/// <summary>
	/// An encryption value converter for <see cref="TimeSpan"/> to a <see cref="byte[]"/>.
	/// </summary>
	public class TimeSpanValueEncrypter : ValueEncrypter<TimeSpan, byte[]> {

		public TimeSpanValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d)) { }
		
		private static byte[] Encrypt(TimeSpan v, IByteEncrypter e, EncryptDirection d) {
			if (v == TimeSpan.Zero)
				return null;
			return EncryptBytes(BitConverter.GetBytes(v.Ticks), e, d);
		}
		private static TimeSpan Decrypt(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (v == null || v.Length == 0)
				return TimeSpan.Zero;
			return TimeSpan.FromTicks(Convert.ToInt64(DecryptBytes(v, e, d)));
		}
	}
	/// <summary>
	/// An encryption value converter for <see cref="TimeSpan?"/> to a <see cref="byte[]"/>.
	/// </summary>
	public class TimeSpanNullableValueEncrypter : ValueEncrypter<TimeSpan?, byte[]> {

		public TimeSpanNullableValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d)) { }
		
		private static byte[] Encrypt(TimeSpan? v, IByteEncrypter e, EncryptDirection d) {
			if (v.HasValue)
				return null;
			return EncryptBytes(BitConverter.GetBytes(v.Value.Ticks), e, d);
		}
		private static TimeSpan? Decrypt(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (v == null || v.Length == 0)
				return null;
			return TimeSpan.FromTicks(Convert.ToInt64(DecryptBytes(v, e, d)));
		}
	}
}
