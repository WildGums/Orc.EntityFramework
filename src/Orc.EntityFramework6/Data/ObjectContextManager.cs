namespace Orc.EntityFramework
{
    using System;
    using Catel;
    using System.Data.Entity.Core.Objects;

    /// <summary>
    /// Provides an automated way to reuse Entity Framework ObjectContext objects within the context of a single data portal operation.
    /// </summary>
    /// <typeparam name="TObjectContext">Type of the object context to use.
    /// </typeparam>
    /// <remarks>
    /// This type stores the object context object in an internal dictionary and uses reference counting through
    /// <see cref="IDisposable" /> to keep the data context object open for reuse by child objects, and to automatically
    /// dispose the object when the last consumer has called Dispose.
    /// </remarks>
    public class ObjectContextManager<TObjectContext> : ContextManager<TObjectContext>
        where TObjectContext : ObjectContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContextManager{TObjectContext}"/> class.
        /// </summary>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="connectionStringManager"></param>
        /// <param name="contextFactory"></param>
        private ObjectContextManager(string databaseNameOrConnectionStringName, string label,
            IConnectionStringManager connectionStringManager, IContextFactory contextFactory) 
            : base(databaseNameOrConnectionStringName, label, null, null, connectionStringManager, contextFactory) { }

        /// <summary>
        /// Initializes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void Initialize(TObjectContext context)
        {
            // No initialization required
        }

        /// <summary>
        /// Gets the ContextManager object for the specified database.
        /// </summary>
        /// <returns>The <see cref="ObjectContextManager{TObjectContext}" />.</returns>
        public static ObjectContextManager<TObjectContext> GetManager(IConnectionStringManager connectionStringManager, 
            IContextFactory contextFactory)
        {
            return GetManager(string.Empty, string.Empty, connectionStringManager, contextFactory);
        }

        /// <summary>
        /// Gets the ContextManager object for the specified database.
        /// </summary>
        /// <param name="databaseNameOrConnectionStringName">The database name or connection string.</param>
        /// <param name="label">Label for this context.</param>
        /// <param name="connectionStringManager"></param>
        /// <param name="contextFactory"></param>
        /// <returns>The ContextManager.</returns>
        public static ObjectContextManager<TObjectContext> GetManager(string databaseNameOrConnectionStringName, string label,
            IConnectionStringManager connectionStringManager, IContextFactory contextFactory)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                label = "default";
            }

            return (ObjectContextManager<TObjectContext>)GetManager(databaseNameOrConnectionStringName, label, () => 
            { 
                return new ObjectContextManager<TObjectContext>(databaseNameOrConnectionStringName, label, 
                    connectionStringManager, contextFactory); 
            });
        }
    }
}
