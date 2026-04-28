namespace Orc.EntityFramework;

using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Globalization;

using Catel;
using Catel.Logging;

using Repositories;
using Microsoft.Extensions.Logging;

#if EF_ASYNC
using System.Threading.Tasks;
#endif

using System.Data.Entity.Core.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

/// <summary>
/// Implementation of the unit of work pattern for entity framework.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(UnitOfWork));

    private readonly IServiceProvider _serviceProvider;

    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="serviceProvider"></param>
    /// <param name="tag">The tag to uniquely identify this unit of work. If <c>null</c>, a unique id will be generated.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="context" /> is <c>null</c>.</exception>
    public UnitOfWork(DbContext context, IServiceProvider serviceProvider, string? tag = null)
    {
        DbContext = context;
        _serviceProvider = serviceProvider;
        Tag = tag ?? UniqueIdentifierHelper.GetUniqueIdentifier<UnitOfWork>().ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets the db context.
    /// </summary>
    /// <value>The db context.</value>
    protected DbContext DbContext { get; private set; }

    /// <summary>
    /// Gets or sets whether verbose logging should be enabled.
    /// <para />
    /// The default value is <c>false</c>.
    /// </summary>
    protected bool EnableVerboseLogging { get; set; }

    /// <summary>
    /// Gets the tag.
    /// </summary>
    /// <value>The tag.</value>
    protected string? Tag { get; private set; }

    /// <summary>
    /// Gets or sets the transaction.
    /// </summary>
    /// <value>The transaction.</value>
#pragma warning disable IDISP008 // Don't assign member with injected and created disposables.
    protected DbTransaction? Transaction { get; set; }
#pragma warning restore IDISP008 // Don't assign member with injected and created disposables.

    /// <summary>
    /// Gets a value indicating whether this instance is currently in a transaction.
    /// </summary>
    /// <value><c>true</c> if this instance is currently in a transaction; otherwise, <c>false</c>.</value>
    public bool IsInTransaction
    {
        get { return Transaction is not null; }
    }

    /// <summary>
    /// Begins a new transaction on the unit of work.
    /// </summary>
    /// <param name="isolationLevel">The isolation level.</param>
    /// <exception cref="InvalidOperationException">A transaction is already running.</exception>
    public virtual void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        Logger.LogDebug("Beginning transaction | {0}", Tag);

        if (Transaction is not null)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Cannot begin a new transaction while an existing transaction is still running. " +
                "Please commit or rollback the existing transaction before starting a new one.");
        }

        OpenConnection();

        var objectContext = DbContext.GetObjectContext();

#pragma warning disable IDISP003 // Dispose previous before re-assigning.
        Transaction = objectContext.Connection.BeginTransaction(isolationLevel);
#pragma warning restore IDISP003 // Dispose previous before re-assigning.

        if (EnableVerboseLogging)
        {
            Logger.LogDebug("Began transaction | {0}", Tag);
        }
    }

    /// <summary>
    /// Rolls back all the changes inside a transaction.
    /// </summary>
    /// <exception cref="InvalidOperationException">No transaction is currently running.</exception>
    public virtual void RollBackTransaction()
    {
        Logger.LogDebug("Rolling back transaction | {0}", Tag);

        if (Transaction is null)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Cannot roll back a transaction when there is no transaction running.");
        }

        Transaction.Rollback();
        ReleaseTransaction();

        if (EnableVerboseLogging)
        {
            Logger.LogDebug("Rolling back transaction | {0}", Tag);
        }
    }

    /// <summary>
    /// Commits all the changes inside a transaction.
    /// </summary>
    /// <exception cref="InvalidOperationException">No transaction is currently running.</exception>
    public virtual void CommitTransaction()
    {
        Logger.LogDebug("Committing transaction | {0}", Tag);

        if (Transaction is null)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Cannot commit a transaction when there is no transaction running.");
        }

        try
        {
            DbContext.SaveChanges();

            Transaction.Commit();

            ReleaseTransaction();

            if (EnableVerboseLogging)
            {
                Logger.LogDebug("Committed transaction | {0}", Tag);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception occurred while committing the transaction, automatically rolling back | {0}", Tag);

            RollBackTransaction();
            throw;
        }
    }

