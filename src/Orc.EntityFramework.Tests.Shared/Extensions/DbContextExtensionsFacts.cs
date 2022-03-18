﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextExtensionsFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.EntityFramework.Tests
{
    using System;
    using System.Data;
    using System.Data.Entity;

    using Catel.Data;
    using Catel.Tests;
    using DbContext;
    using NUnit.Framework;

    public class DbContextExtensionsFacts
    {
        [TestFixture]
        public class TheGetObjectContextMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetObjectContext(null));
            }

            [TestCase]
            public void ReturnsObjectContextForDbContext()
            {
                using (var dbContext = new TestDbContextContainer())
                {
#pragma warning disable IDISP001
                    var objectContext = dbContext.GetObjectContext();
#pragma warning restore IDISP001

                    Assert.IsNotNull(objectContext);
                }
            }
        }

        [TestFixture]
        public class TheGetEntityKeyMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetEntityKey(null, typeof(DbContextProduct), 1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNulEntityType()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dbContext.GetEntityKey(null, 1));
                }
            }

            [TestCase]
            public void ReturnsCorrectKeyValue()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    var keyValue = dbContext.GetEntityKey(typeof(DbContextProduct), 1);

                    Assert.AreEqual("Id", keyValue.EntityKeyValues[0].Key);
                    Assert.AreEqual(1, keyValue.EntityKeyValues[0].Value);
                }
            }
        }

        [TestFixture]
        public class TheGetEntitySetNameMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetEntitySetName(null, typeof(DbContextProduct)));
            }

            [TestCase]
            public void ReturnsCorrectEntitySetName()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    var entitySetName = dbContext.GetEntitySetName(typeof(DbContextProduct));

                    Assert.AreEqual("DbContextProducts", entitySetName);
                }
            }
        }

        [TestFixture]
        public class TheGetFullEntitySetNameMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetFullEntitySetName(null, typeof(DbContextProduct)));
            }

            [TestCase]
            public void ReturnsCorrectEntitySetName()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    var entitySetName = dbContext.GetFullEntitySetName(typeof(DbContextProduct));

                    Assert.AreEqual("TestDbContextContainer.DbContextProducts", entitySetName);
                }
            }   
        }

        [TestFixture]
        public class TheGetTableNameMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetTableName((System.Data.Entity.DbContext)null, typeof(DbContextProduct)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dbContext.GetTableName(null));
                }
            }

            [TestCase]
            public void ReturnsTableNameIncludingSchemaForType()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    var tableName = dbContext.GetTableName<DbContextOrder>();

                    Assert.AreEqual("[dbo].[DbContextOrder]", tableName);
                }
            }
        }

        [TestFixture]
        public class TheSetTransactionLevelMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.SetTransactionLevel(null, IsolationLevel.ReadUncommitted));
            }
        }
    }
}
