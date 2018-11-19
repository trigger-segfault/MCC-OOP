using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TriggersTools.DiscordBots.Utils {
	/// <summary>
	/// A static helper class for loading embedded resource streams.
	/// </summary>
	public static class Embedding {

		#region GetResourceNames

		/// <summary>
		/// Returns the names of all the resources in the calling assembly.
		/// </summary>
		/// 
		/// <returns>An array that contains the names of all the resources.</returns>
		public static string[] GetResources() {
			return GetResources(Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Returns the names of all the resources in the calling assembly that start with the specified
		/// path.
		/// </summary>
		/// 
		/// <param name="path">The path resources must start with.</param>
		/// <returns>An array that contains the names of all the matching resources.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="path"/> is null.
		/// </exception>
		public static string[] GetResources(string path) {
			return GetResources(Assembly.GetCallingAssembly(), path);
		}

		/// <summary>
		/// Returns the names of all the resources in this assembly.
		/// </summary>
		/// 
		/// <param name="assembly">The assembly to get the resources of.</param>
		/// <returns>An array that contains the names of all the resources.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="assembly"/> is null.
		/// </exception>
		public static string[] GetResources(Assembly assembly) {
			return assembly.GetManifestResourceNames();
		}

		/// <summary>
		/// Returns the names of all the resources in this assembly that start with the specified path.
		/// </summary>
		/// 
		/// <param name="assembly">The assembly to get the resources of.</param>
		/// <param name="path">The path resources must start with.</param>
		/// <returns>An array that contains the names of all the matching resources.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="assembly"/> or <paramref name="path"/> is null.
		/// </exception>
		public static string[] GetResources(Assembly assembly, string path) {
			return GetResources(assembly).Where(s => s.StartsWith(path, StringComparison.InvariantCultureIgnoreCase)).ToArray();
		}

		/// <summary>
		/// Returns the names of all the resources in type's the assembly.
		/// </summary>
		/// 
		/// <param name="type">The type whose assembly is used to get the resources of.</param>
		/// <returns>An array that contains the names of all the resources.</returns>
		public static string[] GetResources(Type type) {
			return GetResources(type.Assembly);
		}

		/// <summary>
		/// Returns the names of all the resources in the type's assembly that start with the specified
		/// path.
		/// </summary>
		/// 
		/// <param name="type">The type whose assembly is used to get the resources of.</param>
		/// <param name="path">The path resources must start with.</param>
		/// <returns>An array that contains the names of all the matching resources.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> or <paramref name="path"/> is null.
		/// </exception>
		public static string[] GetResources(Type type, string path) {
			return GetResources(type.Assembly, path);
		}

		#endregion

		#region GetStream

		/// <summary>
		/// Loads the specified manifest resource from the calling assembly.
		/// </summary>
		/// 
		/// <param name="paths">The paths to combine to create the resource path.</param>
		/// <returns>The <see cref="Stream"/> for the resource.</returns>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="path"/> is an empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static Stream GetStream(string path) {
			return GetStream(Assembly.GetCallingAssembly(), path);
		}

		/// <summary>
		/// Loads the specified manifest resource from the calling assembly.
		/// </summary>
		/// 
		/// <param name="paths">The paths to combine to create the resource path.</param>
		/// <returns>The <see cref="Stream"/> for the resource.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="paths"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static Stream GetStream(params string[] paths) {
			return GetStream(Assembly.GetCallingAssembly(), Combine(paths));
		}

		/// <summary>
		/// Loads the specified manifest resource from the specified assembly.
		/// </summary>
		/// 
		/// <param name="assembly">The assembly to load the resource from.</param>
		/// <param name="path">The resource path.</param>
		/// <returns>The <see cref="Stream"/> for the resource.</returns>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="path"/> is an empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="assembly"/> or <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static Stream GetStream(Assembly assembly, string path) {
			Stream stream = assembly.GetManifestResourceStream(path);
			return stream ?? throw new ResourceNotFoundException(assembly, path);
		}

		/// <summary>
		/// Loads the specified manifest resource from the specified assembly.
		/// </summary>
		/// 
		/// <param name="assembly">The assembly to load the resource from.</param>
		/// <param name="paths">The paths to combine to create the resource path.</param>
		/// <returns>The <see cref="Stream"/> for the resource.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="assembly"/> or <paramref name="paths"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static Stream GetStream(Assembly assembly, params string[] paths) {
			return GetStream(assembly, Combine(paths));
		}

		/// <summary>
		/// Loads the specified manifest resource, scoped by the namespace of the specified type,
		/// from this assembly.
		/// </summary>
		/// 
		/// <param name="type">The type whose namespace is used to scope the manifest resource name.</param>
		/// <param name="name">The case-sensitive name of the manifest resource being requested.</param>
		/// <returns>The <see cref="Stream"/> for the resource.</returns>
		/// 
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> or <paramref name="name"/> is null.
		/// </exception>
		public static Stream GetStream(Type type, string name) {
			Stream stream = type.Assembly.GetManifestResourceStream(type, name);
			return stream ?? throw new ResourceNotFoundException(type, name);
		}

		#endregion

		#region ReadAllBytes

		/// <summary>
		/// Loads the specified manifest resource from the calling assembly as a byte array.
		/// </summary>
		/// 
		/// <param name="path">The resource path.</param>
		/// <returns>The byte array of the resource's data.</returns>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="path"/> is an empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static byte[] ReadAllBytes(string path) {
			return ReadAllBytes(Assembly.GetCallingAssembly(), path);
		}

		/// <summary>
		/// Loads the specified manifest resource from the specified assembly as a byte array.
		/// </summary>
		/// 
		/// <param name="assembly">The assembly to load the resource from.</param>
		/// <param name="path">The resource path.</param>
		/// <returns>The byte array of the resource's data.</returns>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="path"/> is an empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="assembly"/> or <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static byte[] ReadAllBytes(Assembly assembly, string path) {
			using (Stream stream = GetStream(assembly, path))
				return stream.ReadToEnd();
		}

		/// <summary>
		/// Loads the specified manifest resource, scoped by the namespace of the specified type,
		/// from this assembly as a byte array
		/// </summary>
		/// 
		/// <param name="type">The type whose namespace is used to scope the manifest resource name.</param>
		/// <param name="name">The case-sensitive name of the manifest resource being requested.</param>
		/// <returns>The byte array of the resource's data.</returns>
		/// 
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> or <paramref name="name"/> is null.
		/// </exception>
		public static byte[] ReadAllBytes(Type type, string name) {
			using (Stream stream = GetStream(type, name))
				return stream.ReadToEnd();
		}

		#endregion

		#region ReadAllText

		/// <summary>
		/// Loads the specified manifest resource from the calling assembly as a string.
		/// </summary>
		/// 
		/// <param name="path">The resource path.</param>
		/// <returns>The string of the resource's data.</returns>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="path"/> is an empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static string ReadAllText(string path) {
			return ReadAllText(Assembly.GetCallingAssembly(), path);
		}

		/// <summary>
		/// Loads the specified manifest resource from the specified assembly as a string.
		/// </summary>
		/// 
		/// <param name="assembly">The assembly to load the resource from.</param>
		/// <param name="path">The resource path.</param>
		/// <returns>The string of the resource's data.</returns>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="path"/> is an empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="assembly"/> or <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static string ReadAllText(Assembly assembly, string path) {
			using (Stream stream = GetStream(assembly, path))
			using (StreamReader reader = new StreamReader(stream))
				return reader.ReadToEnd();
		}

		/// <summary>
		/// Loads the specified manifest resource, scoped by the namespace of the specified type,
		/// from this assembly as a string
		/// </summary>
		/// 
		/// <param name="type">The type whose namespace is used to scope the manifest resource name.</param>
		/// <param name="name">The case-sensitive name of the manifest resource being requested.</param>
		/// <returns>The string of the resource's data.</returns>
		/// 
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> or <paramref name="name"/> is null.
		/// </exception>
		public static string ReadAllText(Type type, string name) {
			using (Stream stream = GetStream(type, name))
			using (StreamReader reader = new StreamReader(stream))
				return reader.ReadToEnd();
		}

		#endregion

		#region SaveToFile

		/// <summary>
		/// Saves the specified manifest resource from the calling assembly to the specified file.
		/// </summary>
		/// 
		/// <param name="path">The resource path.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="path"/> is an empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static void SaveToFile(string path, string file) {
			SaveToFile(Assembly.GetCallingAssembly(), path, file);
		}

		/// <summary>
		/// Saves the specified manifest resource from the specified assembly to the specified file.
		/// </summary>
		/// 
		/// <param name="assembly">The assembly to load the resource from.</param>
		/// <param name="path">The resource path.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="path"/> is an empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="assembly"/> or <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static void SaveToFile(Assembly assembly, string path, string file) {
			using (Stream inStream = GetStream(assembly, path))
			using (FileStream outStream = File.Create(file))
				inStream.CopyTo(outStream);
		}

		/// <summary>
		/// Saves the specified manifest resource, scoped by the namespace of the specified type,
		/// from this assembly to the specified file.
		/// </summary>
		/// 
		/// <param name="type">The type whose namespace is used to scope the manifest resource name.</param>
		/// <param name="name">The case-sensitive name of the manifest resource being requested.</param>
		/// 
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> or <paramref name="name"/> is null.
		/// </exception>
		public static void SaveToFile(Type type, string name, string file) {
			using (Stream inStream = GetStream(type, name))
			using (FileStream outStream = File.Create(file))
				inStream.CopyTo(outStream);
		}

		#endregion

		#region SaveToStream

		/// <summary>
		/// Saves the specified manifest resource, scoped by the namespace of the specified type, from this
		/// assembly to the specified stream.
		/// </summary>
		/// 
		/// <param name="path">The resource path.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="path"/> is an empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static void SaveToStream(string path, Stream stream) {
			SaveToStream(Assembly.GetCallingAssembly(), path, stream);
		}

		/// <summary>
		/// Saves the specified manifest resource, scoped by the namespace of the specified type, from this
		/// assembly to the specified stream.
		/// </summary>
		/// 
		/// <param name="assembly">The assembly to load the resource from.</param>
		/// <param name="path">The resource path.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// <paramref name="path"/> is an empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="assembly"/> or <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		public static void SaveToStream(Assembly assembly, string path, Stream stream) {
			using (Stream inStream = GetStream(assembly, path))
				inStream.CopyTo(stream);
		}

		/// <summary>
		/// Saves the specified manifest resource, scoped by the namespace of the specified type, from this
		/// assembly to the specified stream.
		/// </summary>
		/// 
		/// <param name="type">The type whose namespace is used to scope the manifest resource name.</param>
		/// <param name="name">The case-sensitive name of the manifest resource being requested.</param>
		/// 
		/// <exception cref="ResourceNotFoundException">
		/// The resource could not be located.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> or <paramref name="name"/> is null.
		/// </exception>
		public static void SaveToStream(Type type, string name, Stream stream) {
			using (Stream inStream = GetStream(type, name))
				inStream.CopyTo(stream);
		}

		#endregion

		#region Combine

		/// <summary>
		/// Combines the embedded paths with a '.' separating each part and trimming '.'s.
		/// </summary>
		/// 
		/// <param name="paths">The paths to combine.</param>
		/// <returns>The combined resource path.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="paths"/> is null.
		/// </exception>
		public static string Combine(params string[] paths) {
			return string.Join(".", paths.Select(p => p.Trim('.')));
		}

		/// <summary>
		/// Combines the embedded paths with a '.' separating each part and trimming '.'s.
		/// </summary>
		/// 
		/// <param name="paths">The paths to combine.</param>
		/// <returns>The combined resource path.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="paths"/> or any of its values are null.
		/// </exception>
		public static string Combine(IEnumerable<string> paths) {
			return string.Join(".", paths.Select(p => p.Trim('.')));
		}

		/// <summary>
		/// Combines the embedded paths with a '.' separating each part and trimming '.'s.
		/// </summary>
		/// 
		/// <param name="startPath">The first path to add.</param>
		/// <param name="paths">The remaining paths to add.</param>
		/// <returns>The combined resource path.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="startPath"/> or <paramref name="paths"/> or any of its values are null.
		/// </exception>
		public static string Combine(string startPath, IEnumerable<string> paths) {
			paths = paths.Prepend(startPath);
			return string.Join(".", paths.Select(p => p.Trim('.')));
		}

		#endregion

		#region CombineIgnoreNull

		/// <summary>
		/// Combines the embedded paths with a '.' separating each part and trimming '.'s. Ignores all null
		/// strings.
		/// </summary>
		/// 
		/// <param name="paths">The paths to combine.</param>
		/// <returns>The combined resource path.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="paths"/> is null.
		/// </exception>
		public static string CombineIgnoreNull(params string[] paths) {
			return CombineIgnoreNull((IEnumerable<string>) paths);
		}

		/// <summary>
		/// Combines the embedded paths with a '.' separating each part and trimming '.'s. Ignores all null
		/// strings.
		/// </summary>
		/// 
		/// <param name="paths">The paths to combine.</param>
		/// <returns>The combined resource path.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="paths"/> is null.
		/// </exception>
		public static string CombineIgnoreNull(IEnumerable<string> paths) {
			paths = paths.Where(p => p != null);
			return string.Join(".", paths.Select(p => p.Trim('.')));
		}

		/// <summary>
		/// Combines the embedded paths with a '.' separating each part and trimming '.'s. Ignores all null
		/// strings.
		/// </summary>
		/// 
		/// <param name="startPath">The first path to add.</param>
		/// <param name="paths">The remaining paths to add.</param>
		/// <returns>The combined resource path.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="paths"/> is null.
		/// </exception>
		public static string CombineIgnoreNull(string startPath, IEnumerable<string> paths) {
			paths = paths.Prepend(startPath).Where(p => p != null);
			return string.Join(".", paths.Select(p => p.Trim('.')));
		}

		#endregion

