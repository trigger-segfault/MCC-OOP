using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// Information on native EntityFrameworkCore model types.
	/// </summary>
	public static class DbNativeTypes {
		/// <summary>
		/// The set of natively supported EntityFrameworkCore model types.
		/// </summary>
		/// <remarks>
		/// <a href="https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/entity-data-model-primitive-data-types">Source</a>
		/// </remarks>
		private static readonly HashSet<Type> nativeTypes = new HashSet<Type>() {
			typeof(byte[]),

			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(decimal),

			typeof(char),
			typeof(string),

			typeof(Guid),
			typeof(TimeSpan),
			typeof(DateTime),
			typeof(DateTimeOffset),
		};

		/// <summary>
		/// Gets an array of all supported native model types.
		/// </summary>
		/// <returns>The array of types.</returns>
		public static Type[] GetNativeTypes() => nativeTypes.ToArray();
		/// <summary>
		/// Gets if the specified type is native to EntityFrameworkCore.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns>True if the type is native, and does not need conversion.</returns>
		public static bool IsNativeType(Type type) {
			Type nullableType = Nullable.GetUnderlyingType(type);
			return nativeTypes.Contains(nullableType ?? type);
		}
		/// <summary>
		/// Gets if the specified type is native to EntityFrameworkCore.
		/// </summary>
		/// <typeparam name="T">The type to check.</param>
		/// <returns>True if the type is native, and does not need conversion.</returns>
		public static bool IsNativeType<T>() => IsNativeType(typeof(T));
	}
}
