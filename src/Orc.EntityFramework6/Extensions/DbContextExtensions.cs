﻿namespace Orc.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    
    using System.Text.RegularExpressions;

    using Catel;
    using Catel.Caching;
    using Catel.Logging;
    using Catel.Reflection;

    using ObjectContext = System.Data.Entity.Core.Objects.ObjectContext;
    using DataSpace = System.Data.Entity.Core.Metadata.Edm.DataSpace;
    using EntityKey = System.Data.Entity.Core.EntityKey;
    using EntityKeyMember = System.Data.Entity.Core.EntityKeyMember;
    using EntitySet = System.Data.Entity.Core.Metadata.Edm.EntitySet;
    using EntityState = System.Data.Entity.EntityState;

    /// <summary>
    /// Extensions to the <see cref="DbContext"/> class.
    /// </summary>
    public static partial class DbContextExtensions
    {
        private static readonly ICacheStorage<Tuple<Type, Type>, string[]> _entityKeyCache = new CacheStorage<Tuple<Type, Type>, string[]>();
        private static readonly ICacheStorage<Tuple<Type, Type>, string?> _entityKeyPropertyNameCache = new CacheStorage<Tuple<Type, Type>, string?>();
        private static readonly ICacheStorage<Tuple<Type, Type>, string> _entitySetNameCache = new CacheStorage<Tuple<Type, Type>, string>();
        private static readonly ICacheStorage<Type, string> _tableNameCache = new CacheStorage<Type, string>();

        /// <summary>
        /// Determines whether the specified database context contains the specified entity. It does this by checking all
        /// the entries inside the change tracker of the database context.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if the specified database context contains entity; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public static DbEntityEntry? GetEntityEntry(this DbContext dbContext, object entity)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(entity);

            var entries = dbContext.GetChangeTrackerEntries();

            var entityEntry = entries.FirstOrDefault(x => ReferenceEquals(x, entity));
            return entityEntry;
        }

        /// <summary>
        /// Determines whether the specified database context contains the specified entity. It does this by checking all
        /// the entries inside the change tracker of the database context.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="entityStates">The entity states.</param>
        /// <returns><c>true</c> if the specified database context contains entity; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public static DbEntityEntry? GetEntityEntry(this DbContext dbContext, object entity, EntityState entityStates)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(entity);

            var entries = dbContext.GetChangeTrackerEntries(entityStates);

            var entityEntry = entries.FirstOrDefault(x => ReferenceEquals(x, entity));
            return entityEntry;
        }

        /// <summary>
        /// Determines whether the specified database context contains the specified entity. It does this by checking all
        /// the entries inside the change tracker of the database context.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if the specified database context contains entity; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public static bool ContainsEntityEntry(this DbContext dbContext, object entity)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(entity);

            return GetEntityEntry(dbContext, entity) is not null;
        }

        /// <summary>
        /// Determines whether the specified database context contains the specified entity. It does this by checking all
        /// the entries inside the change tracker of the database context.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="entityStates">The entity states.</param>
        /// <returns><c>true</c> if the specified database context contains entity; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public static bool ContainsEntityEntry(this DbContext dbContext, object entity, EntityState entityStates)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(entity);

            return GetEntityEntry(dbContext, entity, entityStates) is not null;
        }

        /// <summary>
        /// Gets the change tracker entries of the database context.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>List&lt;DbEntityEntry&gt;.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        public static List<DbEntityEntry> GetChangeTrackerEntries(this DbContext dbContext)
        {
            var entries = (from entry in dbContext.ChangeTracker.Entries()
                           select entry).ToList();

            return entries;
        }

        /// <summary>
        /// Gets the change tracker entries of the database context.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="entityStates">The entity states.</param>
        /// <returns>List&lt;DbEntityEntry&gt;.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        public static List<DbEntityEntry> GetChangeTrackerEntries(this DbContext dbContext, EntityState entityStates)
        {
            var states = new List<EntityState>();

            foreach (var value in Enum<EntityState>.GetValues())
            {
                if (Enum<EntityState>.Flags.IsFlagSet(entityStates, value))
                {
                    states.Add(value);
                }
            }

            var entries = (from entry in dbContext.ChangeTracker.Entries()
                           where states.Contains(entry.State)
                           select entry).ToList();

            return entries;
        }

        /// <summary>
        /// Gets the object context from the specified <see cref="DbContext"/>.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <returns>The ObjectContext.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext"/> is <c>null</c>.</exception>
        public static ObjectContext GetObjectContext(this DbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext);

            return ((IObjectContextAdapter)dbContext).ObjectContext;
        }

        /// <summary>
        /// Gets the entity key of the specified entity type in the <see cref="DbContext" />.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="dbEntityEntry">Type of the entity.</param>
        /// <returns>The entity key.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dbEntityEntry"/> is <c>null</c>.</exception>
        public static EntityKey GetEntityKey(this DbContext dbContext, DbEntityEntry dbEntityEntry)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(dbEntityEntry);

            var entityType = dbEntityEntry.GetRequiredEntityType();

            var keySet = _entityKeyCache.GetFromCacheOrFetch(new Tuple<Type, Type>(dbContext.GetType(), entityType), () =>
            {
                var entitySet = dbContext.GetEntitySet(entityType);
                if (entitySet is null)
                {
                    return Array.Empty<string>();
                }

                return (from keyMember in entitySet.ElementType.KeyMembers
                        select keyMember.Name).ToArray();
            });

            var entitySetName = dbContext.GetFullEntitySetName(entityType);
            var currentValues = dbEntityEntry.CurrentValues;

            var keys = new List<EntityKeyMember>();

            foreach (var keySetItem in keySet)
            {
                keys.Add(new EntityKeyMember
                {
                    Key = keySetItem,
                    Value = currentValues[keySetItem]
                });
            }

            var entityKey = new EntityKey(entitySetName, keys.ToArray());
            return entityKey;
        }

        /// <summary>
        /// Gets the entity key of the specified entity type in the <see cref="DbContext"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the T entity.</typeparam>
        /// <param name="dbContext">The db context.</param>
        /// <param name="keyValue">The key value.</param>
        /// <returns>The entity key.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="keyValue"/> is <c>null</c>.</exception>
        public static EntityKey GetEntityKey<TEntity>(this DbContext dbContext, object keyValue)
        {
            return GetEntityKey(dbContext, typeof(TEntity), keyValue);
        }

        /// <summary>
        /// Gets the entity key of the specified entity type in the <see cref="DbContext" />.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="keyValue">The key value.</param>
        /// <returns>The entity key.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entityType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="keyValue"/> is <c>null</c>.</exception>
        public static EntityKey GetEntityKey(this DbContext dbContext, Type entityType, object keyValue) 
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(entityType);
            ArgumentNullException.ThrowIfNull(keyValue);

            var keyPropertyName = _entityKeyPropertyNameCache.GetFromCacheOrFetch(new Tuple<Type, Type>(dbContext.GetType(), entityType), () =>
            {
                var entitySet = GetRequiredEntitySet(dbContext, entityType);
                return entitySet.ElementType.KeyMembers[0].ToString();
            });

            var entitySetName = GetFullEntitySetName(dbContext, entityType);

            var entityKey = new EntityKey(entitySetName, new[] { new EntityKeyMember(keyPropertyName, keyValue) });
            return entityKey;
        }

        /// <summary>
        /// Gets the name of the entity set in the <see cref="DbContext"/> for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="dbContext">The db context.</param>
        /// <returns>The name of the entity set.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext"/> is <c>null</c>.</exception>
        public static string GetEntitySetName<TEntity>(this DbContext dbContext)
        {
            return GetEntitySetName(dbContext, typeof(TEntity));
        }

        /// <summary>
        /// Gets the name of the entity set in the <see cref="DbContext" /> for the specified entity type.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The name of the entity set.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entityType" /> is <c>null</c>.</exception>
        public static string GetEntitySetName(this DbContext dbContext, Type entityType)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(entityType);

            var entitySetName = _entitySetNameCache.GetFromCacheOrFetch(new Tuple<Type, Type>(dbContext.GetType(), entityType), () =>
            {
                var objectContext = dbContext.GetObjectContext();
                var entitySet = objectContext.MetadataWorkspace.GetEntityContainer(objectContext.DefaultContainerName, DataSpace.CSpace)
                    .BaseEntitySets.FirstOrDefault(bes => bes.ElementType.Name == entityType.Name);

                if (entitySet is null && entityType.BaseType is not null)
                {
                    // recursive method call, should be no problem as the compiler wont allow circular base class dependencies
                    return dbContext.GetEntitySetName(entityType.BaseType);
                }

                if (entitySet is null)
                {
                    throw Log.ErrorAndCreateException<NotSupportedException>(string.Format("No EntitySet has been found for the provided Type '{0}'", entityType));
                }

                return entitySet.Name;
            });

            return entitySetName;
        }

        /// <summary>
        /// Gets the full name of the entity in the <see cref="DbContext" /> for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the T entity.</typeparam>
        /// <param name="dbContext">The db context.</param>
        /// <returns>The name of the entity.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        public static string GetFullEntitySetName<TEntity>(this DbContext dbContext)
        {
            return GetFullEntitySetName(dbContext, typeof(TEntity));
        }

        /// <summary>
        /// Gets the full name of the entity in the <see cref="DbContext" /> for the specified entity type.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The name of the entity.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entityType" /> is <c>null</c>.</exception>
        public static string GetFullEntitySetName(this DbContext dbContext, Type entityType) 
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(entityType);

            var entitySetName = GetEntitySetName(dbContext, entityType);
            var objectContext = dbContext.GetObjectContext();

            return string.Format("{0}.{1}", objectContext.DefaultContainerName, entitySetName);
        }

        /// <summary>
        /// Gets the entity set for the specified entity in the specified db context.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The entity set.</returns>
        public static object? GetObjectSet(this DbContext dbContext, Type entityType)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(entityType);

            var objectContext = dbContext.GetObjectContext();
            return GetObjectSet(objectContext, entityType);
        }

        /// <summary>
        /// Gets the entity set for the specified entity in the specified object context.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The entity set.</returns>
        public static object? GetObjectSet(this ObjectContext objectContext, Type entityType)
        {
            ArgumentNullException.ThrowIfNull(objectContext);
            ArgumentNullException.ThrowIfNull(entityType);

            var createObjectSetMethod = objectContext.GetType().GetMethodEx("CreateObjectSet", new Type[] { });
            if (createObjectSetMethod is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>($"No CreateObjectSet has been found for object context");
            }

            var genericCreateObjectSetMethod = createObjectSetMethod.MakeGenericMethod(entityType);

            var objectSet = genericCreateObjectSetMethod.Invoke(objectContext, new object[] { });
            return objectSet;
        }

        /// <summary>
        /// Gets the entity set for the specified entity in the specified db context.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The entity set.</returns>
        public static EntitySet? GetEntitySet(this DbContext dbContext, Type entityType)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(entityType);

            var objectContext = dbContext.GetObjectContext();
            return GetEntitySet(objectContext, entityType);
        }

        /// <summary>
        /// Gets the entity set for the specified entity in the specified db context.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The entity set.</returns>
        public static EntitySet GetRequiredEntitySet(this DbContext dbContext, Type entityType)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(entityType);

            var entitySet = GetEntitySet(dbContext, entityType);
            if (entitySet is null)
            {
                throw new InvalidOperationException($"Could not retrieve entity set for entity type '{entityType.GetSafeFullName()}'");
            }

            return entitySet;
        }

        /// <summary>
        /// Gets the entity set for the specified entity in the specified object context.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The entity set.</returns>
        public static EntitySet? GetEntitySet(this ObjectContext objectContext, Type entityType)
        {
            ArgumentNullException.ThrowIfNull(objectContext);
            ArgumentNullException.ThrowIfNull(entityType);

            var objectSet = GetObjectSet(objectContext, entityType);
            if (objectSet is null)
            {
                return null;
            }

            var entitySet = (EntitySet?)PropertyHelper.GetPropertyValue(objectSet, "EntitySet", false);
            return entitySet;
        }

        /// <summary>
        /// Gets the entity set for the specified entity in the specified db context.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The entity set.</returns>
        public static EntitySet GetRequiredEntitySet(this ObjectContext objectContext, Type entityType)
        {
            ArgumentNullException.ThrowIfNull(objectContext);
            ArgumentNullException.ThrowIfNull(entityType);

            var entitySet = GetEntitySet(objectContext, entityType);
            if (entitySet is null)
            {
                throw new InvalidOperationException($"Could not retrieve entity set for entity type '{entityType.GetSafeFullName()}'");
            }

            return entitySet;
        }

        /// <summary>
        /// Gets the name of the table as it is mapped in the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The table name including the schema.
        /// </returns>
        public static string GetTableName(this DbContext context, Type entityType)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(entityType);

            return _tableNameCache.GetFromCacheOrFetch(entityType, () =>
            {
                var objectContext = context.GetObjectContext();
                return GetTableName(objectContext, entityType);
            });
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The table name including the schema.
        /// </returns>
        public static string GetTableName(this ObjectContext context, Type entityType)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(entityType);

            return _tableNameCache.GetFromCacheOrFetch(entityType, () =>
            {
                var objectSet = GetObjectSet(context, entityType);
                var methodInfo = objectSet?.GetType().GetMethodEx("ToTraceString");
                if (methodInfo is null)
                {
                    throw Log.ErrorAndCreateException<NotSupportedException>($"No ToTraceString has been found on the object set");
                }

                var sql = (string?)methodInfo.Invoke(objectSet, new object?[] { });
                if (string.IsNullOrEmpty(sql))
                {
                    throw Log.ErrorAndCreateException<NotSupportedException>($"Could not find object set sql");
                }

                var regex = new Regex("FROM (?<table>.*) AS", RegexOptions.None, TimeSpan.FromSeconds(1));
                var match = regex.Match(sql);

                var table = match.Groups["table"].Value;
                return table;
            });
        }
    }
}
