namespace Orc.EntityFramework.Tests
{
    using Catel.Data;
    using DbContext;
    using NUnit.Framework;

    public class DbContextManagerFacts
    {
        [TestFixture]
        public class ScopingTest
        {
            [TestCase]
            public void SingleLevelScoping()
            {
                DbContextManager<TestDbContextContainer> manager = null;

                using (manager = DbContextManager<TestDbContextContainer>.GetManager())
                {
                    Assert.That(manager.RefCount, Is.EqualTo(1));
                }

                Assert.That(manager.RefCount, Is.EqualTo(0));
            }

            [TestCase]
            public void MultipleLevelScoping()
            {
                DbContextManager<TestDbContextContainer> manager = null;

                using (manager = DbContextManager<TestDbContextContainer>.GetManager())
                {
                    Assert.That(manager.RefCount, Is.EqualTo(1));

                    using (DbContextManager<TestDbContextContainer>.GetManager())
                    {
                        Assert.That(manager.RefCount, Is.EqualTo(2));

                        using (DbContextManager<TestDbContextContainer>.GetManager())
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
