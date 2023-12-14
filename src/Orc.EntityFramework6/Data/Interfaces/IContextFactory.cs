﻿namespace Orc.EntityFramework
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;

    /// <summary>
    /// Interface defining the functionality for the context factory.
    /// </summary>
    public interface IContextFactory
    {
        /// <summary>
        /// Creates the specified context using the input parameters.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The created context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="contextType" /> is <c>null</c>.</exception>
        object? CreateContext(Type contextType, string databaseNameOrConnectionStringName, string label, DbCompiledModel? model, ObjectContext? context);

        /// <summary>
        /// Creates the specified context using the input parameters.
        /// </summary>
        /// <typeparam name="TContext">The type of the T context.</typeparam>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The created context.</returns>
        TContext? CreateContext<TContext>(string databaseNameOrConnectionStringName, string label, DbCompiledModel? model, ObjectContext? context);
    }
}
