// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringHelperFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if EF5

namespace Orc.EntityFramework.Tests.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Objects;
    using Catel.Data;
    using Catel.Tests;
    using DbContext;
    using NUnit.Framework;
    using ObjectContext;

    public class ConnectionStringHelperFacts
    {
        [TestFixture]
        public class TheDbContextSetConnectionStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ConnectionStringHelper.SetConnectionString((DbContext)null, "dummy"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrWhitespaceConnectionString()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ConnectionStringHelper.SetConnectionString(dbContext, null));
                    ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ConnectionStringHelper.SetConnectionString(dbContext, string.Empty));
                }
            }

            [TestCase]
            public void SetsConnectionString()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    dbContext.SetConnectionString(TestConnectionStrings.DbContextModified);

                    Assert.AreEqual(TestConnectionStrings.DbContextModified, dbContext.Database.Connection.ConnectionString);
                }
            }
        }

        [TestFixture]
        public class TheObjectContextSetConnectionStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ConnectionStringHelper.SetConnectionString((DbContext)null, "dummy"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrWhitespaceConnectionString()
            {
                var connectionString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextDefault);
                using (var objectContext = new TestObjectContextContainer(connectionString))
                {
                    ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ConnectionStringHelper.SetConnectionString(objectContext, null));
                    ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ConnectionStringHelper.SetConnectionString(objectContext, string.Empty));
                }
            }

            [TestCase]
            public void SetsConnectionString()
            {
                var connectionString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextDefault);
                using (var objectContext = new TestObjectContextContainer(connectionString))
                {
                    objectContext.SetConnectionString(TestConnectionStrings.ObjectContextModified);

                    var expectedConnectionString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextModified);
                    Assert.AreEqual(expectedConnectionString, objectContext.Connection.ConnectionString);
                }
            }
        }

        [TestFixture]
        public class TheDbContextGetConnectionStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ConnectionStringHelper.SetConnectionString((DbContext)null, "dummy"));
            }

            [TestCase]
            public void ReturnsNamedConnectionString()
            {
                using (var context = new TestDbContextContainer())
                {
                    var expectedString = string.Format("{0};Application Name=EntityFrameworkMUE", TestConnectionStrings.DbContextDefault);

                    var connectionString = context.GetConnectionString();

                    Assert.IsTrue(string.Equals(expectedString, connectionString, StringComparison.OrdinalIgnoreCase));
                }
            }

            [TestCase]
            public void ReturnsRealConnectionString()
            {
                using (var context = new TestDbContextContainer())
                {
                    var expectedString = TestConnectionStrings.DbContextModified;

                    context.SetConnectionString(TestConnectionStrings.DbContextModified);
                    var connectionString = context.GetConnectionString();

                    Assert.IsTrue(string.Equals(expectedString, connectionString, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        [TestFixture]
        public class TheObjectContextGetConnectionStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullObjectContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ConnectionStringHelper.SetConnectionString((ObjectContext)null, "dummy"));
            }

            [TestCase]
            public void ReturnsNamedConnectionString()
            {
                using (var context = new TestObjectContextContainer())
                {
                    var expectedString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextDefault);

                    var connectionString = context.GetConnectionString();

                    Assert.IsTrue(string.Equals(expectedString, connectionString, StringComparison.OrdinalIgnoreCase));
                }
            }

            [TestCase]
            public void ReturnsRealConnectionString()
            {
                using (var context = new TestObjectContextContainer())
                {
                    var expectedString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextModified);

                    context.SetConnectionString(TestConnectionStrings.ObjectContextModified);
                    var connectionString = context.GetConnectionString();

                    Assert.IsTrue(string.Equals(expectedString, connectionString, StringComparison.OrdinalIgnoreCase));
                }
            }
        }
    }
}

#endif
