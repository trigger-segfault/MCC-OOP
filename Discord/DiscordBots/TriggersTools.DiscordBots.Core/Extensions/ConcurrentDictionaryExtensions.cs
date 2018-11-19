using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TriggersTools.DiscordBots.Extensions {
	/// <summary>
	/// Extensions methods for the <see cref="ConcurrentDictionary{TKey, TValue}"/> class.
	/// </summary>
	public static class ConcurrentDictionaryExtensions {
		/// <summary>
		/// Tries to remove the key from the dictionary only if the value matches.
		/// </summary>
		/// <typeparam name="TKey">The key element type</typeparam>
		/// <typeparam name="TValue">The value element type</typeparam>
		/// <param name="dictionary">The source dictionary to search.</param>
		/// <param name="key">The key to search for.</param>
		/// <param name="value">The value to compare with.</param>
		/// <returns>True if the matching key and value existed and was removed.</returns>
		public static bool TryRemoveValue<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary,
			TKey key, TValue value)
		{
			var collection = (ICollection<KeyValuePair<TKey, TValue>>) dictionary;
			var toRemove = new KeyValuePair<TKey, TValue>(key, value);
			return collection.Remove(toRemove);
		}

		public static bool TryAddOrGet<TKey, TValue>(
			this ConcurrentDictionary<TKey, TValue> dictionary,
			TKey key, ref TValue value) {
			TValue result;
			do {
				if (dictionary.TryAdd(key, value))
					return true;
			}
			while (!dictionary.TryGetValue(key, out result));
			value = result;
			return false;
		}
	}
}
