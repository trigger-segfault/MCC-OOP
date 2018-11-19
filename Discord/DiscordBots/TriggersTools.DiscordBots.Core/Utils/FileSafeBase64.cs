using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Utils {
	/// <summary>
	/// Static methods for converting between <see cref="Convert.ToBase64String"/>'s '+', '/' base-64 and
	/// filename-safe '-', '_' base-64.
	/// </summary>
	public static class FileSafeBase64 {
		/// <summary>
		/// Converts an array of 8-bit unsigned integers to its equivalent string representation that is
		/// encoded with filename-safe base-64 digits.
		/// </summary>
		/// <param name="inArray">An array of 8-bit unsigned integers.</param>
		/// <returns>The string representation, in base 64, of the contents of <paramref name="inArray"/>.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="inArray"/> is null.
		/// </exception>
		public static string ToBase64String(byte[] inArray) {
			return FromFileSafeBase64(Convert.ToBase64String(inArray));
		}
		/// <summary>
		/// Converts the specified string, which encodes binary data as filename-safe base-64 digits, to an
		/// equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="s"/>.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="s"/> is null.
		/// </exception>
		/// <exception cref="FormatException">
		/// The length of <paramref name="s"/>, ignoring white-space characters, is not zero or a multiple of
		/// 4. -or-The format of <paramref name="s"/> is invalid. <paramref name="s"/> contains a
		/// non-filename-safe base-64 character, more than two padding characters, or a non-white
		/// space-character among the padding characters.
		/// </exception>
		public static byte[] FromBase64String(string s) {
			return Convert.FromBase64String(ToFileSafeBase64(s));
		}
		
		/// <summary>
		/// Replaces all instances of '+', '/' with '-', '_' to comply to filename-safe base-64.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>The string with the base-64 characters replaced.</returns>
		public static string ToFileSafeBase64(string s) => s.Replace('+', '-').Replace('/', '_');
		/// <summary>
		/// Replaces all instances of '-', '_' with '+', '/' to comply to filename-unsafe base-64.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>The string with the base-64 characters replaced.</returns>
		public static string FromFileSafeBase64(string s) => s.Replace('-', '+').Replace('_', '/');
	}
}
