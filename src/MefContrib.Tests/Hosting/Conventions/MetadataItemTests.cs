using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class MetadataItemTests
    {
        [Test]
        public void Ctor_should_throw_argumentnullexception_when_name_is_null()
        {
            var exception =
                Catch.Exception(() => new MetadataItem(null, null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_throw_argumentoutofrangeexception_when_name_is_empty()
        {
            var exception =
                Catch.Exception(() => new MetadataItem(string.Empty, null));

            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Ctor_should_throw_argumentnullexception_when_value_is_null()
        {
            var exception =
                Catch.Exception(() => new MetadataItem("Foo", null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Equals_should_return_if_name_is_not_identical_on_instance_being_compared_against()
        {
            var instance =
                new MetadataItem("Foo", new object());

            var results =
                instance.Equals(new MetadataItem("Bar", new object()));

            results.ShouldBeFalse();
        }

        [Test]
        public void Equals_should_return_false_if_value_is_no_identical_on_instance_being_compared_against()
        {
            var instance =
                new MetadataItem("Foo", new object());

            var results =
                instance.Equals(new MetadataItem("Foo", string.Empty));

            results.ShouldBeFalse();
        }
    }
}