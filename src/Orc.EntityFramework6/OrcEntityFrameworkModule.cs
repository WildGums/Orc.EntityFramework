namespace Orc.EntityFramework
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcEntityFrameworkModule
    {
        public static IServiceCollection AddOrcEntityFramework(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            serviceCollection.TryAddSingleton<IConnectionStringManager, ConnectionStringManager>();
            serviceCollection.TryAddSingleton<IContextFactory, ContextFactory>();

            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.EntityFramework", "Orc.EntityFramework.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.EntityFramework", "https://github.com/wildgums/orc.entityframework"));

            return serviceCollection;
        }
    }
}
