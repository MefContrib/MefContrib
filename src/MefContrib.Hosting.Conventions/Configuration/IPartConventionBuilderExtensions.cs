namespace MefContrib.Hosting.Conventions.Configuration
{
    public static class IPartConventionBuilderExtensions
    {
        public static IPartConventionBuilder<PartConvention> ImportConstructor(
            this IPartConventionBuilder<PartConvention> builder)
        {
            builder.Imports(x => x.Import().Members(m => new[] { m.GetGreediestConstructor() }));

            return builder;
        }

        public static IPartConventionBuilder<PartConvention> ExportTypeAs<T>(
            this IPartConventionBuilder<PartConvention> builder)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m }).ContractType<T>());

            return builder;
        }
    }
}