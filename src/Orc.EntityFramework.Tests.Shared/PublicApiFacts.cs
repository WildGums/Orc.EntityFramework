// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.EntityFramework.Tests
{
    using ApiApprover;
    using NUnit.Framework;
    using System.Runtime.CompilerServices;

    [TestFixture]
    public class PublicApiFacts
    {
#if EF6
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public void Orc_EntityFramework_HasNoBreakingChanges()
        {
            var assembly = typeof(UnitOfWork).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
#endif
    }
}