#if EF_ASYNC
    /// <summary>
    /// Commits all the changes inside a transaction.
    /// </summary>
    /// <exception cref="InvalidOperationException">No transaction is currently running.</exception>
    public virtual async Task CommitTransactionAsync()
    {
        Logger.LogDebug("Committing transaction async | {0}", Tag);

        if (Transaction is null)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Cannot commit a transaction when there is no transaction running.");
        }

        try
        {
            await DbContext.SaveChangesAsync();

            Transaction.Commit();

            ReleaseTransaction();

            if (EnableVerboseLogging)
            { 
                Logger.LogDebug("Committed transaction async | {0}", Tag);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception occurred while committing the transaction, automatically rolling back | {0}", Tag);

            RollBackTransaction();
            throw;
        }
    }
#endif

    /// <summary>
    /// Gets the repository that is created specifically for this unit of work.
    /// <para />
    /// Note that the following conditions must be met: <br />
    /// <list type="number">
    /// <item>
    /// <description>
    /// The repository must be registered in the <see cref="IServiceProvider" /> as <see cref="ServiceLifetime.Transient" /> type. 
    ///
    /// If the repository is declared as non-transient, it will be instantiated as new instance anyway.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// The repository must have a constructor accepting a <see cref="DbContext" /> instance.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <returns>The entity repository.</returns>
    /// <exception cref="NotSupportedException">The specified repository type cannot be found.</exception>
    public virtual TEntityRepository GetRepository<TEntityRepository>()
        where TEntityRepository : IEntityRepository
    {
        var serviceDescriptors = _serviceProvider.GetServiceDescriptors(typeof(TEntityRepository));
        var serviceDescriptor = serviceDescriptors.FirstOrDefault(x => !x.IsKeyedService);
        if (serviceDescriptor is null)
        {
            throw Logger.LogErrorAndCreateException<NotSupportedException>("The specified repository type '{0}' cannot be found. Make sure it is registered in the ServiceCollection.", typeof(TEntityRepository).FullName);
        }

        var repository = ActivatorUtilities.CreateInstance(_serviceProvider, serviceDescriptor.ImplementationType!, DbContext);
        return (TEntityRepository)repository;
    }

    /// <summary>
    /// Refreshes the collection inside the unit of work.
    /// </summary>
    /// <param name="refreshMode">The refresh mode.</param>
    /// <param name="collection">The collection.</param>
    public virtual void Refresh(RefreshMode refreshMode, IEnumerable collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        Logger.LogDebug("Refreshing collection | {0}", Tag);

        var objectContext = DbContext.GetObjectContext();
        objectContext.Refresh(refreshMode, collection);

        if (EnableVerboseLogging)
        {
            Logger.LogDebug("Refreshed collection | {0}", Tag);
        }
    }

#if EF_ASYNC
    /// <summary>
    /// Refreshes the collection inside the unit of work.
    /// </summary>
    /// <param name="refreshMode">The refresh mode.</param>
    /// <param name="collection">The collection.</param>
    public virtual async Task RefreshAsync(RefreshMode refreshMode, IEnumerable collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        Logger.LogDebug("Refreshing collection async | {0}", Tag);

        var objectContext = DbContext.GetObjectContext();
        await objectContext.RefreshAsync(refreshMode, collection);

        if (EnableVerboseLogging)
        { 
            Logger.LogDebug("Refreshed collection async | {0}", Tag);
        }
    }
#endif

    /// <summary>
    /// Refreshes the entity inside the unit of work.
    /// </summary>
    /// <param name="refreshMode">The refresh mode.</param>
    /// <param name="entity">The entity.</param>
    public virtual void Refresh(RefreshMode refreshMode, object entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Logger.LogDebug("Refreshing entity | {0}", Tag);

        var objectContext = DbContext.GetObjectContext();
        objectContext.Refresh(refreshMode, entity);

        if (EnableVerboseLogging)
        {
            Logger.LogDebug("Refreshed entity | {0}", Tag);
        }
    }

