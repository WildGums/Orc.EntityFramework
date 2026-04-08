namespace Orc.EntityFramework.Tests;

using Catel.Data;
using DbContext;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

public class ContextManagerFacts
{
    [TestFixture]
    public class TheTypeInstantiation
    {
        [TestCase]
        public void WorksForDbContext()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var connectionStringManager = serviceProvider.GetRequiredService<IConnectionStringManager>();
            var contextFactory = serviceProvider.GetRequiredService<IContextFactory>();

            using (var manager = DbContextManager<TestDbContextContainer>.GetManager(connectionStringManager, contextFactory))
            {
                Assert.That(manager, Is.Not.Null);
            }
        }
    }
}
