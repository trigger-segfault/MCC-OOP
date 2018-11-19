using System;
using System.Collections.Generic;
using System.Linq;

namespace TriggersTools.DiscordBots.Extensions {
	/// <summary>
	/// Extension methods for the <see cref="Random"/> class.
	/// </summary>
	public static class RandomGenericExtensions {

		#region Choose

		/// <summary>
		/// Chooses an item in the list at random.
		/// </summary>
		/// <typeparam name="T">The element type of the collection.</typeparam>
		/// <param name="random">The random number generator.</param>
		/// <param name="source">The source collection to get elements from.</param>
		/// <param name="defaultValue">The default value to return when the collection is empty.</param>
		/// <returns>
		/// The choice of Steins;Gate, or the <paramref name="defaultValue"/> if the collection is empty.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="random"/> or <paramref name="source"/> is null.
		/// </exception>
		public static T Choose<T>(this Random random, T[] source, T defaultValue = default(T)) {
			if (source.Length == 0)
				return defaultValue;
			else
				return source[random.Next(source.Length)];
		}
		/// <summary>
		/// Chooses an item in the list at random.
		/// </summary>
		/// <typeparam name="T">The element type of the collection.</typeparam>
		/// <param name="random">The random number generator.</param>
		/// <param name="source">The source collection to get elements from.</param>
		/// <param name="defaultValue">The default value to return when the collection is empty.</param>
		/// <returns>
		/// The choice of Steins;Gate, or the <paramref name="defaultValue"/> if the collection is empty.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="random"/> or <paramref name="source"/> is null.
		/// </exception>
		public static T Choose<T>(this Random random, List<T> source, T defaultValue = default(T)) {
			if (source.Count == 0)
				return defaultValue;
			else
				return source[random.Next(source.Count)];
		}
		/// <summary>
		/// Chooses an item in the list at random.
		/// </summary>
		/// <typeparam name="T">The element type of the collection.</typeparam>
		/// <param name="random">The random number generator.</param>
		/// <param name="source">The source collection to get elements from.</param>
		/// <param name="defaultValue">The default value to return when the collection is empty.</param>
		/// <returns>
		/// The choice of Steins;Gate, or the <paramref name="defaultValue"/> if the collection is empty.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="random"/> or <paramref name="source"/> is null.
		/// </exception>
		public static T Choose<T>(this Random random, IList<T> source, T defaultValue = default(T)) {
			if (source.Count == 0)
				return defaultValue;
			else
				return source[random.Next(source.Count)];
		}
		/// <summary>
		/// Chooses an item in the list at random.
		/// </summary>
		/// <typeparam name="T">The element type of the collection.</typeparam>
		/// <param name="random">The random number generator.</param>
		/// <param name="source">The source collection to get elements from.</param>
		/// <param name="defaultValue">The default value to return when the collection is empty.</param>
		/// <returns>
		/// The choice of Steins;Gate, or the <paramref name="defaultValue"/> if the collection is empty.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="random"/> or <paramref name="source"/> is null.
		/// </exception>
		public static T Choose<T>(this Random random, IReadOnlyList<T> source, T defaultValue = default(T)) {
			if (source.Count == 0)
				return defaultValue;
			else
				return source[random.Next(source.Count)];
		}
		/// <summary>
		/// Chooses an item in the list at random.
		/// </summary>
		/// <typeparam name="T">The element type of the collection.</typeparam>
		/// <param name="random">The random number generator.</param>
		/// <param name="source">The source collection to get elements from.</param>
		/// <param name="defaultValue">The default value to return when the collection is empty.</param>
		/// <returns>
		/// The choice of Steins;Gate, or the <paramref name="defaultValue"/> if the collection is empty.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="random"/> or <paramref name="source"/> is null.
		/// </exception>
		public static T Choose<T>(this Random random, IEnumerable<T> source, T defaultValue = default(T)) {
			return random.Choose(source.ToList());
		}

		#endregion
	}
}
