namespace Orc.EntityFramework
{
    using System.Linq;
    using System.Reflection;
    using Catel.Reflection;

    /// <summary>
    /// Helper class for the <see cref="DbContextManager{TDbContext}"/> class.
    /// </summary>
    public static class DbContextManagerHelper
    {
        private static readonly MethodInfo _genericCreateDbContextForHttpContextMethod;
        private static readonly MethodInfo _genericDisposeDbContextForHttpContextMethod;

        /// <summary>
        /// Initializes static members of the <see cref="DbContextManagerHelper" /> class.
        /// </summary>
        static DbContextManagerHelper()
        {
            _genericCreateDbContextForHttpContextMethod = (from method in typeof(DbContextManagerHelper).GetMethodsEx(false, true)
                                                           where method.Name == "CreateDbContextForHttpContext" && method.IsGenericMethod
                                                           select method).First();

            _genericDisposeDbContextForHttpContextMethod = (from method in typeof(DbContextManagerHelper).GetMethodsEx(false, true)
                                                            where method.Name == "DisposeDbContextForHttpContext" && method.IsGenericMethod
                                                            select method).First();
        }

        /// <summary>
        /// Gets the db context key to be used in an http context.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the db context.</typeparam>
        /// <returns>The context key.</returns>
        private static string GetDbContextKey<TDbContext>()
        {
            return string.Format("{0}Key", typeof(TDbContext).Name);
        }
    }
}
