namespace Orc.EntityFramework
{
    using System;
    using System.Data.Entity.ModelConfiguration;
    using Catel.Data;

    /// <summary>
    /// Class EntityTypeConfigurationExtensions.
    /// </summary>
    public static class EntityTypeConfigurationExtensions
    {
        /// <summary>
        /// Ignores the catel properties for database mappings.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>EntityTypeConfiguration&lt;TEntity&gt;.</returns>
        public static EntityTypeConfiguration<TEntity> IgnoreCatelProperties<TEntity>(this EntityTypeConfiguration<TEntity> configuration)
            where TEntity : ModelBase
        {
            ArgumentNullException.ThrowIfNull(configuration);

            //configuration.Ignore(x => x.BusinessRuleErrorCount);
            //configuration.Ignore(x => x.BusinessRuleWarningCount);
            //configuration.Ignore(x => x.FieldErrorCount);
            //configuration.Ignore(x => x.FieldWarningCount);
            //configuration.Ignore(x => x.HasErrors);
            //configuration.Ignore(x => x.HasWarnings);
            configuration.Ignore(x => x.IsDirty);
            //configuration.Ignore(x => x.IsInEditSession);
            configuration.Ignore(x => x.IsReadOnly);
            //configuration.Ignore(x => x.Mode);
            //configuration.Ignore(x => x.ValidationContext);
            //configuration.Ignore(x => x.Validator);

            return configuration;
        }
    }
}
