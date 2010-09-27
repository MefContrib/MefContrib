using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class TypeLoaderTests
    {
        private static TypeLoader CreateDefaultTypeLoader()
        {
            return new TypeLoader();
        }

        [Test]
        public void AddTypes_should_throw_argumentnullexception_when_called_with_null()
        {
            var typeLoader =
                CreateDefaultTypeLoader();

            var exception =
                Catch.Exception(() =>typeLoader.AddTypes(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddTypes_should_throw_argumentnullexception_when_function_evaluates_to_null()
        {
            var typeLoader =
                CreateDefaultTypeLoader();

            var exception =
                Catch.Exception(() => typeLoader.AddTypes(() => null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddTypes_should_extract_types_from_function()
        {
            var assembly =
                CSharpAssemblyFactory.Compile(
                    @"
                        public class Foo { }
                        public class Bar { }
                    ");

            var typeLoader =
                CreateDefaultTypeLoader();
            typeLoader.AddTypes(() => assembly.GetExportedTypes().ToList());

            var types =
                typeLoader.GetTypes(t => true);

            types.Count().ShouldEqual(2);
        }

        [Test]
        public void GetParts_should_return_empty_collection_when_no_types_have_been_loaded()
        {
            var typeLoader =
                CreateDefaultTypeLoader();

            var types =
                typeLoader.GetTypes(t => true);

            types.Count().ShouldEqual(0);
        }

        [Test]
        public void GetParts_should_return_emtpy_collection_when_predicate_did_not_match_any_types()
        {
            var typeLoader =
                CreateDefaultTypeLoader();

            var types =
                typeLoader.GetTypes(t => false);

            types.Count().ShouldEqual(0);
        }

        [Test]
        public void GetParts_should_throw_argumentnullexception_when_called_with_null()
        {
            var typeLoader =
                CreateDefaultTypeLoader();

            var exception =
                Catch.Exception(() => typeLoader.GetTypes(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }
    }
}