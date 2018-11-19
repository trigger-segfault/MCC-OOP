using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A unique set of snowflake entity Ids that are added and replaced from one large string at runtime.
	/// <para/>
	/// There is no conversion cost.
	/// </summary>
	public class SnowflakeSet : ICollection<ulong>, IReadOnlyCollection<ulong> {

		#region Fields

		/// <summary>
		/// The actual text representation of the string set.
		/// </summary>
		public string Text { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs an empty <see cref="SnowflakeSet"/>.
		/// </summary>
		public SnowflakeSet() {
			Text = Delimeter;
		}
		/// <summary>
		/// Constructs the <see cref="SnowflakeSet"/> with the specified text.
		/// </summary>
		public SnowflakeSet(string text) {
			if (text == null || text.Length == 0)
				text = Delimeter;
			else
				Text = text;
		}

		#endregion

		#region Properties
		
		/// <summary>
		/// Gets the separator string.
		/// </summary>
		public virtual string Delimeter => "\n";

		#endregion

		#region ICollection Implementation

		/// <summary>
		/// Adds the item to the set, if it does not already exist.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns>True if the item was added.</returns>
		public bool Add(ulong id) {
			if (!Contains(id)) {
				Text += $"{id}{Delimeter}";
				return true;
			}
			return false;
		}
		void ICollection<ulong>.Add(ulong id) => Add(id);
		/// <summary>
		/// Adds the range of items to the set if they do not exist.
		/// </summary>
		/// <param name="items">The items to add.</param>
		public void AddRange(IEnumerable<ulong> ids) {
			foreach (ulong id in ids)
				Add(id);
		}
		/// <summary>
		/// Clears the set of items.
		/// </summary>
		public void Clear() {
			Text = Delimeter;
		}
		/// <summary>
		/// Checks if the set contains the item.
		/// </summary>
		/// <param name="item">The item to check for.</param>
		/// <returns>True if the item exists.</returns>
		public bool Contains(ulong id) {
			return TextIndexOf(id.ToString()) != -1;
		}
		/// <summary>
		/// Converts the search set to an array.
		/// </summary>
		/// <returns>The constructed array.</returns>
		public ulong[] ToArray() {
			string[] split = Text.Split(new string[] { Delimeter }, StringSplitOptions.None);
			return split.Skip(1).Take(split.Length - 2).Select(s => ulong.Parse(s)).ToArray();
		}
		/// <summary>
		/// Copies the set to the array.
		/// </summary>
		/// <param name="array">The array to copy to.</param>
		/// <param name="arrayIndex">The index in the array to copy to at.</param>
		public void CopyTo(ulong[] array, int arrayIndex) {
			ulong[] items = ToArray();
			Array.Copy(items, 0, array, arrayIndex, items.Length);
		}
		/// <summary>
		/// Removes the item from the list.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <returns>True if the item was removed.</returns>
		public bool Remove(ulong id) {
			string item = id.ToString();
			int index = TextIndexOf(item);
			if (index != -1) {
				Text = Text.Remove(index, item.Length + Delimeter.Length);
				return true;
			}
			return false;
		}
		/// <summary>
		/// Gets the number of items in the list.
		/// </summary>
		public int Count {
			get {
				int count = 0;
				for (int i = Delimeter.Length; i < Text.Length; count++) {
					int delimeterIndex = Text.IndexOf(Delimeter, i);
					if (delimeterIndex == -1)
						break;
					i = delimeterIndex + Delimeter.Length;
				}
				return count;
			}
		}
		bool ICollection<ulong>.IsReadOnly => false;

		/// <summary>
		/// Gets the enumerator for the string set.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ulong> GetEnumerator() {
			int start = Delimeter.Length;
			for (int i = start; i < Text.Length;) {
				int delimeterIndex = Text.IndexOf(Delimeter, i);
				if (delimeterIndex == -1)
					break;
				yield return ulong.Parse(Text.Substring(i, delimeterIndex - i));
				i = delimeterIndex + Delimeter.Length;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() => ToArray().GetEnumerator();

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the index of the item in the text.
		/// </summary>
		/// <param name="item">The item to get the index of.</param>
		/// <returns>The text index of the item if it exists, otherwise -1.</returns>
		private int TextIndexOf(string item) {
			int index = Text.IndexOf($"{Delimeter}{item}{Delimeter}");
			return (index == -1 ? -1 : (index + Delimeter.Length));
		}

		#endregion
	}
}
