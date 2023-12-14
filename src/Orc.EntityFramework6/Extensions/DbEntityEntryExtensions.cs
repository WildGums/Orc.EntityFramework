namespace Orc.EntityFramework
{
    using System;
    using System.Data.Entity.Infrastructure;

    using Catel;
    using Catel.Reflection;

    /// <summary>
    /// Extension methods for the DbEntityEntry class.
    /// </summary>
    public static class DbEntityEntryExtensions
    {
        /// <summary>
        /// Gets the type of the entity. Even when proxies are enabled, this will return the 
        /// actual entity type.
        /// </summary>
        /// <param name="dbEntityEntry">The database entity entry.</param>
        /// <returns>The type or <c>null</c> if the type could not be determined.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbEntityEntry"/> is <c>null</c>.</exception>
        public static Type? GetEntityType(this DbEntityEntry dbEntityEntry)
        {
            ArgumentNullException.ThrowIfNull(dbEntityEntry);

            var entity = dbEntityEntry.Entity;
            return entity.GetEntityType();
        }

        /// <summary>
        /// Gets the type of the entity. Even when proxies are enabled, this will return the 
        /// actual entity type.
        /// </summary>
        /// <param name="dbEntityEntry">The database entity entry.</param>
        /// <returns>The type or <c>null</c> if the type could not be determined.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbEntityEntry"/> is <c>null</c>.</exception>
        public static Type GetRequiredEntityType(this DbEntityEntry dbEntityEntry)
        {
            ArgumentNullException.ThrowIfNull(dbEntityEntry);

            var entityType = GetEntityType(dbEntityEntry);
            if (entityType is null)
            {
                throw new InvalidOperationException("Could not get entity type from db entity entry");
            }

            return entityType;
        }

        /// <summary>
        /// Gets the type of the entity. Even when proxies are enabled, this will return the 
        /// actual entity type.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The type or <c>null</c> if the type could not be determined.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is <c>null</c>.</exception>
        public static Type? GetEntityType(this object entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var entityType = entity.GetType();
            return entityType.GetEntityType();
        }

        /// <summary>
        /// Gets the type of the entity. Even when proxies are enabled, this will return the 
        /// actual entity type.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The type or <c>null</c> if the type could not be determined.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is <c>null</c>.</exception>
        public static Type GetRequiredEntityType(this object entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var entityType = GetEntityType(entity);
            if (entityType is null)
            {
                throw new InvalidOperationException("Could not get entity type from entity");
            }

            return entityType;
        }

        /// <summary>
        /// Gets the type of the entity. Even when proxies are enabled, this will return the 
        /// actual entity type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type or <c>null</c> if the type could not be determined.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static Type? GetEntityType(this Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            var activeType = (Type?)type;

            while (activeType is not null && activeType.GetSafeFullName().Contains(".DynamicProxies."))
            {
                activeType = activeType.BaseType;
            }

            return activeType;
        }

        /// <summary>
        /// Gets the current database value. If a value is <c>null</c>, it will be returned as <c>DBNull.Value</c>.
        /// </summary>
        /// <param name="dbEntityEntry">The database entity entry.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbEntityEntry"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public static object? GetCurrentDbValue(this DbEntityEntry dbEntityEntry, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(dbEntityEntry);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            return GetDbValue(dbEntityEntry.CurrentValues, propertyName);
        }

        /// <summary>
        /// Gets the original database value. If a value is <c>null</c>, it will be returned as <c>DBNull.Value</c>.
        /// </summary>
        /// <param name="dbEntityEntry">The database entity entry.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbEntityEntry"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public static object? GetOriginalDbValue(this DbEntityEntry dbEntityEntry, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(dbEntityEntry);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            return GetDbValue(dbEntityEntry.OriginalValues, propertyName);
        }

        /// <summary>
        /// Gets the database value. If a value is <c>null</c>, it will be returned as <c>DBNull.Value</c>.
        /// </summary>
        /// <param name="propertyValues">The property values.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyValues" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public static object? GetDbValue(this DbPropertyValues propertyValues, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(propertyValues);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            var value = propertyValues[propertyName];
            var dbValue = value ?? DBNull.Value;

            return dbValue;
        }
    }
}
