namespace Orc.EntityFramework;

using System;
using System.Data.Entity;
using Catel.Logging;
using Microsoft.Extensions.Logging;

/// <summary>
/// Generic implementation of the <see cref="UnitOfWork"/> which can automatically determine the DbContext type.
/// </summary>
/// <typeparam name="TDbContext">The type of the db context.</typeparam>
public class UnitOfWork<TDbContext> : UnitOfWork, IUnitOfWork<TDbContext>
    where TDbContext : DbContext
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(UnitOfWork<TDbContext>));

    private readonly bool _isInjectedContext;

    private readonly IConnectionStringManager _connectionStringManager;
    private readonly IContextFactory _contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork{TDbContext}"/> class.
    /// </summary>
    /// <param name="connectionStringManager"></param>
    /// <param name="contextFactory"></param>
    /// <param name="serviceProvider"></param>
    public UnitOfWork(IConnectionStringManager connectionStringManager,
        IContextFactory contextFactory, IServiceProvider serviceProvider)
        : this(DbContextManager<TDbContext>.GetManager(connectionStringManager, contextFactory).Context,
              connectionStringManager, contextFactory, serviceProvider)
    {
        // Leave empty
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork{TDbContext}"/> class.
    /// </summary>
    /// <param name="dbContext">The db context. If <c>null</c>, it will be resolved automatically using the <see cref="DbContextManager{T}"/>.</param>
    /// <param name="connectionStringManager"></param>
    /// <param name="contextFactory"></param>
    /// <param name="serviceProvider"></param>
    public UnitOfWork(TDbContext? dbContext, IConnectionStringManager connectionStringManager, 
        IContextFactory contextFactory, IServiceProvider serviceProvider)
        : base(dbContext ?? DbContextManager<TDbContext>.GetManager(connectionStringManager, contextFactory).Context,
              serviceProvider, null)
    {
        _isInjectedContext = (dbContext is not null);
        _connectionStringManager = connectionStringManager;
        _contextFactory = contextFactory;
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
                Logger.LogDebug("Disposing DbContextManager because this is a non-injected DbContext");
            }

            var dbContextManager = DbContextManager<TDbContext>.GetManager(_connectionStringManager, _contextFactory);

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
