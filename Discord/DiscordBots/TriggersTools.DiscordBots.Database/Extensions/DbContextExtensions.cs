using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TriggersTools.DiscordBots.Extensions {
	/// <summary>
	/// Extensions methods for the <see cref="DbContext"/> class.
	/// </summary>
	public static class DbContextExtensions {
		/// <summary>
		/// Marks only this entity's property as modified. All other properties are marked as unchanged.
		/// </summary>
		/// <param name="db">The database context containing this entity.</param>
		/// <param name="entity">The entity to mark the property of.</param>
		/// <param name="propertyName">The name of the property.</param>
		public static void ModifyOnly(this DbContext db, object entity, string propertyName) {
			db.Attach(entity).Property(propertyName).IsModified = true;
		}
		/// <summary>
		/// Marks only this entity's property as modified. All other properties are marked as unchanged.
		/// </summary>
		/// <param name="db">The database context containing this entity.</param>
		/// <param name="entity">The entity to mark the property of.</param>
		/// <param name="propertyExpression">The expression of the property.</param>
		public static void ModifyOnly<TEntity, TProperty>(this DbContext db, TEntity entity,
			Expression<Func<TEntity, TProperty>> propertyExpression)
			where TEntity : class
		{
			db.Attach(entity).Property(propertyExpression).IsModified = true;
		}

		/// <summary>
		/// Marks this entity's property as modified. <see cref="DbContext.Attach"/> must be called first.
		/// </summary>
		/// <param name="db">The database context containing this entity.</param>
		/// <param name="entity">The entity to mark the property of.</param>
		/// <param name="propertyName">The name of the property.</param>
		public static void Modify(this DbContext db, object entity, string propertyName) {
			db.Entry(entity).Property(propertyName).IsModified = true;
		}
		/// <summary>
		/// Marks this entity's property as modified. <see cref="DbContext.Attach"/> must be called first.
		/// </summary>
		/// <param name="db">The database context containing this entity.</param>
		/// <param name="entity">The entity to mark the property of.</param>
		/// <param name="propertyExpression">The expression of the property.</param>
		public static void Modify<TEntity, TProperty>(this DbContext db, TEntity entity,
			Expression<Func<TEntity, TProperty>> propertyExpression)
			where TEntity : class
		{
			db.Entry(entity).Property(propertyExpression).IsModified = true;
		}

		/*public static DbSet<TEntity> AsNoTracking<TEntity>(DbSet<TEntity> dbset, bool condition)
			where TEntity : class
		{
			dbset.AsNoTracking<TEntity>();
		}*/
	}
}
