// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionStringManager.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.EntityFramework
{
    using System;

    /// <summary>
    /// Interface defining the connection string manager.
    /// <para />
    /// This interface is used in combination with the <see cref="DbContextManager{TDbContext}"/>.
    /// </summary>
    public interface IConnectionStringManager
    {
        /// <summary>
        /// Gets the connection string for the specified database.
        /// </summary>
        /// <param name="contextType">The type of the context.</param>
        /// <param name="database">The database.</param>
        /// <param name="label">The label.</param>
        /// <returns>The connection string.</returns>
        string GetConnectionString(Type contextType, string database, string label);
    }
}