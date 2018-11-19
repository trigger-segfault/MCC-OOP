using System;
using System.Reflection;

namespace TriggersTools.DiscordBots.Extensions {
	/// <summary>
	/// Additional extension methods for use with <see cref="Attribute"/>s.
	/// </summary>
	public static class CustomAttributeExtensions {

		#region IsDefined<T>
		
		/// <summary>
		/// Indicates whether custom attributes of a specified type are applied to a specified assembly.
		/// </summary>
		/// <typeparam name="T">The type of the attribute to search for.</typeparam>
		/// <param name="element">The assembly to inspect.</param>
		/// <returns>
		/// true if an attribute of the specified type is applied to element; otherwise, false.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="element"/> is null.
		/// </exception>
		public static bool IsDefined<T>(this Assembly element) where T : Attribute {
			return element.IsDefined(typeof(T));
		}
		/// <summary>
		/// Indicates whether custom attributes of a specified type are applied to a specified module.
		/// </summary>
		/// <typeparam name="T">The type of the attribute to search for.</typeparam>
		/// <param name="element">The module to inspect.</param>
		/// <param name="inherit">true to inspect the ancestors of element; otherwise, false.</param>
		/// <returns>
		/// true if an attribute of the specified type is applied to element; otherwise, false.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="element"/> is null.
		/// </exception>
		public static bool IsDefined<T>(this Module element) where T : Attribute {
			return element.IsDefined(typeof(T));
		}

		/// <summary>
		/// Indicates whether custom attributes of a specified type are applied to a specified parameter.
		/// </summary>
		/// <typeparam name="T">The type of the attribute to search for.</typeparam>
		/// <param name="element">The parameter to inspect.</param>
		/// <returns>
		/// true if an attribute of the specified type is applied to element; otherwise, false.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="element"/> is null.
		/// </exception>
		public static bool IsDefined<T>(this ParameterInfo element) where T : Attribute {
			return element.IsDefined(typeof(T));
		}
		/// <summary>
		/// Indicates whether custom attributes of a specified type are applied to a specified parameter,
		/// and, optionally, applied to its ancestors.
		/// </summary>
		/// <typeparam name="T">The type of the attribute to search for.</typeparam>
		/// <param name="element">The parameter to inspect.</param>
		/// <param name="inherit">true to inspect the ancestors of element; otherwise, false.</param>
		/// <returns>
		/// true if an attribute of the specified type is applied to element; otherwise, false.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="element"/> is null.
		/// </exception>
		public static bool IsDefined<T>(this ParameterInfo element, bool inherit) where T : Attribute {
			return element.IsDefined(typeof(T), inherit);
		}

		/// <summary>
		/// Indicates whether custom attributes of a specified type are applied to a specified member.
		/// </summary>
		/// <typeparam name="T">The type of the attribute to search for.</typeparam>
		/// <param name="element">The member to inspect.</param>
		/// <returns>
		/// true if an attribute of the specified type is applied to element; otherwise, false.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="element"/> is null.
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// <paramref name="element"/> is not a constructor, method, property, event, type, or field.
		/// </exception>
		public static bool IsDefined<T>(this MemberInfo element) where T : Attribute {
			return element.IsDefined(typeof(T));
		}
		/// <summary>
		/// Indicates whether custom attributes of a specified type are applied to a specified member, and,
		/// optionally, applied to its ancestors.
		/// </summary>
		/// <typeparam name="T">The type of the attribute to search for.</typeparam>
		/// <param name="element">The member to inspect.</param>
		/// <param name="inherit">true to inspect the ancestors of element; otherwise, false.</param>
		/// <returns>
		/// true if an attribute of the specified type is applied to element; otherwise, false.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="element"/> is null.
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// <paramref name="element"/> is not a constructor, method, property, event, type, or field.
		/// </exception>
		public static bool IsDefined<T>(this MemberInfo element, bool inherit) where T : Attribute {
			return element.IsDefined(typeof(T), inherit);
		}

		#endregion
	}
}
