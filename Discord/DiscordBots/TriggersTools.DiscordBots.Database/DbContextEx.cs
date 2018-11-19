using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TriggersTools.DiscordBots.Database.Model;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// The base bot database context.
	/// </summary>
	public partial class DbContextEx : DbContext {

		#region Fields

		/// <summary>
		/// True if the <see cref="DbContextEx.OnConfiguring(DbContextOptionsBuilder)"/> method has already
		/// been run.
		/// </summary>
		private bool modelCreated;
		/// <summary>
		/// The override configuration type for the database context.
		/// </summary>
		private string configurationType;
		/// <summary>
		/// True if all encryption and decryption is disabled.
		/// </summary>
		private bool disableEncryption;
		
		/// <summary>
		/// The database provider and configurer.
		/// </summary>
		public IDbProvider DbProvider { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="DiscordDbContext"/>.
		/// </summary>
		/// <param name="services">The service provider.</param>
		/// <param name="dbConfiguration">The bot database configuration service.</param>
		public DbContextEx(IDbProvider dbProvider) {
			DbProvider = dbProvider;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets if the <see cref="DbContextEx.OnConfiguring(DbContextOptionsBuilder)"/> method has already
		/// been run.
		/// </summary>
		public bool IsModelCreated => modelCreated;
		/// <summary>
		/// Gets or sets the override configuration type for the database context.
		/// </summary>
		/// 
		/// <exception cref="InvalidOperationException">
		/// Model has already been created.
		/// </exception>
		public string ConfigurationType {
			get => configurationType;
			set {
				ThrowIfModelCreated();
				configurationType = value;
			}
		}
		/// <summary>
		/// Gets or sets if all encryption and decryption is disabled.
		/// </summary>
		/// 
		/// <exception cref="InvalidOperationException">
		/// Model has already been created.
		/// </exception>
		public bool DisableEncryption {
			get => disableEncryption;
			set {
				ThrowIfModelCreated();
				disableEncryption = value;
			}
		}

		#endregion

		#region Virtual Properties

		/// <summary>
		/// Gets the configuration context type name to lookup in the config file.<para/>
		/// By default this is just the type name.
		/// </summary>
		public virtual string ContextType => GetType().Name;

		#endregion

		#region Override Methods
		
		/// <summary>
		/// Override this method to further configure the model that was discovered by convention from the
		/// entity types exposed in <see cref="DbSet{TEntity}"/> properties on your derived context. The
		/// resulting model may be cached and re-used for subsequent instances of your derived context.
		/// </summary>
		/// <param name="modelBuilder">
		/// The builder being used to construct the model for this context. Databases (and other extensions)
		/// typically define extension methods on this object that allow you to configure aspects of the
		/// model that are specific to a given database.
		/// </param>
		/// <remarks>
		/// If a model is explicitly set on the options for this context (via
		/// <see cref="DbContextOptionsBuilder.UseModel(IModel)"/>) then this method will not be run.
		/// </remarks>
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelCreated = true;
			DbProvider.ModelCreating(modelBuilder, this);
			base.OnModelCreating(modelBuilder);
		}
		/// <summary>
		/// Override this method to configure the database (and other options) to be used for this context.
		/// This method is called for each instance of the context that is created. The base implementation
		/// does nothing. In situations where an instance of <see cref="DbContextOptions"/> may or may not
		/// have been passed to the constructor, you can use <see cref="DbContextOptionsBuilder.IsConfigured"
		/// /> to determine if the options have already been set, and skip some or all of the logic in
		/// <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
		/// </summary>
		/// <param name="optionsBuilder">
		/// A builder used to create or modify options for this context. Databases (and other extensions)
		/// typically define extension methods on this object that allow you to configure the context.
		/// </param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			DbProvider.Configure(optionsBuilder, this);
			base.OnConfiguring(optionsBuilder);
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Throws a <see cref="InvalidOperationException"/> if the model has already been created.
		/// </summary>
		/// 
		/// <exception cref="InvalidOperationException">
		/// Model has already been created.
		/// </exception>
		protected void ThrowIfModelCreated() {
			if (IsModelCreated)
				throw new InvalidOperationException("Model has already been created!");
		}

		#endregion

		#region LoadCollectionsAsync

		/// <summary>
		/// Asynchronously loads all collections for the entity.
		/// </summary>
		/// <param name="entity">The entity to load the collections for.</param>
		public async Task LoadAllCollectionsAsync(object entity) {
			PropertyInfo[] properties = DbReflection.GetPropertiesOfListType(entity.GetType());
			var entry = Entry(entity);
			foreach (PropertyInfo prop in properties) {
				await entry.Collection(prop.Name).LoadAsync().ConfigureAwait(false);
			}
		}
		/// <summary>
		/// Asynchronously loads all collections for the entity.
		/// </summary>
		/// <typeparam name="T">The type of the entity.</typeparam>
		/// <param name="entity">The entity to load the collections for.</param>
		public async Task LoadAllCollectionsAsync<T>(T entity) where T : class {
			PropertyInfo[] properties = DbReflection.GetPropertiesOfListType(entity.GetType());
			var entry = Entry(entity);
			foreach (PropertyInfo prop in properties) {
				await entry.Collection(prop.Name).LoadAsync().ConfigureAwait(false);
			}
		}

		protected async Task<bool> RemoveEndUserDataBase<T>(DbSet<T> dbSet, ulong id)
			where T : class
		{
			T eud = await dbSet.FindAsync(id).ConfigureAwait(false);
			if (eud == null)
				return false;
			await LoadAllCollectionsAsync(eud).ConfigureAwait(false);
			int count = DbReflection.ClearAllLists(eud);
			dbSet.Remove(eud);
			return count > 0;
		}

		#endregion
	}
}
