// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.EntityFramework
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Globalization;

    using Catel;
    using Catel.IoC;
    using Catel.Logging;

    using Repositories;
    using System.Collections;

#if EF_ASYNC
    using System.Threading.Tasks;
#endif

#if EF5
    using SaveOptions = System.Data.Objects.SaveOptions;
    using System.Data.Objects;
#else
    using SaveOptions = System.Data.Entity.Core.Objects.SaveOptions;
    using System.Data.Entity.Core.Objects;
#endif

    /// <summary>
    /// Implementation of the unit of work pattern for entity framework.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly IServiceLocator _serviceLocator;
        private readonly ITypeFactory _typeFactory;

        private bool _disposed;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="tag">The tag to uniquely identify this unit of work. If <c>null</c>, a unique id will be generated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> is <c>null</c>.</exception>
        public UnitOfWork(DbContext context, string tag = null)
        {
            Argument.IsNotNull("context", context);

            _serviceLocator = ServiceLocator.Default;
            _typeFactory = _serviceLocator.ResolveType<ITypeFactory>();

            DbContext = context;
            Tag = tag ?? UniqueIdentifierHelper.GetUniqueIdentifier<UnitOfWork>().ToString(CultureInfo.InvariantCulture);
        }
        #endregion

        #region Properties
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
        protected string Tag { get; private set; }

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
#pragma warning disable IDISP008 // Don't assign member with injected and created disposables.
        protected DbTransaction Transaction { get; set; }
#pragma warning restore IDISP008 // Don't assign member with injected and created disposables.
        #endregion

        #region IUnitOfWork Members
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
            Log.Debug("Beginning transaction | {0}", Tag);

            if (Transaction is not null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot begin a new transaction while an existing transaction is still running. " +
                    "Please commit or rollback the existing transaction before starting a new one.");
            }

            OpenConnection();

            var objectContext = DbContext.GetObjectContext();

#pragma warning disable IDISP003 // Dispose previous before re-assigning.
            Transaction = objectContext.Connection.BeginTransaction(isolationLevel);
