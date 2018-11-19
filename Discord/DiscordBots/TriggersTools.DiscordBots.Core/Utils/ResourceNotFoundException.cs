using System;
using System.IO;
using System.Reflection;
using static TriggersTools.DiscordBots.Utils.Embedding;

namespace TriggersTools.DiscordBots.Utils {
	/// <summary>An exception thrown, when an embedded resource could not be located.</summary>
	public class ResourceNotFoundException : IOException {
		/// <summary>The path that did not lead to a resource.</summary>
		public string Path { get; }
		/// <summary>The optional assembly containg this resource.</summary>
		public Assembly Assembly { get; }
		/// <summary>The optional type associated with this resource.</summary>
		public Type Type { get; }

		/// <summary>Constructs the exception message with the path.</summary>
		/// 
		/// <param name="assembly">The assembly the resource was looked for in.</param>
		/// <param name="paths">The resource path.</param>
		public ResourceNotFoundException(string path)
			: base($"Resource with the path '{path}' could not be found!")
		{
			Path = path;
		}

		/// <summary>Constructs the exception message with the path and assembly.</summary>
		/// 
		/// <param name="paths">The resource path.</param>
		public ResourceNotFoundException(Assembly assembly, string path)
			: base($"Resource with the path '{path}' could not be found in " +
				  $"assembly '{assembly.GetName().Name}`!")
		{
			Assembly = assembly;
			Path = path;
		}

		/// <summary>Constructs the exception message with the combined paths.</summary>
		/// 
		/// <param name="paths">The combined resource path.</param>
		public ResourceNotFoundException(params string[] paths)
			: base($"Resource with the path '{Combine(paths)}' could not be " +
				  $"found!")
		{
			Path = Combine(paths);
		}

		/// <summary>Constructs the exception message with the combined paths and assembly.</summary>
		/// 
		/// <param name="assembly">The assembly the resource was looked for in.</param>
		/// <param name="paths">The combined resource path.</param>
		public ResourceNotFoundException(Assembly assembly, params string[] paths)
			: base($"Resource with the path '{Combine(paths)}' could not be " +
				  $"found in assembly '{assembly.GetName().Name}`!")
		{
			Assembly = assembly;
			Path = Combine(paths);
		}

		/// <summary>Constructs the exception message with the type and name.</summary>
		/// 
		/// <param name="type">The type whose namespace is used to look for the resource.</param>
		/// <param name="name">The name of the resource.</param>
		/// 
		/// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
		public ResourceNotFoundException(Type type, string name)
			: base($"Resource with the path '{Combine(type.Namespace, name)}' could " +
				  $"not be found in assembly '{type.Assembly.GetName().Name}`!")
		{
			Path = Combine(type.FullName, name);
			Assembly = type.Assembly;
			Type = type;
		}
	}
}
