// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableExtensionsFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if EF5

namespace Orc.EntityFramework.Tests
{
    using System;
    using System.Linq;
    using Catel.Data;
    using Catel.Tests;
    using DbContext;
    using DbContext.Repositories;
    using NUnit.Framework;

    public class QueryableExtensionsFacts
    {
        [TestFixture]
        public class TheIncludeMethod
        {

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullQueryable()
            {
                Assert.Throws<ArgumentNullException>(() => QueryableExtensions.Include<DbContextCustomer>(null, x => x.DbContextOrders));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullExpression()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        Assert.Throws<ArgumentNullException>(() => repository.GetAll().Include(null));
                    }
                }
            }

            [TestCase]
            public void IncludesEntitiesUsingExpression()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(42);

                        var existingCustomer = repository.GetAll().Include(x => x.DbContextOrders).FirstOrDefault();

                        Assert.IsNotNull(existingCustomer);
                    }
                }
            }
        }
    }
}

#endif