﻿namespace Orc.EntityFramework
{
    using Catel;
    using System;

    /// <summary>
    /// Default connection string manager.
    /// <para />
    /// This interface is used in combination with the <see cref="DbContextManager{TDbContext}"/>.
    /// </summary>
    public class ConnectionStringManager : IConnectionStringManager
    {
        /// <summary>
        /// Gets the connection string for the specified database.
        /// </summary>
        /// <param name="contextType">The type of the context.</param>
        /// <param name="database">The database.</param>
        /// <param name="label">The label.</param>
        /// <returns>The connection string or <c>null</c> if the connection string cannot be determined.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="contextType"/> is <c>null</c>.</exception>
        public virtual string? GetConnectionString(Type contextType, string database, string label)
        {
            ArgumentNullException.ThrowIfNull(contextType);

            return null;
        }
    }
}
