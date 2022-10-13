namespace Orc.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Threading;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using System.Data.Entity.Core.Objects;

    /// <summary>
    /// Provides an automated way to reuse Entity Framework context objects within the context of a single data portal operation.
    /// </summary>
    /// <typeparam name="TContext">Type of the context object to use.</typeparam>
    /// <remarks>
    /// This type stores the object context object in an internal dictionary and uses reference counting through
    /// <see cref="IDisposable" /> to keep the data context object open for reuse by child objects, and to automatically
    /// dispose the object when the last consumer has called Dispose.
    /// <para />
    /// Note that this class is a base class to share the logic between the <see cref="DbContextManager{TDbContext}"/> and 
    /// <see cref="ObjectContextManager{TObjectContext}"/>.
    /// </remarks>
#pragma warning disable IDISP025 // Class with no virtual dispose method should be sealed.
    public abstract class ContextManager<TContext> : IDisposable
#pragma warning restore IDISP025 // Class with no virtual dispose method should be sealed.
        where TContext : class, IDisposable
    {
        private static readonly object _lock = new object();
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<string, object> _instances = new Dictionary<string, object>();

        private readonly TContext _context;
        private readonly string _contextLogName;
        private readonly string _label;
        private int _refCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextManager{TContext}"/> class.
        /// </summary>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected ContextManager(string databaseNameOrConnectionStringName, string label, DbCompiledModel? model, ObjectContext? context)
        {
            _label = label;
            _contextLogName = GetContextLogName(databaseNameOrConnectionStringName, label);
            ContextLabel = string.Empty;

            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;

            // Option to override or late-bind connection string
            if (string.IsNullOrEmpty(databaseNameOrConnectionStringName))
            {
                var connectionStringManager = dependencyResolver.Resolve<IConnectionStringManager>();
                if (connectionStringManager is not null)
                {
                    var connectionString = connectionStringManager.GetConnectionString(typeof(TContext), databaseNameOrConnectionStringName, label);
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        databaseNameOrConnectionStringName = connectionString;
                    }
                }
            }

            var contextFactory = dependencyResolver.ResolveRequired<IContextFactory>();
            _context = contextFactory.CreateRequiredContext<TContext>(databaseNameOrConnectionStringName, label, model, context);

            Initialize(_context);
        }

        /// <summary>
        /// Gets or sets the context label.
        /// </summary>
        /// <value>The context label.</value>
        private string ContextLabel { get; set; }

        /// <summary>
        /// Gets or sets whether verbose logging should be enabled.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        protected bool EnableVerboseLogging { get; set; }

        /// <summary>
        /// Gets the context object.
        /// </summary>
        public TContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets the current reference count for this
        /// object.
        /// </summary>
        public int RefCount
        {
            get
            {
                lock (_lock)
                {
                    return _refCount;
                }
            }
        }

        /// <summary>
        /// Dispose object, dereferencing or disposing the context it is managing.
        /// </summary>
        public void Dispose()
        {
            DeRef();
        }

        /// <summary>
        /// Initializes the specified context. This method is called right after the context is created and the connection string
        /// has been set.
        /// </summary>
        /// <param name="context">The context.</param>
        protected abstract void Initialize(TContext context);

        private void AddRef()
        {
            lock (_lock)
            {
                _refCount += 1;

                if (EnableVerboseLogging)
                {
                    Log.Debug("Referencing {0}, new ref count is {1}", _contextLogName, _refCount);
                }
            }
        }

        private void DeRef()
        {
            lock (_lock)
            {
                _refCount -= 1;

                if (EnableVerboseLogging)
                {
                    Log.Debug("Dereferencing {0}, new ref count is {1}", _contextLogName, _refCount);
                }

                if (_refCount == 0)
                {
                    if (EnableVerboseLogging)
                    {
                        Log.Debug("Disposing DbContext {0} because it reached a ref count of 0", _contextLogName);
                    }

                    _context.Dispose();
                    _instances.Remove(ContextLabel);
                }
            }
        }

        /// <summary>
        /// Gets the manager and creates the context if necessary.
        /// </summary>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="createContext">The create context.</param>
        /// <returns>ContextManager{TContext}.</returns>
        /// <exception cref="ArgumentException">The <paramref name="databaseNameOrConnectionStringName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="createContext"/> is <c>null</c>.</exception>
        protected static ContextManager<TContext> GetManager(string databaseNameOrConnectionStringName, string label, Func<ContextManager<TContext>> createContext)
        {
            Argument.IsNotNullOrWhitespace("label", label);
            ArgumentNullException.ThrowIfNull(createContext);

            lock (_lock)
            {
                var contextLogLabel = GetContextLogName(databaseNameOrConnectionStringName, label);
                var contextLabel = GetContextName(databaseNameOrConnectionStringName, label);
                ContextManager<TContext> mgr;

                if (_instances.ContainsKey(contextLabel))
                {
                    //Log.Debug("Returning existing instance for {0}", contextLogLabel);

                    mgr = (ContextManager<TContext>)(_instances[contextLabel]);
                }
                else
                {
                    //Log.Debug("Creating new instance for {0}", contextLogLabel);

                    mgr = createContext();
                    mgr.ContextLabel = contextLabel;
                    _instances[contextLabel] = mgr;
                }

                mgr.AddRef();
                return mgr;
            }
        }

        /// <summary>
        /// Gets the name of the context.
        /// </summary>
        /// <param name="databaseNameOrConnectionStringName">The database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <returns>The name of the context.</returns>
        /// <exception cref="ArgumentException">The <paramref name="label" /> is <c>null</c> or whitespace.</exception>
        protected static string GetContextName(string databaseNameOrConnectionStringName, string label)
        {
            Argument.IsNotNullOrWhitespace("label", label);

            var threadId = ThreadHelper.GetCurrentThreadId();
            return string.Format("__ctx:{0}-{1}-{2}", databaseNameOrConnectionStringName ?? "database", label, threadId);
        }

        /// <summary>
        /// Gets the name of the context to be used during logging.
        /// </summary>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <returns>The name of the context for logging purposes.</returns>
        protected static string GetContextLogName(string databaseNameOrConnectionStringName, string label)
        {
            var contextLabel = GetContextName(databaseNameOrConnectionStringName, label);
            return string.Format("Context '{0}' with context label '{1}'", typeof(TContext).FullName, contextLabel);
        }
    }
}
