﻿namespace Orc.EntityFramework
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;

    /// <summary>
    /// Class responsible for instantiating contexts.
    /// </summary>
    public class ContextFactory : IContextFactory
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
        public virtual object? CreateContext(Type contextType, string databaseNameOrConnectionStringName, string label, DbCompiledModel? model, ObjectContext? context)
        {
            object? createdContext = null; 

            if (model is not null)
            {
                if (!string.IsNullOrEmpty(databaseNameOrConnectionStringName))
                {
                    createdContext = Activator.CreateInstance(contextType, databaseNameOrConnectionStringName, model);
                }
                else
                {
                    createdContext = Activator.CreateInstance(contextType, model);
                }
            }
            else if (context is not null)
            {
                createdContext = Activator.CreateInstance(contextType, context, true);
            }
            else if (string.IsNullOrEmpty(databaseNameOrConnectionStringName))
            {
                createdContext = Activator.CreateInstance(contextType);
            }
            else
            {
                createdContext = Activator.CreateInstance(contextType, databaseNameOrConnectionStringName);
            }

            return createdContext;
        }

        /// <summary>
        /// Creates the specified context using the input parameters.
        /// </summary>
        /// <typeparam name="TContext">The type of the T context.</typeparam>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The created context.</returns>
        public TContext? CreateContext<TContext>(string databaseNameOrConnectionStringName, string label, DbCompiledModel? model, ObjectContext? context)
        {
            return (TContext?)CreateContext(typeof(TContext), databaseNameOrConnectionStringName, label, model, context);
        }
    }
}
