namespace Orc.EntityFramework
{
    using System;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;

    public static class IContextFactoryExtensions
    {
        /// <summary>
        /// Creates the specified context using the input parameters.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The created context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="contextType" /> is <c>null</c>.</exception>
        public static object CreateRequiredContext(this IContextFactory contextFactory, Type contextType, string databaseNameOrConnectionStringName, string label, DbCompiledModel? model, ObjectContext? context)
        {
            ArgumentNullException.ThrowIfNull(contextFactory);

            var finalContext = contextFactory.CreateContext(contextType, databaseNameOrConnectionStringName, label, model, context);
            if (finalContext is null)
            {
                throw new InvalidOperationException($"Could not create required context");
            }

            return finalContext;
        }

        /// <summary>
        /// Creates the specified context using the input parameters.
        /// </summary>
        /// <typeparam name="TContext">The type of the T context.</typeparam>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The created context.</returns>
        public static TContext CreateRequiredContext<TContext>(this IContextFactory contextFactory, string databaseNameOrConnectionStringName, string label, DbCompiledModel? model, ObjectContext? context)
        {
            ArgumentNullException.ThrowIfNull(contextFactory);

            var finalContext = contextFactory.CreateContext<TContext>(databaseNameOrConnectionStringName, label, model, context);
            if (finalContext is null)
            {
                throw new InvalidOperationException($"Could not create required context");
            }

            return finalContext;
        }
    }
}
