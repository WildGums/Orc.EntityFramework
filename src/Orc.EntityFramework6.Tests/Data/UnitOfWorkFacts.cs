namespace Orc.EntityFramework.Tests;

using System;
using System.Threading;
using DbContext;
using DbContext.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

public class UnitOfWorkFacts
{
    [TestFixture, RequiresThread(ApartmentState.STA)]
    public class TheIsInTransactionProperty
    {
        [TestCase]
        public void ReturnsTrueWhenInTransaction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var uow = serviceProvider.GetRequiredService<IUnitOfWork<TestDbContextContainer>>())
            {
                Assert.That(uow.IsInTransaction, Is.False);

                uow.BeginTransaction();

                Assert.That(uow.IsInTransaction, Is.True);

                uow.CommitTransaction();

                Assert.That(uow.IsInTransaction, Is.False);
            }
        }
    }

    [TestFixture]
    public class TheBeginTransactionMethod
    {
        [TestCase]
        public void ThrowsInvalidOperationExceptionWhenCalledWhenAlreadyInTransaction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var uow = serviceProvider.GetRequiredService<IUnitOfWork<TestDbContextContainer>>())
            {
                uow.BeginTransaction();

                Assert.Throws<InvalidOperationException>(() => uow.BeginTransaction());
            }
        }
    }

    [TestFixture]
    public class TheRollbackTransactionMethod
    {
        [TestCase]
        public void ThrowsInvalidOperationExceptionWhenCalledWhenNotInTransaction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var uow = serviceProvider.GetRequiredService<IUnitOfWork<TestDbContextContainer>>())
            {
                Assert.Throws<InvalidOperationException>(() => uow.RollBackTransaction());
            }
        }

        // TODO: Check if this item can correctly rollback transactions
    }

    [TestFixture, RequiresThread(ApartmentState.STA)]
    public class TheCommitTransactionMethod
    {
        [TestCase]
        public void ThrowsInvalidOperationExceptionWhenCalledWhenNotInTransaction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var uow = serviceProvider.GetRequiredService<IUnitOfWork<TestDbContextContainer>>())
            {
                Assert.Throws<InvalidOperationException>(() => uow.CommitTransaction());
            }
        }

        [TestCase]
        public void CorrectlyCommitsTransaction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var uow = serviceProvider.GetRequiredService<IUnitOfWork<TestDbContextContainer>>())
            {
                var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();
                var productRepository = uow.GetRepository<IDbContextProductRepository>();
                var orderRepository = uow.GetRepository<IDbContextOrderRepository>();

                uow.BeginTransaction();

                var customer = EFTestHelper.CreateCustomer(451);
                customerRepository.Add(customer);

                var product = EFTestHelper.CreateProduct(451);
                productRepository.Add(product);

                var order = new DbContextOrder
                {
                    OrderCreated = DateTime.Now,
                    Amount = 1,
                    CustomerId = 451,
                    ProductId = 451
                };

                orderRepository.Add(order);

                uow.CommitTransaction();
            }

            using (var uow = serviceProvider.GetRequiredService<IUnitOfWork<TestDbContextContainer>>())
            {
                var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();
                var productRepository = uow.GetRepository<IDbContextProductRepository>();
                var orderRepository = uow.GetRepository<IDbContextOrderRepository>();

                var customer = customerRepository.GetByKey(451);
                Assert.That(customer, Is.Not.Null);

                var product = productRepository.GetByKey(451);
                Assert.That(product, Is.Not.Null);

                var order = orderRepository.FirstOrDefault(x => x.CustomerId == 451 && x.ProductId == 451);
                Assert.That(order, Is.Not.Null);
            }
        }

        [TestCase]
        public void CorrectlyRollbacksTransactionWhenAnErrorOccursWhileSaving()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var uow = serviceProvider.GetRequiredService<IUnitOfWork<TestDbContextContainer>>())
            {
                var orderRepository = uow.GetRepository<IDbContextOrderRepository>();

                uow.BeginTransaction();

                var order = new DbContextOrder
                {
                    Amount = 1,
                    CustomerId = 999,
                    ProductId = 999
                };

                orderRepository.Add(order);

                try
                {
                    uow.CommitTransaction();

                    Assert.Fail("Expected an exception");
                }
                catch (Exception)
                {
                    Assert.That(uow.IsInTransaction, Is.False);
                }
            }
        }
    }

    [TestFixture, RequiresThread(ApartmentState.STA)]
    public class TheSaveChangesMethod
    {
        [TestCase]
        public void ThrowsInvalidOperationExceptionWhenCalledInsideTransaction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var uow = serviceProvider.GetRequiredService<IUnitOfWork<TestDbContextContainer>>())
            {
                uow.BeginTransaction();

                Assert.Throws<InvalidOperationException>(() => uow.SaveChanges());
            }
        }

        [TestCase]
        public void CorrectlySavesChangesWhenNotInTransaction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var uow = serviceProvider.GetRequiredService<IUnitOfWork<TestDbContextContainer>>())
            {
                var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();

                var customer = EFTestHelper.CreateCustomer(401);
                customerRepository.Add(customer);

                uow.SaveChanges();
            }

            using (var uow = serviceProvider.GetRequiredService<IUnitOfWork<TestDbContextContainer>>())
            {
                var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();

                var customer = customerRepository.GetByKey(401);

                Assert.That(customer, Is.Not.Null);
            }
        }
    }
}
