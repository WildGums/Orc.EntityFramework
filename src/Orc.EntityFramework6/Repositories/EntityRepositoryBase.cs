namespace Orc.EntityFramework.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    using Catel;
    using EntityKey = System.Data.Entity.Core.EntityKey;

    /// <summary>
    /// Base class for entity repositories.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
#pragma warning disable IDISP025 // Class with no virtual dispose method should be sealed.
    public class EntityRepositoryBase<TEntity, TPrimaryKey> : IEntityRepository<TEntity, TPrimaryKey>
#pragma warning restore IDISP025 // Class with no virtual dispose method should be sealed.
        where TEntity : class
    {
        private readonly DbContext _dbContext;

        private readonly string _entitySetName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepositoryBase{TEntity, TPrimaryKey}" /> class.
        /// </summary>
        /// <param name="dbContext">The db context. The caller is responsible for correctly disposing the <see cref="DbContext"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        public EntityRepositoryBase(DbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext);

            _dbContext = dbContext;

            _entitySetName = dbContext.GetEntitySetName<TEntity>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets a specific entity by it's primary key value.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        /// <returns>The entity or <c>null</c> if the entity could not be found.</returns>
        public virtual TEntity? GetByKey(TPrimaryKey keyValue)
        {
            ArgumentNullException.ThrowIfNull(keyValue);

            var key = _dbContext.GetEntityKey<TEntity>(keyValue);
            var objectContext = _dbContext.GetObjectContext();

            if (objectContext.TryGetObjectByKey(key, out var originalItem))
            {
                return (TEntity?)originalItem;
            }

            return null;
        }

        /// <summary>
        /// Gets the default query for this repository.
        /// </summary>
        /// <returns>The default queryable for this repository.</returns>
        public virtual IQueryable<TEntity> GetQuery()
        {
            var objectContext = _dbContext.GetObjectContext();
            return objectContext.CreateQuery<TEntity>(_entitySetName);
        }

        /// <summary>
        /// Gets a customized query for this repository.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The customized queryable for this repository.</returns>
        public virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>>? predicate)
        {
            var query = GetQuery();

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            return query;
        }

        /// <summary>
        /// Gets an new entity instance, which may be a proxy if the entity meets the proxy requirements and the underlying context is configured to create proxies.
        /// <para />
        /// Note that the returned proxy entity is NOT added or attached to the set.
        /// </summary>
        /// <returns>The proxy entity</returns>
        public virtual TEntity Create()
        {
            return _dbContext.Set<TEntity>().Create();
        }

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is <c>null</c>.</exception>
        public virtual void Add(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbContext.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Attaches the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public virtual void Attach(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbContext.Set<TEntity>().Attach(entity);
        }

        /// <summary>
        /// Deletes the specified entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public virtual void Delete(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbContext.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Updates changes of the existing entity.
        /// <para />
        /// Note that this method does not actually call <c>SaveChanges</c>, but only updates the entity in the repository.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public virtual void Update(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var objectContext = _dbContext.GetObjectContext();

            var key = objectContext.CreateEntityKey(_entitySetName, entity);

            if (objectContext.TryGetObjectByKey(key, out _))
            {
                objectContext.ApplyCurrentValues(key.EntitySetName, entity);
            }
        }
    }
}
