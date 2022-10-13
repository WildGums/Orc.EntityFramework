namespace Orc.EntityFramework.Tests.DbContext.Repositories
{
    using System.Data.Entity;

    using Orc.EntityFramework.Repositories;

    public interface IDbContextOrderRepository : IEntityRepository<DbContextOrder, int>
    {
    }

    public class DbContextOrderRepository : EntityRepositoryBase<DbContextOrder, int>, IDbContextOrderRepository
    {
        public DbContextOrderRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
