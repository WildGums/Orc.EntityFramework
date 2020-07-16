﻿[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v3.1", FrameworkDisplayName="")]
public static class ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.EntityFramework
{
    public static class ConnectionStringHelper
    {
        public static string GetConnectionString(this System.Data.Entity.Core.Objects.ObjectContext objectContext) { }
        public static string GetConnectionString(this System.Data.Entity.DbContext dbContext) { }
        public static void SetConnectionString(this System.Data.Entity.Core.Objects.ObjectContext objectContext, string connectionString) { }
        public static void SetConnectionString(this System.Data.Entity.DbContext dbContext, string connectionString) { }
    }
    public class ConnectionStringManager : Orc.EntityFramework.IConnectionStringManager
    {
        public ConnectionStringManager() { }
        public virtual string GetConnectionString(System.Type contextType, string database, string label) { }
    }
    public class ContextFactory : Orc.EntityFramework.IContextFactory
    {
        public ContextFactory() { }
        public virtual object CreateContext(System.Type contextType, string databaseNameOrConnectionStringName, string label, System.Data.Entity.Infrastructure.DbCompiledModel model, System.Data.Entity.Core.Objects.ObjectContext context) { }
        public TContext CreateContext<TContext>(string databaseNameOrConnectionStringName, string label, System.Data.Entity.Infrastructure.DbCompiledModel model, System.Data.Entity.Core.Objects.ObjectContext context) { }
    }
    public abstract class ContextManager<TContext> : System.IDisposable
        where TContext :  class, System.IDisposable
    {
        protected ContextManager(string databaseNameOrConnectionStringName, string label, System.Data.Entity.Infrastructure.DbCompiledModel model, System.Data.Entity.Core.Objects.ObjectContext context) { }
        public TContext Context { get; }
        public int RefCount { get; }
        public void Dispose() { }
        protected abstract void Initialize(TContext context);
        protected static string GetContextLogName(string databaseNameOrConnectionStringName, string label) { }
        protected static string GetContextName(string databaseNameOrConnectionStringName, string label) { }
        protected static Orc.EntityFramework.ContextManager<TContext> GetManager(string databaseNameOrConnectionStringName, string label, System.Func<Orc.EntityFramework.ContextManager<TContext>> createContext) { }
    }
    public static class DbContextExtensions
    {
        public static bool ContainsEntityEntry(this System.Data.Entity.DbContext dbContext, object entity) { }
        public static bool ContainsEntityEntry(this System.Data.Entity.DbContext dbContext, object entity, System.Data.Entity.EntityState entityStates) { }
        public static System.Collections.Generic.List<System.Data.Entity.Infrastructure.DbEntityEntry> GetChangeTrackerEntries(this System.Data.Entity.DbContext dbContext) { }
        public static System.Collections.Generic.List<System.Data.Entity.Infrastructure.DbEntityEntry> GetChangeTrackerEntries(this System.Data.Entity.DbContext dbContext, System.Data.Entity.EntityState entityStates) { }
        public static System.Data.Entity.Infrastructure.DbEntityEntry GetEntityEntry(this System.Data.Entity.DbContext dbContext, object entity) { }
        public static System.Data.Entity.Infrastructure.DbEntityEntry GetEntityEntry(this System.Data.Entity.DbContext dbContext, object entity, System.Data.Entity.EntityState entityStates) { }
        public static System.Data.Entity.Core.EntityKey GetEntityKey(this System.Data.Entity.DbContext dbContext, System.Data.Entity.Infrastructure.DbEntityEntry dbEntityEntry) { }
        public static System.Data.Entity.Core.EntityKey GetEntityKey(this System.Data.Entity.DbContext dbContext, System.Type entityType, object keyValue) { }
        public static System.Data.Entity.Core.EntityKey GetEntityKey<TEntity>(this System.Data.Entity.DbContext dbContext, object keyValue) { }
        public static System.Data.Entity.Core.Metadata.Edm.EntitySet GetEntitySet(this System.Data.Entity.Core.Objects.ObjectContext objectContext, System.Type entityType) { }
        public static System.Data.Entity.Core.Metadata.Edm.EntitySet GetEntitySet(this System.Data.Entity.DbContext dbContext, System.Type entityType) { }
        public static string GetEntitySetName(this System.Data.Entity.DbContext dbContext, System.Type entityType) { }
        public static string GetEntitySetName<TEntity>(this System.Data.Entity.DbContext dbContext) { }
        public static string GetFullEntitySetName(this System.Data.Entity.DbContext dbContext, System.Type entityType) { }
        public static string GetFullEntitySetName<TEntity>(this System.Data.Entity.DbContext dbContext) { }
        public static System.Data.Entity.Core.Objects.ObjectContext GetObjectContext(this System.Data.Entity.DbContext dbContext) { }
        public static object GetObjectSet(this System.Data.Entity.Core.Objects.ObjectContext objectContext, System.Type entityType) { }
        public static object GetObjectSet(this System.Data.Entity.DbContext dbContext, System.Type entityType) { }
        public static string GetTableName(this System.Data.Entity.Core.Objects.ObjectContext context, System.Type entityType) { }
        public static string GetTableName(this System.Data.Entity.DbContext context, System.Type entityType) { }
        public static string GetTableName<TEntity>(this System.Data.Entity.DbContext context)
            where TEntity :  class { }
        public static void SetTransactionLevel(this System.Data.Entity.DbContext dbContext, System.Data.IsolationLevel isolationLevel) { }
    }
    public static class DbContextManagerHelper { }
    public class DbContextManager<TDbContext> : Orc.EntityFramework.ContextManager<TDbContext>
        where TDbContext : System.Data.Entity.DbContext
    {
        protected override void Initialize(TDbContext context) { }
        public static Orc.EntityFramework.DbContextManager<TDbContext> GetManager() { }
        public static Orc.EntityFramework.DbContextManager<TDbContext> GetManager(string databaseNameOrConnectionStringName, string label = "default", System.Data.Entity.Infrastructure.DbCompiledModel model = null) { }
    }
    public static class DbEntityEntryExtensions
    {
        public static object GetCurrentDbValue(this System.Data.Entity.Infrastructure.DbEntityEntry dbEntityEntry, string propertyName) { }
        public static object GetDbValue(this System.Data.Entity.Infrastructure.DbPropertyValues propertyValues, string propertyName) { }
        public static System.Type GetEntityType(this System.Data.Entity.Infrastructure.DbEntityEntry dbEntityEntry) { }
        public static System.Type GetEntityType(this System.Type type) { }
        public static System.Type GetEntityType(this object entity) { }
        public static object GetOriginalDbValue(this System.Data.Entity.Infrastructure.DbEntityEntry dbEntityEntry, string propertyName) { }
    }
    public static class EfConnectionStringHelper
    {
        public static string GetEntityFrameworkConnectionString(System.Type contextType, string connectionString) { }
    }
    public static class EntityTypeConfigurationExtensions
    {
        public static System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<TEntity> IgnoreCatelProperties<TEntity>(this System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<TEntity> configuration)
            where TEntity : Catel.Data.ModelBase { }
    }
    public interface IConnectionStringManager
    {
        string GetConnectionString(System.Type contextType, string database, string label);
    }
    public interface IContextFactory
    {
        object CreateContext(System.Type contextType, string databaseNameOrConnectionStringName, string label, System.Data.Entity.Infrastructure.DbCompiledModel model, System.Data.Entity.Core.Objects.ObjectContext context);
        TContext CreateContext<TContext>(string databaseNameOrConnectionStringName, string label, System.Data.Entity.Infrastructure.DbCompiledModel model, System.Data.Entity.Core.Objects.ObjectContext context);
    }
    public static class IEntityRepositoryExtensions
    {
        public static int Count<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate = null)
            where TEntity :  class { }
        public static System.Threading.Tasks.Task<int> CountAsync<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate = null)
            where TEntity :  class { }
        public static void Delete<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate)
            where TEntity :  class { }
        public static System.Linq.IQueryable<TEntity> Find<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate)
            where TEntity :  class { }
        public static TEntity First<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate = null)
            where TEntity :  class { }
        public static System.Threading.Tasks.Task<TEntity> FirstAsync<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate = null)
            where TEntity :  class { }
        public static TEntity FirstOrDefault<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate = null)
            where TEntity :  class { }
        public static System.Threading.Tasks.Task<TEntity> FirstOrDefaultAsync<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate = null)
            where TEntity :  class { }
        public static System.Linq.IQueryable<TEntity> GetAll<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository)
            where TEntity :  class { }
        public static System.Linq.Expressions.Expression<System.Func<TEntity, bool>> GetValidPredicate<TEntity>(this System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate) { }
        public static TEntity Single<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate = null)
            where TEntity :  class { }
        public static System.Threading.Tasks.Task<TEntity> SingleAsync<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate = null)
            where TEntity :  class { }
        public static TEntity SingleOrDefault<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate = null)
            where TEntity :  class { }
        public static System.Threading.Tasks.Task<TEntity> SingleOrDefaultAsync<TEntity>(this Orc.EntityFramework.Repositories.IEntityRepository<TEntity> repository, System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate = null)
            where TEntity :  class { }
    }
    public interface IUnitOfWork : System.IDisposable
    {
        bool IsInTransaction { get; }
        void BeginTransaction(System.Data.IsolationLevel isolationLevel = 4096);
        void CommitTransaction();
        System.Threading.Tasks.Task CommitTransactionAsync();
        TEntityRepository GetRepository<TEntityRepository>()
            where TEntityRepository : Orc.EntityFramework.Repositories.IEntityRepository;
        void Refresh(System.Data.Entity.Core.Objects.RefreshMode refreshMode, System.Collections.IEnumerable collection);
        void Refresh(System.Data.Entity.Core.Objects.RefreshMode refreshMode, object entity);
        System.Threading.Tasks.Task RefreshAsync(System.Data.Entity.Core.Objects.RefreshMode refreshMode, System.Collections.IEnumerable collection);
        System.Threading.Tasks.Task RefreshAsync(System.Data.Entity.Core.Objects.RefreshMode refreshMode, object entity);
        void RollBackTransaction();
        void SaveChanges();
        System.Threading.Tasks.Task SaveChangesAsync();
    }
    public static class IsolationHelper
    {
        public static string TranslateTransactionLevelToSql(System.Data.IsolationLevel isolationLevel) { }
    }
    public class ObjectContextManager<TObjectContext> : Orc.EntityFramework.ContextManager<TObjectContext>
        where TObjectContext : System.Data.Entity.Core.Objects.ObjectContext
    {
        protected override void Initialize(TObjectContext context) { }
        public static Orc.EntityFramework.ObjectContextManager<TObjectContext> GetManager() { }
        public static Orc.EntityFramework.ObjectContextManager<TObjectContext> GetManager(string databaseNameOrConnectionStringName, string label = "default") { }
    }
    public class UnitOfWork : Orc.EntityFramework.IUnitOfWork, System.IDisposable
    {
        public UnitOfWork(System.Data.Entity.DbContext context, string tag = null) { }
        protected System.Data.Entity.DbContext DbContext { get; }
        public bool IsInTransaction { get; }
        protected string Tag { get; }
        protected System.Data.Common.DbTransaction Transaction { get; set; }
        public virtual void BeginTransaction(System.Data.IsolationLevel isolationLevel = 4096) { }
        public virtual void CommitTransaction() { }
        public virtual System.Threading.Tasks.Task CommitTransactionAsync() { }
        public void Dispose() { }
        protected void DisposeDbContext() { }
        public virtual TEntityRepository GetRepository<TEntityRepository>()
            where TEntityRepository : Orc.EntityFramework.Repositories.IEntityRepository { }
        protected virtual void OnDisposing() { }
        protected virtual void OpenConnection() { }
        protected virtual System.Threading.Tasks.Task OpenConnectionAsync() { }
        public virtual void Refresh(System.Data.Entity.Core.Objects.RefreshMode refreshMode, System.Collections.IEnumerable collection) { }
        public virtual void Refresh(System.Data.Entity.Core.Objects.RefreshMode refreshMode, object entity) { }
        public virtual System.Threading.Tasks.Task RefreshAsync(System.Data.Entity.Core.Objects.RefreshMode refreshMode, System.Collections.IEnumerable collection) { }
        public virtual System.Threading.Tasks.Task RefreshAsync(System.Data.Entity.Core.Objects.RefreshMode refreshMode, object entity) { }
        protected virtual void ReleaseTransaction() { }
        public virtual void RollBackTransaction() { }
        public virtual void SaveChanges() { }
        public virtual System.Threading.Tasks.Task SaveChangesAsync() { }
    }
    public class UnitOfWork<TDbContext> : Orc.EntityFramework.UnitOfWork
        where TDbContext : System.Data.Entity.DbContext
    {
        public UnitOfWork(TDbContext dbContext = default) { }
        protected override void OnDisposing() { }
    }
}
namespace Orc.EntityFramework.Repositories
{
    public class EntityRepositoryBase<TEntity, TPrimaryKey> : Orc.EntityFramework.Repositories.IEntityRepository, Orc.EntityFramework.Repositories.IEntityRepository<TEntity>, Orc.EntityFramework.Repositories.IEntityRepository<TEntity, TPrimaryKey>, System.IDisposable
        where TEntity :  class
    {
        public EntityRepositoryBase(System.Data.Entity.DbContext dbContext) { }
        public virtual void Add(TEntity entity) { }
        public virtual void Attach(TEntity entity) { }
        public virtual TEntity Create() { }
        public virtual void Delete(TEntity entity) { }
        public void Dispose() { }
        public virtual TEntity GetByKey(TPrimaryKey keyValue) { }
        public virtual System.Linq.IQueryable<TEntity> GetQuery() { }
        public virtual System.Linq.IQueryable<TEntity> GetQuery(System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate) { }
        public virtual void Update(TEntity entity) { }
    }
    public interface IEntityRepository : System.IDisposable { }
    public interface IEntityRepository<TEntity> : Orc.EntityFramework.Repositories.IEntityRepository, System.IDisposable
        where TEntity :  class
    {
        void Add(TEntity entity);
        void Attach(TEntity entity);
        TEntity Create();
        void Delete(TEntity entity);
        System.Linq.IQueryable<TEntity> GetQuery();
        System.Linq.IQueryable<TEntity> GetQuery(System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate);
        void Update(TEntity entity);
    }
    public interface IEntityRepository<TEntity, TPrimaryKey> : Orc.EntityFramework.Repositories.IEntityRepository, Orc.EntityFramework.Repositories.IEntityRepository<TEntity>, System.IDisposable
        where TEntity :  class
    {
        TEntity GetByKey(TPrimaryKey keyValue);
    }
}