namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;

    public static class PartConventionBuilderExtensions
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

        public static PartConventionBuilder<PartConvention> ExportType(
            this PartConventionBuilder<PartConvention> builder)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m }));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ForTypesAssignableFrom<T>(
            this PartConventionBuilder<PartConvention> builder)
        {
            Predicate<Type> condition =
                t => typeof(T).IsAssignableFrom(t) && !t.IsInterface && t.IsPublic;
            
            builder.ProvideValueFor(x => x.Condition, condition);

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ForTypesWithName(
            this PartConventionBuilder<PartConvention> builder, string name)
        {
            Predicate<Type> condition =
                t => t.Name.Equals(name) && !t.IsInterface && t.IsPublic;

            builder.ProvideValueFor(x => x.Condition, condition);

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ForTypesWhereNamespaceContains(
            this PartConventionBuilder<PartConvention> builder, string value)
        {
            Predicate<Type> condition =
                t => t.Namespace != null && t.Namespace.Contains(value) && !t.IsInterface && t.IsPublic;

            builder.ProvideValueFor(x => x.Condition, condition);

            return builder;
        }
    }
}