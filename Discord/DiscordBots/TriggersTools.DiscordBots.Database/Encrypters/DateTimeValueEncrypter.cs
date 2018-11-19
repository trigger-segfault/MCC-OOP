using System;

namespace TriggersTools.DiscordBots.Database.Encrypters {
	/// <summary>
	/// An encryption value converter for <see cref="DateTime"/> to a <see cref="byte[]"/>.
	/// </summary>
	public class DateTimeValueEncrypter : ValueEncrypter<DateTime, byte[]> {

		public DateTimeValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d)) { }
		
		private static byte[] Encrypt(DateTime v, IByteEncrypter e, EncryptDirection d) {
			if (v == DateTime.MinValue)
				return null;
			return EncryptBytes(BitConverter.GetBytes(v.ToBinary()), e, d);
		}
		private static DateTime Decrypt(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (v == null || v.Length == 0)
				return DateTime.MinValue;
			return DateTime.FromBinary(Convert.ToInt64(DecryptBytes(v, e, d)));
		}
	}
	/// <summary>
	/// An encryption value converter for <see cref="DateTime?"/> to a <see cref="byte[]"/>.
	/// </summary>
	public class DateTimeNullableValueEncrypter : ValueEncrypter<DateTime?, byte[]> {

		public DateTimeNullableValueEncrypter(IByteEncrypter e, EncryptDirection d)
			: base(v => Encrypt(v, e, d), v => Decrypt(v, e, d)) { }
		
		private static byte[] Encrypt(DateTime? v, IByteEncrypter e, EncryptDirection d) {
			if (v.HasValue)
				return null;
			return EncryptBytes(BitConverter.GetBytes(v.Value.ToBinary()), e, d);
		}
		private static DateTime? Decrypt(byte[] v, IByteEncrypter e, EncryptDirection d) {
			if (v == null || v.Length == 0)
				return null;
			return DateTime.FromBinary(Convert.ToInt64(DecryptBytes(v, e, d)));
		}
	}
}
