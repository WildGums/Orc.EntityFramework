// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextManagerFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
                    Assert.AreEqual(1, manager.RefCount);
                }

                Assert.AreEqual(0, manager.RefCount);
            }

            [TestCase]
            public void MultipleLevelScoping()
            {
                DbContextManager<TestDbContextContainer> manager = null;

                using (manager = DbContextManager<TestDbContextContainer>.GetManager())
                {
                    Assert.AreEqual(1, manager.RefCount);

                    using (DbContextManager<TestDbContextContainer>.GetManager())
                    {
                        Assert.AreEqual(2, manager.RefCount);

                        using (DbContextManager<TestDbContextContainer>.GetManager())
                        {
                            Assert.AreEqual(3, manager.RefCount);
                        }

                        Assert.AreEqual(2, manager.RefCount);
                    }

                    Assert.AreEqual(1, manager.RefCount);
                }

                Assert.AreEqual(0, manager.RefCount);
            }
        }
    }
}