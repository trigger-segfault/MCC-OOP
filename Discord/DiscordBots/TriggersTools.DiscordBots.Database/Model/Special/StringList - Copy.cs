using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TriggersTools.DiscordBots.Database.Model.Special {
	public abstract class StringList<T, TList> : List<T> where TList : StringList<T, TList>, new() {

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="StringList{T, TList}"/>.
		/// </summary>
		protected StringList() {
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
		public Action<string> Setter { get; set; }

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
		public abstract T DeserializeItem(string s);
		/// <summary>
		/// Serializes the list item.
		/// </summary>
		/// <param name="item">The list item to serialize.</param>
		/// <returns>The serialized value.</returns>
		public virtual string SerializeItem(T item) => item.ToString();

		/// <summary>
		/// Serializes the entire list.
		/// </summary>
		/// <returns>The serialized list.</returns>
		public virtual string Serialize() {
			return string.Join(Delimeter, this.Select(item => SerializeItem(item)));
		}
		/// <summary>
		/// Deserializes the entire list.
		/// </summary>
		/// <param name="s">The string to deserialize from.</param>
		public virtual void Deserialize(string s) {
			Clear();
			string[] items = s.Split(new[] { Delimeter }, StringSplitOptions.None);
			//Capacity = items.Length;
			for (int i = 0; i < items.Length; i++)
				Add(DeserializeItem(items[i]));
		}

		#endregion

		#region Static Constructors

		/*/// <summary>
		/// Constructs the <see cref="StringList{T, TList}"/> of type <typeparamref name="TList"/> with the
		/// specified string value and value setter.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="setter"></param>
		/// <returns></returns>
		public static TList Create(string s, Action<string> setter) {

		}*/

		#endregion

		/*public class Converter : ValueConverter<TList, string> {

			public Converter() : base(v => v.Serialize(), v => Deserialize(v)) { }

			private static TList Deserialize(string v) {
				TList list = new TList();
				list.Deserialize(v);
				return list;
			}

			/// <summary>
			/// A <see cref="ValueConverterInfo"/> for the default use of this converter.
			/// </summary>
			public static ValueConverterInfo DefaultInfo { get; }
				= new ValueConverterInfo(typeof(TList), typeof(string), i => new Converter());
		}*/
	}
	
	public class StringSnowflakeList : StringList<ulong, StringSnowflakeList> {
		
		public override ulong DeserializeItem(string s) => ulong.Parse(s);
	}
}
