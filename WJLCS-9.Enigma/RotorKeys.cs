using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WJLCS.Enigma.Utils;

namespace WJLCS.Enigma {
	/// <summary>
	/// A collection of prime number keys used for generating rotors.
	/// </summary>
	public class RotorKeys : IReadOnlyList<int> {

		#region Static Fields

		/// <summary>
		/// The genereated list of prime numbers between 3 and 1000.
		/// </summary>
		private static readonly int[] primeNumbers;
		/// <summary>
		/// Used for generating and validating prime numbers.
		/// </summary>
		private static readonly PrimeLibrary.PrimeFinder primeFinder;

		#endregion

		#region Initializer

		/// <summary>
		/// Initializes the <see cref="RotorCollection"/> class.
		/// </summary>
		static RotorKeys() {
			primeFinder = new PrimeLibrary.PrimeFinder();
			primeNumbers = primeFinder.GeneratePrimes(3, 1000).ToArray();
		}

		#endregion

		#region Static Properties

		/// <summary>
		/// Gets the total number of rotor prime number keys between 3 and 1000.
		/// </summary>
		public static int TotalKeyCount => primeNumbers.Length;

		#endregion

		#region Static Accessors

		/// <summary>
		/// Gets the prime number key from the global list at the specified index.
		/// </summary>
		/// <param name="index">The index of the prime number key in the global list.</param>
		/// <returns>The prime number key at the specified index in the global list.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		public static int GetKey(int index) {
			return primeNumbers[index];
		}
		/// <summary>
		/// Gets the index of the prime number key in the global list.
		/// </summary>
		/// <param name="key">The prime number key to get the index of in the global list.</param>
		/// <returns>The index of the prime number key in the global list. -1 if it does not exist.</returns>
		public static int IndexOfKey(int key) {
			return Array.IndexOf(primeNumbers, key);
		}
		
		#endregion
		
		#region Fields

		/// <summary>
		/// The array of rotor prime number keys.
		/// </summary>
		private readonly int[] keys;
		/// <summary>
		/// The precalculated hash.
		/// </summary>
		private readonly int hash;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="RotorKeys"/>.
		/// </summary>
		/// <param name="keys">The array of prime numbers.</param>
		/// <param name="asIndecies">
		/// True if the keys are actually indecies of existing prime numbers.
		/// </param>
		public RotorKeys(int[] keys, bool asIndecies) {
			if (keys == null)
				throw new ArgumentNullException(nameof(keys));
			if (keys.Length == 0)
				throw new ArgumentException($"{nameof(keys)}.{nameof(keys.Length)} is zero!", nameof(keys));
			this.keys = new int[keys.Length];
			if (asIndecies) {
				for (int i = 0; i < keys.Length; i++) {
					int index = keys[i];
					if (index < 0 || index >= TotalKeyCount)
						throw new ArgumentOutOfRangeException(nameof(keys), $"Rotor key index {index} is outside the range of valid prime numbers!");
					this.keys[i] = primeNumbers[index];
				}
			}
			else {
				for (int i = 0; i < keys.Length; i++) {
					int key = keys[i];
					if (key < 3)
						throw new ArgumentOutOfRangeException(nameof(keys), $"Rotor key {key} is less than 3!");
					if (key > 1000)
						throw new ArgumentOutOfRangeException(nameof(keys), $"Rotor key {key} is greater than 1000!");
					else if (!primeFinder.IsPrime(keys[i]))
						throw new ArgumentException($"Rotor key {keys[i]} is not prime!", nameof(keys));
				}
				Array.Copy(keys, this.keys, keys.Length);
			}
			hash = CalculateHash();
		}
		/// <summary>
		/// Calculates the hash code for the immutable steckering.
		/// </summary>
		/// <returns>The calculated hash code.</returns>
		private int CalculateHash() {
			int hash = keys.Length;
			int rotate = 1 + keys.Length % 31;
			for (int i = 0; i < keys.Length; i++) {
				hash ^= keys[i];
				hash = BitRotating.RotateLeft(hash, rotate);
			}
			return hash;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of prime number keys.
		/// </summary>
		public int Count => keys.Length;
		/// <summary>
		/// Gets the prime number key at the specified index.
		/// </summary>
		/// <param name="index">The index of the prime number key.</param>
		/// <returns>The prime number key at the specified index.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		public int this[int index] => keys[index];

		#endregion

		#region Object Overrides

		/// <summary>
		/// Gets the hashcode for the letterset.
		/// </summary>
		public override int GetHashCode() => hash;

		#endregion

		#region IReadOnlyList Methods

		/// <summary>
		/// Gets the index of the prime number rotor key.
		/// </summary>
		/// <param name="key">The prime number key to get the index of.</param>
		/// <returns>The index of the prime number key in the list. -1 if it does not exist.</returns>
		public int IndexOf(int key) {
			return Array.IndexOf(keys, key);
		}
		/// <summary>
		/// Returns true if the rotor keys contains the specified prime number.
		/// </summary>
		/// <param name="key">The prime number key to check for.</param>
		/// <returns>True if the prime number key exists.</returns>
		public bool Contains(int key) {
			return IndexOf(key) != -1;
		}
		/// <summary>
		/// Casts the rotor keys to an integer array.
		/// </summary>
		public int[] ToArray() {
			int[] newSteckering = new int[keys.Length];
			Array.Copy(keys, newSteckering, keys.Length);
			return newSteckering;
		}
		/// <summary>
		/// Gets the enumerator for the rotor keys.
		/// </summary>
		public IEnumerator<int> GetEnumerator() => keys.Cast<int>().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}
}
