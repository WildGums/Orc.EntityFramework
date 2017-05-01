// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextProductRepository.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.EntityFramework.Tests.DbContext.Repositories
{
    using System.Data.Entity;

    using Orc.EntityFramework.Repositories;

    public interface IDbContextProductRepository : IEntityRepository<DbContextProduct, int>
    {
    }

    public class DbContextProductRepository : EntityRepositoryBase<DbContextProduct, int>, IDbContextProductRepository
    {
        #region Constructors
        public DbContextProductRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
        #endregion
    }
}