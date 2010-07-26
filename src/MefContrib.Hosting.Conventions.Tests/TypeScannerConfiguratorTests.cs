namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using MefContrib.Hosting.Conventions.Configuration;

    using NUnit.Framework;

    [TestFixture]
    public class TypeScannerConfiguratorTests
    {
        public TypeScannerConfiguratorTests()
        {
        }

        [Test]
        public void Assembly_should_throw_argumentnullexception_when_called_with_null_assembly()
        {
            var configurator =
                new TypeScannerConfigurator();

            var exception =
                Catch.Exception(() => configurator.Assembly((Assembly)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Assembly_should_return_reference_to_same_configurator_when_called_with_assembly()
        {
            var configurator =
                new TypeScannerConfigurator();

            var reference =
                configurator.Assembly(Assembly.GetExecutingAssembly());

            reference.ShouldBeSameAs(configurator);
        }

        [Test]
        public void Assembly_should_throw_argumentnullexception_when_called_with_null_path()
        {
            var configurator =
                new TypeScannerConfigurator();

            var exception =
                Catch.Exception(() => configurator.Assembly((string)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Assembly_should_return_reference_to_same_configurator_when_called_with_path()
        {
            using (var factory = new AssemblyFactory())
            {
                var configurator =
                    new TypeScannerConfigurator();

                var assembly =
                    factory.Build(@"public class Foo { }");

                var reference =
                    configurator.Assembly((string)assembly);

                reference.ShouldBeSameAs(configurator);
            }
        }

        [Test]
        public void Assembly_should_throw_filenotfoundexception_when_called_with_invalid_path()
        {
            var configurator =
                new TypeScannerConfigurator();

            var path =
                Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            var exception =
                Catch.Exception(() => configurator.Assembly(path));

            exception.ShouldBeOfType<FileNotFoundException>();
        }

        [Test]
        public void Assembly_should_throw_argumentnullexception_when_called_with_null_expression()
        {
            var configurator =
                new TypeScannerConfigurator();

            var exception =
                Catch.Exception(() => configurator.Assembly((Func<Assembly, bool>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Assembly_should_return_reference_to_same_configurator_when_called_with_expression()
        {
            var configurator =
                new TypeScannerConfigurator();

            var reference =
                configurator.Assembly(x => x.FullName.Length > 0);

            reference.ShouldBeSameAs(configurator);
        }

        [Test]
        public void Assembly_should_add_scanner_to_configurator_when_called_with_assembly()
        {
            using (var factory = new AssemblyFactory())
            {
                var configurator =
                    new TypeScannerConfigurator();

                var builtAssembly =
                    factory.Build(
                        @"
                        public class Foo { }
                        class Bar { }
                        public interface IFoo { }
                        interface IBar { }
                        public abstract class Baz { }
                    ");

                configurator.Assembly((Assembly)builtAssembly);

                var scanner =
                    configurator.GetTypeScanner();

                var results =
                    scanner.GetTypes(x => true);

                results.Count().ShouldEqual(1);
            }
        }

        [Test]
        public void Assembly_should_add_scanner_to_configurator_when_called_with_path()
        {
            using (var factory = new AssemblyFactory())
            {
                var configurator =
                    new TypeScannerConfigurator();

                var builtAssembly =
                    factory.Build(
                        @"
                        public class Foo { }
                        class Bar { }
                        public interface IFoo { }
                        interface IBar { }
                        public abstract class Baz { }
                    ");

                configurator.Assembly((string)builtAssembly);

                var scanner =
                    configurator.GetTypeScanner();

                var results =
                    scanner.GetTypes(x => true);

                results.Count().ShouldEqual(1);
            }
        }

        [Test]
        public void Assembly_should_add_scanner_to_configurator_when_called_with_expression()
        {
            var configurator =
                new TypeScannerConfigurator();

            configurator.Assembly(x => true);

            var scanner =
                configurator.GetTypeScanner();

            var results =
                scanner.GetTypes(x => x.Equals(typeof(FakePart)));

            results.Count().ShouldEqual(1);
        }

        [Test]
        public void Directory_should_throw_argumentnullexception_when_called_with_null_path()
        {
            var configurator =
                new TypeScannerConfigurator();

            var exception =
                Catch.Exception(() => configurator.Directory(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Directory_should_throw_argumentoutofrangeexcetion_when_called_with_empty_path()
        {
            var configurator =
                new TypeScannerConfigurator();

            var exception =
                Catch.Exception(() => configurator.Directory(string.Empty));

            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Directory_should_throw_directorynotfoundexception_when_called_with_invalid_path()
        {
            var invalidDirectoryPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                    Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            var configurator =
                new TypeScannerConfigurator();

            var exception =
                Catch.Exception(() => configurator.Directory(invalidDirectoryPath));

            exception.ShouldBeOfType<DirectoryNotFoundException>();
        }

        [Test]
        public void Directory_should_return_reference_to_same_configurator()
        {
            var configurator =
                new TypeScannerConfigurator();

            var reference =
                configurator.Directory(Environment.CurrentDirectory);

            reference.ShouldBeSameAs(configurator);
        }

        [Test]
        public void Directory_should_identify_types_in_each_assembly_found_in_directory_specified_by_path()
        {
            using (var factory = new AssemblyFactory())
            {
                factory.Build(@"public class Foo { }");
                factory.Build(@"public class Bar { }");

                var configurator =
                    new TypeScannerConfigurator();

                configurator.Directory(factory.AssemblyDirectory.FullName);

                var scanner =
                    configurator.GetTypeScanner();

                var results =
                    scanner.GetTypes(x => true);

                results.Count().ShouldEqual(2);
            }
        }

        [Test]
        public void Types_should_throw_argumentnullexception_when_called_with_null_enumerable()
        {
            var configurator =
                new TypeScannerConfigurator();

            var exception =
                Catch.Exception(() => configurator.Types((IEnumerable<Type>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Types_should_throw_argumentnullexception_when_called_with_null_condition()
        {
            var configurator =
                new TypeScannerConfigurator();

            var exception =
                Catch.Exception(() => configurator.Types((Func<Type[]>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Types_should_return_reference_to_same_configurator_when_called_with_enumerable()
        {
            var configurator =
                new TypeScannerConfigurator();

            var reference =
                configurator.Types(new List<Type> { typeof(object) });

            reference.ShouldBeSameAs(configurator);
        }

        [Test]
        public void Types_should_return_reference_to_same_configurator_when_called_with_function()
        {
            var configurator =
                new TypeScannerConfigurator();

            var reference =
                configurator.Types(() => new[] { typeof(object) });

            reference.ShouldBeSameAs(configurator);
        }

        [Test]
        public void Types_should_identify_types_passed_in_as_argument_when_called_with_enumerable()
        {
            var configurator =
                new TypeScannerConfigurator();

            configurator.Types(new List<Type> { typeof(object), typeof(string) });

            var scanner =
                configurator.GetTypeScanner();

            var results =
                scanner.GetTypes(x => true);

            results.Count().ShouldEqual(2);
        }

        [Test]
        public void Types_should_identify_types_passed_in_as_argument_when_called_with_expression()
        {
            var configurator =
                new TypeScannerConfigurator();

            configurator.Types(() => new[] { typeof(object), typeof(string) });

            var scanner =
                configurator.GetTypeScanner();

            var results =
                scanner.GetTypes(x => true);

            results.Count().ShouldEqual(2);
        }

        [Test]
        public void GetTypeScanner_should_return_aggregatedtypescanner_when_no_scanners_have_been_added()
        {
            var configurator =
                new TypeScannerConfigurator();

            var results =
                configurator.GetTypeScanner();

            results.ShouldNotBeNull();
        }
    }
}