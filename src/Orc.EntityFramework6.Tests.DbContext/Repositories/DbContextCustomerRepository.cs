namespace Orc.EntityFramework.Tests.DbContext.Repositories
{
    using System.Data.Entity;

    using Orc.EntityFramework.Repositories;

    public interface IDbContextCustomerRepository : IEntityRepository<DbContextCustomer, int>
    {
    }

    public class DbContextCustomerRepository : EntityRepositoryBase<DbContextCustomer, int>, IDbContextCustomerRepository
    {
        public DbContextCustomerRepository(DbContext dbContext)
            : base(dbContext)
        {   
        }
    }
}
