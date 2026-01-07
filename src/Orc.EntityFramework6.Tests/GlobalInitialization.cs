using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Catel.Logging;
using Catel.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Orc.EntityFramework.Tests;
using Orc.EntityFramework.Tests.DbContext;

/// <summary>
/// Sets the current culture to <c>en-US</c> for all unit tests to prevent tests to fail
/// due to cultural string differences.
/// </summary>
[SetUpFixture]
public class GlobalInitialization
{
    [OneTimeSetUp]
    public static void SetUp()
    {
        LogManager.FallbackLoggerFactory = LoggerFactory.Create(x =>
        {
            if (Debugger.IsAttached)
            {
                x.AddFilter(x => x == LogLevel.Debug);

                x.AddDebug();
            }

            x.AddConsole();
        });

        var culture = new CultureInfo("en-US");
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

        // Required since we do multithreaded initialization
        TypeCache.InitializeTypes(allowMultithreadedInitialization: false);

        // Set a global service provider for helpers such as LanguageHelper
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        Catel.IoC.IoCContainer.ServiceProvider = serviceCollection.BuildServiceProvider();

        using (var dbContext = new TestDbContextContainer())
        {
            dbContext.Database.CreateIfNotExists();

            // Delete all data
            var allOrders = (from x in dbContext.DbContextOrders
                             select x).ToList();
            foreach (var x in allOrders)
            {
                dbContext.DbContextOrders.Remove(x);
            }

            var allCustomers = (from x in dbContext.DbContextCustomers
                                select x).ToList();
            foreach (var x in allCustomers)
            {
                dbContext.DbContextCustomers.Remove(x);
            }

            var allProducts = (from x in dbContext.DbContextProducts
                               select x).ToList();
            foreach (var x in allProducts)
            {
                dbContext.DbContextProducts.Remove(x);
            }

            dbContext.SaveChanges();
        }
    }
}
