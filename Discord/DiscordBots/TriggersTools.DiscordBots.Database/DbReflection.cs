using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TriggersTools.DiscordBots.Extensions;

namespace TriggersTools.DiscordBots.Database {
	public static class DbReflection {

		public static IEnumerable<TableReflectionInfo> GetTables(DbContext db) {
			foreach (PropertyInfo prop in db.GetType().GetProperties()) {
				Type propType = prop.PropertyType;
				if (IsTable(propType))
					yield return new TableReflectionInfo(prop, db);
			}
		}
		public static IEnumerable<MultiTableReflectionInfo> GetTables(params DbContext[] dbs) {
			if (dbs.Length == 0)
				yield break;
			HashSet<Type> types = new HashSet<Type>();
			if (dbs.Any(db1 => dbs.Any(db2 => db1.GetType() != db2.GetType())))
				throw new ArgumentException("DbContexts must all be of the same type!");
			foreach (PropertyInfo prop in dbs[0].GetType().GetProperties()) {
				Type propType = prop.PropertyType;
				if (IsTable(propType))
					yield return new MultiTableReflectionInfo(prop, dbs);
			}
		}

		public static Type[] GetTableEntityTypes(Type dbType) {
			HashSet<Type> entityTypes = new HashSet<Type>();
			foreach (PropertyInfo prop in dbType.GetProperties()) {
				Type propType = prop.PropertyType;
				if (IsTable(propType))
					entityTypes.Add(prop.PropertyType.GetGenericArguments()[0]);
			}
			return entityTypes.ToArray();
		}
		public static Dictionary<Type, PropertyInfo[]> GetEntityTypesAndMappedProperties(Type dbType) {
			Dictionary<Type, PropertyInfo[]> entityProperties = new Dictionary<Type, PropertyInfo[]>();
			foreach (PropertyInfo prop in dbType.GetProperties()) {
				Type propType = prop.PropertyType;
				if (IsTable(propType)) {
					Type entityType = prop.PropertyType.GetGenericArguments()[0];
					TraverseEntityTypesAndMappedProperties(entityType, entityProperties);
				}
			}
			return entityProperties;
		}
		private static void TraverseEntityTypesAndMappedProperties(Type type, Dictionary<Type, PropertyInfo[]> entityProperties) {
			if (entityProperties.ContainsKey(type))
				return;
			entityProperties.Add(type, null);
			List<PropertyInfo> properties = new List<PropertyInfo>();
			foreach (PropertyInfo prop in type.GetProperties()) {
				Type propType = prop.PropertyType;
				if (prop.GetMethod == null || prop.SetMethod == null || prop.IsDefined<NotMappedAttribute>())
					continue;
				if (prop.IsDefined<InversePropertyAttribute>() || prop.IsDefined<ForeignKeyAttribute>()) {
					if (propType.IsGenericType) {
						Type entityType = prop.PropertyType.GetGenericArguments()[0];
						TraverseEntityTypesAndMappedProperties(entityType, entityProperties);
					}
					else {
						TraverseEntityTypesAndMappedProperties(propType, entityProperties);
					}
				}
				properties.Add(prop);
			}
			entityProperties[type] = properties.ToArray();
		}

		public static PropertyInfo[] GetEncryptedProperties(Type entityType) {
			List<PropertyInfo> properties = new List<PropertyInfo>();
			foreach (PropertyInfo prop in entityType.GetProperties()) {
				if (prop.IsDefined<EncryptedAttribute>() && !prop.IsDefined<NotMappedAttribute>())
					properties.Add(prop);
			}
			return properties.ToArray();
		}

		public static PropertyInfo[] GetPropertiesOfListType(Type entityType) {
			List<PropertyInfo> properties = new List<PropertyInfo>();
			foreach (PropertyInfo prop in entityType.GetProperties()) {
				Type propType = prop.PropertyType;
				if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>))
					properties.Add(prop);
			}
			return properties.ToArray();
		}

		public static int ClearAllLists(object entity) {
			int count = 0;
			foreach (PropertyInfo prop in entity.GetType().GetProperties()) {
				Type propType = prop.PropertyType;
				if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>)) {
					IList list = (IList) prop.GetValue(entity);
					count += list.Count;
					list.Clear();
				}
			}
			return count;
		}

		public static IEnumerable<TableReflectionInfo> GetTablesOfBaseTypes(DbContext db,
			params Type[] types)
		{
			return GetTables(db).WhereTablesAreOfBaseTypes(types);
		}
		public static IEnumerable<MultiTableReflectionInfo> GetTablesOfBaseTypes(
			DbContext db1, DbContext db2, params Type[] types)
		{
			return GetTables(db1, db2).WhereTablesAreOfBaseTypes(types);
		}

		public static IEnumerable<TableReflectionInfo> WhereTablesAreOfBaseTypes(
			this IEnumerable<TableReflectionInfo> source, params Type[] types)
		{
			return source.Where(table => types.Any(type => type.IsAssignableFrom(table.EntityType)));
		}
		public static IEnumerable<MultiTableReflectionInfo> WhereTablesAreOfBaseTypes(
			this IEnumerable<MultiTableReflectionInfo> source, params Type[] types)
		{
			return source.Where(table => types.Any(type => type.IsAssignableFrom(table.EntityType)));
		}

		public static bool IsTable(Type propType) {
			return propType.IsGenericType &&
					propType.GetGenericTypeDefinition() == typeof(DbSet<>);
		}

		public static bool IsTableOfType(Type propType, Type entityType) {
			return IsTable(propType) && entityType.IsAssignableFrom(propType.GetGenericArguments()[0]);
		}
	}

	public class TableReflectionInfo {
		public string PropertyName { get; }
		public string TableName { get; }
		public IQueryable Table { get; }
		public Type EntityType { get; }
		
		public TableReflectionInfo(PropertyInfo prop, DbContext db) {
			var attr = prop.GetCustomAttribute<TableAttribute>();
			TableName = attr?.Name ?? prop.Name;
			PropertyName = prop.Name;
			Table = (IQueryable) prop.GetValue(db);
			EntityType = prop.PropertyType.GetGenericArguments()[0];
		}

		public MethodInfo MakeGenericMethod(MethodInfo method) {
			return method.MakeGenericMethod(EntityType);
		}
	}
	public class MultiTableReflectionInfo {
		public string PropertyName { get; }
		public string TableName { get; }
		public IQueryable[] Tables { get; }
		public Type EntityType { get; }

		public MultiTableReflectionInfo(PropertyInfo prop, params DbContext[] dbs) {
			var attr = prop.GetCustomAttribute<TableAttribute>();
			TableName = attr?.Name ?? prop.Name;
			PropertyName = prop.Name;
			Tables = dbs.Select(db => (IQueryable) prop.GetValue(db)).ToArray();
			EntityType = prop.PropertyType.GetGenericArguments()[0];
		}

		public MethodInfo MakeGenericMethod(MethodInfo method) {
			return method.MakeGenericMethod(EntityType);
		}
	}
}
