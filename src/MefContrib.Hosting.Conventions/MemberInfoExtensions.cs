namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Reflection;

    /// <summary>
    /// Contains extension methods for the <see cref="MemberInfo"/> interface.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets the cardinality of the <see cref="MemberInfo"/> instance.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo"/> to check the cardinality of.</param>
        /// <param name="allowDefault"><see langword="true" /> if the default value of the member can be used if no exports match; otherwise <see langword="false" />.</param>
        /// <returns>An <see cref="ImportCardinality"/> value, reflecting the cardinality of the member.</returns>
        /// <exception cref="ImportCardinalityMismatchException">The cardinality could not be retrieved from the provided <see cref="MemberInfo"/>.</exception>
        public static ImportCardinality GetCardinality(this MemberInfo member, bool allowDefault)
        {
            Type importType = null;

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    importType = ((FieldInfo)member).FieldType;
                    break;

                case MemberTypes.Property:
                    importType = ((PropertyInfo)member).PropertyType;
                    break;
            }

            return importType.GetCardinality(allowDefault);
        }

        /// <summary>
        /// Gets the cardinality of the <see cref="Type"/> instance.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check the cardinality of.</param>
        /// <param name="allowDefault"><see langword="true" /> if the default value of the type can be used if no exports match; otherwise <see langword="false" />.</param>
        /// <returns>An <see cref="ImportCardinality"/> value, reflecting the cardinality of the type.</returns>
        /// <exception cref="ImportCardinalityMismatchException">The cardinality could not be retrieved from the provided <see cref="Type"/>.</exception>
        public static ImportCardinality GetCardinality(this Type type, bool allowDefault)
        {
            if (type != null)
            {
                if (type.IsEnumerable())
                {
                    return ImportCardinality.ZeroOrMore;
                }

                return allowDefault ?
                    ImportCardinality.ZeroOrOne : ImportCardinality.ExactlyOne;
            }

            throw new ImportCardinalityMismatchException();
        }

        /// <summary>
        /// Gets the type of the return type for the <see cref="MemberInfo"/>.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo"/> to get the return type of.</param>
        /// <returns>A <see cref="Type"/> instance.</returns>
        /// <remarks>
        /// For <see cref="MemberInfo"/> instances with a <see cref="MemberInfo.MemberType"/> value of <see cref="MemberTypes.TypeInfo"/> or
        /// <see cref="MemberTypes.NestedType"/>, the actuall <see cref="Type"/> will be returned.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Can only retrieve type for fields, properties, methods or types.</exception>
        public static Type GetContractMember(this MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)member).PropertyType;
            }

            if (member.MemberType == MemberTypes.Method)
            {
                return ((MethodInfo)member).ReturnType;
            }

            if (member.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)member).FieldType;
            }

            if (member.MemberType == MemberTypes.TypeInfo ||
                member.MemberType == MemberTypes.NestedType)
            {
                return (Type)member;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Converts the provided <see cref="MemberInfo"/> instance into a <see cref="LazyMemberInfo"/> instance.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo"/> instance to convert.</param>
        /// <returns>A <see cref="LazyMemberInfo"/> instance for the <see cref="MemberInfo"/> instances provided by the <paramref name="member"/> parameter.</returns>
        public static LazyMemberInfo ToLazyMemberInfo(this MemberInfo member)
        {
            return new LazyMemberInfo(member);
        }
    }
}