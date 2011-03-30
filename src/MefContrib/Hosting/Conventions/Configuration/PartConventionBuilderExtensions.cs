namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Linq.Expressions;

    public static class PartConventionBuilderExtensions
    {
        public static ImportConventionBuilder<ImportConvention> ImportProperty(
            this ImportConventionBuilder<ImportConvention> builder, string propertyName)
        {
            builder.Members(m => new[] { m.GetProperty(propertyName) });

            return builder;
        }

        public static ExportConventionBuilder<ExportConvention> ExportProperty(
            this ExportConventionBuilder<ExportConvention> builder, string propertyName)
        {
            builder.Members(m => new[] { m.GetProperty(propertyName) });

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ImportConstructor(
            this PartConventionBuilder<PartConvention> builder)
        {
            builder.Imports(x => x.Import().Members(m => new[] { m.GetGreediestConstructor() }));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ImportProperty<TPart>(
            this PartConventionBuilder<PartConvention> builder, Expression<Func<TPart, object>> expression)
        {
            builder.Imports(i => i.Import().Members(m => new[] { expression.GetTargetMemberInfo() }));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ImportProperty(
            this PartConventionBuilder<PartConvention> builder, string propertyName)
        {
            builder.Imports(i => i.Import().Members(m => new[] { m.GetProperty(propertyName) }));

            return builder;
        }
        
        public static PartConventionBuilder<PartConvention> ImportProperty(
            this PartConventionBuilder<PartConvention> builder, string propertyName, Type contractType)
        {
            builder.Imports(i => i.Import().Members(m => new[] { m.GetProperty(propertyName) }).ContractType(t => contractType));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ImportProperty(
            this PartConventionBuilder<PartConvention> builder, string propertyName, Type contractType, string contractName)
        {
            builder.Imports(i => i.Import().Members(m => new[] { m.GetProperty(propertyName) }).ContractType(t => contractType).ContractName(contractName));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ImportProperty(
            this PartConventionBuilder<PartConvention> builder, string propertyName, string contractName)
        {
            builder.Imports(i => i.Import().Members(m => new[] { m.GetProperty(propertyName) }).ContractName(contractName));

            return builder;
        }
        
        public static PartConventionBuilder<PartConvention> ImportField(
           this PartConventionBuilder<PartConvention> builder, string fieldName)
        {
            builder.Imports(i => i.Import().Members(m => new[] { m.GetField(fieldName) }));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ImportField(
            this PartConventionBuilder<PartConvention> builder, string fieldName, Type contractType)
        {
            builder.Imports(i => i.Import().Members(m => new[] { m.GetField(fieldName) }).ContractType(t => contractType));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ImportField(
            this PartConventionBuilder<PartConvention> builder, string fieldName, Type contractType, string contractName)
        {
            builder.Imports(i => i.Import().Members(m => new[] { m.GetField(fieldName) }).ContractType(t => contractType).ContractName(contractName));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ImportField(
            this PartConventionBuilder<PartConvention> builder, string fieldName, string contractName)
        {
            builder.Imports(i => i.Import().Members(m => new[] { m.GetField(fieldName) }).ContractName(contractName));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportAs<T>(
            this PartConventionBuilder<PartConvention> builder)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m }).ContractType<T>());

            return builder;
        }

        public static PartConventionBuilder<PartConvention> Export(
            this PartConventionBuilder<PartConvention> builder)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m }));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportProperty<TPart>(
            this PartConventionBuilder<PartConvention> builder, Expression<Func<TPart, object>> expression)
        {
            builder.Exports(x => x.Export().Members(m => new[] { expression.GetTargetMemberInfo() }));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportProperty(
            this PartConventionBuilder<PartConvention> builder, string propertyName)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m.GetProperty(propertyName) }));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportProperty(
            this PartConventionBuilder<PartConvention> builder, string propertyName, Type contractType)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m.GetProperty(propertyName) }).ContractType(m => contractType));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportProperty(
            this PartConventionBuilder<PartConvention> builder, string propertyName, Type contractType, string contractName)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m.GetProperty(propertyName) }).ContractType(m => contractType).ContractName(contractName));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportProperty(
            this PartConventionBuilder<PartConvention> builder, string propertyName, string contractName)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m.GetProperty(propertyName) }).ContractName(contractName));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportField(
            this PartConventionBuilder<PartConvention> builder, string fieldName)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m.GetField(fieldName) }));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportField(
            this PartConventionBuilder<PartConvention> builder, string fieldName, Type contractType)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m.GetField(fieldName) }).ContractType(m => contractType));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportField(
            this PartConventionBuilder<PartConvention> builder, string fieldName, Type contractType, string contractName)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m.GetField(fieldName) }).ContractType(m => contractType).ContractName(contractName));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ExportField(
            this PartConventionBuilder<PartConvention> builder, string fieldName, string contractName)
        {
            builder.Exports(x => x.Export().Members(m => new[] { m.GetField(fieldName) }).ContractName(contractName));

            return builder;
        }

        public static PartConventionBuilder<PartConvention> ForType<T>(
            this PartConventionBuilder<PartConvention> builder)
        {
            Predicate<Type> condition =
                t => typeof(T) == t;

            builder.ProvideValueFor(x => x.Condition, condition);

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