namespace Orc.EntityFramework
{
    using System.Data.Entity;
    using Catel.Logging;

    /// <summary>
    /// Generic implementation of the <see cref="UnitOfWork"/> which can automatically determine the DbContext type.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the db context.</typeparam>
    public class UnitOfWork<TDbContext> : UnitOfWork
        where TDbContext : DbContext
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly bool _isInjectedContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork{TDbContext}"/> class.
        /// </summary>
        /// <param name="dbContext">The db context. If <c>null</c>, it will be resolved automatically using the <see cref="DbContextManager{T}"/>.</param>
        public UnitOfWork(TDbContext? dbContext = null)
            : base(dbContext ?? DbContextManager<TDbContext>.GetManager().Context)
        {
            _isInjectedContext = (dbContext is not null);
        }

        /// <summary>
        /// Called when the object is being disposed.
        /// </summary>
        protected override void OnDisposing()
        {
            if (!_isInjectedContext)
            {
                if (EnableVerboseLogging)
                {
                    Log.Debug("Disposing DbContextManager because this is a non-injected DbContext");
                }

                var dbContextManager = DbContextManager<TDbContext>.GetManager();

#pragma warning disable IDISP007 // Don't dispose injected.
                // Note: we need to get the DbContextManager and dispose it twice (once for the call in the ctor, once for this retrieval call)
#pragma warning disable IDISP016 // Don't use disposed instance.
                dbContextManager.Dispose();
#pragma warning restore IDISP016 // Don't use disposed instance.
                dbContextManager.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            }
        }
    }
}
