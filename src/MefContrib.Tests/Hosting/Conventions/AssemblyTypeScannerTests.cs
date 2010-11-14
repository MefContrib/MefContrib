namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Linq;

    using MefContrib.Tests;

    using NUnit.Framework;

    public class AssemblyTypeScannerTests
    {
        [Test]
        public void Ctor_should_throw_argumentnullexception_when_called_with_null()
        {
            var exception =
                Catch.Exception(() => new AssemblyTypeScanner(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_store_assembly_reference_in_assembly_property()
        {
            var assembly =
                CSharpAssemblyFactory.Compile(
                    @"
                        public class Foo
                        {
                        }
                    ");

            var scanner =
                new AssemblyTypeScanner(assembly);

            scanner.Assembly.ShouldBeSameAs(assembly);
        }

        [Test]
        public void GetTypes_should_return_all_types_in_assembly_that_matches_predicate()
        {
            var assembly =
                CSharpAssemblyFactory.Compile(
                    @"
                        public class Foo { }
                        public class Bar { }
                    ");

            var scanner =
                new AssemblyTypeScanner(assembly);

            var types =
                scanner.GetTypes(x => x.Name.Equals("Bar"));

            types.Count().ShouldEqual(1);
        }

        [Test]
        public void GetTypes_should_not_return_non_public_types()
        {
            var assembly =
                CSharpAssemblyFactory.Compile(
                    @"
                        public class Foo { }
                        class Bar { }
                    ");

            var scanner =
                new AssemblyTypeScanner(assembly);

            var types =
                scanner.GetTypes(x => true);

            types.Count().ShouldEqual(1);
        }

        [Test]
        public void GetTypes_should_not_return_abstract_types()
        {
            var assembly =
                CSharpAssemblyFactory.Compile(
                    @"
                        public class Foo { }
                        public abstract class Bar { }
                    ");

            var scanner =
                new AssemblyTypeScanner(assembly);

            var types =
                scanner.GetTypes(x => true);

            types.Count().ShouldEqual(1);
        }

        [Test]
        public void GetTypes_should_only_return_class_types()
        {
            var assembly =
                CSharpAssemblyFactory.Compile(
                    @"
                        public class Foo { }
                        public struct Bar { }
                    ");

            var scanner =
                new AssemblyTypeScanner(assembly);

            var types =
                scanner.GetTypes(x => true);

            types.Count().ShouldEqual(1);
        }
    }
}