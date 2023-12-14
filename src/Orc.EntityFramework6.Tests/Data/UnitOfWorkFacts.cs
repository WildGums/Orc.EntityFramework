﻿namespace Orc.EntityFramework.Tests
{
    using System;

    using Catel.Data;
    using DbContext;
    using DbContext.Repositories;
    using NUnit.Framework;

    public class UnitOfWorkFacts
    {
        [TestFixture]
        public class TheIsInTransactionProperty
        {
            [TestCase]
            public void ReturnsTrueWhenInTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
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
                using (var uow = new UnitOfWork<TestDbContextContainer>())
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
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    Assert.Throws<InvalidOperationException>(() => uow.RollBackTransaction());
                }
            }

            // TODO: Check if this item can correctly rollback transactions
        }

        [TestFixture]
        public class TheCommitTransactionMethod
        {
            [TestCase]
            public void ThrowsInvalidOperationExceptionWhenCalledWhenNotInTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    Assert.Throws<InvalidOperationException>(() => uow.CommitTransaction());
                }
            }

            [TestCase]
            public void CorrectlyCommitsTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();
                    var productRepository = uow.GetRepository<IDbContextProductRepository>();
                    var orderRepository = uow.GetRepository<IDbContextOrderRepository>();

                    uow.BeginTransaction();

                    var customer = EFTestHelper.CreateCustomer(451);
                    customerRepository.Add(customer);

                    var product = EFTestHelper.CreateProduct(451);
                    productRepository.Add(product);

                    var order = new DbContextOrder { OrderCreated = DateTime.Now, Amount = 1, CustomerId = 451, ProductId = 451 };
                    orderRepository.Add(order);

                    uow.CommitTransaction();
                }

                using (var uow = new UnitOfWork<TestDbContextContainer>())
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
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    var orderRepository = uow.GetRepository<IDbContextOrderRepository>();

                    uow.BeginTransaction();

                    var order = new DbContextOrder { Amount = 1, CustomerId = 999, ProductId = 999 };
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

        [TestFixture]
        public class TheSaveChangesMethod
        {
            [TestCase]
            public void ThrowsInvalidOperationExceptionWhenCalledInsideTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    uow.BeginTransaction();

                    Assert.Throws<InvalidOperationException>(() => uow.SaveChanges());
                }
            }

            [TestCase]
            public void CorrectlySavesChangesWhenNotInTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();

                    var customer = EFTestHelper.CreateCustomer(401);
                    customerRepository.Add(customer);

                    uow.SaveChanges();
                }

                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();

                    var customer = customerRepository.GetByKey(401);

                    Assert.That(customer, Is.Not.Null);
                }
            }
        }
    }
}
