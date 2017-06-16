// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextOrderRepository.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.EntityFramework.Tests.DbContext.Repositories
{
    using System.Data.Entity;

    using Orc.EntityFramework.Repositories;

    public interface IDbContextOrderRepository : IEntityRepository<DbContextOrder, int>
    {
    }

    public class DbContextOrderRepository : EntityRepositoryBase<DbContextOrder, int>, IDbContextOrderRepository
    {
        #region Constructors
        public DbContextOrderRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
        #endregion
    }
}