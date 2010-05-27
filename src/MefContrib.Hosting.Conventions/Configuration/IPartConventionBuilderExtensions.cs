namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;

    public static class IPartConventionBuilderExtensions
    {
        public static PartConventionBuilder<PartConvention> ImportConstructor(
            this PartConventionBuilder<PartConvention> builder)
        {
            builder.Imports(x => x.Import().Members(m => new[] { m.GetGreediestConstructor() }));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportTypeAs<T>(
            this PartConventionBuilder<PartConvention> builder)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m }).ContractType<T>());

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ForTypesAssignableFrom<T>(
            this PartConventionBuilder<PartConvention> builder)
        {
            Predicate<Type> condition =
                t => typeof(T).IsAssignableFrom(t) && !t.IsInterface;
            
            builder.ProvideValueFor(x => x.Condition, condition);

            return builder;
        }
    }
}