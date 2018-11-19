using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// A list of strings that are added and replaced from one large string at runtime.<para/>
	/// There is no conversion cost.
	/// </summary>
	public class StringList : IList<string>, IReadOnlyList<string> {

		#region Fields

		/// <summary>
		/// The actual text representation of the string list.
		/// </summary>
		public string Text { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs an empty <see cref="StringList"/>.
		/// </summary>
		public StringList() {
			Text = Delimeter;
		}
		/// <summary>
		/// Constructs the <see cref="StringList"/> with the specified text.
		/// </summary>
		public StringList(string text) {
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

		#region IList Implementation

		/// <summary>
		/// Adds the item to the list.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(string item) {
			Text += $"{item}{Delimeter}";
		}
		/// <summary>
		/// Adds the range of items to the list.
		/// </summary>
		/// <param name="items">The items to add.</param>
		public void AddRange(IEnumerable<string> items) {
			if (items.Any())
				Text += $"{string.Join(Delimeter, items)}{Delimeter}";
		}
		/// <summary>
		/// Clears the list of items.
		/// </summary>
		public void Clear() {
			Text = Delimeter;
		}
		/// <summary>
		/// Checks if the list contains the item.
		/// </summary>
		/// <param name="item">The item to check for.</param>
		/// <returns>True if the item exists.</returns>
		public bool Contains(string item) {
			return TextIndexOf(item) != -1;
		}
		/// <summary>
		/// Converts the search set to an array.
		/// </summary>
		/// <returns>The constructed array.</returns>
		public string[] ToArray() {
			string[] split = Text.Split(new string[] { Delimeter }, StringSplitOptions.None);
			string[] items = new string[split.Length - 2];
			Array.Copy(split, 1, items, 0, items.Length);
			return items;
		}
		/// <summary>
		/// Copies the set to the array.
		/// </summary>
		/// <param name="array">The array to copy to.</param>
		/// <param name="arrayIndex">The index in the array to copy to at.</param>
		public void CopyTo(string[] array, int arrayIndex) {
			string[] items = ToArray();
			Array.Copy(items, 0, array, arrayIndex, items.Length);
		}
		/// <summary>
		/// Removes the item from the list.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <returns>True if the item was removed.</returns>
		public bool Remove(string item) {
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
		bool ICollection<string>.IsReadOnly => false;

		/// <summary>
		/// Gets the enumerator for the string set.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<string> GetEnumerator() {
			int start = Delimeter.Length;
			for (int i = start; i < Text.Length;) {
				int delimeterIndex = Text.IndexOf(Delimeter, i);
				if (delimeterIndex == -1)
					break;
				yield return Text.Substring(i, delimeterIndex - i);
				i = delimeterIndex + Delimeter.Length;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() => ToArray().GetEnumerator();
		
		public int IndexOf(string item) {
			int count = 0;
			for (int i = Delimeter.Length; i < Text.Length; count++) {
				int delimeterIndex = Text.IndexOf(Delimeter, i);
				if (delimeterIndex == -1)
					break;
				string itemAtIndex = Text.Substring(i, delimeterIndex - i);
				if (itemAtIndex == item)
					return count;
				i = delimeterIndex + Delimeter.Length;
			}
			return -1;
		}

		public void Insert(int index, string item) {
			int textIndex = TextIndexOfIndex(index);
			if (textIndex == -1)
				throw new ArgumentOutOfRangeException(nameof(index));
			Text = Text.Insert(textIndex, $"{item}{Delimeter}");
		}
		public void InsertRange(int index, IEnumerable<string> items) {
			int textIndex = TextIndexOfIndex(index);
			if (textIndex == -1)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (items.Any())
				Text = Text.Insert(textIndex, $"{string.Join(Delimeter, items)}{Delimeter}");
		}

		public void RemoveAt(int index) {
			var (textStart, textLength) = TextIndexesOfIndex(index);
			if (textStart == -1)
				throw new ArgumentOutOfRangeException(nameof(index));
			Text = Text.Remove(textStart, textLength);
		}
		public void Remove(int startIndex, int count) {
			if (count == 0)
				return;
			var (textStart, textLength) = TextIndexesOfIndex(startIndex, count);
			if (textStart == -1)
				throw new ArgumentOutOfRangeException(nameof(startIndex));
			Text = Text.Remove(textStart, textLength);
		}

		public string this[int index] {
			get {
				var (textStart, textLength) = TextIndexesOfIndex(index);
				if (textStart == -1)
					throw new IndexOutOfRangeException();
				return Text.Substring(textStart, textLength);
			}
			set {
				if (value == null)
					throw new ArgumentNullException();
				var (textStart, textLength) = TextIndexesOfIndex(index);
				if (textStart == -1)
					throw new IndexOutOfRangeException();
				Text = Text.Remove(textStart, textLength).Insert(textStart, value);
			}
		}

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
		/// <summary>
		/// Gets the text index of the item index in the text.
		/// </summary>
		/// <param name="index">The item index to look for.</param>
		/// <returns>The text index if found, otherwise -1.</returns>
		private int TextIndexOfIndex(int index) {
			int count = 0;
			for (int i = Delimeter.Length; i < Text.Length; count++) {
				int delimeterIndex = Text.IndexOf(Delimeter, i);
				if (delimeterIndex == -1)
					break;
				if (count == index)
					return delimeterIndex + Delimeter.Length;
				i = delimeterIndex + Delimeter.Length;
			}
			return -1;
		}
		/// <summary>
		/// Gets the text index of the item index in the text.
		/// </summary>
		/// <param name="relativeIndex">
		/// The relative item index to look for. The startTextIndex is considered as 0.
		/// </param>
		/// <returns>The text index if found, otherwise -1.</returns>
		private int TextIndexOfRelativeIndex(int relativeIndex, int startTextIndex) {
			int count = 0;
			if (relativeIndex == 0)
				return startTextIndex;
			for (int i = startTextIndex; i < Text.Length; count++) {
				int delimeterIndex = Text.IndexOf(Delimeter, i);
				if (delimeterIndex == -1)
					break;
				if (count == relativeIndex)
					return delimeterIndex + Delimeter.Length;
				i = delimeterIndex + Delimeter.Length;
			}
			return -1;
		}
		/// <summary>
		/// Gets the text index of the item index in the text.
		/// </summary>
		/// <param name="index">The item index to look for.</param>
		/// <returns>The text index and count if found, otherwise (-1, 0).</returns>
		private (int start, int length) TextIndexesOfIndex(int index) {
			int indexCount = 0;
			if (index == 0) {
				int textEndIndex = Text.IndexOf(Delimeter, Delimeter.Length);
				if (textEndIndex == -1)
					return (-1, 0);
				return (Delimeter.Length, textEndIndex - Delimeter.Length);
			}
			for (int i = Delimeter.Length; i < Text.Length; indexCount++) {
				int delimeterIndex = Text.IndexOf(Delimeter, i);
				if (delimeterIndex == -1 || delimeterIndex + Delimeter.Length == Text.Length)
					break;
				if (indexCount == index) {
					int textEndIndex = Text.IndexOf(Delimeter, delimeterIndex + Delimeter.Length);
					if (index == -1)
						break;
					int startIndex = delimeterIndex + Delimeter.Length;
					return (startIndex, textEndIndex - startIndex);
				}
				i = delimeterIndex + Delimeter.Length;
			}
			return (-1, 0);
		}
		/// <summary>
		/// Gets the text index of the item index in the text.
		/// </summary>
		/// <param name="index">The item index to look for.</param>
		/// <param name="count">The number of indexes to retrieve.</param>
		/// <returns>The text index and count if found, otherwise (-1, 0).</returns>
		private (int start, int length) TextIndexesOfIndex(int index, int count) {
			int indexCount = 0;
			if (index == 0) {
				int textEndIndex = TextIndexOfIndex(count);
				if (textEndIndex == -1)
					return (-1, 0);
				return (Delimeter.Length, textEndIndex);
			}
			int textStartIndex = -1;
			for (int i = 0; i < Text.Length; indexCount++) {
				int delimeterIndex = Text.IndexOf(Delimeter, i);
				if (delimeterIndex == -1 || delimeterIndex + Delimeter.Length == Text.Length)
					break;
				if (indexCount == index) {
					if (textStartIndex == -1) {
						// Our next target index
						index += count;
						textStartIndex = delimeterIndex + Delimeter.Length;
					}
					else {
						return (textStartIndex, delimeterIndex - textStartIndex);
					}
				}
				i = delimeterIndex + Delimeter.Length;
			}
			return (-1, 0);
		}

		#endregion
	}
}
