namespace Orc.EntityFramework.Tests
{
    using Catel.Data;
    using DbContext;
    using NUnit.Framework;

#if EF5
    using ObjectContext;
#endif

    public class ContextManagerFacts
    {
        [TestFixture]
        public class TheTypeInstantiation
        {
            [TestCase]
            public void WorksForDbContext()
            {
                using (var manager = DbContextManager<TestDbContextContainer>.GetManager())
                {
                    Assert.IsNotNull(manager);
                }
            }

#if EF5
            [TestCase]
            public void WorksForObjectContext()
            {
                using (var manager = ObjectContextManager<TestObjectContextContainer>.GetManager())
                {
                    Assert.IsNotNull(manager);
                }
            }
#endif
        }
    }
}