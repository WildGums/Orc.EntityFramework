// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectContextManagerFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if EF5

namespace Orc.EntityFramework.Tests
{
    using Catel.Data;
    using NUnit.Framework;
    using ObjectContext;

    public class ObjectContextManagerFacts
    {
        [TestFixture]
        public class ScopingTest
        {
            [TestCase]
            public void SingleLevelScoping()
            {
                ObjectContextManager<TestObjectContextContainer> manager = null;

                using (manager = ObjectContextManager<TestObjectContextContainer>.GetManager())
                {
                    Assert.AreEqual(1, manager.RefCount);
                }

                Assert.AreEqual(0, manager.RefCount);
            }

            [TestCase]
            public void MultipleLevelScoping()
            {
                ObjectContextManager<TestObjectContextContainer> manager = null;

                using (manager = ObjectContextManager<TestObjectContextContainer>.GetManager())
                {
                    Assert.AreEqual(1, manager.RefCount);

                    using (ObjectContextManager<TestObjectContextContainer>.GetManager())
                    {
                        Assert.AreEqual(2, manager.RefCount);

                        using (ObjectContextManager<TestObjectContextContainer>.GetManager())
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

#endif