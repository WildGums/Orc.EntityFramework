namespace Orc.EntityFramework.Tests
{
    using System;
    using System.Data;
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
                Assert.Throws<ArgumentNullException>(() => DbContextExtensions.GetObjectContext(null));
            }

            [TestCase]
            public void ReturnsObjectContextForDbContext()
            {
                using (var dbContext = new TestDbContextContainer())
                {
#pragma warning disable IDISP001
                    var objectContext = dbContext.GetObjectContext();
#pragma warning restore IDISP001

                    Assert.That(objectContext, Is.Not.Null);
                }
            }
        }

        [TestFixture]
        public class TheGetEntityKeyMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                Assert.Throws<ArgumentNullException>(() => DbContextExtensions.GetEntityKey(null, typeof(DbContextProduct), 1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNulEntityType()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    Assert.Throws<ArgumentNullException>(() => dbContext.GetEntityKey(null, 1));
                }
            }

            [TestCase]
            public void ReturnsCorrectKeyValue()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    var keyValue = dbContext.GetEntityKey(typeof(DbContextProduct), 1);

                    Assert.That(keyValue.EntityKeyValues[0].Key, Is.EqualTo("Id"));
                    Assert.That(keyValue.EntityKeyValues[0].Value, Is.EqualTo(1));
                }
            }
        }

        [TestFixture]
        public class TheGetEntitySetNameMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                Assert.Throws<ArgumentNullException>(() => DbContextExtensions.GetEntitySetName(null, typeof(DbContextProduct)));
            }

            [TestCase]
            public void ReturnsCorrectEntitySetName()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    var entitySetName = dbContext.GetEntitySetName(typeof(DbContextProduct));

                    Assert.That(entitySetName, Is.EqualTo("DbContextProducts"));
                }
            }
        }

        [TestFixture]
        public class TheGetFullEntitySetNameMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                Assert.Throws<ArgumentNullException>(() => DbContextExtensions.GetFullEntitySetName(null, typeof(DbContextProduct)));
            }

            [TestCase]
            public void ReturnsCorrectEntitySetName()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    var entitySetName = dbContext.GetFullEntitySetName(typeof(DbContextProduct));

                    Assert.That(entitySetName, Is.EqualTo("TestDbContextContainer.DbContextProducts"));
                }
            }   
        }

        [TestFixture]
        public class TheGetTableNameMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullContext()
            {
                Assert.Throws<ArgumentNullException>(() => DbContextExtensions.GetTableName((System.Data.Entity.DbContext)null, typeof(DbContextProduct)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    Assert.Throws<ArgumentNullException>(() => dbContext.GetTableName(null));
                }
            }

            [TestCase]
            public void ReturnsTableNameIncludingSchemaForType()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    var tableName = dbContext.GetTableName<DbContextOrder>();

                    Assert.That(tableName, Is.EqualTo("[dbo].[DbContextOrder]"));
                }
            }
        }

        [TestFixture]
        public class TheSetTransactionLevelMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                Assert.Throws<ArgumentNullException>(() => DbContextExtensions.SetTransactionLevel(null, IsolationLevel.ReadUncommitted));
            }
        }
    }
}
