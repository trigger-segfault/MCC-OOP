using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TriggersTools.DiscordBots.Database.Model {
	public abstract class BlobList<T, TList> : IList<T> where TList : BlobList<T, TList>, new() {

		#region Fields

		/// <summary>
		/// The stored list value.
		/// </summary>
		protected readonly List<T> list = new List<T>();

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="BlobList{T, TList}"/>.
		/// </summary>
		protected BlobList() {
			//CollectionChanged += OnCollectionChanged;
		}

		/// <summary>
		/// Constructs the <see cref="BlobList{T, TList}"/>.
		/// </summary>
		protected BlobList(Action<byte[]> setter) {
			Setter = setter;
			//CollectionChanged += OnCollectionChanged;
		}

		#endregion

		#region Event Handlers

		/*/// <summary>
		/// Saves changes to the list when the collection is changed.
		/// </summary>
		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			Setter.Invoke(Serialize());
		}*/

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the property setter that assigns changes to the list.<para/>
		/// This is a required property.
		/// </summary>
		public Action<byte[]> Setter { get; private set; }

		#endregion

		#region Virtual Properties
		
		/// <summary>
		/// Gets the separator string.
		/// </summary>
		public virtual string Delimeter { get; } = ";";

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Deserializes a list item.
		/// </summary>
		/// <param name="s">The string to deserialize from.</param>
		/// <returns>The deserialized value.</returns>
		public abstract T DeserializeItem(BinaryReader reader);
		/// <summary>
		/// Serializes the list item.
		/// </summary>
		/// <param name="item">The list item to serialize.</param>
		/// <returns>The serialized value.</returns>
		public abstract void SerializeItem(BinaryWriter writer, T item);

		/// <summary>
		/// Serializes the entire list.
		/// </summary>
		/// <returns>The serialized list.</returns>
		public virtual byte[] Serialize() {
			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream)) {
				int count = Count;
				writer.Write(count);
				for (int i = 0; i < count; i++)
					SerializeItem(writer, this[i]);
				return stream.ToArray();
			}
		}
		/// <summary>
		/// Deserializes the entire list.
		/// </summary>
		/// <param name="s">The string to deserialize from.</param>
		public virtual void Deserialize(byte[] data) {
			Clear();
			using (Stream stream = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(stream)) {
				int count = reader.ReadInt32();
				list.Capacity = count;
				for (int i = 0; i < count; i++)
					Add(DeserializeItem(reader));
			}
		}

		#endregion

		#region Static Constructors

		/// <summary>
		/// Constructs the <see cref="StringList{T, TList}"/> of type <typeparamref name="TList"/> with the
		/// specified string value and value setter.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="setter"></param>
		/// <returns></returns>
		public static TList Create(ref TList list, byte[] data, Action<byte[]> setter) {
			if (data == null) {
				data = new byte[4];
				setter(data);
			}
			if (list == null) {
				list = new TList { Setter = setter };
				list.Deserialize(data);
			}
			return list;
		}

		#endregion

		#region List Implementation

		protected void Save() => Setter.Invoke(Serialize());

		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the first occurrence within
		/// the entire list.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the list. The value can be null for reference types.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of item within the entire list, if found; otherwise, –1.
		/// </returns>
		public int IndexOf(T item) => list.IndexOf(item);
		/// <summary>
		/// Inserts an element into the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert. The value can be null for reference types.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is less than 0. -or- <paramref name="index"/> is greater than <see
		/// cref="Count"/>.
		/// </exception>
		public void Insert(int index, T item) {
			list.Insert(index, item);
			Save();
		}
		/// <summary>
		/// Inserts the elements of a collection into the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the new elements should be inserted.</param>
		/// <param name="collection">
		/// The collection whose elements should be inserted into the list. The collection itself cannot be
		/// null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference
		/// type.
		/// </param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="collection"/> is null.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is less than 0. -or- <paramref name="index"/> is greater than <see
		/// cref="Count"/>.
		/// </exception>
		public void InsertRange(int index, IEnumerable<T> collection) {
			list.InsertRange(index, collection);
			Save();
		}
		/// <summary>
		/// Removes the element at the specified index of the list.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is less than 0. -or- <paramref name="index"/> is equal to or greater
		/// than <see cref="Count"/>.
		/// </exception>
		public void RemoveAt(int index) {
			list.RemoveAt(index);
			Save();
		}
		/// <summary>
		/// Adds an object to the end of the list.
		/// </summary>
		/// <param name="item">
		/// The object to be added to the end of the list. The value can be null for reference types.
		/// </param>
		public void Add(T item) {
			list.Add(item);
			Save();
		}
		/// <summary>
		/// Adds the elements of the specified collection to the end of the list.
		/// </summary>
		/// <param name="collection">
		/// The collection whose elements should be added to the end of the list. The collection itself
		/// cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a
		/// reference type.
		/// </param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="collection"/> is null.
		/// </exception>
		public void AddRange(IEnumerable<T> collection) {
			list.AddRange(collection);
			Save();
		}

		/// <summary>
		/// Removes all elements from the list.
		/// </summary>
		public void Clear() {
			list.Clear();
			Save();
		}
		/// <summary>
		/// Determines whether an element is in the list.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the list. The value can be null for reference types.
		/// </param>
		/// <returns>true if item is found in the list; otherwise, false.</returns>
		public bool Contains(T item) => list.Contains(item);
		/// <summary>
		/// Copies the entire list to a compatible one-dimensional array, starting at the specified index of
		/// the target array.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the
		/// elements copied from list. The <see cref="Array"/> must have zero-based indexing.
		/// </param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="array"/> is null.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> is less than 0.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The number of elements in the source list is greater than the available space from arrayIndex to
		/// the end of the destination array.
		/// </exception>
		public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
		/// <summary>
		/// Removes the first occurrence of a specific object from the list.
		/// </summary>
		/// <param name="item">
		/// The object to remove from the list. The value can be null for reference types.
		/// </param>
		/// <returns>
		/// true if item is successfully removed; otherwise, false. This method also returns false if item
		/// was not found in the list.
		/// </returns>
		public bool Remove(T item) {
			if (list.Remove(item)) {
				Save();
				return true;
			}
			return false;
		}
		
		public T this[int index] {
			get => list[index];
			set {
				list[index] = value;
				Save();
			}
		}
		public int Count => list.Count;
		public bool IsReadOnly => false;

		public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

		#endregion
	}
	
	public class BlobSnowflakeList : BlobList<ulong, BlobSnowflakeList> {

		public override ulong DeserializeItem(BinaryReader reader) => reader.ReadUInt64();

		public override void SerializeItem(BinaryWriter writer, ulong item) => writer.Write(item);
	}
}
