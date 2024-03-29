﻿namespace Orc.EntityFramework.Tests
{
    using Catel.Data;
    using DbContext;
    using NUnit.Framework;

    public class ConnectionStringManagerFacts
    {
        [TestFixture]
        public class TheGetConnectionStringMethod
        {
            [TestCase]
            public void ReturnsNullByDefault()
            {
                var connectionStringManager = new ConnectionStringManager();

                Assert.That(connectionStringManager.GetConnectionString(typeof(TestDbContextContainer), null, null), Is.EqualTo(null));
            }
        }
    }
}
