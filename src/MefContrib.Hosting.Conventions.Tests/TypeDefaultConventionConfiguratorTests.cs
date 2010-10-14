namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Linq;

    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;

    using NUnit.Framework;

    public class TypeDefaultConventionConfiguratorTests
    {
        [Test]
        public void ForType_should_not_return_null()
        {
            var configurator =
                new TypeDefaultConventionConfigurator();

            var builder =
                configurator.ForType<object>();

            builder.ShouldNotBeNull();
        }

        [Test]
        public void GetDefaultConventions_should_return_conventions_for_each_configured_type()
        {
            var configurator =
                new TypeDefaultConventionConfigurator();

            configurator.ForType<string>();
            configurator.ForType<object>();

            var conventions =
                configurator.GetDefaultConventions();

            conventions.Count().ShouldEqual(2);
        }
    }
}