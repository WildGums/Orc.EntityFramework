namespace Orc.EntityFramework;

using System;
using System.Data;
using System.Data.Entity;
using Catel;
using Catel.Logging;
using Microsoft.Extensions.Logging;

/// <summary>
/// Extensions to the <see cref="DbContext"/> class.
/// </summary>
public static partial class DbContextExtensions
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(DbContextExtensions));

    /// <summary>
    /// Sets the transaction level of the specified <see cref="DbContext"/>.
    /// </summary>
    /// <param name="dbContext">The db context.</param>
    /// <param name="isolationLevel">The isolation level to set.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="dbContext"/> is <c>null</c>.</exception>
    public static void SetTransactionLevel(this DbContext dbContext, IsolationLevel isolationLevel)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        Logger.LogInformation("Setting transaction isolation level to '{0}' for DbContext '{1}'", isolationLevel, ObjectToStringHelper.ToFullTypeString(dbContext));

        var sqlCommand = IsolationHelper.TranslateTransactionLevelToSql(isolationLevel);
        var objectContext = dbContext.GetObjectContext();
        objectContext.ExecuteStoreCommand(sqlCommand);
    }
}
