using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace TriggersTools.DiscordBots.Extensions {
	/// <summary>
	/// Extensions for dependency injection services.
	/// </summary>
	public static class ServiceExtensions {
		
		/// <summary>
		/// Filters the service descriptors to only valid services.
		/// </summary>
		/// <param name="source">The enumerable source.</param>
		/// <param name="provider">The service provider.</param>
		/// <returns>A filtered enumerable of service descriptors.</returns>
		public static IEnumerable<object> WhereValidServices(this IEnumerable<ServiceDescriptor> source,
			IServiceProvider provider)
		{
			foreach (ServiceDescriptor sd in source) {
				object service = null;
				//try {
					service = provider.GetService(sd.ServiceType);
					if (service == null)
						continue;
				/*} catch {
					continue;
				}*/
				yield return service;
			}
		}
		/// <summary>
		/// Filters the service descriptors to only valid services.
		/// </summary>
		/// <typeparam name="T">The type to cast all returned services to.</typeparam>
		/// <param name="source">The enumerable source.</param>
		/// <param name="provider">The service provider.</param>
		/// <returns>A filtered enumerable of service descriptors.</returns>
		public static IEnumerable<T> WhereValidServices<T>(this IEnumerable<ServiceDescriptor> source,
			IServiceProvider provider)
		{
			foreach (ServiceDescriptor sd in source) {
				object service = null;
				//try {
					service = provider.GetRequiredService(sd.ServiceType);
					if (service == null)
						continue;
				/*} catch {
					continue;
				}*/
				yield return (T) service;
			}
		}

		
		/// <summary>
		/// Gets all services.
		/// </summary>
		/// <param name="services">The service collection to search through.</param>
		/// <param name="provider">The service provider attached to the service collection.</param>
		/// <returns>The collection of matching services.</returns>
		public static IEnumerable<object> GetServices(this IServiceCollection services,
			IServiceProvider provider)
		{
			return services
				.Where(sd => !sd.ServiceType.IsGenericTypeDefinition)
				.WhereValidServices(provider);
		}
		/// <summary>
		/// Gets all services of the specified <paramref name="lifetime"/>.
		/// </summary>
		/// <param name="services">The service collection to search through.</param>
		/// <param name="provider">The service provider attached to the service collection.</param>
		/// <param name="lifetime">The required lifetime of the service. Null for any type.</param>
		/// <returns>The collection of matching services.</returns>
		public static IEnumerable<object> GetServices(this IServiceCollection services,
			IServiceProvider provider, ServiceLifetime lifetime)
		{
			return services
				.Where(sd => !sd.ServiceType.IsGenericTypeDefinition)
				.Where(sd => sd.Lifetime == lifetime)
				.WhereValidServices(provider);
		}

		/// <summary>
		/// Gets all services of the specified base type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The base service type.</typeparam>
		/// <param name="services">The service collection to search through.</param>
		/// <param name="provider">The service provider attached to the service collection.</param>
		/// <returns>The collection of matching services.</returns>
		public static IEnumerable<T> GetServices<T>(this IServiceCollection services,
			IServiceProvider provider) {
			return services
				.Where(sd => !sd.ServiceType.IsGenericTypeDefinition)
				.Where(sd => typeof(T).IsAssignableFrom(sd.ImplementationType))
				.WhereValidServices<T>(provider);
		}
		/// <summary>
		/// Gets all services of the specified base type <typeparamref name="T"/> and of the specified
		/// <paramref name="lifetime"/>.
		/// </summary>
		/// <typeparam name="T">The base service type.</typeparam>
		/// <param name="services">The service collection to search through.</param>
		/// <param name="provider">The service provider attached to the service collection.</param>
		/// <param name="lifetime">The required lifetime of the service. Null for any type.</param>
		/// <returns>The collection of matching services.</returns>
		public static IEnumerable<T> GetServices<T>(this IServiceCollection services,
			IServiceProvider provider, ServiceLifetime lifetime)
		{
			return services
				.Where(sd => !sd.ServiceType.IsGenericTypeDefinition)
				.Where(sd => sd.Lifetime == lifetime)
				.Where(sd => typeof(T).IsAssignableFrom(sd.ImplementationType))
				.WhereValidServices<T>(provider);
		}
	}
}
