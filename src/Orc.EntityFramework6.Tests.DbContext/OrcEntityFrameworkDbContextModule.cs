namespace Orc.EntityFramework
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Orc.EntityFramework.Tests.DbContext.Repositories;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcEntityFrameworkDbContextModule
    {
        public static IServiceCollection AddOrcEntityFrameworkDbContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient<IDbContextCustomerRepository, DbContextCustomerRepository>();
            serviceCollection.TryAddTransient<IDbContextOrderRepository, DbContextOrderRepository>();
            serviceCollection.TryAddTransient<IDbContextProductRepository, DbContextProductRepository>();

            return serviceCollection;
        }
    }
}
