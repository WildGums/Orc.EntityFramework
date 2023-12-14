namespace Orc.EntityFramework.Tests
{
    using System;
    using System.Linq;
    using DbContext;
    using DbContext.Repositories;
    using NUnit.Framework;

    public class EntityRepositoryFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                Assert.Throws<ArgumentNullException>(() => new DbContextOrderRepository(null));
            }
        }

        [TestFixture]
        public class TheGetByKeyMethod
        {
            [TestCase]
            public void ReturnsNullIfKeyIsInvalid()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        Assert.That(repository.GetByKey(12345), Is.Null);
                    }
                }
            }

            [TestCase]
            public void ReturnsEntityIfKeyIsValid()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(42);

                        var existingCustomer = repository.GetByKey(42);

                        Assert.That(existingCustomer, Is.Not.Null);
                    }
                }
            }
        }

        [TestFixture]
        public class TheSingleMethod
        {
            [TestCase]
            public void ThrowsExceptionWhenTableDoesNotContainEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        Assert.Throws<InvalidOperationException>(() => repository.Single(x => x.Id == 999));
                    }
                }
            }

            [TestCase]
            public void ReturnsEntityWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(1);

                        var customer = repository.Single(x => x.Id == 1);

                        Assert.That(customer, Is.Not.Null);
                        Assert.That(customer.Id, Is.EqualTo(1));
                    }
                }
            }
        }

        [TestFixture]
        public class TheSingleOrDefaultMethod
        {
            [TestCase]
            public void ReturnsNullWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        var customer = repository.SingleOrDefault(x => x.Id == 999);
                        Assert.That(customer, Is.Null);
                    }
                }
            }

            [TestCase]
            public void ReturnsEntityWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(1);

                        var customer = repository.SingleOrDefault(x => x.Id == 1);

                        Assert.That(customer, Is.Not.Null);
                        Assert.That(customer.Id, Is.EqualTo(1));
                    }
                }
            }
        }

        [TestFixture]
        public class TheFirstMethod
        {
            [TestCase]
            public void ThrowsExceptionWhenTableDoesNotContainEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        Assert.Throws<InvalidOperationException>(() => repository.First(x => x.Id == 999));
                    }
                }
            }

            [TestCase]
            public void ReturnsEntityWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(1);

                        var customer = repository.First();

                        Assert.That(customer, Is.Not.Null);
                        Assert.That(customer.Id, Is.EqualTo(1));
                    }
                }
            }
        }

        [TestFixture]
        public class TheFirstOrDefaultMethod
        {
            [TestCase]
            public void ReturnsNullWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        var customer = repository.FirstOrDefault(x => x.Id == 999);
                        Assert.That(customer, Is.Null);
                    }
                }
            }

            [TestCase]
            public void ReturnsEntityWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(1);

                        var customer = repository.FirstOrDefault();

                        Assert.That(customer, Is.Not.Null);
                        Assert.That(customer.Id, Is.EqualTo(1));
                    }
                }
            }
        }

        [TestFixture]
        public class TheAddMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        Assert.Throws<ArgumentNullException>(() => repository.Add(null));
                    }
                }  
            }

            [TestCase]
            public void AddsNonExistingEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        var customer = EFTestHelper.CreateCustomer(1234);

                        repository.Add(customer);

                        dbContext.SaveChanges();

                        var fetchedCustomer = repository.GetByKey(1234);
                        Assert.That(fetchedCustomer, Is.EqualTo(customer));
                    }
                }  
            }
        }

        [TestFixture]
        public class TheAttachMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        Assert.Throws<ArgumentNullException>(() => repository.Attach(null));
                    }
                }
            }

            [TestCase]
            public void AddsNonExistingEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        var customer = EFTestHelper.CreateCustomer(1235);

                        repository.Attach(customer);

                        dbContext.SaveChanges();

                        var fetchedCustomer = repository.GetByKey(1235);
                        Assert.That(fetchedCustomer, Is.EqualTo(customer));
                    }
                }
            }
        }

        [TestFixture]
        public class TheDeleteMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        Assert.Throws<ArgumentNullException>(() => repository.Delete((DbContextCustomer)null));
                    }
                }
            }

            [TestCase]
            public void DeletesSpecificEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(201);

                        var customer1 = repository.GetByKey(201);

                        Assert.That(customer1, Is.Not.Null);

                        repository.Delete(customer1);

                        dbContext.SaveChanges();

                        var customer2 = repository.GetByKey(201);

                        Assert.That(customer2, Is.Null);
                    }
                }
            }

            [TestCase]
            public void SucceedsWhenNoEntitiesMatchFilter()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        repository.Delete(x => x.Id == 999);

                        dbContext.SaveChanges();
                    }
                }
            }

            [TestCase]
            public void SucceedsWhenEntitiesMatchFilter()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(201);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(202);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(203);

                        repository.Delete(x => x.Id >= 201 && x.Id <= 203);

                        dbContext.SaveChanges();

                        Assert.That(repository.GetByKey(201), Is.Null);
                        Assert.That(repository.GetByKey(202), Is.Null);
                        Assert.That(repository.GetByKey(203), Is.Null);
                    }
                }
            }
        }

        [TestFixture]
        public class TheUpdateMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        Assert.Throws<ArgumentNullException>(() => repository.Update(null));
                    }
                }
            }

            [TestCase]
            public void UpdatesEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(301);

                        var customer1 = repository.GetByKey(301);
                        customer1.Name = "John Doe";

                        repository.Update(customer1);

                        dbContext.SaveChanges();

                        var customer2 = repository.GetByKey(301);

                        Assert.That(customer2, Is.Not.Null);
                        Assert.That(customer2.Name, Is.EqualTo("John Doe"));
                    }
                }
            }
        }

        [TestFixture]
        public class TheFindMethod
        {
            [TestCase]
            public void ReturnsCorrectEntities()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(100);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(101);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(102);

                        var customers = repository.Find(x => x.Id >= 100 && x.Id <= 102).ToList();

                        Assert.That(customers.Count, Is.EqualTo(3));
                    }
                }
            }
        }

        [TestFixture]
        public class TheGetAllMethod
        {
            [TestCase]
            public void ReturnsCorrectEntities()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(100);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(101);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(102);

                        var customers = repository.GetAll().ToList();

                        Assert.That(customers.Count >= 3, Is.True);
                    }
                }
            }
        }

        [TestFixture]
        public class TheCountMethod
        {
            [TestCase]
            public void ReturnsCorrectEntityCount()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(100);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(101);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(102);

                        var customerCount = repository.Count(x => x.Id >= 100 && x.Id <= 102);

                        Assert.That(customerCount, Is.EqualTo(3));
                    }
                }
            }
        }
    }
}
