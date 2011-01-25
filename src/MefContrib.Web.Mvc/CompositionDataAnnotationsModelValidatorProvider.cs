namespace MefContrib.Web.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Reflection;

    /// <summary>
    /// CompositionDataAnnotationsModelValidatorProvider
    /// </summary>
    public class CompositionDataAnnotationsModelValidatorProvider
        : DataAnnotationsModelValidatorProvider
    {
        /// <summary>
        /// The dependency builder.
        /// </summary>
        private readonly IDependencyBuilder builder;     
        
        /// <summary>
        /// The method info to get the attribute from the DataAnnotationsModelValidatorProvider
        /// </summary>
        private readonly MethodInfo getAttributeMethodInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionDataAnnotationsModelValidatorProvider"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public CompositionDataAnnotationsModelValidatorProvider(IDependencyBuilder builder)
        {
            this.builder = builder;
            this.getAttributeMethodInfo =
                    typeof(DataAnnotationsModelValidator).GetMethod("get_Attribute", BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        }

        /// <summary>
        /// Gets a list of validators.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="context">The context.</param>
        /// <param name="attributes">The list of validation attributes.</param>
        /// <returns>A list of validators.</returns>
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
        {
            var validators = base.GetValidators(metadata, context, attributes);
            foreach (var modelValidator in validators.OfType<DataAnnotationsModelValidator>())
            {
                var attribute = this.getAttributeMethodInfo.Invoke(modelValidator, new object[0]);
                this.builder.Build(attribute);
            }

            return validators;
        }
    }
}
