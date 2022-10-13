// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.EntityFramework.Tests
{
    using ApprovalTests;
    using ApprovalTests.Namers;
    using NUnit.Framework;
    using PublicApiGenerator;
    using System.IO;
    using System.Reflection;
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

        internal static class PublicApiApprover
        {
            public static void ApprovePublicApi(Assembly assembly)
            {
                var publicApi = ApiGenerator.GeneratePublicApi(assembly, new ApiGeneratorOptions());
                var writer = new ApprovalTextWriter(publicApi, "cs");
                var approvalNamer = new AssemblyPathNamer(assembly.Location);
                Approvals.Verify(writer, approvalNamer, Approvals.GetReporter());
            }
        }

        internal class AssemblyPathNamer : UnitTestFrameworkNamer
        {
            private readonly string _name;

            public AssemblyPathNamer(string assemblyPath)
            {
                _name = Path.GetFileNameWithoutExtension(assemblyPath);

            }
            public override string Name
            {
                get { return _name; }
            }
        }
#endif
    }
}