#if EF_ASYNC
    /// <summary>
    /// Refreshes the entity inside the unit of work.
    /// </summary>
    /// <param name="refreshMode">The refresh mode.</param>
    /// <param name="entity">The entity.</param>
    public virtual async Task RefreshAsync(RefreshMode refreshMode, object entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Logger.LogDebug("Refreshing entity async | {0}", Tag);

        var objectContext = DbContext.GetObjectContext();
        await objectContext.RefreshAsync(refreshMode, entity);

        if (EnableVerboseLogging)
        { 
            Logger.LogDebug("Refreshed entity async | {0}", Tag);
        }
    }
#endif

    /// <summary>
    /// Saves the changes inside the unit of work.
    /// </summary>
    /// <exception cref="InvalidOperationException">A transaction is running. Call CommitTransaction instead.</exception>
    public virtual void SaveChanges()
    {
        Logger.LogDebug("Saving changes | {0}", Tag);

        if (IsInTransaction)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("A transaction is running. Call CommitTransaction instead.");
        }

        DbContext.SaveChanges();

        if (EnableVerboseLogging)
        {
            Logger.LogDebug("Saved changes | {0}", Tag);
        }
    }

#if EF_ASYNC
    /// <summary>
    /// Saves the changes inside the unit of work.
    /// </summary>
    /// <exception cref="InvalidOperationException">A transaction is running. Call CommitTransaction instead.</exception>
    public virtual async Task SaveChangesAsync()
    {
        Logger.LogDebug("Saving changes async | {0}", Tag);

        if (IsInTransaction)
        {
            throw Logger.LogErrorAndCreateException < InvalidOperationException >("A transaction is running. Call CommitTransaction instead");
        }

        await DbContext.SaveChangesAsync();

        if (EnableVerboseLogging)
        { 
            Logger.LogDebug("Saved changes async | {0}", Tag);
        }
    }
#endif

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        if (_disposed)
        {
            return;
        }

        OnDisposing();

        _disposed = true;
    }

    /// <summary>
    /// Called when the object is being disposed.
    /// </summary>
    protected virtual void OnDisposing()
    {
    }

    /// <summary>
    /// Disposes the db context.
    /// </summary>
    protected void DisposeDbContext()
    {
        if (DbContext is not null)
        {
#pragma warning disable IDISP007 // Don't dispose injected.
            DbContext.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
        }
    }

    /// <summary>
    /// Opens the connection to the database.
    /// </summary>
    protected virtual void OpenConnection()
    {
        var objectContext = DbContext.GetObjectContext();
        if (objectContext.Connection.State != ConnectionState.Open)
        {
            if (EnableVerboseLogging)
            {
                Logger.LogDebug("Opening connection | {0}", Tag);
            }

            objectContext.Connection.Open();

            if (EnableVerboseLogging)
            {
                Logger.LogDebug("Opened connection | {0}", Tag);
            }
        }
    }

#if EF_ASYNC
    /// <summary>
    /// Opens the connection to the database.
    /// </summary>
    protected virtual async Task OpenConnectionAsync()
    {
        var objectContext = DbContext.GetObjectContext();
        if (objectContext.Connection.State != ConnectionState.Open)
        {
            if (EnableVerboseLogging)
            { 
                Logger.LogDebug("Opening connection async | {0}", Tag);
            }

            await objectContext.Connection.OpenAsync();

            if (EnableVerboseLogging)
            { 
                Logger.LogDebug("Opened connection async | {0}", Tag);
            }
        }
    }
#endif

    /// <summary>
    /// Releases the transaction.
    /// </summary>
    protected virtual void ReleaseTransaction()
    {
        if (Transaction is not null)
        {
            Logger.LogDebug("Releasing transaction | {0}", Tag);

            Transaction.Dispose();
            Transaction = null;

            if (EnableVerboseLogging)
            {
                Logger.LogDebug("Released transaction | {0}", Tag);
            }
        }
    }
}
