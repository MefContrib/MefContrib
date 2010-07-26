using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class TypeScannerTests
    {
        private static TypeScanner CreateDefaultTypeScanner()
        {
            return new TypeScanner();
        }

        [Test]
        public void Ctor_should_throw_argumentnullexception_when_called_with_null_enumerable()
        {
            var exception =
                Catch.Exception(() => new TypeScanner((IEnumerable<Type>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_throw_argumentnullexception_when_called_with_null_expression()
        {
            var exception =
                Catch.Exception(() => new TypeScanner((Func<IEnumerable<Type>>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddTypes_should_throw_argumentnullexception_when_called_with_null()
        {
            var scanner =
                CreateDefaultTypeScanner();

            var exception =
                Catch.Exception(() => scanner.AddTypes(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddTypes_should_throw_argumentnullexception_when_function_evaluates_to_null()
        {
            var scanner =
                CreateDefaultTypeScanner();

            var exception =
                Catch.Exception(() => scanner.AddTypes(() => null));

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

            var scanner =
                CreateDefaultTypeScanner();
            scanner.AddTypes(() => assembly.GetExportedTypes().ToList());

            var types =
                scanner.GetTypes(t => true);

            types.Count().ShouldEqual(2);
        }

        [Test]
        public void GetParts_should_return_empty_collection_when_no_types_have_been_loaded()
        {
            var scanner =
                CreateDefaultTypeScanner();

            var types =
                scanner.GetTypes(t => true);

            types.Count().ShouldEqual(0);
        }

        [Test]
        public void GetParts_should_return_emtpy_collection_when_predicate_did_not_match_any_types()
        {
            var scanner =
                CreateDefaultTypeScanner();

            var types =
                scanner.GetTypes(t => false);

            types.Count().ShouldEqual(0);
        }

        [Test]
        public void GetParts_should_throw_argumentnullexception_when_called_with_null()
        {
            var scanner =
                CreateDefaultTypeScanner();

            var exception =
                Catch.Exception(() => scanner.GetTypes(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }
    }
}