#if NET462
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T value) {
			yield return value;
			foreach (T srcValue in source)
				yield return srcValue;
		}

		public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T value) {
			foreach (T srcValue in source)
				yield return srcValue;
			yield return value;
		}
#endif
		/// <summary>
		/// Reads the remaining bytes in the stream and advances the current position.
		/// </summary>
		/// 
		/// <param name="stream">The stream to read from.</param>
		/// <returns>A byte array with the remaining bytes.</returns>
		/// 
		/// <exception cref="NotSupportedException">The stream does not support reading.</exception>
		/// <exception cref="IOException">An I/O error occurred.</exception>
		/// <exception cref="ObjectDisposedException">The stream is closed.</exception>
		public static byte[] ReadToEnd(this Stream stream) {
			using (MemoryStream memoryStream = new MemoryStream()) {
				stream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}
		/// <summary>Reads the remaining bytes in the stream and advances the current position.</summary>
		/// 
		/// <param name="reader">The <see cref="BinaryReader"/> to read with.</param>
		/// <returns>A byte array with the remaining bytes.</returns>
		/// 
		/// <exception cref="IOException">An I/O error occurred.</exception>
		/// <exception cref="NotSupportedException">The stream does not support reading.</exception>
		/// <exception cref="ObjectDisposedException">The stream is closed.</exception>
		public static byte[] ReadToEnd(this BinaryReader reader) {
			return reader.BaseStream.ReadToEnd();
		}
	}
}
