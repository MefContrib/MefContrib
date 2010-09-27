using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class RequiredMetadataItemTests
    {
        [Test]
        public void Implicit_cast_to_keyvaluepair_of_string_type_should_set_value_property_to_value_of_type_property()
        {
            var instance =
                new RequiredMetadataItem(string.Empty, typeof(string));

            KeyValuePair<string, Type> pair = instance;

            pair.Value.ShouldEqual(instance.Type);
        }

        [Test]
        public void Implicit_cast_to_keyvaluepair_of_string_type_should_set_key_property_to_value_of_name_property()
        {
            var instance =
                new RequiredMetadataItem("Foo", null);

            KeyValuePair<string, Type> pair = instance;

            pair.Key.ShouldEqual(instance.Name);
        }

        [Test]
        public void Implicit_cast_to_keyvaluepair_of_string_type_should_not_throw_exception()
        {
            var instance =
                new RequiredMetadataItem();

            KeyValuePair<string, Type> pair;

            var exception =
                Catch.Exception(() => pair = instance);

            exception.ShouldBeNull();
        }

        [Test]
        public void Equals_should_return_true_if_identical_to_instance_being_compared_against()
        {
            var instance =
                new RequiredMetadataItem("Name", typeof(string));

            var results =
                instance.Equals(new RequiredMetadataItem("Name", typeof(string)));

            results.ShouldBeTrue();
        }

        [Test]
        public void Equals_should_return_false_when_name_is_not_identical_to_instance_being_compared_against()
        {
            var instance =
                new RequiredMetadataItem("Foo", null);

            var results =
                instance.Equals(new RequiredMetadataItem("Bar", null));

            results.ShouldBeFalse();
        }

        [Test]
        public void Equals_should_return_false_when_type_is_not_identical_to_instance_being_compared_against()
        {
            var instance =
                new RequiredMetadataItem(string.Empty, typeof(string));

            var results =
                instance.Equals(new RequiredMetadataItem(string.Empty, typeof(object)));

            results.ShouldBeFalse(); 
        }
    }
}