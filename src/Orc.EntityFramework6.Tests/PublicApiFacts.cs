namespace Orc.EntityFramework.Tests
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using PublicApiGenerator;
    using VerifyNUnit;

    [TestFixture]
    public class PublicApiFacts
    {
#if EF6
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Orc_EntityFramework_HasNoBreakingChanges_Async()
        {
            var assembly = typeof(UnitOfWork).Assembly;

            await PublicApiApprover.ApprovePublicApiAsync(assembly);
        }

        internal static class PublicApiApprover
        {
            public static async Task ApprovePublicApiAsync(Assembly assembly)
            {
                var publicApi = ApiGenerator.GeneratePublicApi(assembly, new ApiGeneratorOptions());
                await Verifier.Verify(publicApi);
            }
        }
#endif
    }
}
