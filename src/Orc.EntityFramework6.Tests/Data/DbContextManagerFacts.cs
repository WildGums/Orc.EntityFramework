namespace Orc.EntityFramework.Tests
{
    using Catel.Data;
    using DbContext;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public class DbContextManagerFacts
    {
        [TestFixture]
        public class ScopingTest
        {
            [TestCase]
            public void SingleLevelScoping()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var connectionStringManager = serviceProvider.GetRequiredService<IConnectionStringManager>();
                var contextFactory = serviceProvider.GetRequiredService<IContextFactory>(); 

                DbContextManager<TestDbContextContainer> manager = null;

                using (manager = DbContextManager<TestDbContextContainer>.GetManager(connectionStringManager, contextFactory))
                {
                    Assert.That(manager.RefCount, Is.EqualTo(1));
                }

                Assert.That(manager.RefCount, Is.EqualTo(0));
            }

            [TestCase]
            public void MultipleLevelScoping()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var connectionStringManager = serviceProvider.GetRequiredService<IConnectionStringManager>();
                var contextFactory = serviceProvider.GetRequiredService<IContextFactory>();

                DbContextManager<TestDbContextContainer> manager = null;

                using (manager = DbContextManager<TestDbContextContainer>.GetManager(connectionStringManager, contextFactory))
                {
                    Assert.That(manager.RefCount, Is.EqualTo(1));

                    using (DbContextManager<TestDbContextContainer>.GetManager(connectionStringManager, contextFactory))
                    {
                        Assert.That(manager.RefCount, Is.EqualTo(2));

                        using (DbContextManager<TestDbContextContainer>.GetManager(connectionStringManager, contextFactory))
                        {
                            Assert.That(manager.RefCount, Is.EqualTo(3));
                        }

                        Assert.That(manager.RefCount, Is.EqualTo(2));
                    }

                    Assert.That(manager.RefCount, Is.EqualTo(1));
                }

                Assert.That(manager.RefCount, Is.EqualTo(0));
            }
        }
    }
}
