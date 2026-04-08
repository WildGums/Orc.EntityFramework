namespace Orc.EntityFramework;

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using Catel;
using Catel.Logging;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides an automated way to reuse Entity Framework DbContext objects within the context of a single data portal operation.
/// </summary>
/// <typeparam name="TDbContext">Type of the db context to use.
/// </typeparam>
/// <remarks>
/// This type stores the object context object in an internal dictionary and uses reference counting through
/// <see cref="IDisposable" /> to keep the data context object open for reuse by child objects, and to automatically
/// dispose the object when the last consumer has called Dispose.
/// </remarks>
public class DbContextManager<TDbContext> : ContextManager<TDbContext>
    where TDbContext : DbContext
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(DbContextManager<TDbContext>));

    private readonly IConnectionStringManager _connectionStringManager;
    private readonly IContextFactory _contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbContextManager{TDbContext}"/> class.
    /// </summary>
    /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
    /// <param name="label">The label.</param>
    /// <param name="model">The model.</param>
    /// <param name="context">The context.</param>
    /// <param name="connectionStringManager"></param>
    /// <param name="contextFactory"></param>
    private DbContextManager(string databaseNameOrConnectionStringName, string label,
        DbCompiledModel? model, ObjectContext? context,
        IConnectionStringManager connectionStringManager, IContextFactory contextFactory)
        : base(databaseNameOrConnectionStringName, label, model, context, connectionStringManager, contextFactory)
    {
        _connectionStringManager = connectionStringManager;
        _contextFactory = contextFactory;
        // Note: leave empty
    }

    /// <summary>
    /// Initializes the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    protected override void Initialize(TDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            context.Database.Initialize(false);
        }
        catch (Exception)
        {
            Logger.LogWarning("Failed to initialize database context '{0}', probably the connection cannot be established", context.GetType().FullName);
        }
    }

    /// <summary>
    /// Gets the ContextManager object for the specified database.
    /// </summary>
    /// <returns>The <see cref="DbContextManager{TDbContext}" />.</returns>
    public static DbContextManager<TDbContext> GetManager(IConnectionStringManager connectionStringManager, IContextFactory contextFactory)
    {
        return GetManager(string.Empty, null, null, connectionStringManager, contextFactory);
    }

    /// <summary>
    /// Gets the ContextManager object for the specified database.
    /// </summary>
    /// <param name="databaseNameOrConnectionStringName">The database name or connection string.</param>
    /// <param name="label">Label for this context.</param>
    /// <param name="model">Database Compiled model.</param>
    /// <param name="connectionStringManager"></param>
    /// <param name="contextFactory"></param>
    /// <returns>The ContextManager.</returns>
    public static DbContextManager<TDbContext> GetManager(string databaseNameOrConnectionStringName, 
        string? label, DbCompiledModel? model,
        IConnectionStringManager connectionStringManager, IContextFactory contextFactory)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            label = "default";
        }

        return (DbContextManager<TDbContext>)GetManager(databaseNameOrConnectionStringName, label, () => 
        { 
            return new DbContextManager<TDbContext>(databaseNameOrConnectionStringName, label, model, 
                null, connectionStringManager, contextFactory); 
        });
    }
}
