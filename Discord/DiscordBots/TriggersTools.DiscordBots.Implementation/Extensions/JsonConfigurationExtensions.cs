using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration {
	public static class JsonConfigurationExtensions {
		public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder,
			Assembly assembly, string baseNamespace, string path)
		{
			return builder.AddJsonFile(new EmbeddedFileProvider(assembly, baseNamespace), path);
		}
		public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder,
			Assembly assembly, string path)
		{
			return builder.AddJsonFile(new EmbeddedFileProvider(assembly), path);
		}
		public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder,
			Type assemblyType, string path)
		{
			return builder.AddJsonFile(new EmbeddedFileProvider(assemblyType.Assembly, assemblyType.Namespace), path);
		}
		public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder,
			IFileProvider provider, string path)
		{
			return builder.AddJsonFile(provider, path, false, false);
		}
	}
}
