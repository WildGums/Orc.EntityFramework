﻿namespace Orc.EntityFramework.Tests
{
    using DbContext;
    using DbContext.Repositories;

    public static class EFTestHelper
    {
        public static DbContextCustomer CreateCustomer(int id)
        {
            var customer = new DbContextCustomer { Id = id, Name = "Geert van Horrik", Street = "Unknown", Country = "The Netherlands" };
            return customer;
        }

        public static DbContextProduct CreateProduct(int id)
        {
            var product = new DbContextProduct { Id = id, Name = "Very special product" };
            return product;
        }

        public static void CreateCustomerIfNotAlreadyExists(int id)
        {
            using (var dbContext = new TestDbContextContainer())
            {
                using (var repository = new DbContextCustomerRepository(dbContext))
                {
                    var existingCustomer = repository.FirstOrDefault(x => x.Id == id);
                    if (existingCustomer is null)
                    {
                        var customer = CreateCustomer(id);

                        repository.Add(customer);

                        dbContext.SaveChanges();
                    }
                }
            }
        }
    }
}