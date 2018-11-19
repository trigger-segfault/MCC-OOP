using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	public class CommandSet : ICollection<string>, IReadOnlyCollection<string> {

		#region Fields

		private readonly HashSet<string> set = new HashSet<string>();

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="CommandSet"/>.
		/// </summary>
		protected CommandSet() {
		}

		/// <summary>
		/// Constructs the <see cref="CommandSet"/>.
		/// </summary>
		protected CommandSet(Action<string> setter) {
			Setter = setter;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the property setter that assigns changes to the set.<para/>
		/// This is a required property.
		/// </summary>
		public Action<string> Setter { get; private set; }
		
		/// <summary>
		/// Gets the separator string.
		/// </summary>
		public virtual string Delimeter { get; protected set; } = "\n";

		#endregion

		#region Serialize

		/// <summary>
		/// Serializes the entire list.
		/// </summary>
		/// <returns>The serialized list.</returns>
		public string Serialize() {
			return string.Join(Delimeter, set) + Delimeter;
		}
		/// <summary>
		/// Deserializes the entire list.
		/// </summary>
		/// <param name="s">The string to deserialize from.</param>
		public void Deserialize(string s) {
			set.Clear();
			string[] items = s.Split(new[] { Delimeter }, StringSplitOptions.None);
			int length = items.Length - 1;
			for (int i = 0; i < length; i++)
				set.Add(items[i]);
		}

		#endregion

		#region ICollection Implementation

		public bool Add(string item) {
			throw new NotImplementedException();
		}

		void ICollection<string>.Add(string item) {
			Add(item);
		}

		public void Clear() {
			throw new NotImplementedException();
		}

		public bool Contains(string item) {
			throw new NotImplementedException();
		}

		public void CopyTo(string[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		public bool Remove(string item) {
			throw new NotImplementedException();
		}

		public int Count { get; }
		public bool IsReadOnly { get; }

		public IEnumerator<string> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}

		#endregion

		#region Static Constructors

		/// <summary>
		/// Constructs the <see cref="StringList{T, TList}"/> of type <typeparamref name="TList"/> with the
		/// specified string value and value setter.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="setter"></param>
		/// <returns></returns>
		public static StringStringSet Create(ref StringStringSet set, string s, Action<string> setter) {
			if (s == null) {
				s = string.Empty;
				setter(string.Empty);
			}
			if (set == null) {
				set = new StringStringSet(setter);
				set.Deserialize(s);
			}
			return set;
		}

		#endregion
	}
}