#pragma warning restore IDISP003 // Dispose previous before re-assigning.

            if (EnableVerboseLogging)
            {
                Log.Debug("Began transaction | {0}", Tag);
            }
        }

        /// <summary>
        /// Rolls back all the changes inside a transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">No transaction is currently running.</exception>
        public virtual void RollBackTransaction()
        {
            Log.Debug("Rolling back transaction | {0}", Tag);

            if (Transaction is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot roll back a transaction when there is no transaction running.");
            }

            Transaction.Rollback();
            ReleaseTransaction();

            if (EnableVerboseLogging)
            {
                Log.Debug("Rolling back transaction | {0}", Tag);
            }
        }

        /// <summary>
        /// Commits all the changes inside a transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">No transaction is currently running.</exception>
        public virtual void CommitTransaction()
        {
            Log.Debug("Committing transaction | {0}", Tag);

            if (Transaction is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot commit a transaction when there is no transaction running.");
            }

            try
            {
                DbContext.SaveChanges();

                Transaction.Commit();

                ReleaseTransaction();

                if (EnableVerboseLogging)
                {
                    Log.Debug("Committed transaction | {0}", Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while committing the transaction, automatically rolling back | {0}", Tag);

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
            Log.Debug("Committing transaction async | {0}", Tag);

            if (Transaction is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot commit a transaction when there is no transaction running.");
            }

            try
            {
                await DbContext.SaveChangesAsync();

                Transaction.Commit();

                ReleaseTransaction();

                if (EnableVerboseLogging)
                { 
                    Log.Debug("Committed transaction async | {0}", Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while committing the transaction, automatically rolling back | {0}", Tag);

                RollBackTransaction();
                throw;
            }
        }
#endif

        /// <summary>
        /// Gets the repository that is created specificially for this unit of work.
        /// <para />
        /// Note that the following conditions must be met: <br />
        /// <list type="number">
        /// <item>
        /// <description>
        /// The container must be registered in the <see cref="ServiceLocator" /> as <see cref="RegistrationType.Transient" /> type. If the
        /// repository is declared as non-transient, it will be instantiated as new instance anyway.
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
            var registrationInfo = _serviceLocator.GetRegistrationInfo(typeof(TEntityRepository));
            if (registrationInfo is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("The specified repository type '{0}' cannot be found. Make sure it is registered in the ServiceLocator.", typeof(TEntityRepository).FullName);
            }

            var repository = _typeFactory.CreateInstanceWithParameters(registrationInfo.ImplementingType, DbContext);
            return (TEntityRepository)repository;
        }

        /// <summary>
        /// Refreshes the collection inside the unit of work.
        /// </summary>
        /// <param name="refreshMode">The refresh mode.</param>
        /// <param name="collection">The collection.</param>
        public virtual void Refresh(RefreshMode refreshMode, IEnumerable collection)
        {
            Log.Debug("Refreshing collection | {0}", Tag);

            var objectContext = DbContext.GetObjectContext();
            objectContext.Refresh(refreshMode, collection);

            if (EnableVerboseLogging)
            {
                Log.Debug("Refreshed collection | {0}", Tag);
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
            Log.Debug("Refreshing collection async | {0}", Tag);

            var objectContext = DbContext.GetObjectContext();
            await objectContext.RefreshAsync(refreshMode, collection);

            if (EnableVerboseLogging)
            { 
                Log.Debug("Refreshed collection async | {0}", Tag);
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
            Log.Debug("Refreshing entity | {0}", Tag);

            var objectContext = DbContext.GetObjectContext();
            objectContext.Refresh(refreshMode, entity);

            if (EnableVerboseLogging)
            {
                Log.Debug("Refreshed entity | {0}", Tag);
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
            Log.Debug("Refreshing entity async | {0}", Tag);

            var objectContext = DbContext.GetObjectContext();
            await objectContext.RefreshAsync(refreshMode, entity);

            if (EnableVerboseLogging)
            { 
                Log.Debug("Refreshed entity async | {0}", Tag);
            }
        }
#endif

        /// <summary>
        /// Saves the changes inside the unit of work.
        /// </summary>
        /// <exception cref="InvalidOperationException">A transaction is running. Call CommitTransaction instead.</exception>
        public virtual void SaveChanges()
        {
            Log.Debug("Saving changes | {0}", Tag);

            if (IsInTransaction)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("A transaction is running. Call CommitTransaction instead.");
            }

            DbContext.SaveChanges();

            if (EnableVerboseLogging)
            {
                Log.Debug("Saved changes | {0}", Tag);
            }
        }

#if EF_ASYNC
        /// <summary>
        /// Saves the changes inside the unit of work.
        /// </summary>
        /// <exception cref="InvalidOperationException">A transaction is running. Call CommitTransaction instead.</exception>
        public virtual async Task SaveChangesAsync()
        {
            Log.Debug("Saving changes async | {0}", Tag);

            if (IsInTransaction)
            {
                throw Log.ErrorAndCreateException < InvalidOperationException >("A transaction is running. Call CommitTransaction instead");
            }

            await DbContext.SaveChangesAsync();

            if (EnableVerboseLogging)
            { 
                Log.Debug("Saved changes async | {0}", Tag);
            }
        }
#endif
        #endregion

        #region Implementation of IDisposable
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Methods
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
        #endregion

        #endregion

        #region Methods
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
                    Log.Debug("Opening connection | {0}", Tag);
                }

                objectContext.Connection.Open();

                if (EnableVerboseLogging)
                {
                    Log.Debug("Opened connection | {0}", Tag);
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
                    Log.Debug("Opening connection async | {0}", Tag);
                }

                await objectContext.Connection.OpenAsync();

                if (EnableVerboseLogging)
                { 
                    Log.Debug("Opened connection async | {0}", Tag);
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
                Log.Debug("Releasing transaction | {0}", Tag);

                Transaction.Dispose();
                Transaction = null;

                if (EnableVerboseLogging)
                {
                    Log.Debug("Released transaction | {0}", Tag);
                }
            }
        }
        #endregion
    }
}
