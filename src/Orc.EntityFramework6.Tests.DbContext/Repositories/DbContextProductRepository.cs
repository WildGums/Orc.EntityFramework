namespace Orc.EntityFramework.Tests.DbContext.Repositories
{
    using System.Data.Entity;

    using Orc.EntityFramework.Repositories;

    public interface IDbContextProductRepository : IEntityRepository<DbContextProduct, int>
    {
    }

    public class DbContextProductRepository : EntityRepositoryBase<DbContextProduct, int>, IDbContextProductRepository
    {
        public DbContextProductRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
