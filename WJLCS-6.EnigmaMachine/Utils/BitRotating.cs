using System;
using System.Collections.Generic;
using System.Text;

namespace WJLCS.Enigma.Utils {
	/// <summary>
	/// A static helper class for rotating bits.
	/// </summary>
	internal static class BitRotating {

		/// <summary>
		/// Rotates <paramref name="value"/> to the left by <paramref name="count"/> bits.
		/// </summary>
		/// <param name="value">The value to rotate.</param>
		/// <param name="count">The number of bits to shift by.</param>
		/// <returns>The rotated value.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="count"/> is less than zero.
		/// </exception>
		public static int RotateLeft(int value, int count) {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} is less than zero!");
			int bits = sizeof(int) * 8;
			count %= bits;
			if (count == 0)
				return value;
			return unchecked((int) (((uint) value << count) | ((uint) value >> (bits - count))));
		}

		/// <summary>
		/// Rotates <paramref name="value"/> to the right by <paramref name="count"/> bits.
		/// </summary>
		/// <param name="value">The value to rotate.</param>
		/// <param name="count">The number of bits to shift by.</param>
		/// <returns>The rotated value.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="count"/> is less than zero.
		/// </exception>
		public static int RotateRight(int value, int count) {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} is less than zero!");
			int bits = sizeof(int) * 8;
			count %= bits;
			if (count == 0) return value;
			return unchecked((int) (((uint) value >> count) | ((uint) value << (bits - count))));
		}

		/// <summary>
		/// Rotates <paramref name="value"/> to the by <paramref name="count"/> bits to the left when
		/// <paramref name="count"/> is positive, and right when <see cref="count"/> is negative.
		/// </summary>
		/// <param name="value">The value to rotate.</param>
		/// <param name="count">The number of bits to shift by.</param>
		/// <returns>The rotated value.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="count"/> is less than zero.
		/// </exception>
		public static int Rotate(int value, int count) {
			if (count > 0)
				return RotateLeft(value, count);
			else if (count < 0)
				return RotateRight(value, -count);
			return value;
		}
	}
}
