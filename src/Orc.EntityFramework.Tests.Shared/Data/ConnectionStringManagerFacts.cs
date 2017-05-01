// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringManagerFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.EntityFramework.Tests
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

                Assert.AreEqual(null, connectionStringManager.GetConnectionString(typeof(TestDbContextContainer), null, null));
            }
        }
    }
}
